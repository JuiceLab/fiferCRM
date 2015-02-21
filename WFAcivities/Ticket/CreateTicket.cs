using Interfaces.Support;
using Settings;
using SupportContext;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFActivities.Extension;

namespace WFActivities.Ticket
{
    /// <summary>
    /// CodeActivity for create ticket
    /// </summary>
    public sealed class CreateTicket : CodeActivity
    {
        //Ticket for create
        public InArgument<ITicket> NoveltyTicket { get; set; }

        //Participiant for set Bookmarks
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarksParticipant>();
            base.CacheMetadata(metadata);
        }
        //Execute for call Create method
        protected override void Execute(CodeActivityContext context)
        {
            ITicket ticket = NoveltyTicket.Get(context);
            ticket.Create();
            var regParticipant = context.GetExtension<BookmarksParticipant>();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var ticketQuery = wfContext.TicketsQueries.FirstOrDefault(m => m.TicketId == ticket.TicketId);
                if (ticketQuery == null)
                {
                    ticketQuery = new TicketsQuery()
                    {
                        TicketId = ticket.TicketId,
                        WorkflowId = context.WorkflowInstanceId,
                        TicketQueryId = Guid.NewGuid(),
                    };
                    wfContext.TicketsQueries.Add(ticketQuery);
                }
                else
                {
                    ticketQuery.WorkflowId = context.WorkflowInstanceId;
                }
                try
                {
                    wfContext.SaveChanges();
                    regParticipant.IdWF = ticket.TicketId.ToString();
                }
                catch (UpdateException uex)
                {
                    Trace.WriteLine(uex.StackTrace);
                }
            }
        }
    }
}
