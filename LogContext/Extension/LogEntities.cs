using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogContext
{
    public partial class LogEntities
    {
        public LogEntities(string connectioString)
            :base(connectioString)
        { }
    }
}
