using System.Web.Mvc;

namespace fifer_crm.Areas.Finances
{
    public class FinancesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Finances";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Finances_default",
                "Finances/{controller}/{action}/{id}",
                new { cointroller="Payment", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}