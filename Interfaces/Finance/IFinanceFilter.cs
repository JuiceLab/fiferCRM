using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Finance
{
    public interface IFinanceFilter
    {
        DateTime[] DateRange { get;}
        IList<string> DateRangeInvariant { get; set; }
        IList<decimal> PriceRange { get; set; }
        Dictionary<int, object> AdditionalFilters { get; set; }
    }
}
