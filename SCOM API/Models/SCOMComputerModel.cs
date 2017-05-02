using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMComputerModel
    {
        public string DisplayName { get; set; }
        public string HealthState { get; set; }
        public bool InMaintenance { get; set; }
        public Guid ID { get; set; }
    }
}