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
    public class SCOMComputerController : ApiController
    {
        ManagementGroup mg = null;
        public SCOMComputerController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            ManagementGroupConnectionSettings mgSettings = new ManagementGroupConnectionSettings(SCOMSERVER);

            mg = ManagementGroup.Connect(mgSettings);
        }

        /// <summary>
        ///Gets all windows computers.
        /// </summary>
        [Route("API/WindowsComputers")]
        public IHttpActionResult GetComputerPartialMonitoringObject()
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Windows.Computer'");
            IList<ManagementPackClass> monitoringClasses = mg.EntityTypes.GetClasses(classCriteria);


            List<PartialMonitoringObject> windowsComputerObjects = new List<PartialMonitoringObject>();

            IObjectReader<PartialMonitoringObject> reader = mg.EntityObjects.GetObjectReader<PartialMonitoringObject>(monitoringClasses[0], ObjectQueryOptions.Default);

            windowsComputerObjects.AddRange(reader);

            List<SCOMComputerModel> SCOMComputers = new List<SCOMComputerModel>();

            foreach (PartialMonitoringObject windowsComputerObject in windowsComputerObjects)
            {
                SCOMComputerModel SCOMComputer = new SCOMComputerModel();
                SCOMComputer.DisplayName = windowsComputerObject.DisplayName;
                SCOMComputer.HealthState = windowsComputerObject.HealthState.ToString();
                SCOMComputer.InMaintenance = windowsComputerObject.InMaintenanceMode;

                SCOMComputer.ID = windowsComputerObject.Id;

                SCOMComputers.Add(SCOMComputer);
            }

            return Json(SCOMComputers);

        }

        /// <summary>
        /// Get computer object from computername.
        /// </summary>
        [Route("API/WindowsComputers")]
        public IHttpActionResult GetComputerPartialMonitoringObjectByName(string ComputerName)
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Windows.Computer'");
            IList<ManagementPackClass> monitoringClasses = mg.EntityTypes.GetClasses(classCriteria);
            if (string.IsNullOrEmpty(ComputerName))
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }
            else
            {
                MonitoringObjectCriteria criteria = new MonitoringObjectCriteria(string.Format("Name = '{0}'", ComputerName), monitoringClasses[0]);


                List<PartialMonitoringObject> windowsComputerObjects = new List<PartialMonitoringObject>();

                IObjectReader<PartialMonitoringObject> reader = mg.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);

                windowsComputerObjects.AddRange(reader);

                List<SCOMComputerModel> SCOMComputers = new List<SCOMComputerModel>();

                foreach (PartialMonitoringObject windowsComputerObject in windowsComputerObjects)
                {
                    SCOMComputerModel SCOMComputer = new SCOMComputerModel();
                    SCOMComputer.ID = windowsComputerObject.Id;
                    SCOMComputer.DisplayName = windowsComputerObject.DisplayName;
                    SCOMComputer.HealthState = windowsComputerObject.HealthState.ToString();
                    SCOMComputer.InMaintenance = windowsComputerObject.InMaintenanceMode;
                    
                    

                    SCOMComputers.Add(SCOMComputer);
                }

                return Json(SCOMComputers);
            }
        }
    }
}///END
