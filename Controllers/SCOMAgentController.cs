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
            var SCOMSERVER = ConfigurationManager.AppSettings["ScomSdkServer"];
            mg = ManagementGroup.Connect(SCOMSERVER);
        }

        /// <summary>
        /// Gets all agents from management group. Requires SCOM Administrator previlgies
        /// </summary>
        [Route("API/Agents")]
        public IList<AgentManagedComputer> GetAllScomAgents()
        {
            var Agents = mg.Administration.GetAllAgentManagedComputers();
            return Agents;
        }
    }
    } ///END
