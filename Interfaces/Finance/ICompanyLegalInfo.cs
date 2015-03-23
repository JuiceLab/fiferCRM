using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Finance
{
    public interface ICompanyLegalInfo
    {
        int LegalEntityId { get; set; }
        string LegalName { get; set; }
        
        string Phones { get; set; }

        string Mails { get; set; }

        string Sites { get; set; }

        string Address { get; set; }
        int? DistrictId { get; set; }
        Guid City { get; set; }
        string AddApp { get; set; }
        string Street { get; set; }
        string App { get; set; }
        string Offices { get; set; }
    }
}
