using Interfaces.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class GeoEditModel : IGeoAddr
    {
        public int GeoId { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set;}
    }
}
