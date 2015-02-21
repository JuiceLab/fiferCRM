﻿using CompanyRepositories;
using CRMRepositories;
using EnumHelper;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketModel;
using TicketRepositories;

namespace fifer_crm.Areas.Task.Controllers
{
    [Authorize, CRMLogAttribute]
    public class CallController : BaseFiferController
    {
        public ActionResult Index()
        {
            CRMWrapViewModel model = new CRMWrapViewModel(_userId, true);
            model.Menu = GetMenu("Звонки");
            return View(model);
        }

        public ActionResult Edit(Guid? taskId, string taskNumber, int? companyId = null, Guid? customerId=null)
        {
            CallTicket ticket = new CallTicket()
            {
                TicketStatus = (byte)TicketStatus.Novelty,
                DateStartedStr  = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")
            };

            
            StaffRepository staffRepository = new StaffRepository();
             var users = staffRepository.GetSubordinatedUsers(_userId);
             ViewBag.Assign = users;

            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            var customers = repository.GetCustomers4Subordinate(users.Select(m => Guid.Parse(m.Value)), companyId);
            ViewBag.Customers = (customerId.HasValue) ? customers.Where(m => m.Value == customerId.ToString()) : customers;

            if (companyId.HasValue)
            {
                TaskTicketRepository ticketRepository = new TaskTicketRepository();
                taskId = ticketRepository.GetFirstCallTask(customers.Where(m=> !string.IsNullOrEmpty(m.Value)).Select(m=>Guid.Parse(m.Value)).ToList());
            } 
            if (taskId.HasValue)
                ticket.Load(taskId.Value);
            ticket.OwnerId = _userId;
            return PartialView(ticket);
        }

        [HttpPost]
        public ActionResult Edit(CallTicket model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            CRMLocalContext.Customer customer = repository.GetCustomerByGuid(model.ObjId.Value);
            model.Phone = customer.Phone;
            model.Save();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
    }
}