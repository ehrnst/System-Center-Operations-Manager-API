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
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using SCOM_API.Models;
using System.Configuration;

namespace SCOM_API.Controllers
{
    public class SCOMObjectController : ApiController
    {
        ManagementGroup mg = null;
        public SCOMObjectController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            ManagementGroupConnectionSettings mgSettings = new ManagementGroupConnectionSettings(SCOMSERVER);

            mg = ManagementGroup.Connect(mgSettings);
        }

        /// <summary>
        /// Gets information about a monitoring object.
        /// </summary>
        /// <param name="id">Monitoring object GUID</param>
        [HttpGet]
        [Route("API/MonitoringObject/{id}")]
        public IHttpActionResult GetMonitoringObject(Guid id)
        {
            //get the monitoring object by Guid
            var monObject = mg.EntityObjects.GetObject<MonitoringObject>(id, ObjectQueryOptions.Default);

            List<SCOMMonitoringObjectModel> MonitoringObject = new List<SCOMMonitoringObjectModel>();

            //Add properties
            SCOMMonitoringObjectModel mObject = new SCOMMonitoringObjectModel();
            mObject.displayName = monObject.DisplayName;
            mObject.healthState = monObject.HealthState.ToString();
            mObject.stateLastModified = monObject.StateLastModified.ToString();
            mObject.inMaintenance = monObject.InMaintenanceMode;
            mObject.path = monObject.Path;

            MonitoringObject.Add(mObject);

            return Json(MonitoringObject);

        }

    }
}//END
