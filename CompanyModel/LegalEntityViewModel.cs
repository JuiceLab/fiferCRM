using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class LegalEntityViewModel : ILegalDetail
    {
        public int LegalEntityId { get; set; }
        [DisplayName("ИНН"), Required]
        public Int64 INN { get; set; }
        [DisplayName("КПП"), Required]
        public Int64 KPP { get; set; }
        [DisplayName("ОГРН"), Required]
        public Int64 OGRN { get; set; }
        [DisplayName("р/c"), Required]
        public Int64 RS { get; set; }
        [DisplayName("к/c")]
        public Int64? KS { get; set; }
        [DisplayName("БИК")]
        public Int64? BIK { get; set; }
        [DisplayName("В каком банке?")]
        public string PaymentLocation { get; set; }
        public bool IsActive { get; set; }
    }
}
