using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class CompanyAddressViewModel
    {
        public int AddrId { get; set; }
        public int CompanyId { get; set; }
        public bool IsLegal { get; set; }
        [DisplayName("Почтовый индекс")]
        public int? ZipCode { get; set; }
        [DisplayName("Город"), StringLength(128, ErrorMessage = "Название города не должно превышать 128 символов."), Required]
        public string City { get; set; }
        [DisplayName("Улица"), StringLength(256, ErrorMessage = "Название улицы не должно превышать 256 символов."), Required]
        public string Street { get; set; }
        [DisplayName("Дом"), Required]
        public int Number { get; set; }
        [DisplayName("Доп. адрес (строение, корпус и т.п.)")]
        public string AddNumber { get; set; }
        [DisplayName("Телефоны")]
        public string Phones { get; set; }
        [DisplayName("Электронная почта")]
        public string Mails { get; set; }
        [DisplayName("Широта")]
        public string Latitude { get; set; }
        [DisplayName("Долгота")]
        public string Longitude { get; set; }
        public IEnumerable<LegalEntityViewModel> LegalEntities { get; set; }
    }
}
