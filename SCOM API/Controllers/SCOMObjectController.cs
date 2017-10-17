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
using System.Web.Http.Description;

namespace SCOM_API.Controllers
{
    [RoutePrefix("API/MonitoringObject")]
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
        [Route("{id}")]
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

            if (monObject != null)
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

        /// <summary>
        ///Gets all monitoring objects of specified class
        ///Limited properties returned. Use MonitoringObject/Id endpoint for more details
        /// </summary>
        ///<param name="classId">Your class guid</param>
        /// <response code="200">OK: returns member of class</response>
        /// <response code="204">class found but no objects / no members exist</response>
        /// <response code="400">Bad request check classId</response>
        /// <response code="404">No such class</response>

        [Route("class/{classId}")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SCOMClassObjectModel>))]
        public IHttpActionResult GetClassPartialMonitoringObject(Guid classId)
        {

            if (classId == Guid.Empty)
            {
                throw new HttpResponseException(Request
                .CreateResponse(HttpStatusCode.BadRequest));
            }

            //Use the input class id to create a criteria for our query
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria(string.Format("Id = '{0}'", classId.ToString()));
            IList<ManagementPackClass> monitoringClasses = mg.EntityTypes.GetClasses(classCriteria);


            List<PartialMonitoringObject> inputClassObjects = new List<PartialMonitoringObject>();

            IObjectReader<PartialMonitoringObject> reader = mg.EntityObjects.GetObjectReader<PartialMonitoringObject>(monitoringClasses[0], ObjectQueryOptions.Default);

            inputClassObjects.AddRange(reader);

            List<SCOMClassObjectModel> classObjects = new List<SCOMClassObjectModel>();

            //If objects are found add them to list and return
            if (inputClassObjects.Count > 0)
            {

                foreach (PartialMonitoringObject classObject in inputClassObjects)
                {
                    SCOMClassObjectModel inputClassObject = new SCOMClassObjectModel();
                    inputClassObject.id = classObject.Id;
                    inputClassObject.displayName = classObject.DisplayName;
                    inputClassObject.healthState = classObject.HealthState.ToString();
                    inputClassObject.path = classObject.Path;

                    classObjects.Add(inputClassObject);
                }

                return Json(classObjects);
            }

            //if class does not have any monitoring objects return 'no content'
            else
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NoContent);
                message.Content = new StringContent("Class found but no object exist");
                throw new HttpResponseException(message);
            }

        }
    }

}
//END
