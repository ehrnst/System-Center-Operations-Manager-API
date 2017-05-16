using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMComputerModel
    {
        public string displayName { get; set; }
        public string healthState { get; set; }
        public bool inMaintenance { get; set; }
        public bool isAvailable { get; set; }
        public Guid id { get; set; }
    }
}