using EnumHelper;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WFActivities.Extension;

namespace StateMachine
{
    public class TicketHost
    {
        protected static TimeSpan timeout = TimeSpan.FromSeconds(300);

        public IEnumerable<BookmarkInfo> _activeBookMarks;
        public BookmarksParticipant _ticketParticipant;
        public ConnectionParticipant _connectionParticipant;

        public WorkflowApplication Host { get; private set; }
        public IEnumerable<BookmarkInfo> ActiveBookmarks
        {
            get { return _activeBookMarks; }
            private set { _activeBookMarks = value; }
        }

        public BookmarksParticipant TicketParticipant
        {
            get { return _ticketParticipant; }
            set { _ticketParticipant = value; }
        }

        public ConnectionParticipant ConnectionParticipant
        {
            get { return _connectionParticipant; }
            set { _connectionParticipant = value; }
        }

        public TicketHost(WorkflowApplication host, Dictionary<XName, object> scope, SqlWorkflowInstanceStore InstanceStore, BookmarksParticipant participiant, ConnectionParticipant connectParticipiant = null)
        {
            Host = host;
            // Add the WorkflowHostType value to workflow application so that it stores this data in the instance store when persisted
            host.AddInitialInstanceValues(scope);
            host.InstanceStore = InstanceStore;
            host.PersistableIdle += args =>
            {
                ActiveBookmarks = args.Bookmarks;
                return PersistableIdleAction.Unload;
            };
            host.OnUnhandledException = e =>
            {
                var _exception = e.UnhandledException;
                return UnhandledExceptionAction.Abort;
            };
            // Setup the persistence participant
            _ticketParticipant = participiant;
            host.Extensions.Add(_ticketParticipant);
            if (connectParticipiant != null)
            {
                _connectionParticipant = connectParticipiant;
                host.Extensions.Add(_connectionParticipant);
            }
#if DEBUG
            // Output Workflow Tracking to VS Debug Window
            //host.Extensions.Add(new TraceTrackingParticipant());
#endif
        }

        public IEnumerable<byte> GetAvailableTransition(IEnumerable<string> bookmarks)
        {
            if (bookmarks == null)
                bookmarks = new List<string>();
            List<byte> availableCommands = new List<byte>();
            if (bookmarks != null)
            {
                foreach (WFCommand command in Enum.GetValues(typeof(WFCommand)))
                {
                    if (bookmarks.Any(m => m == ((byte)command).ToString()))
                        availableCommands.Add((byte)command);
                }
            }
            return availableCommands.AsEnumerable();
        }
    }
}
