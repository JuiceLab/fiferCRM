using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }
        [DisplayName("Отдел"), Required]
        public string Title { get; set; }
        [DisplayName("Описание")]
        public string Description { get; set; }
        [DisplayName("Активен?")]
        public bool IsActive { get; set; }
        [DisplayName("Рабочие группы")]
        public List<int> Groups { get; set; }
        public DepartmentViewModel()
        {
            Groups = new List<int>();
        }

    }
}
