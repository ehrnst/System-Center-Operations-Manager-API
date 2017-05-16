using Microsoft.EnterpriseManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMMonitoringObjectModel
    {
        public Guid id { get; set; }
        public string displayName { get; set; }
        public string fullName { get; set; }
        public string healthState { get; set; }
        public bool inMaintenance { get; set; }
        public string stateLastModified { get; set; }
        public string path { get; set; }

        public List<SCOMObjectModelChild> relatedObjects { get; set; }
    }

    public class SCOMObjectModelChild
    {
        public Guid id { get; set; }
        public string displayName { get; set; }

        public string fullName { get; set; }

        public bool inMaintenance { get; set; }

        public string path { get; set; }

        public string healthState { get; set; }
    }
}