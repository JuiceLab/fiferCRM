using AuthService.AuthorizeAttribute;
using LoaderService.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace fifer_auth.Controllers
{
    [AuthorizeFuncRole(Profile="Бог системы")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexRefreshCRMCompanies()
        {
            DemoDataLoader loader = new DemoDataLoader((Guid)Membership.GetUser().ProviderUserKey);
            var existingFile = new FileInfo(Server.MapPath("~/demo_deploy/") + "1.xlsx");
            loader.RefreshCRMCompanies(existingFile.OpenRead(), true);

            return Content("Ok");
        }

        public ActionResult IndexUsers()
        {
            return View();
        }

        public ActionResult IndexFunctionalRoles()
        {
            return View();
        }

        public ActionResult IndexRoles()
        {
            return View();
        }

        public ActionResult IndexRules()
        {
            return View();
        }

        public ActionResult UserMenu()
        {
            return PartialView();
        }
    }
}