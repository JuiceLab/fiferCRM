using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class SiteProjectModel
    {
        public int SiteId { get; set; }
        public Guid SiteGuid { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public byte Type { get; set; }
        public int CountPages { get; set; }
    }
}
