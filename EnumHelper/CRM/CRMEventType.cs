using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public  enum  CRMEventType:byte
    {
        [StringValue("Задача")]
        Task=1,
        [StringValue("Звонок")]
        TaskCall = 2,
        [StringValue("Встреча")]
        Meeting = 3,
    }
}
