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
    public class SCOMMaintenanceController : ApiController
    {
        ManagementGroup mg = null;
        public SCOMMaintenanceController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            ManagementGroupConnectionSettings mgSettings = new ManagementGroupConnectionSettings(SCOMSERVER);

            mg = ManagementGroup.Connect(mgSettings);
        }

        /// <summary>
        /// Puts the specified computers and all hosted objects in to maintenance.
        /// </summary>
        /// <param name="Data">Json string with computername minutes and comment</param>
        /// <example>
        /// {
        ///     "DisplayName": "webserver.fqdn",
        ///     "Minutes": 10,
        ///     "comment": "doing maintenance"
        /// }
        /// </example>
        /// <response code="201">Successfully added maintenance mode for computer</response>
        /// <response code="400">Bad request. Check json input</response>
        /// <response code="409">Conflict computer already in maintenance</response>
        [HttpPost]
        [Route("API/ComputerMaintenance")]
        public IHttpActionResult EnableComputerMaintenance(SCOMComputerMaintenanceModel Data)
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Windows.Computer'");
            IList<ManagementPackClass> monClasses = mg.EntityTypes.GetClasses(classCriteria);
            MonitoringObjectCriteria criteria = new MonitoringObjectCriteria(string.Format("Name = '{0}'", Data.DisplayName.ToString()), monClasses[0]);
            List<MonitoringObject> monObjects = new List<MonitoringObject>();
            
            List<SCOMComputerMaintenanceModel> MaintenanceComputers = new List<SCOMComputerMaintenanceModel>();

            ///travers trough all classes to get monitoring objects
            foreach (ManagementPackClass monClass in monClasses)
            {
                monObjects.AddRange(mg.EntityObjects.GetObjectReader<MonitoringObject>(criteria, ObjectQueryOptions.Default));
            }


            foreach (MonitoringObject monObject in monObjects)
                if (!monObject.InMaintenanceMode)
                {
                    {
                        //set maintenance properties
                        DateTime startTime = DateTime.UtcNow;
                        DateTime schedEndTime = DateTime.UtcNow.AddMinutes(Data.Minutes);
                        MaintenanceModeReason reason = MaintenanceModeReason.PlannedOther;
                        string comment = Data.comment;

                        monObject.ScheduleMaintenanceMode(startTime, schedEndTime, reason, comment);

                        //Add properties to list
                        SCOMComputerMaintenanceModel maintenanceComputer = new SCOMComputerMaintenanceModel();
                        maintenanceComputer.DisplayName = monObject.DisplayName;
                        maintenanceComputer.EndTime = schedEndTime;
                        maintenanceComputer.Minutes = Data.Minutes;
                        maintenanceComputer.comment = comment;
                        
                        //add computers to list
                        MaintenanceComputers.Add(maintenanceComputer);

                    }
                }

            //If computer already in maintenance. Do nothing and list info
            else
                {
                    MaintenanceWindow MaintenanceWindow = monObject.GetMaintenanceWindow();

                    SCOMComputerMaintenanceModel maintenanceComputer = new SCOMComputerMaintenanceModel();
                    maintenanceComputer.DisplayName = monObject.DisplayName;
                    maintenanceComputer.EndTime = MaintenanceWindow.ScheduledEndTime;
                    maintenanceComputer.Minutes = Data.Minutes;
                    maintenanceComputer.comment = "Computer already in maintenance";

                    MaintenanceComputers.Add(maintenanceComputer);

                    throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.Conflict));

                }

            //Return list of computers as Json
            return Json(MaintenanceComputers);

        }

    }
}//END
