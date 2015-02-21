using EnumHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EnumHelper;
using TicketModel;
using StateMachine.Accessor;
using TicketRepositories;
using System.Globalization;
using NotifyEventModel;

namespace fifer_wf.Controllers
{
    public class FiferTaskTicketController : ApiController
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;


        [HttpPost]
        public bool Post(FiferTaskTicket value, WFTaskCommand idCommand)
        {
            try
            {
                Guid userGuid = value.CreatedBy;
                FiferTaskAccessor wrapper = new FiferTaskAccessor();
                if (value.IsHighPriority)
                    value.Priority = 10;
                if (!string.IsNullOrEmpty(value.DateStartedStr))
                    value.DateStarted = Convert.ToDateTime(value.DateStartedStr, ruDateFormat);
                var ticket = wrapper.RunTicketWF(value, idCommand, userGuid);
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