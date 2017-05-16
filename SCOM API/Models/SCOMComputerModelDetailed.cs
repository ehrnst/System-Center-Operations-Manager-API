using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMComputerModelDetailed
    {
        public string displayName { get; set; }
        public string healthState { get; set; }
        public bool inMaintenance { get; set; }

        public bool isAvailable { get; set; }
        public Guid id { get; set; }
        public int relatedObjectsCount { get; set; }

        public List<SCOMComputerModelChild> relatedObjects { get; set; }
    }

    public class SCOMComputerModelChild
    {
        public Guid id { get; set; }
        public string displayName { get; set; }

        public string fullName { get; set; }

        public bool inMaintenance { get; set; }

        public string path { get; set; }

        public string healthState { get; set; }
    }
}