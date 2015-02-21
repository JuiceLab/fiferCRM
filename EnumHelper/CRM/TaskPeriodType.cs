using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumHelper;

namespace EnumHelper.CRM
{
    public enum TaskPeriodType : byte
    {
        [StringValue("Ежедневно")]
        Daily = 1,
        [StringValue("Еженедельно")]
        Weekly = 2,
        [StringValue("Ежемесячно")]
        Monthly = 3,
        [StringValue("Ежегодно")]
        Yearly = 4,
        [StringValue("Ежеквартально")]
        Quaterly = 5
    }
}
