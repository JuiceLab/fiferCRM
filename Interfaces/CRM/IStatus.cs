using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface IStatus
    {
        [DisplayName("Статус")]
        string Status { get; set; }

        [DisplayName("Просмотр")]
        bool IsNonePreview { get; set; }

        Guid OwnerId { get; set; }

        Guid ChangeStatus(int statusId, string customStatus = "");

        void ChangePreview(bool value);
    }
}
