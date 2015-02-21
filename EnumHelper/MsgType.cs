using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum MsgType:byte
    {
        [StringValue("Сообщения")]
        Msg=1,
        [StringValue("Статусы")]
        Status = 2,
        [StringValue("Уведомления")]
        Notify = 3,
        [StringValue("Лог изменений")]
        ModifyLog = 4
    }
}
