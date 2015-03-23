using EnumHelper;
using Settings;
using StateMachine.Dispatcher;
using StateMachine.Interface;
using SupportContext;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketModel;
using EntityRepository;
using StateMachine.SupportSM;
using EnumHelper.CRM;
using Interfaces.CRM;

namespace StateMachine.Accessor
{
    public class MeetingTaskAccessor : BaseAccessor
    {
        private const string FindExistingInstancesSql = @"use [TicketWF]
delete
from [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
where Value1 = @IdWF";

        /// <summary>
        /// The workflow interface for init SM.
        /// </summary>
        private IWorkflowDispatcher workflow = null;

        public MeetingTaskAccessor()
        {
            WorkflowVersionMap = new Dictionary<WorkflowIdentity, System.Activities.Activity>();
            WorkflowVersionMap.Add(new WorkflowIdentity
            {
                Name = "MeetingTask v1",
                Version = new Version(1, 0, 0, 0)
            }, new StateMachine.MeetingSM.MeetingSM_v1());

            //WorkflowVersionMap.Add(new WorkflowIdentity
            //{
            //    Name = "TaskTicket v2",
            //    Version = new Version(2, 0, 0, 0)
            //}, new SupportSM_V2());
            //WorkflowVersionMap.Add(new WorkflowIdentity
            //{
            //    Name = "TaskTicket v3",
            //    Version = new Version(3, 0, 0, 0)
            //}, new SupportSM_V3());
            workflow = new MeetingTaskDispatcher<StateMachine.MeetingSM.MeetingSM_v1>();
            workflow.RegistrationIdentity = WorkflowVersionMap.FirstOrDefault().Key;
        }

        /// <summary>
        /// Creates the novelty ticket.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="userGuid">The user GUID.</param>
        public void CreateNoveltyTicket(MeetingTicket ticket, Guid userGuid)
        {
            ticket.CreatedBy = userGuid;
            var localWF = new MeetingTaskDispatcher<StateMachine.MeetingSM.MeetingSM_v1>();
            localWF.InitObj(ticket);
        }

        /// <summary>
        /// Runs the ticket WF.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="idCommand">The id command for resume WF.</param>
        /// <param name="userGuid">The user GUID.</param>
        public MeetingTicket RunTicketWF(MeetingTicket ticket, WFMeetingCommand idCommand, Guid userGuid)
        {
            var idNotifer = Guid.Empty;

            if (idCommand == WFMeetingCommand.Create)
            {
                workflow.InitObj((IMeetingTicket)ticket);
                idNotifer = new Guid(workflow.PromotedParticipant.IdWF);
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    ticket.OwnerId = userGuid;
                    ticket.Load(context.TicketsQueries.FirstOrDefault(m => m.WorkflowId == idNotifer).TicketId.Value);
                }
            }

            else
            {
                SetVersionWF(ticket.TicketId);
                workflow.ChangeState((int)idCommand, ticket, IdentityInstance);
                ticket.OwnerId = userGuid;
                ticket.Load(ticket.TicketId);
            }
            return ticket;
        }

        public Guid RunManagmentTicketWF(MeetingTicket ticket, WFMeetingCommand idCommand)
        {
            string comment = ticket.CurrentComment;
            Guid userGuid = ticket.CreatedBy;
            if (string.IsNullOrEmpty(ticket.CurrentComment))
            {
                ticket.CurrentComment = "";
            }

            var idNotifer = Guid.Empty;
            if (idCommand == WFMeetingCommand.Create)
            {
                workflow.InitObj(ticket);
                ticket.Load(idNotifer);
            }
            else
            {
                using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    SetVersionWF(ticket.TicketId);
                    if (IdentityInstance == null)
                        workflow.InitObj(ticket);
                    workflow.ChangeState((int)idCommand, ticket, IdentityInstance);
                    SupportContext.CallTicket acctualTicket = wfContext.GetUnitById<SupportContext.CallTicket>(ticket.TicketId);
                    //if (acctualTicket.TicketStatus == (byte)TicketStatus.Closed)
                    //{
                    //    TryCloseTicketIncident(ticket);
                    //}
                }
            }
           
            return ticket.TicketId;
        }

        /// <summary>
        /// Gets the available bookmarks.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <returns></returns>
        public IEnumerable<string> GetBookmarks(Guid ticketId, Guid userId)
        {
            SetVersionWF(ticketId);
            MeetingTicket ticket = new MeetingTicket(userId);
            ticket.Load(ticketId);

            workflow.OwnerId = userId;
            workflow.GetActiveBookmarks(ticketId, IdentityInstance);
            return workflow.PromotedParticipant.Bookmarks;
        }

        /// <summary>
        /// Shows the ticket WF.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public MeetingTicket ShowTicketWF(Guid ticketId, Guid userGuid)
        {
            MeetingTicket ticket = new MeetingTicket(userGuid);
            ticket.Load(ticketId);
            ticket.OwnerId = userGuid;
            SetVersionWF(ticket.TicketId);
            workflow.ChangeState((int)WFMeetingCommand.View, ticket, IdentityInstance);
            return ticket;
        }

        /// <summary>
        /// Sets the apropriate version for  WF.
        /// </summary>
        /// <param name="activationId">The ticket id.</param>
        private void SetVersionWF(Guid ticketId)
        {
            if (ticketId != Guid.Empty)
            {
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    var id = context.TicketsQueries.FirstOrDefault(m => m.TicketId == ticketId).WorkflowId;
                    var instanceStore = workflow.InstanceStore;
                    var instanceId = GetInstanceIdFromObjId(id.HasValue ? id.Value : Guid.NewGuid(), "MeetingTask");
                    if (instanceId != Guid.Empty)
                    {
                        var activity = LoadVersionedWF(instanceId, instanceStore);
                        if (activity.Key != null)
                        {
                            // Set dispatcher workflow typed
                            var type = activity.Value.GetType();
                            Type genericListType = typeof(MeetingTaskDispatcher<>).MakeGenericType(type);
                            workflow = (IWorkflowDispatcher)Activator.CreateInstance(genericListType);
                            workflow.RegistrationIdentity = activity.Key;
                        }
                        else
                        {
                            // Set dispatcher workflow typed
                            workflow = new MeetingTaskDispatcher<StateMachine.MeetingSM.MeetingSM_v1>(true);
                        }
                        workflow.InstanceStore = instanceStore;

                    }
                }
            }
        }
    }
}
