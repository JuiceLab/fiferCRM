using AccessRepositories;
using CompanyRepositories;
using EnumHelper;
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
using TaskModel;
using TicketModel;
using TicketRepositories;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class CommonTaskController : BaseFiferController
    {
        TaskTicketRepository _repository = new TaskTicketRepository();

        
        [DisplayName("Обработка задачи")]
        public ActionResult TaskProcess(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            ticket.OwnerId = (Guid)Membership.GetUser().ProviderUserKey;
            return PartialView(ticket);
        }


        [DisplayName("Задать исполнителя задачи")]
        public ActionResult TaskAssign(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            ticket.OwnerId = (Guid)Membership.GetUser().ProviderUserKey;

            StaffRepository accessRepository = new StaffRepository();
            ViewBag.Assign = accessRepository.GetSubordinatedUsers((Guid)Membership.GetUser().ProviderUserKey);
            return PartialView(ticket);
        } 

        [DisplayName("Задать дату выполнения задачи")]
        public ActionResult TaskDateTransfer(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            if (!ticket.DateStarted.HasValue || ticket.DateStarted.Value < DateTime.Now)
                ticket.DateStartedStr = DateTime.Now.ToString("dd.MM.yyyy");
            ticket.OwnerId = (Guid)Membership.GetUser().ProviderUserKey;
            return PartialView(ticket);
        } 

        [DisplayName("История сообщений по задаче")]
        public ActionResult TaskHistory(Guid taskId, string taskNumber)
        {
            IEnumerable<MessageViewModel> messages = _repository.GetMessages4Item(taskId);
            IEnumerable<MessageViewModel> statuses = _repository.GetStatus4Task(taskId);
            
            StaffRepository staffRepository = new StaffRepository();
            var employees = staffRepository.GetEmployees(_userId).ToList();
            foreach (var item in messages)
            {
                var exist = employees.FirstOrDefault(m => m.UserId == item.UserId);
                item.Title = exist.FirstName + ' ' + exist.LastName;
                item.IconPath = exist.PhotoPath; 
            }
            foreach (var item in statuses)
            {
                var exist = employees.FirstOrDefault(m => m.UserId == item.UserId);
                item.Title = exist.FirstName + ' ' + exist.LastName;
                item.IconPath = exist.PhotoPath;
            }
            ViewBag.Number = taskNumber;
            return PartialView(messages.Union(statuses));
        }

        [DisplayName("Загрузка доступных действий для задачи")]
        public ActionResult Actions4Task(Guid taskId)
        {
            Actions4ProcessHelper actionPanel = new Actions4ProcessHelper();
            var model = actionPanel.ManagmentTask(taskId, (Guid)Membership.GetUser().ProviderUserKey);
            return PartialView(model);
        }

        public ActionResult GetTaskTable(Guid userId)
        {
            var model = new TaskWrapViewModel((Guid)Membership.GetUser().ProviderUserKey, false);
            return PartialView("TaskTable", model.SearchFilter.SearchResult);
        }

       
    }
}