using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBoardModel.EditorModel
{
    public class PasteBoardEditModel
    {
        public Guid ProjectId { get; set; }
        [Required, DisplayName("Компания"), MaxLength(128)]
        public string CompanyName { get; set; }
        [Required, DisplayName("Название сайта"), MaxLength(128)]
        public string Name { get; set; }
        [DisplayName("Изображение заголовка")]
        public string HeaderImagePath { get; set; }
        [DisplayName("Логотип сайта")]
        public string LogoPath { get; set; }
        [DisplayName("Телефон"), MaxLength(64)]
        public string Phone { get; set; }
        [DisplayName("Слоган компании"), MaxLength(256)]
        public string Thesis { get; set; }
        [DisplayName("Город"), MaxLength(256)]
        public string CityName { get; set; }
        [DisplayName("Адрес"), MaxLength(512)]
        public string Address { get; set; }
        [DisplayName("Электронная почта"), MaxLength(256)]
        public string Mail { get; set; }
    }
}
