using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum WFStatus : byte
    {
        [StringValue("Новый")]
        Novelty = 1,
        [StringValue("Обработка")]
        Processed = 2,
        [StringValue("Ожидает закрытия")]
        WaitConfirm = 3,
        [StringValue("Выполнен")]
        Completed = 4,
        [StringValue("Отмена")]
        Cancellation = 5,
        [StringValue("Не подтверждено")]
        NonConfirm = 6,
        [StringValue("Просмотрен")]
        Opened = 7,
        [StringValue("Оставлен комментарий")]
        Commented = 8,
        [StringValue("Отложен")]
        Scheduling = 9,
        [StringValue("Назначен исполнитель")]
        Assigned = 10,
    }
}
