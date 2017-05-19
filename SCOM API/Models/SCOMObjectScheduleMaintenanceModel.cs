using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectSchedMaintenanceModel
    {
        public string scheduleName { get; set; }

        public string id { get; set; }
        
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string comment { get; set; }
    }
}