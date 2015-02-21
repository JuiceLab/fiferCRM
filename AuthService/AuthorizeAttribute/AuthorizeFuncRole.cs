using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UserContext;

namespace AuthService.AuthorizeAttribute
{
    public class AuthorizeFuncRole : System.Web.Mvc.AuthorizeAttribute
    {
        // Custom property
        public string Profile { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            string privilegeLevels = string.Join("", GetUserFuncRole(httpContext.User.Identity.Name.ToString()));

            if (privilegeLevels.Contains(this.Profile))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private IEnumerable<string> GetUserFuncRole(string login)
        {
            List<string> roles = new List<string>();
            using (UserEntities accessContext = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                roles = accessContext.Users
                    .FirstOrDefault(m => m.Login == login)
                    .UserFunctionalRoles
                    .Select(m => m.FunctionalRole.Name)
                    .ToList();
            }
            return roles;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }

    }
}
