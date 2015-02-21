using FinanceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Finance
{
    public interface IPayment : ITransactionInfo
    {
        int PaymentId { get; set; }
        byte Type { get; set; }
        bool IsCash { get; set; }
        bool DeleteAvailable { get; set; }
        string Title { get; set; }
        string PayBeforeInvariant { get; set; }
        decimal TotalSubmitted { get; set; }
        string Comment { get; set; }
        DateTime PayBefore { get; set; }
        IList<IPaymentDetail> PaymentDetails { get; set; }
    }
}
