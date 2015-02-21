using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AccessModel
{
    public class CompanyRegisterModel
    {
        [DisplayName("Полное название компании"),
        Required(ErrorMessage = "Укажите полное название компании"),
        StringLength(128, ErrorMessage = "Название компании не должно превышать 128 символов.")]
        public string CompanyName { get; set; }

        [DisplayName("Согласен на предоставление данных другим компаниям, работающим в системе")]
        public bool IsPublic { get; set; }

        [DisplayName("Фамилия"),
        Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения"),
        StringLength(50, ErrorMessage = "Фамилия должна быть длиной от 3 до 50 символов.", MinimumLength = 3)]
        public string FirstName { get; set; }

        [DisplayName("Имя"),
        Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения"),
        StringLength(50, ErrorMessage = "Имя должен быть длиной от 3 до 50 символов.", MinimumLength = 3)]
        public string LastName { get; set; }
        [DisplayName("Email"),
        DataType(DataType.EmailAddress),
        Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        public string Email { get; set; }

        [DisplayName("Контактный телефон"), Required(ErrorMessage = "Контактный телефон обязателен для заполнения")]
        public string Phone { get; set; }

        [DisplayName("Почтовый индекс")]
        public int? ZipCode { get; set; }
        
        [DisplayName("Регион"), Required(ErrorMessage = "Укажите регион")]
        public int DistrictId { get; set; }
        [DisplayName("Город"), Required(ErrorMessage = "Укажите город"),
        StringLength(128, ErrorMessage = "Название города не должно превышать 128 символов.")]
        public string City { get; set; }
        [DisplayName("Улица"), Required(ErrorMessage = "Укажите улицу"),
        StringLength(256, ErrorMessage = "Название улицы не должно превышать 256 символов.")]
        public string Street { get; set; }
        [DisplayName("Дом"), Required(ErrorMessage = "Поле 'дом' обязательно для заполнения")]
        public int? Number { get; set; }
        [DisplayName("Доп. адрес (строение, корпус и т.п.)")]
        public string AddNumber { get; set; }
        [DisplayName("Юр. адрес совпадает с фактическим")]
        public bool IsLegalAddress { get; set; }
        public List<SelectListItem> AvailableCities { get; set; }
        public CompanyRegisterModel()
        {
            IsPublic = true;
            AvailableCities = new List<SelectListItem>();
        }
    }
}
