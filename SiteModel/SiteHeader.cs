using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class SiteHeader : ISiteHeader
    {
        public string HeaderImage { get; set; }

        public string LogoPath { get; set; }

        public string Name { get; set; }

        public string Thesis { get; set; }

        public string Phone { get; set; }

        public string CityName { get; set; }
    }
}
