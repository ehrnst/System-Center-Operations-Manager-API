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
        /// <param name="Id">Monitoring object GUID</param>
        /// <response code="400">Bad request check Id</response>
        /// <response code="404">Object not found</response>
        [HttpGet]
        [Route("API/MonitoringObject/{id}")]
        public IHttpActionResult GetMonitoringObject(Guid Id)
        {
            //Check if guid is not empty
            if (Id == Guid.Empty)
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            //get the monitoring object by Guid
            var monObject = mg.EntityObjects.GetObject<MonitoringObject>(Id, ObjectQueryOptions.Default);

            List<SCOMMonitoringObjectModel> MonitoringObject = new List<SCOMMonitoringObjectModel>();

            if (monObject !=null)
            {
                //Get related objects
                ReadOnlyCollection<PartialMonitoringObject> RelatedObjects = monObject.GetRelatedPartialMonitoringObjects();
                
                //Add properties
                SCOMMonitoringObjectModel mObject = new SCOMMonitoringObjectModel();
                mObject.id = monObject.Id;
                mObject.displayName = monObject.DisplayName;
                mObject.fullName = monObject.FullName;
                mObject.path = monObject.Path;
                mObject.healthState = monObject.HealthState.ToString();
                mObject.stateLastModified = monObject.StateLastModified.ToString();
                mObject.inMaintenance = monObject.InMaintenanceMode;

                //Populate a list of child objects
                List<SCOMObjectModelChild> SCOMMonitoringObjectChildObjects = new List<SCOMObjectModelChild>();
                foreach (PartialMonitoringObject RelatedObject in RelatedObjects)
                {
                    SCOMObjectModelChild ChildObject = new SCOMObjectModelChild();
                    ChildObject.id = RelatedObject.Id;
                    ChildObject.displayName = RelatedObject.DisplayName;
                    ChildObject.fullName = RelatedObject.FullName;
                    ChildObject.inMaintenance = RelatedObject.InMaintenanceMode;
                    ChildObject.path = RelatedObject.Path;
                    ChildObject.healthState = RelatedObject.HealthState.ToString();

                    SCOMMonitoringObjectChildObjects.Add(ChildObject);
                }
                //Add the list of all child objects to property of the monitoring object
                mObject.relatedObjects = SCOMMonitoringObjectChildObjects;

                MonitoringObject.Add(mObject);
                
                //Return object
                return Json(MonitoringObject);
            }

            //If no object found return error code
            else
            { 
           
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent("Monitoring object not found");
                throw new HttpResponseException(message);
            }
        }

    }
}//END
