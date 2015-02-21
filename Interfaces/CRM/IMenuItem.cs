using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface IMenuItem
    {
        string Icon { get; set; }
        string Name { get; set; }
        string Url { get; set; }
        bool IsActive { get; set; }
    }
}
