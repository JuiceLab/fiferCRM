using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CompanyPaymentInfoEdit
    {
        public int? LegalEnitityId { get; set; }

        public Guid From { get; set; }

        public Guid? FromCompany { get; set; }

        public DateTime Created { get; set; }
        public string FromFullName { get; set; }

        public CRMCompanyEditModel CompanyLegalInfo { get; set; }

        public CustomerEditModel CustomerInfo { get; set; }

    }
}
