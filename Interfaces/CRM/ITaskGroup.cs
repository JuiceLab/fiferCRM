using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ITaskGroup
    {
        int GroupTaskId { get; set; }
        Guid TicketId { get; set; }
        List<string> AssignedUsers { get; set; }
        List<string> AssignedUserNames { get; set; }
        List<string> AssignedGroups { get; set; }
        List<string> AssignedDepartments { get; set; }
        List<string> UserStatuses { get; set; }
    }
}
