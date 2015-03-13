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
        [StringValue("Платеж")]
        EndPayment = 4,
        [StringValue("Звонок")]
        EndCall = 5,
        [StringValue("Завершена")]
        EndMeet = 6
    }
    
}
