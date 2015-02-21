using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Finance
{
    public interface ILegalDetail
    {
        int LegalEntityId { get; set; }
        Int64 INN { get; set; }
        Int64 KPP { get; set; }
        Int64 OGRN { get; set; }
        Int64 RS { get; set; }
        Int64? KS { get; set; }
        Int64? BIK { get; set; }
        string PaymentLocation { get; set; }
        bool IsActive { get; set; }
    }
}
