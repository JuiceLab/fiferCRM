using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public enum WFMeetingCommand
    : byte
    {
        [StringValue("Создать")]
        Create = 1,
        [StringValue("Создать звонок")]
        CreateCall = 2,
        [StringValue("Создать платеж")]
        CreatePayment = 3,
        [StringValue("Создать новую встречу")]
        CreateMeet = 4,
        [StringValue("Перенести встречу")]
        ReMeet = 5,
        [StringValue("Назначить исполнителя")]
        Assign = 6,
        [StringValue("Просмотр")]
        View = 7,
        [StringValue("Комментарий")]
        Comment = 8,
        [StringValue("Завершить")]
        EndMeet = 9
    }
}
