using EnumHelper;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFActivities.Extension;

namespace StateMachine.Interface
{
    public interface IWorkflowDispatcher
    {
        WorkflowIdentity RegistrationIdentity { get; set; }
        SqlWorkflowInstanceStore InstanceStore { get; set; }
        BookmarksParticipant PromotedParticipant { get; }
        TicketHost InitObj(object data);
        TicketHost ChangeState(int cmd, object value);
        TicketHost ChangeState(int cmd, object value, WorkflowApplicationInstance instance);
        TicketHost ChangeState(int cmd, object value, WorkflowApplicationInstance instance, Guid guid);
        IEnumerable<byte> GetActiveBookmarks(Guid ticketId, WorkflowApplicationInstance instance);
        Guid GetInstanceIdFromObjId(Guid objId);
    }
}
