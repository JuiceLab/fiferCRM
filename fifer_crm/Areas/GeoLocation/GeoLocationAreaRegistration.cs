using System.Web.Mvc;

namespace fifer_crm.Areas.GeoLocation
{
    public class GeoLocationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GeoLocation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "GeoLocation_default",
                "GeoLocation/{controller}/{action}/{id}",
                new { controller = "Adress", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}