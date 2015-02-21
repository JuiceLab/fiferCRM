using EnumHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TicketModel;
using StateMachine.Accessor;
using TicketRepositories;
using System.Globalization;
using NotifyEventModel;
using CRMRepositories;

namespace fifer_wf.Controllers
{
    public class TaskTicketController : ApiController
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

        [HttpPost]
        public bool Post(FiferTaskTicket value, WFTaskCommand idCommand)
        {
            try
            {
                Guid userGuid = value.CreatedBy;
                TaskAccessor wrapper = new TaskAccessor();
                var ticket = new TaskTicket()
                {
                    CategoryId = value.CategoryId,
                    Title = value.Title,
                    Msg = value.Msg,
                    DateStartedStr = value.DateStartedStr,
                    IsHighPriority = value.IsHighPriority,
                    CreatedBy = value.CreatedBy,
                    Assigned = value.Assigned,
                    TicketId = value.TicketId
                };

                if (value.IsHighPriority)
                    value.Priority = 10;
                if (!string.IsNullOrEmpty(value.DateStartedStr))
                    ticket.DateStarted = Convert.ToDateTime(value.DateStartedStr, ruDateFormat).Add(Convert.ToDateTime(value.TimeStartedStr).TimeOfDay);
                var ticketProcessed = wrapper.RunTicketWF(ticket, idCommand, userGuid);
                var curItem = new FiferTaskTicket();
                curItem.Load(ticketProcessed.TicketId);
                if(value.HasGroup)
                {
                    curItem.HasGroup = true;
                    curItem.Group = value.Group;
                    curItem.UpdateGroup();
                };
                if (value.HasPeriods)
                {
                    curItem.HasPeriods = true;
                    
                    curItem.Periods = value.Periods;
                    curItem.Periods[0].TicketId = curItem.TicketId;
                    curItem.UpdatePeriods(userGuid);
                }

                if (value.NotifyDate.HasValue && value.Assigned.HasValue)
                {
                    LocalNotifyRepository notifyRepository = new LocalNotifyRepository(userGuid);
                    notifyRepository.CreateNotify(value.NotifyDate.Value, userGuid, value.Assigned.Value, curItem.TicketId, 
                        string.Format("Для Вас есть новая задача. #{0}", curItem.TicketNumber));
                }
                string msg = idCommand == WFTaskCommand.Create ?
                    "Задача успешно создана. Номер задачи "
                    : "Обновлена задача ";
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString(string.Format("{0}{1}{2}{3}", NotifySingletonWrapper.Instance.MainHost, "Callback/", "TaskTicketCallback?owner=" + ticket.OwnerId + "&msg=", msg + ticket.TicketNumber));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [HttpGet]
        public bool Transfer(Guid taskId,Guid userId, string date)
        {
            try
            {
                TaskTicket task = new TaskTicket();
                task.Load(taskId);
                task.DateStarted = Convert.ToDateTime(date, ruDateFormat);
                task.OwnerId = userId;
                TaskAccessor wrapper = new TaskAccessor();
                var ticket = wrapper.RunTicketWF(task, WFTaskCommand.Sсhedule, userId);
                string msg = "Обновлена задача ";
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString(string.Format("{0}{1}{2}", NotifySingletonWrapper.Instance.MainHost, "TaskTicketCallback?owner=" + ticket.OwnerId + "&msg=", msg + ticket.TicketNumber));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public bool SetViewed(Guid taskId, Guid userId)
        {
            try
            {
                TaskTicket task = new TaskTicket();
                task.Load(taskId);
                task.OwnerId = userId;
                task.CurrentComment = string.Empty;
                TaskAccessor wrapper = new TaskAccessor();
                wrapper.RunTicketWF(task, WFTaskCommand.View, userId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public bool SetExpiredTasks(Guid? userId, string date)
        {
            try
            {
                TaskTicketRepository repository = new TaskTicketRepository();
                IEnumerable<Guid> taskIds = repository.GetExpiredTasks();
                Parallel.ForEach(taskIds, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (taskId) =>
                {
                    TaskTicket task = new TaskTicket();
                    task.Load(taskId);
                    if(userId.HasValue)
                        task.OwnerId = userId.Value;
                    TaskAccessor wrapper = new TaskAccessor();
                    wrapper.RunTicketWF(task, WFTaskCommand.Expired, userId.Value);
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}