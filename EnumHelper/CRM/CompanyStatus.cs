using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public enum CompanyStatus : byte
    {
        [StringValue("Не установлен")]
        Undefined = 0,
        [StringValue("Новая")]
        Novelty = 1,
        [StringValue("Клиент")]
        Client = 2,
        [StringValue("Потенциальный Клиент")]
        Potential = 3,
        [StringValue("Запрет обзвона")]
        Reject = 4,
        [StringValue("Не подтврежден")]
        NonConfirm = 5,
        [StringValue("Конкурент")]
        Concurent = 6,
        [StringValue("Бывший клиент")]
        Later = 7
    }
}
