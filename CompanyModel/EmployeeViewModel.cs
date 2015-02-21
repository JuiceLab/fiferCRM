using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public Guid UserId { get; set; }
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
        [DisplayName("Фото сотрудника")]
        public string PhotoPath { get; set; }
        [DisplayName("Последний вход")]
        public DateTime LastLogin { get; set; }
        [DisplayName("Должность")]
        public int? PositionId { get; set; }
        public string Position { get; set; }
        [DisplayName("Отдел")]
        public string Department { get; set; }
        public int? PassportId { get; set; }
        [DisplayName("Уволен?")]
        public bool IsDismissed { get; set; }
        [DisplayName("Онлайн?")]
        public bool IsOnline { get; set; }
        [DisplayName("Руководящая должность?")]
        public bool IsKeyEmployee { get; set; }
        [DisplayName("Ссылки на соц.ресуры( через ',')")]
        public string SocialLinks { get; set; }

        [DisplayName("Скайп")]
        public string Skype { get; set; }

        [DisplayName("ICQ")]
        public string ICQ { get; set; }

        [DisplayName("Положительные характеристики")]
        public string PositiveOpinion { get; set; }
        [DisplayName("Отрицательные характеристики")]
        public string NegativeOpinion { get; set; }
        [DisplayName("Уровень доступа к данным компании"), Required(ErrorMessage = "Обязательно укажите уровень доступа")]
        public int FuncRoleId { get; set; }

        [DisplayName("Руководитель группы")]
        public int? GroupHeaderId { get; set; }

        public List<int> WorkGroups { get; set; }

        [DisplayName("Оклад")]
        public decimal? Salary { get; set; }
        [DisplayName("% от сделки")]
        public double? TransactionPercent { get; set; }

        [DisplayName("Телефон")]
        public string Phone { get; set; }

        [DisplayName("EMail")]
        public string Mail { get; set; }

    }
}
