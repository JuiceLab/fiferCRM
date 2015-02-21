using CRMLocalContext;
using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class PaymentViewModel : IPayment
    {
        public Guid OwnerId { get; set; }
        public Guid CompanyGuid { get; set; }

        public int PaymentId { get; set; }

        public string Title { get; set; }
        public bool DeleteAvailable { get; set; }

        public byte Type { get; set; }
        public string Comment { get; set; }
        public DateTime PayBefore { get; set; }
        public string PayBeforeInvariant { get; set; }
        public decimal TotalSubmitted { get; set; }
        public IList<IPaymentDetail> PaymentDetails { get; set; }

        public int TransactionId { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }
        public decimal Total { get; set; }

        public bool IsExpired { get; set; }

        public ICompanyPaymentInfo InvoicedInfo { get; set; }

        public bool IsCash { get; set; }

        public PaymentViewModel()
        { 
            InvoicedInfo = new CompanyPaymentInfo() ;
            PaymentDetails = new List<IPaymentDetail>();
        }


    }
}
