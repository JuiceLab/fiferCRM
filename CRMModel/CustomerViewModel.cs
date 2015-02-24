using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CustomerViewModel : CustomerEditModel
    {
        public string AssignedName { get; set; }

        [DisplayName("ФИО")]
        public string FullNameGenetive { get; set; }
        public Guid? CallId { get; set; }
        public Guid? MeetingId { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateTime? CallDate { get; set; }
        public DateTime? MeetDate { get; set; }
        public bool HasTodayEvent
        {
            get
            {
                return (PaymentDate.HasValue && PaymentDate.Value.Date == DateTime.Now.Date)
                    || (CallDate.HasValue && CallDate.Value.Date == DateTime.Now.Date)
                    || (MeetDate.HasValue && MeetDate.Value.Date == DateTime.Now.Date);
            }
        }
        public Guid CityGuid { get; set; }    
    }
}
