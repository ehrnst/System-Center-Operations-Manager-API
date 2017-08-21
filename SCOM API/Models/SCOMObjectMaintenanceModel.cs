using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectMaintenanceModel
    {
        /// <summary>
        /// Monitoring object id
        /// </summary>
        [Required]
        public string id { get; set; }

        public string displayName { get; set; }
        /// <summary>
        /// Minutes to maintenance
        /// </summary>
        [Required]
        public int Minutes { get; set; }

        public DateTime EndTime { get; set; }
        /// <summary>
        /// Maintenance comment
        /// </summary>
        public string comment { get; set; }
    }
}