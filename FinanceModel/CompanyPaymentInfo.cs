using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class CompanyPaymentInfo : ICompanyPaymentInfo
    {
        public int? LegalEnitityId { get; set; }

        public Guid From { get; set; }

        public Guid? FromCompany { get; set; }

        public string FromFullName { get; set; }

        public ICompanyLegalInfo CompanyLegalInfo { get; set; }

        public ICustomerInfo CustomerInfo { get; set; }
    }
}
