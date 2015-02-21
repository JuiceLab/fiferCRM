using System.Web.Mvc;

namespace fifer_crm.Areas.WebSite
{
    public class WebSiteAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WebSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WebSite_default",
                "WebSite/{controller}/{action}/{id}",
                new { controller = "SiteManage", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}