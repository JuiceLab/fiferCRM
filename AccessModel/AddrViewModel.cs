using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModel
{
    public class AddrViewModel
    {
        [DisplayName("Почтовый индекс")]
        public int ZipCode { get; set; }
        [DisplayName("Город"), Required(ErrorMessage = "Поле 'Город' обязательно для заполнения"),
        StringLength(128, ErrorMessage = "Название города не должно превышать 128 символов.")]
        public string City { get; set; }
        [DisplayName("Улица"), Required(ErrorMessage = "Поле 'Улица' обязательно для заполнения"),
        StringLength(256, ErrorMessage = "Название улицы не должно превышать 256 символов.")]
        public string Street { get; set; }
        [DisplayName("Дом"), Required(ErrorMessage = "Поле 'дом' обязательно для заполнения")]
        public int Number { get; set; }
        [DisplayName("Доп. адрес (строение, корпус и т.п.)")]
        public string AddNumber { get; set; }
        [DisplayName("Телефоны")]
        public string Phones { get; set; }
        [DisplayName("Электронная почта")]
        public string Mails { get; set; }
        [DisplayName("Широта")]
        public double Latitude { get; set; }
        [DisplayName("Долгота")]
        public string Longitude { get; set; }
    }
}
