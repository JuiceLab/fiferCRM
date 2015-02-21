using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class TransactionInfo : ITransactionInfo
    {
        public Guid OwnerId { get; set; }
        public int TransactionId { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }

        public decimal Total { get; set; }

        public bool IsExpired { get; set; }
        public ICompanyPaymentInfo InvoicedInfo { get; set; }

    }
}
