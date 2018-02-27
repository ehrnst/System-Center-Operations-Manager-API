using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Security.Principal;

namespace SCOM_API.Controllers
{

    public class SCOMPerfController : ApiController
    {
        SqlConnection DWConnection = new SqlConnection();
        public SCOMPerfController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            var SCOMDW = ConfigurationManager.AppSettings["ScomDWServer"];
            builder["Data Source"] = SCOMDW;
            builder["Integrated Security"] = "SSPI";
            builder["Initial Catalog"] = "OperationsManagerDW";



            DWConnection.ConnectionString = builder.ConnectionString;
        }

        private DataTable dataTable = new DataTable();

        #region rawData

        /// <summary>
        /// Get RAW performance data from a specific managedEntity and metric
        /// </summary>
        /// <example>
        /// API/Perf/5f2f477c-3b19-4ce8-b27a-eef59b9dc377?counterName=PercentMemoryUsed
        /// </example>
        /// <param name="managedEntityId">The guid of your managed entity, ie: windows computer</param>
        /// <param name="counterName">The performance counter you want to retrieve data from</param>
        /// <param name="startDate">Optionally add your start date. Data will be pulled between start and end dates</param>
        /// <param name="endDate">Optionally add your start date. Data will be pulled between start and end dates</param>

        [HttpGet]
        [Route("API/Perf/{managedEntityId:Guid}")]
        public IHttpActionResult GetPerformanceData(Guid managedEntityId, string counterName, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (WindowsImpersonationContext context = (WindowsIdentity.GetCurrent()).Impersonate())
            {
                if (managedEntityId == Guid.Empty && string.IsNullOrEmpty(counterName))
                {
                    throw new HttpResponseException(Request
                    .CreateResponse(HttpStatusCode.BadRequest));
                }


                else
                {

                    if (!endDate.HasValue)
                    {
                        endDate = DateTime.UtcNow;
                    }

                    if (!startDate.HasValue)
                    {
                        startDate = (DateTime)SqlDateTime.MinValue;
                    }

                    // Construct the actual DW sql query
                    string sqlQuery = @"
            USE OperationsManagerDW
            SELECT DateTime, SampleValue, ObjectName, InstanceName, CounterName
            FROM
            Perf.vPerfRaw INNER JOIN
            vPerformanceRuleInstance ON Perf.vPerfRaw.PerformanceRuleInstanceRowId = vPerformanceRuleInstance.PerformanceRuleInstanceRowId INNER JOIN
            vPerformanceRule ON vPerformanceRuleInstance.RuleRowId = vPerformanceRule.RuleRowId INNER JOIN
            vRelationship ON Perf.vPerfRaw.ManagedEntityRowId = vRelationship.TargetManagedEntityRowId INNER JOIN
            vManagedEntity ON vRelationship.SourceManagedEntityRowId = vManagedEntity.ManagedEntityRowId
            WHERE ManagedEntityGuid = @entity
            AND vPerformanceRule.CounterName = @counter
            AND DateTime between @startDate and @endDate
            ORDER BY Perf.vPerfRaw.DateTime DESC";

                    try
                    {

                        // Initiate command and add parameters

                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;
                        SqlParameter counter = sqlCmd.Parameters.Add("@counter", SqlDbType.VarChar, 256);
                        counter.Value = counterName;
                        sqlCmd.Parameters.AddWithValue("@entity", managedEntityId);
                        sqlCmd.Parameters.AddWithValue("@startDate", startDate);
                        sqlCmd.Parameters.AddWithValue("@endDate", endDate);

                        sqlCmd.CommandText = sqlQuery;
                        sqlCmd.Connection = DWConnection;


                        // Connect SQL
                        DWConnection.Open();
                        // Fill datatable with result from SQL query
                        SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                        da.Fill(dataTable);

                        // Close connections and reurn dataTable
                        da.Dispose();
                        DWConnection.Close();
                        return Ok(dataTable);
                    }

                    catch (Exception Ex)
                    {

                        HttpResponseMessage exeption = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        exeption.Content = new StringContent(Ex.ToString());
                        throw new HttpResponseException(exeption);
                    }
                }

            }

        }

        #endregion

        #region hourlyData

        /// <summary>
        /// Get hourly performance data from a specific managedEntity and metric
        /// </summary>
        /// <example>
        /// API/Perf/5f2f477c-3b19-4ce8-b27a-eef59b9dc377?counterName=PercentMemoryUsed
        /// </example>
        /// <param name="managedEntityId">The guid of your managed entity, ie: windows computer</param>
        /// <param name="counterName">The performance counter you want to retrieve data from</param>
        /// <param name="startDate">Optionally add your start date. Data will be pulled between start and end dates</param>
        /// <param name="endDate">Optionally add your start date. Data will be pulled between start and end dates</param>

