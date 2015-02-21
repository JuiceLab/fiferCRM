using System.Web.Mvc;

namespace fifer_crm.Areas.Task
{
    public class TaskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Task";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Task_default",
                "Task/{controller}/{action}/{id}",
                new { controller = "MainTask", action = "IndexTask", id = UrlParameter.Optional }
            );
        }
    }
}