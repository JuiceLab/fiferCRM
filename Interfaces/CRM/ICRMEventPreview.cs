using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ICRMEventPreview
    {
        byte TypeId { get; set; }
        byte StatusId { get; set; }
        Guid EventId { get; set; }
        DateTime EventDate { get; set; }
        Guid? ContactId { get; set; }
        Guid OwnerId { get; set; }
        string OwnerName { get; set; }
        string ContactName { get; set; }
    }
}
