using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class SiteMenuItem : ISiteMenuItem
    {
        public bool IsCurrent { get; set; }

        public int MenuItemId { get; set; }

        public int? ParentMenuItemId { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsActive { get; set; }
    }
}
