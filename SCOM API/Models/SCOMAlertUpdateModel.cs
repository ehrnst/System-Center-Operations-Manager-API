using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCOM_API.Models
{
    public class SCOMAlertUpdateModel
    {
        /// <summary>
        /// Alert resolution state
        /// </summary>
        public string resolutionState { get; set; }

        /// <summary>
        /// Connected ticket id
        /// </summary>
        public string ticketId { get; set; }

        /// <summary>
        /// TfsWorkItemId
        /// </summary>
        public string tfsWorkItemId { get; set; }

        /// <summary>
        /// TfsWorkItemOwner
        /// </summary>
        public string tfsWorkItemOwner { get; set; }

        /// <summary>
        /// Alert owner
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// Alert custom field 1
        /// </summary>
        public string customField1 { get; set; }

        /// <summary>
        /// Alert custom field 2
        /// </summary>
        public string customField2 { get; set; }

        /// <summary>
        /// Alert custom field 3
        /// </summary>
        public string customField3 { get; set; }

        /// <summary>
        /// Alert custom field 4
        /// </summary>
        public string customField4 { get; set; }

        /// <summary>
        /// Alert custom field 5
        /// </summary>
        public string customField5 { get; set; }

        /// <summary>
        /// Alert custom field 6
        /// </summary>
        public string customField6 { get; set; }

        /// <summary>
        /// Alert custom field 7
        /// </summary>
        public string customField7 { get; set; }

        /// <summary>
        /// Alert custom field 8
        /// </summary>
        public string customField8 { get; set; }

        /// <summary>
        /// Alert custom field 9
        /// </summary>
        public string customField9 { get; set; }

        /// <summary>
        /// Alert custom field 10
        /// </summary>
        public string customField10 { get; set; }
    }
}