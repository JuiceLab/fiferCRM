using AccessRepositories;
using CompanyRepositories;
using CRMRepositories;
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
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            Dictionary<string, List<MessageViewModel>> model = new Dictionary<string, List<MessageViewModel>>();
            model.Add("Изменения", repository.GetModifyLog(taskId));
          
            model.Add("Сообщения", _repository.GetMessages4Item(taskId).ToList());
            model.Add("Статусы", _repository.GetStatus4Task(taskId).ToList());
        
            StaffRepository staffRepository = new StaffRepository();
            var users = staffRepository.GetSubordinatedUsers(_userId);
            var photos = staffRepository.GetEmployeesPhoto(_userId);

            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(model.SelectMany(m => m.Value).Select(m => m.UserId).Distinct());
            foreach (var item in model.SelectMany(m => m.Value))
            {
                if (item.UserId != Guid.Empty)
                {
                    var owner = names.FirstOrDefault(m => m.UserId == item.UserId);
                    item.Title = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    item.IconPath = photos[item.UserId];
                }
            }
          
            ViewBag.Number = taskNumber;
            return PartialView(model);

        }

        [DisplayName("Загрузка доступных действий для задачи")]
        public ActionResult Actions4Task(Guid taskId)
        {
            Actions4ProcessHelper actionPanel = new Actions4ProcessHelper();
            var model = actionPanel.ManagmentTask(taskId, (Guid)Membership.GetUser().ProviderUserKey);
            return PartialView(model);
        }

        [DisplayName("Загрузка таблицы задач")]
        public ActionResult GetTaskTable(Guid userId)
        {
            var model = new TaskWrapViewModel((Guid)Membership.GetUser().ProviderUserKey, false);
            return PartialView("TaskTable", model.SearchFilter.SearchResult);
        }

       
    }
}