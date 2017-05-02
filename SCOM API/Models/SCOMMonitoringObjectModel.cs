using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMMonitoringObjectModel
    {
        public string displayName { get; set; }
        public string healthState { get; set; }
        public bool inMaintenance { get; set; }
        public string stateLastModified { get; set; }

        public string classes { get; set; }
        public string path { get; set; }
    }
}