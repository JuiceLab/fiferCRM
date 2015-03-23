using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompanyModel
{
    public class CompanyViewModel
    {
        public Guid? Guid { get; set; }
        public int CompanyId { get; set; }
        public int? CurLegalAddrId { get; set; }
        public int CurAddrId { get; set; }

        [DisplayName("Полное название компании"), StringLength(128, ErrorMessage = "Название компании не должно превышать 128 символов.")]
        public string CompanyName { get; set; }

        [DisplayName("Короткое название компании")]
        public string PublicCompanyName { get; set; }
        
        [DisplayName("Сайт комании")]
        public string SiteUrl{ get; set; }
        [DisplayName("ICQ")]
        public string ICQ { get; set; }
        [DisplayName("Электронная почта")]
        public string Mail { get; set; }
        [DisplayName("Скайп")]
        public string Skype { get; set; }
        [DisplayName("Факс")]
        public string Fax { get; set; }
        
        [DisplayName("Услуги")]
        public IEnumerable<int> Services { get; set; }
        public IEnumerable<SelectListItem> AvailableServices { get; set; }

        [DisplayName("Согласен на предоставление данных другим компаниям, работающим в системе")]
        public bool IsPublic { get; set; }

        public string Logo { get; set; }

        public string UserPhoto { get; set; }

        public IEnumerable<CompanyAddressViewModel> Addresses { get; set; }

        public IEnumerable<CompanyEmployeeViewModel> KeyEmployees { get; set; }

        public LegalEntityViewModel LegalEntity { get; set; }

        public int DistrictId { get; set; }
    }
}
