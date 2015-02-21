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

namespace StateMachine.Accessor
{
    public class SupportAccessor : BaseAccessor
    {
        private const string FindExistingInstancesSql = @"use [TicketWF]
delete
from [System.Activities.DurableInstancing].[InstancePromotedPropertiesTable]
where Value1 = @IdWF";

        /// <summary>
        /// The workflow interface for init SM.
        /// </summary>
        private IWorkflowDispatcher workflow = null;

        public SupportAccessor()
        {
            WorkflowVersionMap = new Dictionary<WorkflowIdentity, System.Activities.Activity>();
            WorkflowVersionMap.Add(new WorkflowIdentity
            {
                Name = "SupportTicket v1",
                Version = new Version(1, 0, 0, 0)
            }, new StateMachine.SupportSM.SupportSM());

            //WorkflowVersionMap.Add(new WorkflowIdentity
            //{
            //    Name = "SupportTicket v2",
            //    Version = new Version(2, 0, 0, 0)
            //}, new SupportSM_V2());
            //WorkflowVersionMap.Add(new WorkflowIdentity
            //{
            //    Name = "SupportTicket v3",
            //    Version = new Version(3, 0, 0, 0)
            //}, new SupportSM_V3());
            workflow = new TicketDispatcher<StateMachine.SupportSM.SupportSM>();
            workflow.RegistrationIdentity = WorkflowVersionMap.FirstOrDefault().Key;
        }

        /// <summary>
        /// Creates the novelty ticket.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="userGuid">The user GUID.</param>
        public void CreateNoveltyTicket(SupportTicket ticket, Guid userGuid)
        {
            ticket.CreatedBy = userGuid;
            var localWF = new TicketDispatcher<StateMachine.SupportSM.SupportSM>();
            localWF.InitObj(ticket);
        }

        /// <summary>
        /// Runs the ticket WF.
        /// </summary>
        /// <param name="ticket">The ticket model.</param>
        /// <param name="idCommand">The id command for resume WF.</param>
        /// <param name="userGuid">The user GUID.</param>
        public SupportTicket RunTicketWF(SupportTicket ticket, WFCommand idCommand, Guid userGuid)
        {
            var idNotifer = Guid.Empty;

            if (idCommand == WFCommand.Create)
            {

                workflow.InitObj(ticket);
                idNotifer = new Guid(workflow.PromotedParticipant.IdWF);
                ticket.Load(idNotifer);
            }

            else
            {
                SetVersionWF(ticket.TicketId);
                workflow.ChangeState((int)idCommand, ticket, IdentityInstance);
                ticket.Load(ticket.TicketId);
            }
            return ticket;
        }

        public Guid RunManagmentTicketWF(SupportTicket ticket, WFCommand idCommand)
        {
            string comment = ticket.CurrentComment;
            ticket.ExpiredDate = DateTime.UtcNow.AddMinutes(30);
            Guid userGuid = ticket.CreatedBy;
            if (string.IsNullOrEmpty(ticket.CurrentComment))
            {
                ticket.CurrentComment = "";
            }

            var idNotifer = Guid.Empty;
            if (idCommand == WFCommand.Create)
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
                    Ticket acctualTicket = wfContext.GetUnitById<Ticket>(ticket.TicketId);
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
            SupportTicket ticket = new SupportTicket();
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
        public SupportTicket ShowTicketWF(Guid ticketId, Guid userGuid)
        {
            SupportTicket ticket = new SupportTicket();
            ticket.Load(ticketId);
            ticket.OwnerId = userGuid;
            SetVersionWF(ticket.TicketId);
            workflow.ChangeState((int)WFCommand.Confirm, ticket, IdentityInstance);
            return ticket;
        }

        public void ClearTicketQueue(Guid ticketId)
        {
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var ticket = wfContext.TicketsQueries.FirstOrDefault(m => m.TicketId == ticketId);
                if (ticket != null)
                {
                    wfContext.TicketsQueries.Remove(ticket);
                    wfContext.SaveChanges();
                }
            }

            using (
               var connection =
                   new SqlConnection(AccessSettings.LoadSettings().InstanceStore))
            {
                var command = new SqlCommand(FindExistingInstancesSql, connection);

                command.Parameters.AddWithValue("@IdWF", ticketId.ToString());
                connection.Open();
                var dataReader = command.ExecuteReader();
                dataReader.Read();
            }
        }

        /// <summary>
        /// Sets the apropriate version for  WF.
        /// </summary>
        /// <param name="activationId">The ticket id.</param>
        private void SetVersionWF(Guid ticketId)
        {
            if (ticketId != Guid.Empty)
            {
                var instanceStore = workflow.InstanceStore;
                var instanceId = GetInstanceIdFromObjId(ticketId, "Ticket");
                if (instanceId != Guid.Empty)
                {
                    var activity = LoadVersionedWF(instanceId, instanceStore);
                    if (activity.Key != null)
                    {
                        var type = activity.Value.GetType();
                        Type genericListType = typeof(TicketDispatcher<>).MakeGenericType(type);
                        workflow = (IWorkflowDispatcher)Activator.CreateInstance(genericListType);
                        workflow.RegistrationIdentity = activity.Key;
                    }
                    else
                    {
                        workflow = new TicketDispatcher<StateMachine.SupportSM.SupportSM>(true);
                    }
                    workflow.InstanceStore = instanceStore;
                }
                //else
                //{
                //    using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                //    {
                //        SupportTicket ticket = new SupportTicket();
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
