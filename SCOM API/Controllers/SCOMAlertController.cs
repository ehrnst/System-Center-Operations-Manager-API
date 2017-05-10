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
        [Route("Alerts/{ComputerName}")]
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
                ///Get alerts
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
        /// Updates the specified alert.
        /// </summary>
        /// <param name="ResolutionState">If specified as 255 (closed) and alert is raised by monitor. The corresponding monitor will be reset</param>
        /// <param name="TicketId">set if you want to update alert with a ticket id</param>
        /// <param name="Id">the alert GUID</param>
        [HttpPut]
        [Route("Alerts")]
        public IList<MonitoringAlert> UpdateAlertById(Guid Id, byte ResolutionState, string TicketId = "")
        {
            if (string.IsNullOrEmpty(ResolutionState.ToString()))
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }


            //alert criteria
            var Criteria = string.Format("Id = '{0}'", Id);
            MonitoringAlertCriteria alertCriteria = new MonitoringAlertCriteria(Criteria);
            var alerts = mg.OperationalData.GetMonitoringAlerts(alertCriteria, default(DateTime));
            foreach (MonitoringAlert a in alerts)
            {
                //If resolution state is set to closed and alert is raised by a monitor. Reset the monitor
                if (a.IsMonitorAlert & ResolutionState.ToString() == "255")
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
                    
                    if (string.IsNullOrWhiteSpace(TicketId))
                    {
                        a.ResolutionState = ResolutionState;
                        string comment = "Changed resolution state (API)";
                        a.Update(comment);

                    }
                    //If ticket id is specified.
                    else
                    {
                        a.ResolutionState = ResolutionState;
                        a.TicketId = TicketId;
                        string comment = "Changed resolution state and ticket id (API)";
                        a.Update(comment);
                    }
                    
                }

            }
            return alerts;

        }

    }
}
//END
