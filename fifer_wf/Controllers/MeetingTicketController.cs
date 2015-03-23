using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EnumHelper;
using TicketModel;
using StateMachine.Accessor;
using NotifyEventModel;
using EnumHelper.CRM;
using CRMRepositories;
using System.Globalization;

namespace fifer_wf.Controllers
{
    public class MeetingTicketController : ApiController
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

        [HttpPost]
        public bool Post(MeetingTicket value, WFMeetingCommand idCommand)
        {
            try
            {
                Guid userGuid = value.OwnerId;
                if (!string.IsNullOrEmpty(value.DateStartedStr))
                    value.DateStarted = Convert.ToDateTime(value.DateStartedStr, ruDateFormat)
                        .Add(Convert.ToDateTime(value.TimeStartedStr).TimeOfDay);
                MeetingTaskAccessor wrapper = new MeetingTaskAccessor();
                MeetingTicket ticket = wrapper.RunTicketWF(value, idCommand, userGuid);
                if (value.DateStarted.HasValue && value.Assigned.HasValue
                    && (idCommand == WFMeetingCommand.Create || idCommand == WFMeetingCommand.Assign))
                {
                    LocalNotifyRepository notifyRepository = new LocalNotifyRepository(userGuid);
                    notifyRepository.CreateNotify(value.NotifyDate.Value, userGuid, value.Assigned.Value, ticket.TicketId,
                        string.Format("Для Вас есть новая встреча. Цели: {0}", value.Goals));
                }
                string msg = idCommand == WFMeetingCommand.Create ?
                   "Встреча успешно создана "
                   : "Обновлена встреча";
               using (WebClient wc = new WebClient())
               {
                   wc.DownloadString(string.Format("{0}{1}{2}{3}", NotifySingletonWrapper.Instance.MainHost, "Callback/", "MeetingTaskCallback?owner=" + ticket.OwnerId + "&msg=", msg));
               }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public bool SetViewed(Guid ticketId, Guid userId)
        {
            try
            {
                MeetingTicket ticket = new MeetingTicket(userId);
                ticket.Load(ticketId);
                ticket.OwnerId = userId;
                MeetingTaskAccessor wrapper = new MeetingTaskAccessor();
                wrapper.RunTicketWF(ticket, WFMeetingCommand.View, userId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
