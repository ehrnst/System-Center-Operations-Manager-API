using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Web;
using Microsoft.SystemCenter.OperationsManagerV10.Commands;
using Microsoft.EnterpriseManagement.Configuration;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using SCOM_API.Models;
using System.Configuration;
using System.Web.Http.Description;
using Microsoft.EnterpriseManagement.Monitoring.MaintenanceSchedule;
using Swashbuckle.Swagger.Annotations;

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
        [ResponseType(typeof(IEnumerable<SCOMComputerMaintenanceModel>))]
        [Route("API/ComputerMaintenance")]
        public IHttpActionResult EnableComputerMaintenance(SCOMComputerMaintenanceModel Data)
        {
            ManagementPackClassCriteria classCriteria = new ManagementPackClassCriteria("Name = 'Microsoft.Windows.Computer'");
            IList<ManagementPackClass> monClasses = mg.EntityTypes.GetClasses(classCriteria);
            MonitoringObjectCriteria criteria = new MonitoringObjectCriteria(string.Format("Name = '{0}'", Data.DisplayName.ToString()), monClasses[0]);
            List<MonitoringObject> monObjects = new List<MonitoringObject>();

            List<SCOMComputerMaintenanceModel> MaintenanceComputers = new List<SCOMComputerMaintenanceModel>();

            //travers trough all classes to get monitoring objects
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

                //If computer already in maintenance. Throw conflict message
                else
                {
                    MaintenanceWindow MaintenanceWindow = monObject.GetMaintenanceWindow();

                    SCOMComputerMaintenanceModel maintenanceComputer = new SCOMComputerMaintenanceModel();

                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                    message.Content = new StringContent("Computer already in maintenance " + MaintenanceWindow.ScheduledEndTime);
                    throw new HttpResponseException(message);

                }

            //Return list of computers as Json
            return Json(MaintenanceComputers);

        }



        /// <summary>
        /// Puts the specified monitoring object in maintenance mode.
        /// </summary>
        /// <param name="Data">Json string with Id, # of minutes and a comment</param>
        /// <example>
        /// {
        ///     "id": "Guid",
        ///     "Minutes": 10,
        ///     "comment": "doing maintenance"
        /// }
        /// </example>
        /// <response code="201">Successfully added maintenance mode for the object</response>
        /// <response code="400">Bad request. Check json input</response>
        /// <response code="409">Conflict: object already in maintenance</response>
        [HttpPost]
        [ResponseType(typeof(IEnumerable<SCOMObjectMaintenanceModel>))]
        [Route("API/ObjectMaintenance")]
        public IHttpActionResult EnableObjectMaintenance(SCOMObjectMaintenanceModel Data)
        {
            //Validate input
            if (!ModelState.IsValid)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.BadRequest);
                message.Content = new StringContent("Missing a required parameter?");
                throw new HttpResponseException(message);
            }

            //create a Guid from the json input
            var ObjectId = new Guid(Data.id);
            //get the monitoring object by Guid
            var monObject = mg.EntityObjects.GetObject<MonitoringObject>(ObjectId, ObjectQueryOptions.Default);

            List<SCOMMonitoringObjectModel> MonitoringObjects = new List<SCOMMonitoringObjectModel>();

            List<SCOMObjectMaintenanceModel> MaintenanceObjects = new List<SCOMObjectMaintenanceModel>();

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
                    SCOMObjectMaintenanceModel maintenanceObject = new SCOMObjectMaintenanceModel();
                    maintenanceObject.displayName = monObject.DisplayName;
                    maintenanceObject.id = monObject.Id.ToString();
                    maintenanceObject.EndTime = schedEndTime;
                    maintenanceObject.Minutes = Data.Minutes;
                    maintenanceObject.comment = comment;

                    //add computers to list
                    MaintenanceObjects.Add(maintenanceObject);

                }
            }

            //If object already in maintenance. Throw conflict message
            else
            {
                MaintenanceWindow MaintenanceWindow = monObject.GetMaintenanceWindow();

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Conflict);
                message.Content = new StringContent("Object already in maintenance until" + MaintenanceWindow.ScheduledEndTime);
                throw new HttpResponseException(message);

            }

            //Return list of computers as Json
            return Json(MaintenanceObjects);

        }

        /// <summary>
        /// Updates or ends existing maintenance mode for object.
        /// </summary>
        /// <param name="Data">Json string with object id and new endTime</param>
        /// <param name="EndNow">If true, maintenance will end</param>
        /// <example>
        /// {
        ///     "id": "Guid",
        ///     "EndTime": "2017-07-07T19:00:00.000Z"
        /// }
        /// </example>
        /// <response code="200">Successfully updated maintenance mode</response>
        /// <response code="400">Bad request. Check json input</response>
        /// <response code="304">Object not in maintenance. Nothing to update</response>

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "Object maintenance mode updated")]
        [ResponseType(typeof(HttpResponseMessage))]
        [Route("API/ObjectMaintenance")]
        public IHttpActionResult UpdateObjectMaintenance(SCOMUpdateObjectMaintenanceModel Data, bool EndNow = false)
        {
            //Validate input
            if (ModelState.IsValid)
            {
                //create a Guid from the json input
                var ObjectId = new Guid(Data.id);
                //get the monitoring object by Guid
                var monObject = mg.EntityObjects.GetObject<MonitoringObject>(ObjectId, ObjectQueryOptions.Default);

                //If object not in maintenance not modified
                if (!monObject.InMaintenanceMode)
                {
                    {
                        HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.NotModified);
                        res.Content = new StringContent("Specified object not in maintenance mode. Nothing to update...");
                        throw new HttpResponseException(res);
                    }
                }

                //If object in maintenanance update
                else
                {
                    //If endNow parameter validate true. End maintenance mode
                    if (EndNow.Equals(true))
                    {
                        monObject.StopMaintenanceMode(DateTime.UtcNow, TraversalDepth.Recursive);

                    }

                    // Get the maintenance window
                    MaintenanceWindow MaintenanceWindow = monObject.GetMaintenanceWindow();

                    //If user specifies an end date
                    if (Data.EndTime > DateTime.MinValue)
                    {

                        //Compare specified end time with current maintenance end time
                        int TimeCompare = DateTime.Compare(Data.EndTime, MaintenanceWindow.ScheduledEndTime);
                        //Update end time but use same reason and comment
                        monObject.UpdateMaintenanceMode(Data.EndTime, MaintenanceWindow.Reason, MaintenanceWindow.Comments);
                    }

                }
                // creating OK response
                return Ok(new { message = "Updated maintenance mode", monitoringObjectId = Data.id });

            }

            // throw error message
            else
            {
                HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.BadRequest);
                res.Content = new StringContent("Please check request body");
                throw new HttpResponseException(res);
            }
        }


        /// <summary>
        /// Creates a new maintenance schedule with the specified monitoring objects.
        /// </summary>
        /// <param name="Data">scheduleName, object ids, StartTime, EndTime, comment are mandatory</param>
        /// <example>
        /// {
        ///     "scheduleName": "string",
        ///     "id": "[monitoringObjectId's]",
        ///     "StartTime": "2017-05-22T07:01:00.374Z",
        ///     "EndTime": "2017-05-22T08:01:00.374Z",
        ///     "comment": "doing maintenance"
        /// }
        /// </example>
        /// <remarks>
        /// ##SCOM 2016 Only##
        /// Only non recurring schedules are supported. Maintenance reason will be hard coded to 'PlannedOther'
        /// Use the other endpoints to obtain your MonitoringObjectId
        /// </remarks>
        /// <response code="201">Successfully added maintenance mode for the object</response>
        /// <response code="400">Bad request. Check json input</response>

        [HttpPost]
        [ResponseType(typeof(IEnumerable<SCOMObjectSchedMaintenanceModel>))]
        [Route("API/MaintenanceSchedule")]
        public IHttpActionResult ScheduleObjectMaintenance(SCOMObjectSchedMaintenanceModel Data)
        {
            //Validate post
            if (!ModelState.IsValid)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.BadRequest);
                message.Content = new StringContent("Missing a required parameter?");
                throw new HttpResponseException(message);
            }

            //LOOP through the id array and add each GUID to the object list
            System.Collections.Generic.List<System.Guid> ObjectList = new System.Collections.Generic.List<System.Guid>();
            string[] array = Data.id;
            foreach (string s in array)
            {
                var item = new Guid(s);

                ObjectList.Add(item);
            }

            //create a recurrencePattern this is 'sourced' from OmCommands.10.dll ( new-scommaintenanceSchedule CMDLET )
            //read more: https://docs.microsoft.com/en-us/powershell/systemcenter/systemcenter2016/operationsmanager/vlatest/new-scommaintenanceschedule
            OnceRecurrence recurrencePattern;
            recurrencePattern = new OnceRecurrence(1, 1, 0, 0, 0, 0);


            //Getting data from Json post
            string comments = Data.comment;
            bool isRecurrence = false;
            bool isEnabled = true;
            bool recursive = true;
            string displayname = Data.scheduleName;
            //Create a timspan and return duration
            DateTime activeStartTime = Data.StartTime;
            DateTime activeEndDate = Data.EndTime;
            TimeSpan span = activeEndDate.Subtract(activeStartTime);
            int duration = (int)span.TotalMinutes;

            //Create the Maintenance schedule
            MaintenanceSchedule Sched = new MaintenanceSchedule(mg, displayname, recursive, isEnabled, ObjectList, duration, activeStartTime, activeEndDate, MaintenanceModeReason.PlannedOther, comments, isRecurrence, recurrencePattern);

            //Create the maintenance schedule
            System.Guid guid = MaintenanceSchedule.CreateMaintenanceSchedule(Sched, mg);

            //Add properties to class
            var shed = MaintenanceSchedule.GetMaintenanceScheduleById(guid, mg);
            List<SCOMObjectSchedMaintenanceModel> MaintenanceScheduleList = new List<SCOMObjectSchedMaintenanceModel>();
            SCOMObjectSchedMaintenanceModel mSched = new SCOMObjectSchedMaintenanceModel();
            mSched.scheduleId = guid;
            mSched.scheduleName = shed.ScheduleName;
            mSched.id = array;
            mSched.StartTime = shed.ActiveStartTime;
            mSched.EndTime = shed.ScheduledEndTime;
            mSched.comment = shed.Comments;

            MaintenanceScheduleList.Add(mSched);
            //return the post/class as Json
            return Json(MaintenanceScheduleList);


        }


    }
}//END
