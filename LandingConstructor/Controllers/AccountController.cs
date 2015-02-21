using AccessModel;
using AccessRepositories;
using AuthService.FiferMembership;
using CompanyRepositories;
using ConstructorRepositories;
using SiteConstructor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SiteConstructor.Controllers
{
    public class AccountController : Controller
    {
        IFormsAuthenticationService FormsService { get; set; }
        IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // GET: Account
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //todo create temp page for many account 4 one email
            if (MembershipService.ValidateUser(model.Email, model.Password))
            {
                MembershipRepository membershipRepository = new MembershipRepository();
                var user = membershipRepository.Context.Users.FirstOrDefault(m => m.Login == model.Email || m.Mail == model.Email);
                CompanyRepository companyRepository = new CompanyRepository();
                var company = companyRepository.GetCompanyByUserId(user.UserId);
                var site = companyRepository.GetSiteProjectByUserId(user.UserId);
                if (site != null && site.C_CompanyId == company.CompanyId)
                {
                    FormsService.SignIn(model.Email, model.RememberMe);
                  var domain =  SiteSettingsWrapper.Instance.SiteIds.FirstOrDefault(m=>m.Value == site.SiteGuid).Key;
                  return Redirect("http://" + HttpContext.Request.Url.Authority + "/" + domain + "/" + "index.html");
                }
            }
            return RedirectToAction ("Login");
        }

        // GET: Account
        public ActionResult SignIn()
        {
            return PartialView(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult SignIn(LoginViewModel model, Guid siteId)
        {
            //todo create temp page for many account 4 one email
            if (MembershipService.ValidateUser(model.Email, model.Password))
            {
                MembershipRepository membershipRepository = new MembershipRepository();
                var user = membershipRepository.Context.Users.FirstOrDefault(m => m.Login == model.Email || m.Mail == model.Email);
                CompanyRepository companyRepository = new CompanyRepository();
                var company = companyRepository.GetCompanyByUserId(user.UserId);
                var site = companyRepository.GetSiteProject(siteId);
                if (site.C_CompanyId == company.CompanyId)
                    FormsService.SignIn(model.Email, model.RememberMe);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            FormsService.SignOut();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Refresh()
        {
            var item = SiteSettingsWrapper.ResetValues;
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

    }
}