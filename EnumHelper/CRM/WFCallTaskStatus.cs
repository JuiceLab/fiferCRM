using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public enum WFCallTaskStatus : byte
    {
        [StringValue("Звонок")]
        FirstCall = 1,
        [StringValue("Повторный звонок")]
        Recall = 2,
        [StringValue("Назначена встреча")]
        Meeting = 3,
        [StringValue("Звонок завершен")]
        EndCall = 4,
        [StringValue("Оставлен комментарий")]
        Commented = 5,
        [StringValue("Просмотр")]
        Viewed = 6,
        [StringValue("Назначен исполнитель")]
        Assigned = 7,
        [StringValue("Не отвечает")]
        NotResponse = 8,
        [StringValue("Неверный номер")]
        WrongNumber = 9,
    }
}
