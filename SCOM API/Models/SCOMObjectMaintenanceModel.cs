using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectMaintenanceModel
    {
        /// <summary>
        /// Monitoring object id
        /// </summary>
        public string id { get; set; }

        public string displayName { get; set; }
        /// <summary>
        /// Minutes to maintenance
        /// </summary>
        public int Minutes { get; set; }

        public DateTime EndTime { get; set; }
        /// <summary>
        /// Maintenance comment
        /// </summary>
        public string comment { get; set; }
    }
}