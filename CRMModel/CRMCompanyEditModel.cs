using CompanyModel;
using Interfaces.Company;
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
    public class CRMCompanyEditModel : ICompanyLegalInfo
    {
        public int LegalEntityId { get; set; }
        [DisplayName("Название(внутреннее)")]
        [Required]
        public string LegalName { get; set; }

        [DisplayName("Название")]
        [Required]
        public string PublicName { get; set; }

        [DisplayName("Телефоны")]
        public string Phones { get; set; }

        [DisplayName("Почта")]
        public string Mails { get; set; }

        [DisplayName("Сайты")]
        public string Sites { get; set; }
        [DisplayName("Скайп")]
        public string Skype { get; set; }

        [DisplayName("Регион/ область")]
        public int? DistrictId { get; set; }

        [DisplayName("Город")]
        public int City { get; set; }
        public Guid CityGuid { get; set; }

        [DisplayName("Адрес")]
        public string Address { get; set; }
        [DisplayName("Корпус")]
        public string AddApp { get; set; }
        [DisplayName("Улица")]
        public string Street { get; set; }
        [DisplayName("Дом")]
        public string App { get; set; }
        [DisplayName("Офисы")]
        public string Offices { get; set; }

        [DisplayName("Статус компании")]
        public byte StatusId { get; set; }

        [DisplayName("Доп. информация или заметка об организации"), MaxLength(1014)]
        public string Comment { get; set; }

        [DisplayName("Логотип")]
        public string PhotoPath { get; set; }

        [DisplayName("Закреплен за")]
        public Guid? AssignedBy { get; set; }

        [DisplayName("Виды деятельности")]
        public IList<int> Activities { get; set; }
        public GeoEditModel GeoAddr { get; set; }

        public IList<LegalEntityViewModel> Details { get; set; }
    }
}
