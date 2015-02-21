using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class PositionViewModel
    {
        public int PosId { get; set; }
        [DisplayName("Должность"), Required]
        public string Name { get; set; }
        [DisplayName("Должность в род. падеже")]
        public string NameGenetive { get; set; }
        [DisplayName("Отдел"),Required]
        public int DepartmentId { get; set; }
        [DisplayName("Отдел")]
        public string DepartmentName { get; set; }
        [DisplayName("Подчиняется")]
        public int? HeaderPosId { get; set; }
        [DisplayName("Активен?")]
        public bool IsActive { get; set; }

    }
}
