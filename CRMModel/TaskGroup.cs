using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class TaskGroup : ITaskGroup
    {
        public int GroupTaskId { get; set; }

        public Guid TicketId { get; set; }
        public List<string> AssignedUsers { get; set; }

        public List<string> AssignedUserNames { get; set; }
        public List<string> AssignedGroups { get; set; }
        public List<string> AssignedDepartments { get; set; }

        public List<string> UserStatuses { get; set; }
        public TaskGroup() {
            AssignedUsers = new List<string>();
            AssignedUserNames = new List<string>();
            AssignedGroups = new List<string>();
            AssignedDepartments = new List<string>();
            UserStatuses = new List<string>();
        }
    }
}
