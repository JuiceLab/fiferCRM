using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.Mailer
{
    public enum CustomerMail : byte
    {
        [StringValue("Обратная связь на сайте")]
        Feedback = 1
    }
}
