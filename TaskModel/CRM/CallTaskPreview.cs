using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskModel.CRM
{
    public class CallTaskPreview
    {
        public string TaskNumber { get; set; }
        public Guid CallTaskId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoto { get; set; }
        public Guid? AssignId { get; set; }
        public string AssignName { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public byte StatusId { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
    }
}
