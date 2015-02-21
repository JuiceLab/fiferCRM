using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskModel.Task;

namespace FilterModel
{
    public class TaskSearchFilter
    {
        IFormatProvider provider = new CultureInfo("ru-RU").DateTimeFormat;

        public List<string> Assigned { get; set; }
        public List<byte> Statuses { get; set; }

        public List<SelectListItem> AssignedAvailable { get; set; }
        
        public DateTime[] DateRange
        {
            get
            {
                return DateRangeInvariant != null ?
                    DateRangeInvariant.Select(m => Convert.ToDateTime(m, provider)).ToArray()
                    : null;
            }
        }

        public IList<string> DateRangeInvariant { get; set; }
        public IEnumerable<TaskPreview> SearchResult { get; set; }

        public TaskSearchFilter()
        {
            DateRangeInvariant = new List<string>() { 
                DateTime.Now.ToString("dd.MM.yyyy"),
                DateTime.Now.AddMonths(1).ToString("dd.MM.yyyy") 
            };
        }

    }
}
