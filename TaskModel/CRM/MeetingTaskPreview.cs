using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskModel.CRM
{
    public class MeetingTaskPreview
    {
        public Guid MeetingId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public byte StatusId { get; set; }
        public string Result { get; set; }
    }
}
