//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRMLocalContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class LegalActivity
    {
        public int LegalActivityId { get; set; }
        public string ActivityFullName { get; set; }
        public int C_LegalEntityId { get; set; }
        public int C_ServiceId { get; set; }
        public Nullable<double> EmployeePercentBonus { get; set; }
        public Nullable<decimal> SelfCost { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public bool HasNDS { get; set; }
    
        public virtual LegalEnityDetail LegalEnityDetail { get; set; }
        public virtual CompanyService CompanyService { get; set; }
    }
}
