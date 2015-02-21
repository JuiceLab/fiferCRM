using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Finance
{
    public interface IFinanceStatInteractive : ICompanyInfo
    {
        Guid UserId { get; set; }
        IFinanceFilter FinanceFilter { get; set; }
        IDictionary<DateTime, IEnumerable<decimal>> Balance { get; }
        IDictionary<DateTime , IEnumerable<IPayment>> Transaction { get; set; }
    }
}
