using CRMModel;
using Interfaces.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketModel.Interfaces
{
    public interface IFiferTask : ITicket
    {
        DateTime? NotifyDate { get; }
        string NotifyBeforeStr { get; set; }
        TaskGroup Group { get; set; }
        List<TaskPeriod> Periods { get; set; }
        bool HasGroup { get; set; }
        bool HasPeriods { get; set; }
        void CreatePeriods(Guid userId);
        void LoadPeriods(Guid userId);
        void UpdatePeriods(Guid userId);

        void CreateGroup();
        void LoadGroup();
        void UpdateGroup();

    }
}
