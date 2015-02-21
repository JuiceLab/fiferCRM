using EnumHelper;
using Settings;
using StateMachine.Interface;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WFActivities.Extension;

namespace StateMachine.Dispatcher
{
    /// <summary>
    /// Base Dispatcher for rule workflow's processes
    /// </summary>
    /// <typeparam name="TActivity">The type of the activity.</typeparam>
    public class BaseDispatcher<TActivity> : IWorkflowDispatcher
         where TActivity : System.Activities.Activity, new()
    {
        private const int delayRestartTimers = 30;

        /// <summary>
        ///  Name for identity type of workflows
        /// </summary>
        protected string PromotionName = "Base";

        /// <summary>
        ///  Bookmark Participiant for resuming workflows in WF
        /// </summary>
        protected BookmarksParticipant promotedParticipiant = new BookmarksParticipant("MVC/Base");

        protected IDisposable cancellation;
        //protected Entry4Work workersWF;

        public BookmarksParticipant PromotedParticipant { get { return promotedParticipiant; } }

        /// <summary>
        /// Gets or sets the registration identity.
        /// </summary>
        /// <value>
        ///  Property for identity version of workflow
        /// </value>
        public WorkflowIdentity RegistrationIdentity { get; set; }

        /// <summary>
        /// Gets or sets the instance store.
        /// </summary>
        /// <value>
        /// Property for init Persistance's store
        /// </value>
        public SqlWorkflowInstanceStore InstanceStore { get; set; }
        /// <summary>
        ///  workflow generic type
        /// </summary>
        public TActivity RegistrationWorkflow;
        /// <summary>
        /// The find existing instances sql.
        /// </summary>
        private const string FindExistingInstancesSql = @"SELECT InstanceId, Value1      
            FROM [System.Activities.DurableInstancing].[InstancePromotedProperties]
            WHERE PromotionName = @PromotionName 
            AND Value1 = @IdWF";

        public BaseDispatcher()
        {
            InstanceStore =
              new SqlWorkflowInstanceStore(AccessSettings.LoadSettings().InstanceStore)
              {
                  RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(InstanceDetectionPeriod),
                  HostLockRenewalPeriod = TimeSpan.FromSeconds(InstanceDetectionPeriod),
                  InstanceLockedExceptionAction = InstanceLockedExceptionAction.NoRetry,
                  EnqueueRunCommands = false,
              };

            RegistrationIdentity = null;
        }


        /// <summary>
        /// Monitors the and runnuble workflow of this instance's type.
        /// </summary>
        private void MonitorAndRun()
        {
            while (true)
            {
#if DEBUG
                Trace.WriteLine("Monitoring instance store for workflows with expired timers");
#endif
                WaitForRunnableInstance();
                var host = CreateWorkflowApplication().Host;
                try
                {
                    host.LoadRunnableInstance(Timeout);
                    host.Run();
                }
                catch (InstanceNotReadyException)
                {
                    // Sometimes this will happen - not a problem, just retry
                    Trace.WriteLine(
                        string.Format(
                            "InstanceNotReadyException waiting {0} seconds and retrying...", InstanceDetectionPeriod));
                    Thread.Sleep(TimeSpan.FromSeconds(InstanceDetectionPeriod));
                }
                catch (InstancePersistenceCommandException exception)
                {
                    // Some other kind of persistence problem
                    // TODO: Need to log these in your preferred logging system
                    Trace.WriteLine(exception.Message);
                }
            }
        }

        public static void MonitorRegistrations()
        {
            //                    Task.Factory.StartNew(MonitorAndRun);
        }

        //todo switch off monitor
        private static void WaitForRunnableInstance()
        {
            //bool foundWorkflow;

            //do
            //{
            //    foundWorkflow =
            //        InstanceStore.WaitForEvents(InstanceHandle, TimeSpan.MaxValue).Any(
            //            persistenceEvent => persistenceEvent == HasRunnableWorkflowEvent.Value);
            //}
            //while (!foundWorkflow);
#if DEBUG
            Trace.WriteLine("Found registration workflow with expired timer");
#endif

        }


        /// <summary>
        /// Loads the registration workflow with instance guid.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        protected TicketHost LoadRegistrationWorkflow(Guid instanceId)
        {
            var host = CreateWorkflowApplication();
            host.Host.Load(instanceId);
            return host;
        }

        /// <summary>
        /// Loads the registration workflow with versioned instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        protected TicketHost LoadRegistrationWorkflow(WorkflowApplicationInstance instance)
        {
            var host = CreateWorkflowApplication();
            host.Host.Load(instance);
            return host;
        }

        /// <summary>
        /// Resumes the workflow with versioned instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="command">The command.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected TicketHost ResumeWorkflow(WorkflowApplicationInstance instance, int command, object value)
        {
            var host = LoadRegistrationWorkflow(instance);
            return BindHostEvent(command, value, host);
        }

        /// <summary>
        /// Resumes the workflow with instance id.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="command">The command.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected TicketHost ResumeWorkflow(Guid instanceId, int command, object value)
        {
            var host = LoadRegistrationWorkflow(instanceId);
            return BindHostEvent(command, value, host);
        }

        /// <summary>
        ///   The workflow host type name.
        /// </summary>
        /// <remarks>
        ///   Create a unique name that is used to associate instances in the instance store hosts that can load them. This is needed to prevent a Workflow host from loading
        ///   instances that have different implementations. The unique name should change whenever the implementation of the workflow changes to prevent workflow load exceptions.
        ///   For the purposes of the demo we create a unique name every time the program is run.
        /// </remarks>
        protected XName workflowHostTypeName = XName.Get("defaultHost");

        /// <summary>
        ///   The workflow host type property name.
        /// </summary>
        /// <remarks>
        ///   A well known property that is needed by WorkflowApplication and the InstanceStore
        /// </remarks>
        protected readonly XName WorkflowHostTypePropertyName =
            XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties").GetName("WorkflowHostType");

        /// <summary>
        ///   The instance detection period
        /// </summary>
        protected static int InstanceDetectionPeriod = 600;

        protected InstanceHandle InstanceHandle;

        protected static TimeSpan Timeout = TimeSpan.FromSeconds(300);

        protected void CreateInstanceStoreOwner()
        {
            InstanceHandle = InstanceStore.CreateInstanceHandle();

            var ownerCommand = new CreateWorkflowOwnerCommand();
            ownerCommand.InstanceOwnerMetadata.Add(
                WorkflowHostTypePropertyName, new InstanceValue(workflowHostTypeName));

            InstanceStore.DefaultInstanceOwner =
                InstanceStore.Execute(InstanceHandle, ownerCommand, TimeSpan.FromSeconds(delayRestartTimers)).InstanceOwner;
        }

        protected TicketHost BindHostEvent(int command, object value, TicketHost host)
        {
            var commandDone = new AutoResetEvent(false);
            // Run the workflow until idle, or complete
            host.Host.Completed += completedEventArgs => commandDone.Set();
            host.Host.Idle += args => commandDone.Set();
            Exception abortedException = null;

            host.Host.Aborted += abortedEventArgs =>
            {
                abortedException = abortedEventArgs.Reason;
                commandDone.Set();
            };

            // Resume the bookmark and Wait for the workflow to complete
            // ReSharper disable AssignNullToNotNullAttribute

            AutoResetEvent waitHandler = new AutoResetEvent(false);
            host.Host.Unloaded = (e) =>
            {
                //var deleteOwnerCmd = new DeleteWorkflowOwnerCommand();
                //InstanceStore.Execute(InstanceHandle, deleteOwnerCmd, TimeSpan.FromSeconds(30));
                waitHandler.Set();
            };
            // add Log4Net...
            if (host.Host.ResumeBookmark(command.ToString(), value) != BookmarkResumptionResult.Success) // ReSharper restore AssignNullToNotNullAttribute
            {
                throw new InvalidOperationException("Unable to resume registration process with command " + command);
            }

            if (!commandDone.WaitOne(Timeout))
            {
                throw new TimeoutException("Timeout waiting to confirm registration");
            }

            if (abortedException != null)
            {
                throw abortedException;
            }
            waitHandler.WaitOne();
            return host;
        }

        protected virtual TicketHost CreateWorkflowApplication(object arguments = null)
        {
            if (RegistrationWorkflow == null)
            {
                RegistrationWorkflow = new TActivity();
            }

            var host = arguments != null
                           ? RegistrationIdentity != null ? new WorkflowApplication(
                                 RegistrationWorkflow, new Dictionary<string, object> { { PromotionName, arguments } }, RegistrationIdentity)
                                 : new WorkflowApplication(
                                 RegistrationWorkflow, new Dictionary<string, object> { { PromotionName, arguments } })
                           : RegistrationIdentity != null ?
                               new WorkflowApplication(RegistrationWorkflow, RegistrationIdentity)
                               : new WorkflowApplication(RegistrationWorkflow);
            var scope = new Dictionary<XName, object> { { WorkflowHostTypePropertyName, workflowHostTypeName } };
            return new TicketHost(host, scope, InstanceStore, promotedParticipiant);
        }

        public virtual TicketHost InitObj(object data)
        {
            if (RegistrationWorkflow == null)
            {
                RegistrationWorkflow = new TActivity();
            }

            var host = CreateWorkflowApplication(data);
            AutoResetEvent waitHandler = new AutoResetEvent(false);
            host.Host.Unloaded = (e) =>
            {
                waitHandler.Set();
            };
            host.Host.Run();
            waitHandler.WaitOne();
            return host;
        }


        #region Implement IWorkflowDispatcher Interface
        public virtual TicketHost ChangeState(int cmd, object value, WorkflowApplicationInstance instance)
        {
            if (instance.InstanceId != new Guid())
            {
                return ResumeWorkflow(instance, cmd, value);
            }
            else return null;
        }
        public virtual TicketHost ChangeState(int cmd, object value) { return null; }
        public virtual TicketHost ChangeState(int cmd, object value, WorkflowApplicationInstance instance, Guid guid) { return null; }
        public virtual IEnumerable<byte> GetActiveBookmarks(Guid ticketId, WorkflowApplicationInstance instance) { return null; }
        public virtual Guid GetInstanceIdFromObjId(Guid objId)
        {
            var guid = Guid.Empty;
            // Get Workflow ID of user by querying promoted properties
            using (
                var connection =
                    new SqlConnection(AccessSettings.LoadSettings().InstanceStore))
            {
                var command = new SqlCommand(FindExistingInstancesSql, connection);

                command.Parameters.AddWithValue("@PromotionName", PromotionName);
                command.Parameters.AddWithValue("@IdWF", objId.ToString());
                connection.Open();
                var dataReader = command.ExecuteReader();
                dataReader.Read();

                guid = dataReader.HasRows ? dataReader.GetGuid(0) : Guid.Empty;
                connection.Close();
            }
            return guid;
        }
        #endregion
    }
}
