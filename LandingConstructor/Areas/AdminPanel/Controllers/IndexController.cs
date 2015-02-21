using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteConstructor.Areas.AdminPanel.Controllers
{
    [Authorize]
    public class IndexController : Controller
    {
        //
        // GET: /AdminPanel/Index/
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult UserMenu()
        {
            return PartialView();
        }
	}
}