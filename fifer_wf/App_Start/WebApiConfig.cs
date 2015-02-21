using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace fifer_wf
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "Support",
               routeTemplate: "api/support/{action}",
               defaults: new { controller = "SupportTicket"}
           );

            config.Routes.MapHttpRoute(
              name: "Task",
              routeTemplate: "api/task/{action}",
              defaults: new { controller = "TaskTicket" }
          );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
