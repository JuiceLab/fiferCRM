using CompanyModel;
using CompanyRepositories;
using ConstructorRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using NotifyEventModel;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.WebSite.Controllers
{
    [Authorize, CRMLogAttribute]
    public class SiteManageController : BaseFiferController
    {
        [DisplayName("Страница сайтов компании")]
        // GET: WebSite/SiteManage
        public ActionResult Index()
        {
            WebSiteWrapModel model = new WebSiteWrapModel(_userId);
            model.Menu = GetMenu("Сайты компании");
            ViewBag.Profile = model.UserPhoto;
            return View(model);
        }

        [DisplayName("Проверка сущестования адреса")]
        public ActionResult IsExistUrl(string url)
        {
            CompanyRepository repository = new CompanyRepository();
            return Json(new { isExist = repository.IsExistSiteUrl(url) }, JsonRequestBehavior.AllowGet);
        }

        [DisplayName("Форма создания сайта")]
        public ActionResult Edit(int? siteId)
        {

            return PartialView(new SiteProjectModel());
        }

        [HttpPost]
        [DisplayName("Создание сайта")]
        public ActionResult Edit(SiteProjectModel model)
        {
            CompanyRepository repository = new CompanyRepository();
            var siteId = repository.UpdateWebSite(model, _userId);
            
            CompanyContext.WebSite site = repository.GetSiteProject(siteId);
            PasteBoardRepository pasteboardRepository = new PasteBoardRepository(site.DbSource);
            pasteboardRepository.CreateCompanyDB(siteId, site.DbSource, repository.GetCompanyCityByUserId(_userId));
            using (WebClient client = new WebClient())
            {
                client.DownloadString("http://site.bizinvit.ru/Account/Refresh");
            }
            return RedirectToAction("Index");
        }

    }
}