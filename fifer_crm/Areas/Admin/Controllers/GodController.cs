using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace fifer_crm.Areas.Admin.Controllers
{
    [AuthorizeFuncRole(Profile = "Бог системы"), CRMLogAttribute]
    public class GodController : BaseFiferController
    {
        // GET: God
        [DisplayName("Страница верховного руководства")]
        public ActionResult Index()
        {
            FullControlWrapViewModel model = new FullControlWrapViewModel(_userId);
            ViewBag.OnlyCRM = false;
            return View(model);
        }

        public ActionResult CRMTable(bool onlyCRM = false)
        {
            FullControlWrapViewModel model = new FullControlWrapViewModel(_userId, onlyCRM);
            ViewBag.OnlyCRM = onlyCRM;

            return PartialView("CompaniesTable", model.Companies);
        }

        public ActionResult EditService(int? serviceId)
        {
            CRMRepository crmRepository = new CRMRepository();
            var model = crmRepository.GetService(serviceId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditService(ServiceEditModel service)
        {
            CRMRepository crmRepository = new CRMRepository();
            crmRepository.UpdateService(service);
            return RedirectToAction("Index");
        }

        public ActionResult EditCity(int? cityId)
        {
            CRMRepository repository = new CRMRepository();
            var model = repository.GetCity(cityId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditCity(CityPreview city)
        {
            CRMRepository repository = new CRMRepository();
            repository.UpdateCity(city);
            return RedirectToAction("Index");
        }
    }
}