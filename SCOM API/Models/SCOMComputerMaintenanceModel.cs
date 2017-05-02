using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMComputerMaintenanceModel
    {
        public string DisplayName { get; set; }

        public int Minutes { get; set; }

        public DateTime EndTime { get; set; }

        public string comment { get; set; }
    }
}