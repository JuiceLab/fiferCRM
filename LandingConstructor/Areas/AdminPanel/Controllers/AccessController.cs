using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteConstructor.Areas.AdminPanel.Controllers
{
    public class AccessController : Controller
    {
        [HttpGet]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string pass, string login, bool remember)
        {
            if (login == "admin" && pass == "admin001")
            {
                FormsAuthenticationTicket Ticket = new FormsAuthenticationTicket(1,
                  login.ToLower(),
                  System.DateTime.Now,
                  System.DateTime.Now.AddMonths(1),
                  false,
                  login,
                  FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(Ticket);
                HttpCookie HTTPCook = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(HTTPCook);
                FormsAuthentication.SetAuthCookie(login, remember);
            }

            return RedirectToAction("HomePage", "Index", new { Area = "AdminPanel" });
        }
	}
}