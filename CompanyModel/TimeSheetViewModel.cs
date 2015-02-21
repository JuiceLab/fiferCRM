using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class TimeSheetViewModel
    {
        public int EmployeeId { get; set; }
        public string  UserName { get; set; }
        public DateTime Created { get; set; }
        public string IpIn { get; set; }
        public TimeSpan TimeIn { get; set; }
        public string IpOut { get; set; }
        public TimeSpan? TimeOut { get; set; }

    }
}
