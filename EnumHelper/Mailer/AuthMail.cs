using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.Mailer
{
    public enum AuthMail : byte
    {
        [StringValue("Новая компания")]
        NewCustomer = 1,
        [StringValue("Новый сотрудник")]
        NewEmployee = 2,
        [StringValue("Сброс пароля")]
        ResetPass = 3,
        [StringValue("Одноразовый токен доступа")]
        RemoteToken = 4
    }
}
