using System.Web.Mvc;

namespace SiteConstructor.Areas.PasteBoard
{
    public class PasteBoardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PasteBoard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PasteBoard_default",
                "PasteBoard/{controller}/{action}/{id}",
                new {controller="Default", action = "DefaultPage", id = UrlParameter.Optional }
            );
        }
    }
}