using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CRMEventPreview : ICRMEventPreview
    {
        public byte TypeId { get; set; }
        public byte StatusId { get; set; }
        public Guid EventId { get; set; }
        public DateTime EventDate { get; set; }
        public Guid? ContactId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string ContactName { get; set; }
    }
}
