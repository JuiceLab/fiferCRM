using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CityPreview
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }

        public int Code { get; set; }
        public int RegionCode { get; set; }

        public int NumCompanies { get; set; }
        public int NumCustomers { get; set; }
    }
}
