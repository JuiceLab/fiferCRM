using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class CompanyEmployeeViewModel
    {
        public int? EmployeeId { get; set; }
        [DisplayName("Фамилия")]
        public string FirstName { get; set; }
        [DisplayName("Имя")]
        public string LastName { get; set; }
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }
        [DisplayName("ФИО в род. падеже")]
        public string FullNameGenetive { get; set; }
        [DisplayName("Дата рождения")]
        public DateTime? BirthDate { get; set; }
        [DisplayName("Фотография сотрудника")]
        public string PhotoPath { get; set; }
        [DisplayName("Должность")]
        public string PositionName { get; set; }
        [DisplayName("Должность в род. падеже")]
        public string PositionNameGenetive { get; set; }

        public int? Position { get; set; }

        public Guid? UserId { get; set; }
        [DisplayName("Последний вход")]
        public DateTime? LastLogin { get; set; }
        [DisplayName("Уволен")]
        public bool IsDissmissed { get; set; }
    }
}
