using EnumHelper;
using EnumHelper.CRM;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class TaskPeriod : ITaskPeriod
    {
        public Guid TaskPeriodId { get; set; }
        public Guid? Assigned { get; set; }
        public Guid TicketId { get; set; }
      
        [DisplayName("Периодичность")]
        public TaskPeriodType PeriodType { get; set; }
        
        [DisplayName("Статус")]
        public WFTaskStatus StatusId { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateBefore { get; set; }
        public TimeSpan? NotifyBefore { get; set; }
        public string Comment { get; set; }
        public string AddNumber { get; set; }
        public string DateBeforeStr { get; set; }
        public string DateStartedStr { get; set; }
        public string TimeStartedStr { get; set; }
    }
}
