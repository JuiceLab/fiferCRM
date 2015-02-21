using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum WFTaskCommand : byte
    {
        [StringValue("Создать задачу")]
        Create = 1,
        [StringValue("Просмотреть задачу")]
        View = 2,
        [StringValue("Назначить исполнителя")]
        Assign = 3,
        [StringValue("Оставить комменатирий")]
        Comment = 4,
        [StringValue("Перенести")]
        Sсhedule = 5,
        [StringValue("Выполнить")]
        Complete = 6,
        [StringValue("Просрочить")]
        Expired = 7
    }
}
