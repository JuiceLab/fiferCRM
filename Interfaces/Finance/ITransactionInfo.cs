using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Finance
{
    public interface ITransactionInfo
    {
        Guid OwnerId { get; set; }
        int TransactionId { get; set; }
        string Number { get; set; }
        string Status { get; set; }
        decimal Total { get; set; }
        bool IsExpired { get; set; }
        ICompanyPaymentInfo InvoicedInfo { get; set; }
    }
}
