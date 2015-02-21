using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum TaskStatus : byte
    {
        [StringValue("Новая")]
        Novelty = 1,
        [StringValue("В обработке")]
        Processing = 2,
        [StringValue("Выполнена")]
        Completed= 3,
        [StringValue("Обновлена")]
        Updated = 4,
        [StringValue("Ошибка")]
        Error = 5,
        [StringValue("Просрочена")]
        Expired = 6
    }
}
