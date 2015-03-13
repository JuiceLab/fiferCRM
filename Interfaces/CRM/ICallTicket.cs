using Interfaces.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ICallTicket : ITicket
    {
        string Phone { get; set; }
        string NotifyBeforeStr { get; set; }
        Guid? PrevCallId { get; set; }
    }
}
