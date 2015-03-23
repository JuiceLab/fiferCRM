using AuthService.AuthorizeAttribute;
using ContentSearchService;
using EnumHelper;
using fifer_crm.Controllers;
using fifer_crm.Models;
using FilterModel;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TicketRepositories;

namespace fifer_crm.Areas.Task.Controllers
{
    [Authorize, CRMLogAttribute]
    public class MainTaskController : HomeController
    {
        // GET: Task/MainTask
        [DisplayName("Текущие задачи")]
        public ActionResult IndexTask()
        {
            var model = new TaskWrapViewModel((Guid)Membership.GetUser().ProviderUserKey, false);
            model.Menu = GetMenu("Активные задачи");
            ViewBag.Profile = model.UserPhoto;

            return View(model);
        }

        [DisplayName("Архив задач")]
        public ActionResult IndexCompletedTask()
        {
            var model = new TaskWrapViewModel((Guid)Membership.GetUser().ProviderUserKey, false, TaskStatus.Completed);
            model.Menu = GetMenu("Архив задач");
            ViewBag.Profile = model.UserPhoto;

            return View(model);
        }

        [HttpPost]
        [DisplayName("Форма поиска задач")]
        public ActionResult SearchResult(TaskSearchFilter searchFilter)
        {
            TaskSearch search = new TaskSearch(_userId);
            searchFilter = search.SearchTasks(searchFilter);
            return PartialView(searchFilter);
        }

        [HttpPost]
        [DisplayName("Поиск задач")]
        public ActionResult GetTasks(TaskSearchFilter searchFilter)
        {
            TaskSearch search = new TaskSearch(_userId);
            return Json(new { data = search.SearchJsonTasks(searchFilter) }, JsonRequestBehavior.AllowGet);
        }
    }
}