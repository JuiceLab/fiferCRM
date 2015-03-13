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
    public class CallTicketController : ApiController
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

        [HttpPost]
        public bool Post(TicketModel.CallTicket value, WFCallTaskCommand idCommand)
        {
            try
            {
                Guid userGuid = value.CreatedBy;
                if (!string.IsNullOrEmpty(value.DateStartedStr))
                    value.DateStarted = Convert.ToDateTime(value.DateStartedStr, ruDateFormat)
                        .Add(Convert.ToDateTime(value.TimeStartedStr).TimeOfDay);
                CallTaskAccessor wrapper = new CallTaskAccessor();
                TicketModel.CallTicket ticket = wrapper.RunTicketWF(value, idCommand, userGuid);
                if (value.DateStarted.HasValue && value.Assigned.HasValue
                    && (idCommand == WFCallTaskCommand.Create || idCommand == WFCallTaskCommand.Assign))
                {
                    LocalNotifyRepository notifyRepository = new LocalNotifyRepository(userGuid);
                    notifyRepository.CreateNotify(value.NotifyDate.Value, userGuid, value.Assigned.Value, ticket.TicketId,
                        string.Format("Для Вас есть новый звонок клиенту. #{0}", ticket.TicketNumber));
                }
               string msg = idCommand == WFCallTaskCommand.Create ?
                   "Звонок успешно создан. Номер звонка "
                   : "Обновлен звонок";
               using (WebClient wc = new WebClient())
               {
                   wc.DownloadString(string.Format("{0}{1}{2}{3}", NotifySingletonWrapper.Instance.MainHost, "Callback/", "CallTaskCallback?owner=" + ticket.OwnerId + "&msg=", msg + ticket.TicketNumber));
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
                TicketModel.CallTicket ticket = new TicketModel.CallTicket();
                ticket.Load(ticketId);
                ticket.OwnerId = userId;
                CallTaskAccessor wrapper = new CallTaskAccessor();
                wrapper.RunTicketWF(ticket, WFCallTaskCommand.View, userId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
