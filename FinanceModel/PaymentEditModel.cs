using CRMLocalContext;
using CRMModel;
using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class PaymentEditModel
    {
        public Guid OwnerId { get; set; }
        public int PaymentId { get; set; }

        public string Title { get; set; }

        public byte Type { get; set; }
        public string Comment { get; set; }
        public DateTime PayBefore { get; set; }
        public string PayBeforeInvariant { get; set; }
        public decimal? TotalSubmitted { get; set; }
        public IList<PaymentDetailViewModel> PaymentDetails { get; set; }

        public int TransactionId { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }
        public byte StatusId { get; set; }
        public decimal Total { get; set; }

        public bool IsExpired { get; set; }

        public CompanyPaymentInfoEdit InvoicedInfo { get; set; }

        public bool IsCash { get; set; }

        public PaymentEditModel()
        { 
            InvoicedInfo = new CompanyPaymentInfoEdit() ;
            PaymentDetails = new List<PaymentDetailViewModel>();
        }


    }
}
