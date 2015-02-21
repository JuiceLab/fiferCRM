using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FinanceModel
{
    public class FinanceClause : ICompanyInfo
    {
        public IEnumerable<ActivityServiceItem> PaymentServices { get; set; }
        public IEnumerable<SelectListItem> Expenses { get; set; }


        public int CompanyId { get; set; }

        public string Logo { get; set; }

        public string Name { get; set; }

        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
    }
}
