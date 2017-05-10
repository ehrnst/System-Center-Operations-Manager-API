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
    [RoutePrefix("API/Computer")]
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

        [Route("Windows")]
        [HttpGet]
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
        /// <response code="404">ComputerName empty or computer cannot be found</response>
        /// <response code="400">Bad request</response>
        [Route("Windows/{ComputerName}")]
        [HttpGet]
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

                //Check if computers are in list
                if (windowsComputerObjects.Count > 0)
                {


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

                    //Successful return
                    return Json(SCOMComputers);
                }

                //If computer cannot be found
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                    message.Content = new StringContent("Cannot find Computer. Please see input");
                    throw new HttpResponseException(message);
                }
            }

        }

        /// <summary>
        ///Gets all Linux Computers.
        /// </summary>

        [Route("Linux")]
        [HttpGet]
        public IHttpActionResult GetLinuxComputers()
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Linux.Computer'");
            IList<ManagementPackClass> monitoringClasses = mg.EntityTypes.GetClasses(classCriteria);


            List<PartialMonitoringObject> linuxComputerObjects = new List<PartialMonitoringObject>();

            IObjectReader<PartialMonitoringObject> reader = mg.EntityObjects.GetObjectReader<PartialMonitoringObject>(monitoringClasses[0], ObjectQueryOptions.Default);

            linuxComputerObjects.AddRange(reader);

            List<SCOMComputerModel> SCOMLinuxComputers = new List<SCOMComputerModel>();

            foreach (PartialMonitoringObject linuxComputerObject in linuxComputerObjects)
            {
                SCOMComputerModel SCOMComputer = new SCOMComputerModel();
                SCOMComputer.DisplayName = linuxComputerObject.DisplayName;
                SCOMComputer.HealthState = linuxComputerObject.HealthState.ToString();
                SCOMComputer.InMaintenance = linuxComputerObject.InMaintenanceMode;

                SCOMComputer.ID = linuxComputerObject.Id;

                SCOMLinuxComputers.Add(SCOMComputer);
            }

            return Json(SCOMLinuxComputers);

        }

        /// <summary>
        /// Get computer object from computername.
        /// </summary>
        /// <response code="404">ComputerName empty or computer cannot be found</response>
        /// <response code="400">Bad request</response>
        [Route("Linux/{ComputerName}")]
        [HttpGet]
        public IHttpActionResult GetLinuxComputersByName(string ComputerName)
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Linux.Computer'");
            IList<ManagementPackClass> monitoringClasses = mg.EntityTypes.GetClasses(classCriteria);
            if (string.IsNullOrEmpty(ComputerName))
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }
            else
            {
                MonitoringObjectCriteria criteria = new MonitoringObjectCriteria(string.Format("Name = '{0}'", ComputerName), monitoringClasses[0]);


                List<PartialMonitoringObject> linuxComputerObjects = new List<PartialMonitoringObject>();

                IObjectReader<PartialMonitoringObject> reader = mg.EntityObjects.GetObjectReader<PartialMonitoringObject>(criteria, ObjectQueryOptions.Default);

                linuxComputerObjects.AddRange(reader);

                //Check if computers are in list
                if (linuxComputerObjects.Count > 0)
                {


                    List<SCOMComputerModel> SCOMLinuxComputers = new List<SCOMComputerModel>();

                    foreach (PartialMonitoringObject linuxComputerObject in linuxComputerObjects)
                    {
                        SCOMComputerModel SCOMComputer = new SCOMComputerModel();
                        SCOMComputer.ID = linuxComputerObject.Id;
                        SCOMComputer.DisplayName = linuxComputerObject.DisplayName;
                        SCOMComputer.HealthState = linuxComputerObject.HealthState.ToString();
                        SCOMComputer.InMaintenance = linuxComputerObject.InMaintenanceMode;



                        SCOMLinuxComputers.Add(SCOMComputer);
                    }

                    //Successful return
                    return Json(SCOMLinuxComputers);
                }

                //If computer cannot be found
                else
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                    message.Content = new StringContent("Cannot find Computer. Please see input");
                    throw new HttpResponseException(message);
                }
            }

        }

    }
}//END
