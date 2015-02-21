using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface ISiteHeader
    {
        string HeaderImage { get; set; }
        string LogoPath { get; set; }
        string Name { get; set; }
        string Thesis { get; set; }
        string Phone { get; set; }
        string CityName { get; set; }
    }
}
