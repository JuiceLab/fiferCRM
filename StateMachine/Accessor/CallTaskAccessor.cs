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
    public class CallTaskAccessor : BaseAccessor
    {
        private const string FindExistingInstancesSql = @"use [TicketWF]
delete
from [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
where Value1 = @IdWF";

        /// <summary>
        /// The workflow interface for init SM.
        /// </summary>
        private IWorkflowDispatcher workflow = null;

        public CallTaskAccessor()
        {
            WorkflowVersionMap = new Dictionary<WorkflowIdentity, System.Activities.Activity>();
            WorkflowVersionMap.Add(new WorkflowIdentity
            {
                Name = "CallTask v1",
                Version = new Version(1, 0, 0, 0)
            }, new StateMachine.CallSM.CallSM_v1());

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
            workflow = new CallTaskDispatcher<StateMachine.CallSM.CallSM_v1>();
            workflow.RegistrationIdentity = WorkflowVersionMap.FirstOrDefault().Key;
        }

        /// <summary>
        /// Creates the novelty ticket.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="userGuid">The user GUID.</param>
        public void CreateNoveltyTicket(TicketModel.CallTicket ticket, Guid userGuid)
        {
            ticket.CreatedBy = userGuid;
            var localWF = new CallTaskDispatcher<StateMachine.CallSM.CallSM_v1>();
            localWF.InitObj(ticket);
        }

        /// <summary>
        /// Runs the ticket WF.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="idCommand">The id command for resume WF.</param>
        /// <param name="userGuid">The user GUID.</param>
        public TicketModel.CallTicket RunTicketWF(TicketModel.CallTicket ticket, WFCallTaskCommand idCommand, Guid userGuid)
        {
            var idNotifer = Guid.Empty;

            if (idCommand == WFCallTaskCommand.Create)
            {
                workflow.InitObj((ICallTicket)ticket);
                idNotifer = new Guid(workflow.PromotedParticipant.IdWF);
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    ticket.Load(context.TicketsQueries.FirstOrDefault(m => m.WorkflowId == idNotifer).TicketId.Value);
                }
            }

            else
            {
                SetVersionWF(ticket.TicketId);
                workflow.ChangeState((int)idCommand, ticket, IdentityInstance);
                ticket.Load(ticket.TicketId);
            }
            return ticket;
        }

        public Guid RunManagmentTicketWF(TicketModel.CallTicket ticket, WFCallTaskCommand idCommand)
        {
            string comment = ticket.CurrentComment;
            Guid userGuid = ticket.CreatedBy;
            if (string.IsNullOrEmpty(ticket.CurrentComment))
            {
                ticket.CurrentComment = "";
            }

            var idNotifer = Guid.Empty;
            if (idCommand == WFCallTaskCommand.Create)
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
        public IEnumerable<string> GetBookmarks(Guid ticketId)
        {
            SetVersionWF(ticketId);
            TicketModel.CallTicket ticket = new TicketModel.CallTicket();
            ticket.Load(ticketId);
            workflow.GetActiveBookmarks(ticketId, IdentityInstance);
            return workflow.PromotedParticipant.Bookmarks;
        }

        /// <summary>
        /// Shows the ticket WF.
        /// </summary>
        /// <param name="ticketId">The ticket id.</param>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public TicketModel.CallTicket ShowTicketWF(Guid ticketId, Guid userGuid)
        {
            TicketModel.CallTicket ticket = new TicketModel.CallTicket();
            ticket.Load(ticketId);
            ticket.OwnerId = userGuid;
            SetVersionWF(ticket.TicketId);
            workflow.ChangeState((int)WFCallTaskCommand.View, ticket, IdentityInstance);
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
                    var instanceId = GetInstanceIdFromObjId(id.HasValue? id.Value :Guid.NewGuid(), "CallTask");
                if (instanceId != Guid.Empty)
                {
                    var activity = LoadVersionedWF(instanceId, instanceStore);
                    if (activity.Key != null)
                    {
                        // Set dispatcher workflow typed
                        var type = activity.Value.GetType();
                        Type genericListType = typeof(CallTaskDispatcher<>).MakeGenericType(type);
                        workflow = (IWorkflowDispatcher)Activator.CreateInstance(genericListType);
                        workflow.RegistrationIdentity = activity.Key;
                    }
                    else
                    {
                        // Set dispatcher workflow typed
                        workflow = new CallTaskDispatcher<StateMachine.CallSM.CallSM_v1>(true);
                    }
                    workflow.InstanceStore = instanceStore;

                }
            }
                //else
                //{
                //    using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                //    {
                //        TicketModel.CallTicket ticket = new TicketModel.CallTicket();
                //        ticket.Load(ticketId);
                //        ticket.AutoClose();
                //        ticket.TicketId = Guid.Empty;
                //        ticket.CurrentCommentId = Guid.NewGuid();
                //        workflow.InitObj(ticket);
                //       // workflow.ChangeState((int)WFCommand.Confirm, ticket);
                //    }
                //}
            }
        }

        /// <summary>
        /// Cheks clerk's role.
        /// </summary>
        /// <param name="guid">The GUID for check.</param>
        /// <returns></returns>
        //private bool ChekRole(Guid guid)
        //{
        //    using (AccessEntities access = new AccessEntities())
        //    {
        //        // Privilegy priority
        //        return access.FunctionalRole.Any(m => m.UserFunctionalRole.Any(n => n.C_UserId == guid && n.IsAvailable) && m.IsAvailable && (m.Name == "Отдел технической поддержки" || m.Name == "Отдел закупок"));
        //    }
        //}
    }
}
