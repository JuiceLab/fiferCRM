using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class WorkGroupViewModel
    {
        public int GroupId { get; set; }
        [DisplayName("Название"), Required]
        public string Name { get; set; }
        [DisplayName("Описание")]
        public string Descritpion { get; set; }

        public string Departments { get; set; }
        public string Employees { get; set; }

    }
}
