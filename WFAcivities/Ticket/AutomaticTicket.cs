using EnumHelper;
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
    /// CodeActivity for create ticket
    /// </summary>
    public sealed class AutomaticTicket : CodeActivity
    {
        // Title of ticket
        [RequiredArgument]
        public InArgument<string> Title { get; set; }
        //Name of envelope Object
        [RequiredArgument]
        public InArgument<string> TypeMaster { get; set; }
        // Ticket content
        public InArgument<string> InformationMessage { get; set; }
        // Create Id
        public InArgument<Guid> CreatedBy { get; set; }
        // Ticket's date start
        public InArgument<DateTime?> DateStart { get; set; }
        // return Guid of created ticket
        public OutArgument<Guid> TicketId { get; set; }

        // Create novelty ticket
        protected override void Execute(CodeActivityContext context)
        {
            SupportTicket workTicket = new SupportTicket();
            var dateStart = DateStart.Get(context);
            if (dateStart != null)
                workTicket.DateStarted = dateStart;
            // todo category name identifier from category db
            workTicket.AutoTicket(Title.Get(context), InformationMessage.Get(context), TypeMaster.Get(context), context.WorkflowInstanceId, TicketStatus.Novelty);
            context.SetValue(TicketId, workTicket.TicketId);
        }
    }
}
