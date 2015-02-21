using EnumHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskModel.Ticket
{
    public class TicketPreview
    {
        public string TicketNumber { get; set; }
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public TicketStatus Status { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }

        public int MsgCount { get; set; }
    }
}
