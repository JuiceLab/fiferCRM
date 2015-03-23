using Interfaces.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface IMeetingTicket 
    {

        int MeetingId { get; set; }
        [DisplayName("Клиент"), Required]
        Guid CustomerId { get; set; }
        [DisplayName("Цель встречи")]
        string Goals { get; set; }
        [DisplayName("Результат")]
        string Result { get; set; }
        string Calls { get; set; }
    }
}
