using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompanyModel
{
    public class ServiceEditModel
    {
        public int? ParentId { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; }

        public List<SelectListItem> AvailableParents { get; set; }
    }
}
