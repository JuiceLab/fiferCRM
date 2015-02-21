using System.Web.Mvc;

namespace fifer_crm.Areas.ERP
{
    public class ERPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ERP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ERP_default",
                "ERP/{controller}/{action}/{id}",
                new { controller="Company", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}