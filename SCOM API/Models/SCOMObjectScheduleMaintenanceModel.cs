using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectSchedMaintenanceModel
    {
        /// <summary>
        /// Guid for the maintenance schedule
        /// </summary>
        /// remarks GUID will be created
        public Guid scheduleId { get; set; }

        /// <summary>
        /// Name of the maintenance schedule
        /// </summary>
        [Required]
        public string scheduleName { get; set; }

        /// <summary>
        /// Monitoring object ID(s)
        /// </summary>
        [Required]
        public string[] id { get; set; }

        /// <summary>
        /// Start time and date
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time and date
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [Required]
        public string comment { get; set; }
    }
}