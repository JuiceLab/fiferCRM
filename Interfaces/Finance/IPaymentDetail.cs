using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceModel
{
    public interface IPaymentDetail
    {
        int ItemId { get; set; }
        bool HasNDS { get; set; }
        string Name { get; set; }
        double? Tax { get; set; }
        decimal Cost { get; set; }
        double Qty { get; set; }
        decimal Total { get; set; }
    }
}
