using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterModel
{
    public class LegalEntitySearch : ILegalEntitySearch
    {
        public bool IsExist { get; set; }

        public Guid CompanyId { get; set; }
        [DisplayName("ИНН")]
        public Int64? INN { get; set; }
        [DisplayName("КПП")]
        public Int64? KPP { get; set; }
        [DisplayName("ОГРН")]
        public Int64? OGRN { get; set; }
        [DisplayName("р/c")]
        public Int64? RS { get; set; }
        [DisplayName("Название компании")]
        public string CompanyName { get; set; }
        public int ActivityId { get; set; }

        [DisplayName("Вид деятельности")]
        public string ActivityName { get; set; }
        [DisplayName("Сайт")]
        public string WebSite { get; set; }
        [DisplayName("Телефон")]
        public string Phone { get; set; }

        public string MaskedToPhone { get {  return string.IsNullOrEmpty(Phone)? Phone : Phone.Replace("+", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty).Replace((" "), string.Empty); } }

        [DisplayName("Электронная почта")]
        public string EMail { get; set; }

        public int DistrictId { get; set; }

        [DisplayName("Города")]
        public int? City { get; set; }


        [DisplayName("Виды деятельности")]
        public List<int> Services { get; set; }

        public bool IsLegalSearch {get
            { return OGRN.HasValue || RS.HasValue || INN.HasValue || KPP.HasValue; }
        }
    }
}
