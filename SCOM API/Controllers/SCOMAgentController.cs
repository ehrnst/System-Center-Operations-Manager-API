using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Administration;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Web;
using Microsoft.EnterpriseManagement.Configuration;
using System.Configuration;

namespace SCOM_API.Controllers
{
    public class SCOMAgentController : ApiController
    {
        ManagementGroup mg = null;
        public SCOMAgentController()
        {
            System.Security.Principal.WindowsImpersonationContext impersonationContext;
            impersonationContext =
                ((System.Security.Principal.WindowsIdentity)User.Identity).Impersonate();
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            ManagementGroupConnectionSettings mgSettings = new ManagementGroupConnectionSettings(SCOMSERVER);

            mg = ManagementGroup.Connect(mgSettings);
        }

        /// <summary>
        /// Gets all agents from management group. Requires SCOM Administrator previlgies
        /// </summary>
        [Route("API/Agents")]
        public IList<PartialAgentManagedComputer> GetScomAgents()
        {
            var Agents = mg.Administration.GetPartialAgentManagedComputers();

            return Agents;
        }


        /// <summary>
        /// Gets agent managed computer. 
        /// Requires SCOM Administrator previlgies
        /// </summary>
        [Route("API/Agents/{Id:Guid}")]
        public PartialAgentManagedComputer GetScomAgents(Guid Id)
        {
            var Agent = mg.Administration.GetPartialAgentManagedComputer(Id);
            return Agent;
        }
    }
} //END
