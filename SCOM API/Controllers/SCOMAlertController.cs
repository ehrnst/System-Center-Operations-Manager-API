using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Administration;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Web;
using Microsoft.EnterpriseManagement.Configuration;
using System.Configuration;
using System.Collections.ObjectModel;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Web.Http.Description;
using SCOM_API.Models;

namespace SCOM_API.Controllers
{
    [RoutePrefix("API")]
    public class SCOMAlertController : ApiController
    {
        ManagementGroup mg = null;

        public SCOMAlertController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            ManagementGroupConnectionSettings mgSettings = new ManagementGroupConnectionSettings(SCOMSERVER);

            mg = ManagementGroup.Connect(mgSettings);

        }

        /// <summary>
        /// Get all active/new alerts
        /// </summary>
        [HttpGet]
        [Route("Alerts")]
        public IList<MonitoringAlert> GetAlerts()
        {
            MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria("ResolutionState = '0'");

            var OpenAlerts = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));
            return OpenAlerts;
        }

        /// <summary>
        /// Gets single alert based on ID
        /// </summary>
        /// <param name="Id">Guid of the alert</param>
        /// <response code="400">Bad request. Check Id parameter</response>
        /// <response code="404">Not found</response>
        [HttpGet]
        [Route("Alerts/{Id:Guid}")]
        public IList<MonitoringAlert> GetAlertById(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            var Criteria = string.Format("Id = '{0}'", Id);

            MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
            var Alert = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

            return Alert;


        }

        /// <summary>
        /// Get all alerts related to the computername.
        /// </summary>
        /// <param name="ComputerName">FQDN of the windows computer</param>
        /// <param name="IncClosed">if true closed alert is also returned. Default is 'false'</param>
        [HttpGet]
        [Route("Alerts/Computer/{ComputerName}")]
        public IList<MonitoringAlert> GetAlertByComputerName(string ComputerName, bool? IncClosed = false)
        {
            if (string.IsNullOrEmpty(ComputerName))
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            //If include closed alerts
            if (IncClosed == true)
            {
                //Set alert criteria
                var Criteria = string.Format("MonitoringObjectPath = '{0}'", ComputerName);
                //Get alerts
                MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
                var Alert = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

                return Alert;
            }

            else
            {
                //set alert criteria
                var Criteria = string.Format("MonitoringObjectPath = '{0}' AND ResolutionState = 0", ComputerName);
                //Get alerts
                MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
                var Alert = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

                return Alert;
            }
        }

        /// <summary>
        /// Get all alerts related to the specific monitoring object.
        /// </summary>
        /// <param name="MonitoringObjectId">SCOM GUID of the monitoring object</param>
        /// <param name="IncClosed">if true closed alert is also returned. Default is 'false'</param>
        [HttpGet]
        [Route("Alerts/MonitoringObject/{MonitoringObjectId:Guid}")]
        public IList<MonitoringAlert> GetAlertByMonitoringObjectId(Guid MonitoringObjectId, bool? IncClosed = false)
        {
            if (MonitoringObjectId == Guid.Empty)
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            //If include closed alerts
            if (IncClosed == true)
            {
                //Set alert criteria
                var Criteria = string.Format("MonitoringObjectId = '{0}'", MonitoringObjectId);
                ///Get alerts
                MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
                var Alert = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

                return Alert;
            }

            else
            {
                //set alert criteria
                var Criteria = string.Format("MonitoringObjectId = '{0}' AND ResolutionState = 0", MonitoringObjectId);
                //Get alerts
                MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
                var Alert = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

                return Alert;
            }
        }

        /// <summary>
        /// Updates the specific alert.
        /// </summary>
        /// <param name="Properties">Json object to update alert properties.
        /// All set properties are available.
        /// For more information please see technet documentation "http://bit.ly/2zblZLh"</param>
        /// <param name="Id">Specify alert guid you want to update</param>
        [HttpPut]
        [ResponseType(typeof(HttpResponseMessage))]
        [Route("Alerts/{Id:Guid}")]
        //public IList<MonitoringAlert> UpdateAlertById([FromUri()]Guid Id, [FromBody()] SCOMAlertUpdateModel Properties)
        public IHttpActionResult UpdateAlertById([FromUri()]Guid Id, [FromBody()] SCOMAlertUpdateModel Properties)
        {
            if (Id == Guid.Empty)
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            // Declare variables
            var ResolutionState = Properties.resolutionState;

            //alert criteria
            var Criteria = string.Format("Id = '{0}'", Id);
            MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
            var alerts = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));

            foreach (MonitoringAlert a in alerts)
            {
                //If resolution state is set to closed and alert is raised by a monitor. Reset the monitor
                if (a.IsMonitorAlert & ResolutionState == "255")
                {
                    //Get object and monitor that raised the alert
                    Guid monitoringObjectId = a.MonitoringObjectId;
                    var monitorId = a.RuleId;
                    var monitor = mg.Monitoring.GetMonitor(monitorId);
                    var monObject = mg.EntityObjects.GetObject<MonitoringObject>(monitoringObjectId, ObjectQueryOptions.Default);
                    //reset the monitor to 'close' the alert
                    monObject.ResetMonitoringState(monitor);

                }


                else
                {

                    if (string.IsNullOrEmpty(ResolutionState))
                    {
                        a.TfsWorkItemId = Properties.tfsWorkItemId;
                        a.TfsWorkItemOwner = Properties.tfsWorkItemOwner;
                        a.Owner = Properties.owner;
                        a.CustomField1 = Properties.customField1;
                        a.CustomField2 = Properties.customField2;
                        a.CustomField3 = Properties.customField3;
                        a.CustomField4 = Properties.customField4;
                        a.CustomField5 = Properties.customField5;
                        a.CustomField6 = Properties.customField6;
                        a.CustomField7 = Properties.customField7;
                        a.CustomField8 = Properties.customField8;
                        a.CustomField9 = Properties.customField9;
                        a.CustomField10 = Properties.customField10;

                        string comment = "Alert properties updated";
                        a.Update(comment);

                    }
                    //If resolution state is specified
                    else
                    {
                        a.TfsWorkItemId = Properties.tfsWorkItemId;
                        a.TfsWorkItemOwner = Properties.tfsWorkItemOwner;
                        a.Owner = Properties.owner;
                        a.CustomField1 = Properties.customField1;
                        a.CustomField2 = Properties.customField2;
                        a.CustomField3 = Properties.customField3;
                        a.CustomField4 = Properties.customField4;
                        a.CustomField5 = Properties.customField5;
                        a.CustomField6 = Properties.customField6;
                        a.CustomField7 = Properties.customField7;
                        a.CustomField8 = Properties.customField8;
                        a.CustomField9 = Properties.customField9;
                        a.CustomField10 = Properties.customField10;

                        //Convert string resolution state to byte type
                        var Res = Convert.ToByte(ResolutionState);
                        a.ResolutionState = Res;
                        a.TicketId = Properties.ticketId;
                        var comments = $"Updated and changed resolution state: {ResolutionState}";
                        string comment = comments;
                        a.Update(comment);
                    }

                }

            }
            // creating OK response
            return Ok(new { message = "Alert updated", alertId = Id.ToString() });

        }

    }
}
//END
