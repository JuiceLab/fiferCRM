using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Company
{
  public interface IGeoAddr
    {
        int GeoId { get; set; }
        double Lat { get; set; }
        double Long { get; set; }
        string Address { get; set; }
        string Comment { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
    }
}
