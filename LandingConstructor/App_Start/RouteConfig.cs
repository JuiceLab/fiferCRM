using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SiteConstructor
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico");

            routes.MapRoute(
                name: "SiteIndexAnchored",
                url: "{site}/{anchor}.html",
                defaults: new { controller = "Default", action = "DefaultPage", Area = "PasteBoard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "SiteIndexAnchoredDefault",
              url: "{site}/",
              defaults: new { controller = "Default", action = "DefaultPage", Area = "PasteBoard", anchor="index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
                 name: "Default",
                 url: "{controller}/{action}",
                 defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
             );
        }
    }
}
