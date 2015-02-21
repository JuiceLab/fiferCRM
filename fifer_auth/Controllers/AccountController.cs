using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using fifer_auth.Models;
using System.Web.Security;
using AccessRepositories;
using AuthService.FiferMembership;
using System.Web.Routing;

namespace fifer_auth.Controllers
{
    public class AccountController : Controller
    {
        MembershipRepository _repository = new MembershipRepository();

        IFormsAuthenticationService FormsService { get; set; }
        IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        [HttpGet]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string pass, string login, bool remember)
        {
            if (MembershipService.ValidateUser(login, pass))
            {
                FormsService.SignIn(login, remember);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("LogOn");
        }
    }
}