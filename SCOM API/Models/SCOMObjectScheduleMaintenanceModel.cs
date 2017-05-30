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
        /// Name of the maintenance schedule
        /// </summary>
        [Required]
        public string scheduleName { get; set; }
        [Required]
        /// <summary>
        /// Monitoring object ID(s)
        /// </summary>
        public string[] id { get; set; }
        [Required]
        /// <summary>
        /// Start time and date
        /// </summary>
        public DateTime StartTime { get; set; }
        [Required]
        /// <summary>
        /// End time and date
        /// </summary>
        public DateTime EndTime { get; set; }
        [Required]
        /// <summary>
        /// Comment
        /// </summary>
        public string comment { get; set; }
    }
}