using Interfaces.Support;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketModel;

namespace WFActivities.Ticket
{
    /// <summary>
    /// CodeActivity for close  ticket
    /// </summary>
    public sealed class CloseTicket : CodeActivity
    {
        // Guid of ticket
        public InArgument<Guid> TicketId { get; set; }

        // Close ticket
        protected override void Execute(CodeActivityContext context)
        {
            Guid ticketId = context.GetValue(this.TicketId);
            ITicket ticket = new SupportTicket();
            ticket.Load(ticketId);
            ticket.AutoClose();
        }
    }
}
