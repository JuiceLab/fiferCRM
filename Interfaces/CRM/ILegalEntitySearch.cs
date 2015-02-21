using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CRM
{
    public interface ILegalEntitySearch
    {
        [DisplayName("Регион/область")]
        int DistrictId { get; set; }
        [DisplayName("ИНН")]
        Int64? INN { get; set; }
        [DisplayName("КПП")]
        Int64? KPP { get; set; }
        [DisplayName("ОГРН")]
        Int64? OGRN { get; set; }
        [DisplayName("р/c")]
        Int64? RS { get; set; }
        [DisplayName("Название компании")]
        string CompanyName { get; set; }
        int ActivityId { get; set; }

        [DisplayName("Вид деятельности")]
        string ActivityName { get; set; }
        [DisplayName("Сайт")]
        string WebSite { get; set; }
        [DisplayName("Телефон")]
        string Phone { get; set; }
        [DisplayName("Электронная почта")]
        string EMail { get; set; }

        [DisplayName("Города")]
        List<int> City { get; set; }

        [DisplayName("Виды деятельности")]
        List<int> Services { get; set; }
    }
}
