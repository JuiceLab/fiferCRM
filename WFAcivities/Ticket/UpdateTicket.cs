using Interfaces.Support;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFActivities.Ticket
{
    /// <summary>
    /// CodeActivity for update ticket's content
    /// </summary>
    public sealed class UpdateTicket : CodeActivity
    {
        // Current ticket's state
        public InArgument<ITicket> AcctualTicketState { get; set; }

        //Execute save
        protected override void Execute(CodeActivityContext context)
        {
            ITicket ticket = AcctualTicketState.Get(context);
            ticket.Save();
        }
    }
}
