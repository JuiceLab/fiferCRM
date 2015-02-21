using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CRMCompanyViewModel : CRMCompanyEditModel
    {
        public Guid CompanyGuid { get; set; }
        public string LogoPath { get; set; }
        public string AssignedName { get; set; }
        public string ActivitiesStr { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CallDate { get; set; }
        public DateTime? MeetDate { get; set; }

    }
}
