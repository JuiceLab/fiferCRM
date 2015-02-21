using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface IGeoPage : IPage
    {
        string GeoLocationTitle { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        string Address { get; set; }
    }
}
