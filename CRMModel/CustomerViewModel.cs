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
        public Guid CityGuid { get; set; }    
    }
}
