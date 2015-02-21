using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class SiteFooter : ISiteFooter
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Mail { get; set; }
    }
}
