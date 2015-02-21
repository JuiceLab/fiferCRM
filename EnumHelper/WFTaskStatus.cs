using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum WFTaskStatus : byte
    {
        [StringValue("Ожидает обработки")]
        NoveltyDefault = 0,
        [StringValue("Новая")]
        Novelty = 1,
        [StringValue("Обработка")]
        Processed = 2,
        [StringValue("Просмотр")]
        Views = 3,
        [StringValue("Выполнена")]
        Completed = 4,
        [StringValue("Оставлен комментарий")]
        Commented = 5,
        [StringValue("Перенесена")]
        Scheduling = 6,
        [StringValue("Назначен исполнитель")]
        Assigned = 7,
        [StringValue("Просрочена")]
        Expired = 8
    }
}