        [HttpGet]
        [Route("API/Perf/Hourly/{managedEntityId:Guid}")]
        public IHttpActionResult GetHourlyPerformanceData(Guid managedEntityId, string counterName, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (WindowsImpersonationContext context = (WindowsIdentity.GetCurrent()).Impersonate())
            {
                if (managedEntityId == Guid.Empty && string.IsNullOrEmpty(counterName))
                {
                    throw new HttpResponseException(Request
                    .CreateResponse(HttpStatusCode.BadRequest));
                }


                else
                {

                    if (!endDate.HasValue)
                    {
                        endDate = DateTime.UtcNow;
                    }

                    if (!startDate.HasValue)
                    {
                        startDate = (DateTime)SqlDateTime.MinValue;
                    }

                    // Construct the actual DW sql query
                    string sqlQuery = @"
            USE OperationsManagerDW
            SELECT DateTime, MaxValue, AverageValue, MinValue, StandardDeviation, ObjectName, InstanceName, CounterName
            FROM
            Perf.vPerfHourly INNER JOIN
            vPerformanceRuleInstance ON Perf.vPerfHourly.PerformanceRuleInstanceRowId = vPerformanceRuleInstance.PerformanceRuleInstanceRowId INNER JOIN
            vPerformanceRule ON vPerformanceRuleInstance.RuleRowId = vPerformanceRule.RuleRowId INNER JOIN
            vRelationship ON Perf.vPerfHourly.ManagedEntityRowId = vRelationship.TargetManagedEntityRowId INNER JOIN
            vManagedEntity ON vRelationship.SourceManagedEntityRowId = vManagedEntity.ManagedEntityRowId
            WHERE ManagedEntityGuid = @entity
            AND vPerformanceRule.CounterName = @counter
            AND DateTime between @startDate and @endDate
            ORDER BY Perf.vPerfHourly.DateTime DESC";

                    try
                    {
                        // Initiate command and add parameters
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;
                        SqlParameter counter = sqlCmd.Parameters.Add("@counter", SqlDbType.VarChar, 256);
                        counter.Value = counterName;
                        sqlCmd.Parameters.AddWithValue("@entity", managedEntityId);
                        sqlCmd.Parameters.AddWithValue("@startDate", startDate);
                        sqlCmd.Parameters.AddWithValue("@endDate", endDate);

                        sqlCmd.CommandText = sqlQuery;
                        sqlCmd.Connection = DWConnection;


                        // Connect SQL
                        DWConnection.Open();
                        // Fill datatable with result from SQL query
                        SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                        da.Fill(dataTable);

                        // Close connections and reurn dataTable
                        da.Dispose();
                        DWConnection.Close();
                        return Ok(dataTable);
                    }

                    catch (Exception Ex)
                    {

                        HttpResponseMessage exeption = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        exeption.Content = new StringContent(Ex.ToString());
                        throw new HttpResponseException(exeption);
                    }
                }

            }
        }
        #endregion

        #region dailyData

        /// <summary>
        /// Get daily aggragated performance data from a specific managedEntity and metric
        /// </summary>
        /// <example>
        /// API/Perf/5f2f477c-3b19-4ce8-b27a-eef59b9dc377?counterName=PercentMemoryUsed
        /// </example>
        /// <param name="managedEntityId">The guid of your managed entity, ie: windows computer</param>
        /// <param name="counterName">The performance counter you want to retrieve data from</param>
        /// <param name="startDate">Optionally add your start date. Data will be pulled between start and end dates</param>
        /// <param name="endDate">Optionally add your start date. Data will be pulled between start and end dates</param>

        [HttpGet]
        [Route("API/Perf/Daily/{managedEntityId:Guid}")]
        public IHttpActionResult GetDailyPerformanceData(Guid managedEntityId, string counterName, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (WindowsImpersonationContext context = (WindowsIdentity.GetCurrent()).Impersonate())
            {
                if (managedEntityId == Guid.Empty && string.IsNullOrEmpty(counterName))
                {
                    throw new HttpResponseException(Request
                    .CreateResponse(HttpStatusCode.BadRequest));
                }


                else
                {

                    if (!endDate.HasValue)
                    {
                        endDate = DateTime.UtcNow;
                    }

                    if (!startDate.HasValue)
                    {
                        startDate = (DateTime)SqlDateTime.MinValue;
                    }

                    // Construct the actual DW sql query
                    string sqlQuery = @"
            USE OperationsManagerDW
            SELECT DateTime, MaxValue, AverageValue, MinValue, StandardDeviation, ObjectName, InstanceName, CounterName
            FROM
            Perf.vPerfDaily INNER JOIN
            vPerformanceRuleInstance ON Perf.vPerfDaily.PerformanceRuleInstanceRowId = vPerformanceRuleInstance.PerformanceRuleInstanceRowId INNER JOIN
            vPerformanceRule ON vPerformanceRuleInstance.RuleRowId = vPerformanceRule.RuleRowId INNER JOIN
            vRelationship ON Perf.vPerfDaily.ManagedEntityRowId = vRelationship.TargetManagedEntityRowId INNER JOIN
            vManagedEntity ON vRelationship.SourceManagedEntityRowId = vManagedEntity.ManagedEntityRowId
            WHERE ManagedEntityGuid = @entity
            AND vPerformanceRule.CounterName = @counter
            AND DateTime between @startDate and @endDate
            ORDER BY Perf.vPerfDaily.DateTime DESC";

                    try
                    {
                        // Initiate command and add parameters
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;
                        SqlParameter counter = sqlCmd.Parameters.Add("@counter", SqlDbType.VarChar, 256);
                        counter.Value = counterName;
                        sqlCmd.Parameters.AddWithValue("@entity", managedEntityId);
                        sqlCmd.Parameters.AddWithValue("@startDate", startDate);
                        sqlCmd.Parameters.AddWithValue("@endDate", endDate);

                        sqlCmd.CommandText = sqlQuery;
                        sqlCmd.Connection = DWConnection;

                        // Connect SQL
                        DWConnection.Open();

                        if (DWConnection.State == ConnectionState.Closed)
                        {
                            throw new HttpResponseException(HttpStatusCode.ServiceUnavailable);
                        }


                        // Fill datatable with result from SQL query
                        SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                        da.Fill(dataTable);

                        // Close connections and reurn dataTable
                        da.Dispose();
                        DWConnection.Close();
                        return Ok(dataTable);
                    }

                    catch (Exception Ex)
                    {

                        HttpResponseMessage exeption = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        exeption.Content = new StringContent(Ex.ToString());
                        throw new HttpResponseException(exeption);
                    }
                }

            }
        }
        #endregion

    }
}
//END
