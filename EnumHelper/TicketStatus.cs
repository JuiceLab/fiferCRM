using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper
{
    public enum TicketStatus : byte
    {
        [StringValue("Новый")]
        Novelty = 1,
        [StringValue("В обработке")]
        Processing = 2,
        [StringValue("Закрыт")]
        Closed = 3,
        [StringValue("Обновлен")]
        Updated = 4,
        [StringValue("Ошибка")]
        Error = 5,
        [StringValue("Запланирован")]
        RoadMaped = 6
    }

  
}
