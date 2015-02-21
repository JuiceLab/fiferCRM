using EnumHelper;
using EnumHelper.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ITaskPeriod
    {
        Guid TaskPeriodId { get; set; }
        Guid? Assigned { get; set; }
        Guid TicketId { get; set; }
        TaskPeriodType PeriodType { get; set; }
        WFTaskStatus StatusId { get; set; }
        DateTime DateStarted { get; set; }
        DateTime DateBefore { get; set; }
        TimeSpan? NotifyBefore { get; set; }
        string Comment { get; set; }
        string AddNumber { get; set; }

        string DateBeforeStr{get;set;}
        string DateStartedStr { get; set; }

        string TimeStartedStr { get; set; }
    }
}
