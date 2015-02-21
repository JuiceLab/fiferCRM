using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Workspace.Controllers
{
    [Authorize, CRMLogAttribute]
    public class RoleRouteController : Controller
    {
        // GET: Workspace/RoleRoute
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Finance", new { Ares="Workspace"});
        }
    }
}