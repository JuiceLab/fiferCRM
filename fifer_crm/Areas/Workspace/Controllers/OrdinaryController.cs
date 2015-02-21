using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketRepositories;

namespace fifer_crm.Areas.Workspace.Controllers
{
    [Authorize, CRMLogAttribute]
    public class OrdinaryController : BaseFiferController
    {
        [DisplayName("Страница сотрудника")]
        public ActionResult IndexEmployee()
        {
            DepartmentRepository repository = new DepartmentRepository();

            var model = new EmployeeWrapViewModel(_userId);
            model.Menu = GetMenu("Текущие задачи");
            return View(model);
        }

        [DisplayName("Расписание на дату")]
        public ActionResult EventsTimeLine(string date, int shift)
        {
            TaskTicketRepository repository = new TaskTicketRepository();
            IFormatProvider provider = new CultureInfo("ru-RU").DateTimeFormat;

            var ids = new List<Guid>() { _userId };
            CRMLocalRepository crmRepository = new CRMLocalRepository(_userId);
            var date4Timeline = string.IsNullOrEmpty(date)? DateTime.Now.Date.AddDays(shift) : Convert.ToDateTime(date, provider).AddDays(shift);
            var model = repository.SheduledEvents(ids, date4Timeline);
            crmRepository.UpdateSheduledEvents(model, ids);
            ViewBag.Date = date4Timeline.ToString("dd.MM.yyyy");
            return PartialView(model[date4Timeline]);
        }
    }
}