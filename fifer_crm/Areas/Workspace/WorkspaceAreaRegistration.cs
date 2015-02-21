using System.Web.Mvc;

namespace fifer_crm.Areas.Workspace
{
    public class WorkspaceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Workspace";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Workspace_default",
                "Workspace/{controller}/{action}/{id}",
                new { controller="RoleRoute", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}