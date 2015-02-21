using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Admin.Controllers
{
     [Authorize, CRMLogAttribute]
    public class CRMManagerController : BaseFiferController
    {
        // GET: Admin/CRMManager
        public ActionResult Index()
        {
            ManageControlWrapViewModel model = new ManageControlWrapViewModel(_userId);
            ViewBag.OnlyCRM = true;
            return View(model);
        }

        public ActionResult EditCategory(int? categoryId)
        {
            CRMLocalRepository crmRepository = new CRMLocalRepository(_userId);
            var model = crmRepository.GetCategory(categoryId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditCategory(SelectListItem item)
        {
            CRMLocalRepository crmRepository = new CRMLocalRepository(_userId);
            crmRepository.UpdateCategory(item);
            return RedirectToAction("Index");
        }
    }
}