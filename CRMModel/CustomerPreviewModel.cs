using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMModel
{
    public class CustomerPreviewModel
    {
        public Guid CustomerId { get; set; }
        [DisplayName("Фамилия")]
        public string FirstName { get; set; }
        [DisplayName("Имя")]
        public string LastName { get; set; }
        [DisplayName("Отчество")]
        public string Patronymic { get; set; }
        [DisplayName("Телефон")]
        public string Phone { get; set; }
        [DisplayName("Почта")]
        public string Mail { get; set; }
        [DisplayName("Должность")]
        public string Position { get; set; }        
        [DisplayName("ЛПР?")]
        public bool IsKeyEmployee { get; set; }
    }
}
