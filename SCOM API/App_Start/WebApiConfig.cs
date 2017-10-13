using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SCOM_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API routes
            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "System Center Operations Manager API",
                routeTemplate: "API/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );


            // Web API configuration and services
            config.Filters.Add(new AuthorizeAttribute());
            
        }
    }
}
