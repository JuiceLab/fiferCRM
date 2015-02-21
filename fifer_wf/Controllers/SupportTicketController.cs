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

namespace fifer_wf.Controllers
{
    public class SupportTicketController : ApiController
    {

        [HttpPost]
        public bool Post(SupportTicket value, WFCommand idCommand)
        {
            try
            {
                Guid userGuid = value.CreatedBy;
                SupportAccessor wrapper = new SupportAccessor();
               var ticket = wrapper.RunTicketWF(value, idCommand, userGuid);
               string msg = idCommand == WFCommand.Create ? 
                   "Ваше сообщение отправлено в поддержку. Номер тикета "
                   : "Обновлен тикет ";
               using (WebClient wc = new WebClient())
               {
                   wc.DownloadString(string.Format("{0}{1}{2}{3}", NotifySingletonWrapper.Instance.MainHost, "Callback/", "SupportTicketCallback?owner=" + ticket.OwnerId + "&msg=", msg + ticket.TicketNumber));
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
                SupportTicket ticket = new SupportTicket();
                ticket.Load(ticketId);
                ticket.OwnerId = userId;
                SupportAccessor wrapper = new SupportAccessor();
                wrapper.RunTicketWF(ticket, WFCommand.Views, userId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
