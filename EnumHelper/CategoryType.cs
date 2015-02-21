using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum CategoryType : byte
    {
        [StringValue("Тикет поддержки")]
        Ticket = 1,

        [StringValue("Рабочая задача")]
        Task = 2
    }
}
