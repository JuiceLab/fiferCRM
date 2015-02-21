using Interfaces.CRM;
using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class FinanceStatInteractive : IFinanceStatInteractive
    {
        public Guid UserId { get; set; }

        public IFinanceFilter FinanceFilter { get; set; }

        public IDictionary<DateTime, IEnumerable<IPayment>> Transaction{ get; set; }

        public IDictionary<DateTime, IEnumerable<decimal>> Balance
        {
            get
            {
                Dictionary<DateTime, IEnumerable<decimal>> model = new Dictionary<DateTime, IEnumerable<decimal>>();
                foreach (var item in Transaction)
                {
                    model.Add(item.Key, new decimal[]{ 
                        item.Value.Where(m=>m.Total<0).Sum(m=>m.Total), 
                        item.Value.Where(m=>m.Total>0).Sum(m=>m.Total) });
                }
                return model;
            }
        }

        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }

        public int CompanyId { get; set; }

        public string Logo { get; set; }

        public string Name { get; set; }
    }
}
