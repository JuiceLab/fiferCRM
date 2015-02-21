using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Support
{
    public interface ITicket : IStatus
    {
        [DisplayName("ID")]
        Guid TicketId { get; set; }
        //int TicketNumber { get; set; }
        Guid? MasterId { get; set; }
        [DisplayName("Заголовок"), Required]
        string Title { get; set; }
        Guid TicketOwnerId { get; set; }
        Guid CreatedBy { get; set; }

        [DisplayName("Создан")]
        DateTime DateCreated { get; set; }

        [DisplayName("Активен с")]
        DateTime? DateStarted { get; set; }

        [DisplayName("Истекает")]
        DateTime? TimeToExpired { get; set; }

        [DisplayName("Статус тикета")]
        string TicketStatusStr { get; set; }

        [DisplayName("Заметка")]
        string Msg { get; set; }

        byte? TicketStatus { get; set; }

        [DisplayName("Тип тикета")]
        int? CategoryId { get; set; }

        [DisplayName("Тип тикета")]
        string CategoryName { get; set; }

        [DisplayName("Комментарий")]
        string CurrentComment { get; set; }
        Guid CurrentCommentId { get; set; }

        [DisplayName("Исполнитель")]
        string AssignedName { get; set; }

        [DisplayName("Исполнитель")]
        Guid? Assigned { get; set; }

        DateTime? DateAssigned { get; set; }

        [DisplayName("Изменен")]
        DateTime DateModified { get; set; }

        [DisplayName("Приоритет")]
        byte Priority { get; set; }

        Guid CreateMaster(string masterType, Guid wfGuid, Guid? masterId);

        Guid? ObjId { get; set; }
        object EnvelopObj { get; set; }
        IDictionary<string, IEnumerable<Guid>> CommonActionTickets { get; set; }

        void Create();
        void Load(Guid guid);

        void Save();
        void AutoClose();

        void LoadTicketByObjId(Guid objID);

    }
}
