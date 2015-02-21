using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.Payment
{
    public enum PaymentStatus:byte
    {
        [StringValue("Новый")]
        Novelty=1,
        [StringValue("Передан руководителю")]
        WaitConfirm = 2,
        [StringValue("Ожидает оплаты")]
        WaitPays = 3,
        [StringValue("Оплачено")]
        Payed = 4,
        [StringValue("Закрыто актом")]
        ActClosed = 5
    }
}
