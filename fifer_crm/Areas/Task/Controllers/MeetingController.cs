using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Helpers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TicketModel;

namespace fifer_crm.Areas.Task.Controllers
{
    [Authorize, CRMLogAttribute]
    public class MeetingController : BaseFiferController
    {
        [DisplayName("Страница встреч")]
        public ActionResult Index()
        {
            CRMWrapViewModel model = new CRMWrapViewModel(_userId, true);
            model.Menu = GetMenu("Встречи");
            ViewBag.Profile = model.UserPhoto;
            return View(model);
        }

        [DisplayName("Новая встреча")]
        public ActionResult Edit(Guid? taskId, int? companyId = null, Guid? customerId = null, bool isNovelty=false)
        {
            StaffRepository staffRepository = new StaffRepository();
            var users = staffRepository.GetSubordinatedUsers(_userId);

            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            MeetingTicket model = new MeetingTicket(_userId)
            {
                DateStartedStr = DateTime.Now.Date.AddDays(1).ToString("dd.MM.yyyy"),
            };
            if (companyId.HasValue && !isNovelty)
            {
              taskId =  repository.GetLastCompanyMeeting(companyId.Value);
            }

            if(taskId.HasValue)
                model.Load(taskId.Value);
            var customers = repository.GetCustomers4Subordinate(users.Select(m => Guid.Parse(m.Value)), companyId);
            ViewBag.Customers = (customerId.HasValue) ? customers.Where(m => m.Value == customerId.ToString()) : customers;
            InitSubordinatedUsers();   
            return PartialView(model);
        }

        [DisplayName("Загрузка доступных действий для звонка")]
        public ActionResult Actions4MeetingTask(Guid taskId)
        {
            Actions4ProcessHelper actionPanel = new Actions4ProcessHelper();
            var model = actionPanel.ManagmentMeetingTask(taskId, (Guid)Membership.GetUser().ProviderUserKey);
            return PartialView(model);
        }
    }
}