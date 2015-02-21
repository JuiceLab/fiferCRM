using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum WFCommand : byte
    {
        [StringValue("Рассылка уведомлений")]
        SendMail = 1,
        [StringValue("Назначить исполнителя")]
        Assigned = 2,
        [StringValue("Согласовать")]
        Confirm = 3,
        [StringValue("Подтвердить")]
        Submit = 4,
        [StringValue("Отменить")]
        Cancel = 5,
        [StringValue("Создать")]
        Create = 6,
        [StringValue("Завершить")]
        Complete = 7,
        [StringValue("Приостановить")]
        Suspend = 8,
        [StringValue("Заблокировать")]
        Blocked = 9,
        [StringValue("Перенос")]
        Transfer = 10,
        [StringValue("Комментарий")]
        Comment = 11,
        [StringValue("Просмотрено")]
        Views = 12,
        [StringValue("Отклонить")]
        Reject = 13
    }
}
