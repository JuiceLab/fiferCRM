using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModel
{
    public class EmployeeRegisterModel : RegisterModel
    {
        [DisplayName("Должность"),
        Required(ErrorMessage = "Должность обязательна для заполнения")]
        public int PositionId { get; set; }

        [DisplayName("Уровень доступа к данным компании"),
        Required(ErrorMessage = "Обязательно укажите уровень доступа")]
        public int FuncRoleId { get; set; }
        
        [DisplayName("Оклад")]
        public decimal? Salary { get; set; }
        [DisplayName("% от сделки")]
        public double? TransactionPercent { get; set; }
    }
}
