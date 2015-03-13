using Interfaces.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface IMeetingTicket 
    {
        int MeetingId { get; set; }
        Guid CustomerId { get; set; }
        string Goals { get; set; }
        string Result { get; set; }
        string Calls { get; set; }
    }
}
