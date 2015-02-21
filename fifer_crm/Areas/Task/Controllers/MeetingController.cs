using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Task.Controllers
{
    [Authorize, CRMLogAttribute]
    public class MeetingController : BaseFiferController
    {
        public ActionResult Index()
        {
            CRMWrapViewModel model = new CRMWrapViewModel(_userId, true);
            model.Menu = GetMenu("Звонки");
            return View(model);
        }

        public ActionResult Edit(Guid? taskId, int? companyId = null, Guid? customerId = null)
        {
            StaffRepository staffRepository = new StaffRepository();
            var users = staffRepository.GetSubordinatedUsers(_userId);

            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            MeetingEditModel model = repository.GetMeetingEditModel(taskId, _userId, companyId);
            var customers = repository.GetCustomers4Subordinate(users.Select(m => Guid.Parse(m.Value)), companyId);
            ViewBag.Customers = (customerId.HasValue) ? customers.Where(m => m.Value == customerId.ToString()) : customers;
                
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Edit(MeetingEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.UpdateMeeting(model);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
    }
}