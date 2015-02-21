using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class PaymentDetailViewModel : IPaymentDetail
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double? Tax { get; set; }
        public bool HasNDS { get; set; }
        public decimal Cost { get; set; }
        public double Qty { get; set; }
        public decimal Total { get; set; }
    }
}
