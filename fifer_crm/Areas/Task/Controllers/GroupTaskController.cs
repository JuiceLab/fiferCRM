using AccessRepositories;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using EnumHelper;
using fifer_crm.Controllers;
using fifer_crm.Helpers;
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

namespace fifer_crm.Areas.Task.Controllers
{
    [Authorize, CRMLogAttribute]
    public class GroupTaskController : BaseFiferController
    {
        TaskTicketRepository _repository = new TaskTicketRepository();

        [DisplayName("Создание задачи")]
        public ActionResult TaskEdit()
        {
            FiferTaskTicket ticket = new FiferTaskTicket()
            {
                CreatedBy = (Guid)Membership.GetUser().ProviderUserKey,
                DateStartedStr = DateTime.Now.ToString("dd.MM.yyyy"),
                Periods = new List<TaskPeriod>() { new TaskPeriod() },
                Group = new TaskGroup()
            };
            StaffRepository staffRepository = new StaffRepository();
            var users  = staffRepository.GetSubordinatedUsers((Guid)Membership.GetUser().ProviderUserKey);
           
            ViewBag.Assign = users;
            ViewBag.AssignGroup = staffRepository.GetGroups((Guid)Membership.GetUser().ProviderUserKey);
            ViewBag.AssignDepartments = staffRepository.GetAvailableDepartmetns((Guid)Membership.GetUser().ProviderUserKey);
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            ViewBag.Categories = repository.GetTaskCatergories();
            ViewBag.Command = (byte)WFTaskCommand.Create;
            return PartialView(ticket);
        }

        [DisplayName("Обработка задачи")]
        public ActionResult TaskProcess(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            ticket.OwnerId = (Guid)Membership.GetUser().ProviderUserKey;
            ticket.CreatedBy = ticket.OwnerId;
            return PartialView(ticket);
        }

        [DisplayName("Групповая задача")]
        public ActionResult TaskGroup(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            StaffRepository staffRepository = new StaffRepository();
            var users = staffRepository.GetSubordinatedUsers((Guid)Membership.GetUser().ProviderUserKey);

            ViewBag.Assign = users;
            ViewBag.AssignGroup = staffRepository.GetGroups((Guid)Membership.GetUser().ProviderUserKey);
            ViewBag.AssignDepartments = staffRepository.GetAvailableDepartmetns((Guid)Membership.GetUser().ProviderUserKey);
            
            return PartialView(ticket);
        }

        [DisplayName("Обновление групповой задачи")]
        [HttpPost]
        public ActionResult TaskGroup(FiferTaskTicket ticket)
        {
            _repository.UpdateGroup(ticket.Group);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [DisplayName("Периодическая задача")]
        public ActionResult TaskPeriods(Guid taskId)
        {
            FiferTaskTicket ticket = new FiferTaskTicket();
            ticket.Load(taskId);
            StaffRepository staffRepository = new StaffRepository();
            var users = staffRepository.GetSubordinatedUsers((Guid)Membership.GetUser().ProviderUserKey);

            ViewBag.Assign = users;
            return PartialView(ticket);
        }

        public ActionResult TaskPeriod(Guid periodId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            TaskPeriod period = repository.GetPeriod(periodId);
           StaffRepository staffRepository = new StaffRepository();
           var users = staffRepository.GetSubordinatedUsers((Guid)Membership.GetUser().ProviderUserKey);

           ViewBag.Assign = users;
           return PartialView(period);
        }

        [DisplayName("Обновление групповой задачи")]
        [HttpPost]
        public ActionResult TaskPeriod(TaskPeriod period)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            repository.UpdatePeriod(period);
            return Json(new { }, JsonRequestBehavior.AllowGet);
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
                if (exist != null)
                {
                    item.Title = exist.FirstName + ' ' + exist.LastName;
                    item.IconPath = exist.PhotoPath;
                }
            }
            foreach (var item in statuses)
            {
                var exist = employees.FirstOrDefault(m => m.UserId == item.UserId);
                if (exist != null)
                {
                    item.Title = exist.FirstName + ' ' + exist.LastName;
                    item.IconPath = exist.PhotoPath;
                }
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

        
    }
}