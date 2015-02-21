using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ICompanyInfo
    {
        int CompanyId { get; set; }
        string Logo { get; set; }
        string Name { get; set; }
        Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
    }
}
