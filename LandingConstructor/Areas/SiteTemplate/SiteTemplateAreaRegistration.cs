using System.Web.Mvc;

namespace SiteConstructor.Areas.SiteTemplate
{
    public class SiteTemplateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SiteTemplate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SiteTemplate_default",
                "SiteTemplate/{controller}/{action}/{id}",
                new {controller ="TemplateCss", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}