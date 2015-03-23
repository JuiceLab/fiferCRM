using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public enum WFMeetingStatus : byte
    {
        [StringValue("Новая встреча")]
        Novelty = 1,
        [StringValue("Перенесенная встреча")]
        TransferMeet = 2,
        [StringValue("Назначен исполнитель")]
        AssignMeet = 3,
        [StringValue("Заврешена платежом")]
        EndPayment = 4,
        [StringValue("Заврешена звоноком")]
        EndCall = 5,
        [StringValue("Заврешена встречей")]
        EndMeet = 6,
        [StringValue("Оставлен комментарий")]
        Commented = 7,
        [StringValue("Просмотр")]
        Viewed = 8,
        [StringValue("Завершена")]
        Ended = 9
    }
    
}
