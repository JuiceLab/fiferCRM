using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class MeetingEditModel : IMeetingTicket
    {
        public int MeetingId { get; set; }
        [DisplayName("Контакт")]
        public Guid CustomerId { get; set; }
        public Guid OwnerId { get; set; }
        [DisplayName("Дата")]
        public string Date { get; set; }
        [DisplayName("Цели")]
        public string Goals { get; set; }
        [DisplayName("Результат")]
        public string Result { get; set; }
        [DisplayName("Статус")]
        public byte StatusId { get; set; }
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        public string Calls { get; set; }
    }
}
