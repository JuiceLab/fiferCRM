using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface ISiteMenuItem : IMenuItem
    {
        bool IsCurrent { get; set; }
        int MenuItemId { get; set; }
        int? ParentMenuItemId { get; set; }
    }
}
