using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum TimeBreakType : byte
    {
        [StringValue("Обед")]
        Dinner=1,
        [StringValue("Перерыв")]
        CoffeeBreak = 2,
        [StringValue("Конец рабочего дня")]
        EndDay = 3,
        [StringValue("Другое")]
        Other = 4
    }
}
