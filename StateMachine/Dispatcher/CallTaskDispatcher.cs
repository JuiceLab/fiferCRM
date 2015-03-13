using EnumHelper;
using Interfaces.Support;
using Settings;
using SupportContext;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TicketModel;
using WFActivities.Extension;

namespace StateMachine.Dispatcher
{
    public class CallTaskDispatcher<TActivity> : BaseDispatcher<TActivity>
      where TActivity : System.Activities.Activity, new()
    {
        #region Constructors and Destructors


        public CallTaskDispatcher()
            : base()
        {
            PromotionName = "CallTask";
            promotedParticipiant = new BookmarksParticipant("MVC/CallTask");
            // ReSharper disable AssignNullToNotNullAttribute
            workflowHostTypeName = XName.Get(typeof(TActivity).FullName, "MVCCallTask");
            // ReSharper restore AssignNullToNotNullAttribute

            var instanceDetectionSetting = ConfigurationManager.AppSettings["InstanceDetectionPeriod"];
            if (!string.IsNullOrWhiteSpace(instanceDetectionSetting))
            {
                if (!int.TryParse(instanceDetectionSetting, out InstanceDetectionPeriod))
                {
                    throw new InvalidOperationException(
                        "Invalid InstanceDetectionPeriod in configuration.  Must be a number in seconds");
                }
            }
            promotedParticipiant.Promote(InstanceStore, PromotionName);
        }

        public CallTaskDispatcher(bool isInstanceCreate = true)
            : base()
        {
            PromotionName = "CallTask";
            promotedParticipiant = new BookmarksParticipant("MVC/CallTask");
            // ReSharper disable AssignNullToNotNullAttribute
            workflowHostTypeName = XName.Get(typeof(TActivity).FullName, "MVCCallTask");
            // ReSharper restore AssignNullToNotNullAttribute

            var instanceDetectionSetting = ConfigurationManager.AppSettings["InstanceDetectionPeriod"];
            if (!string.IsNullOrWhiteSpace(instanceDetectionSetting))
            {
                if (!int.TryParse(instanceDetectionSetting, out InstanceDetectionPeriod))
                {
                    throw new InvalidOperationException(
                        "Invalid InstanceDetectionPeriod in configuration.  Must be a number in seconds");
                }
            }
            if (isInstanceCreate)
            {
                promotedParticipiant.Promote(InstanceStore, PromotionName);
                //  CreateInstanceStoreOwner();
            }
        }

        #endregion

        #region Public Methods and Operators
        public override TicketHost ChangeState(int cmd, object value)
        {
            //TicketPeristenceParticipant participant;
            //  participant = new TicketPeristenceParticipant();
            var instanceId = GetInstanceIdFromObjId(((ITicket)value).TicketId);
            if (instanceId != new Guid())
            {
                return ResumeWorkflow(instanceId, cmd, value);
            }
            else return null;
        }

        public override IEnumerable<byte> GetActiveBookmarks(Guid ticketId, WorkflowApplicationInstance instance)
        {
            TicketModel.CallTicket ticket = new TicketModel.CallTicket();
            ticket.Load(ticketId);
            if (ticket.TicketStatus == (byte)EnumHelper.TaskStatus.Completed)
                return new List<byte>();
            var instanceId = GetInstanceIdFromObjId(ticketId);
            if (instanceId == Guid.Empty)
                return new List<byte>();
            if (ticketId == new Guid() || instanceId == new Guid())
                return new List<byte>() { (byte)WFTaskCommand.Create };

            var host = CreateWorkflowApplication();

            var commandDone = new AutoResetEvent(false);
            // Run the workflow until idle, or complete
            host.Host.Completed += completedEventArgs => commandDone.Set();
            host.Host.Idle += args => commandDone.Set();
            Exception abortedException = null;

            host.Host.Load(instance);

            AutoResetEvent waitHandler = new AutoResetEvent(false);
            host.Host.Unloaded = (e) =>
            {
                commandDone.Set();
                waitHandler.Set();
            };

            var bookmarks = host.GetAvailableTransition(host.TicketParticipant.Bookmarks);
            host.TicketParticipant.IdWF = instanceId.ToString();
            host.Host.Unload();
            if (!commandDone.WaitOne(Timeout))
            {
                throw new TimeoutException("Timeout waiting to confirm registration");
            }

            if (abortedException != null)
            {
                throw abortedException;
            }
            waitHandler.WaitOne();
            return bookmarks.Count() != 0 ?
                bookmarks
                : new List<byte>() { (byte)WFTaskCommand.Create };
        }

        public override Guid GetInstanceIdFromObjId(Guid objId)
        {
            var guid = Guid.Empty;
            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var id = context.TicketsQueries.FirstOrDefault(m => m.TicketId == objId).WorkflowId.Value;


                // Get Workflow ID of user by querying promoted properties
                using (
                    var connection =
                        new SqlConnection(AccessSettings.LoadSettings().InstanceStore))
                {
                    var command = new SqlCommand(FindExistingInstancesSql, connection);

                    command.Parameters.AddWithValue("@PromotionName", PromotionName);
                    command.Parameters.AddWithValue("@IdWF", id.ToString());
                    connection.Open();
                    var dataReader = command.ExecuteReader();
                    dataReader.Read();

                    guid = dataReader.HasRows ? dataReader.GetGuid(0) : Guid.Empty;

                    connection.Close();
                }
            }
            return guid;
            
        }
        #endregion

        #region Observer Imlementation

        public virtual void Subscribe()
        {
            //cancellation = provider.Subscribe(this);
            //workersWF = new Entry4Work(provider);
        }

        public virtual void Unsubscribe()
        {
            cancellation.Dispose();
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            //TLogger.Log
            //throw new NotImplementedException();
        }

        public void OnNext(SupportTicket value)
        {
            Ticket existTicket = null;
            using (TicketEntities dc = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                existTicket = dc.Tickets.FirstOrDefault(m => m.TicketId == value.TicketId);
            }

            //if (existTicket == null)
            //{
            //    value.Command = null;
            //   // value.Ticket.IdWF = value.TicketId.ToString();
            //    value.Ticket.DateModifiedStr = value.Ticket.DateModifed.ToString();
            //}
            //workersWF.InstanceWorkspace.TaskQueue.Enqueue(value);
            //if (!workersWF.WaitingEnd)
            //    workersWF.ProcessQueue();
        }
        #endregion
    }
}
