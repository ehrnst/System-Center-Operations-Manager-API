using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMObjectMaintenanceModel
    {
        public string id { get; set; }

        public string displayName { get; set; }

        public int Minutes { get; set; }

        public DateTime EndTime { get; set; }

        public string comment { get; set; }
    }
}