using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMClassObjectModel
    {
        public string displayName { get; set; }
        public string healthState { get; set; }
        public string path { get; set; }
        public Guid id { get; set; }
    }
}