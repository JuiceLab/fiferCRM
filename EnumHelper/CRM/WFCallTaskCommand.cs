using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.CRM
{
    public enum WFCallTaskCommand : byte
    {
        [StringValue("Создать")]
        Create =1,
        [StringValue("Назначить исполнителя")]
        Assign = 2,
        [StringValue("Просмотр")]
        View = 3,
        [StringValue("Оставить комментарий")]
        Comment = 4,
        [StringValue("Не отвечает")]
        NotResponsed = 5,
        [StringValue("Неверный номер")]
        PhoneWrong = 6,
        [StringValue("Перезвонить")]
        Recall= 7,
        [StringValue("Назначить встречу")]
        Meeting = 8,
        [StringValue("Завершить звонок")]
        EndCall = 9,
        [StringValue("Создать новый звонок")]
        EndAndCreateCall = 10
    }
}
