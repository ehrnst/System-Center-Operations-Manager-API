using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectSchedMaintenanceModel
    {
        /// <summary>
        /// Name of the maintenance schedule
        /// </summary>
        public string scheduleName { get; set; }
        /// <summary>
        /// Monitoring object ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Start time and date
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// End time and date
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string comment { get; set; }
    }
}