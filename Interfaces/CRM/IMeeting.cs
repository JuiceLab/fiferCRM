using Interfaces.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface IMeeting
    {
        Guid OwnerId { get; set; }
        DateTime Date { get; set; }
        string DateStr { get; set; }
        byte StatusId { get; set; }
        string Comment { get; set; }
    }
}
