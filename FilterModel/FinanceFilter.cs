using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterModel
{
    public class FinanceFilter : IFinanceFilter
    {
        IFormatProvider provider = new CultureInfo("ru-RU").DateTimeFormat;

        public DateTime[] DateRange
        {
            get
            {
                return DateRangeInvariant !=null? 
                    DateRangeInvariant.Select(m => Convert.ToDateTime(m, provider)).ToArray() 
                    : null;
            }
        }

        public IList<string> DateRangeInvariant { get; set; }
        
        public IList<decimal> PriceRange { get; set; }

        public Dictionary<int, object> AdditionalFilters { get; set; }

        public FinanceFilter()
        { 
            var isWeekend= DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday;
            DateRangeInvariant = new string[] { DateTime.Now.Date.AddDays(-7).ToShortDateString(), DateTime.Now.AddDays(isWeekend ? 3 : 1).ToShortDateString() };
        }

        public FinanceFilter(string from, string to)
        {
            DateRangeInvariant = new string[] { from, to };
        }
    }
}
