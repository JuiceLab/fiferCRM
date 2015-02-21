using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index(string viewName)
        {
            return Redirect("http://freedomdom.ru/" + viewName);
        }
    }
}