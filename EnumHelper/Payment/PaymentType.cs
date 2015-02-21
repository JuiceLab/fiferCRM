using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.Payment
{
    public enum PaymentType:byte
    {
        [StringValue("Счет")]
        Payment=1,
        [StringValue("Счет к оплате")]
        PaymentInvoice = 2,
        [StringValue("Расход")]
        Expense = 3
    }
}
