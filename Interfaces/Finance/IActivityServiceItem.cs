using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Finance
{
    public class IActivityServiceItem
    {
        int ActivityId { get; set; }
        int? ParentId { get; set; }
        decimal? Cost { get; set; }
        double? Tax { get; set; }
        string Name { get; set; }
        string ActivityName { get; set; }

    }
}
