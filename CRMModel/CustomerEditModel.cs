using CompanyModel;
using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CustomerEditModel : ICustomerInfo
    {
        public Guid Guid { get; set; }
        public int CustomerId { get; set; }
        [DisplayName("Фамилия")]
        [Required]
        public string FirstName { get; set; }
        [DisplayName("Имя")]
        [Required]
        public string LastName { get; set; }
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }
        
        [DisplayName("Должность")]
        public string PositionName { get; set; }
        
        [DisplayName("Телефон")]
        public string Phone { get; set; }

        [DisplayName("Фотография")]
        public string PhotoPath { get; set; }

        [DisplayName("Почта")]
        public string Mail { get; set; }

        [DisplayName("Комментарий"), MaxLength(1014)]
        public string Comment { get; set; }
        [DisplayName("Социальные сети")]
        public string SocialLinks { get; set; }
        [DisplayName("Скайп")]
        public string Skype { get; set; }
        public Guid UserId { get; set; }
        
        [DisplayName("ЛПР?")]
        public bool IsLPR { get; set; }

        [DisplayName("Статус клиента")]
        public byte StatusId { get; set; }
        [DisplayName("Закреплен за")]
        public Guid? AssignedBy { get; set; }
        public Guid? CompanyId { get; set; }
        public PassportViewModel Passport { get; set; }
    }
}
