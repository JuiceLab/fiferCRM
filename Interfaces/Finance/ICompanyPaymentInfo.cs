using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Finance
{
    public interface ICompanyPaymentInfo
    {
        int? LegalEnitityId { get; set; }
        Guid From { get; set; }
        Guid? FromCompany { get; set; }
        string FromFullName { get; set; }
        ICompanyLegalInfo CompanyLegalInfo  {get; set;}
        ICustomerInfo CustomerInfo { get; set; }
    }
}
