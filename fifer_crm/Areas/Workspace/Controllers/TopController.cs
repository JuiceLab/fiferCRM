using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Workspace.Controllers
{
    [Authorize, CRMLogAttribute]
    public class TopController : Controller
    {
        // GET: Workspace/Top
        public ActionResult Index()
        {
            return View();
        }
    }
}