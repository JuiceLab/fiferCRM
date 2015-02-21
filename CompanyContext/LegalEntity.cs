//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CompanyContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class LegalEntity
    {
        public LegalEntity()
        {
            this.Companies = new HashSet<Company>();
            this.LegalEnitity4Companies = new HashSet<Company>();
        }
    
        public int LegalEntityId { get; set; }
        public bool IsActive { get; set; }
        public long INN { get; set; }
        public long KPP { get; set; }
        public long OGRN { get; set; }
        public long RS { get; set; }
        public Nullable<long> KS { get; set; }
        public Nullable<long> BIK { get; set; }
        public string PayLocation { get; set; }
        public Nullable<int> C_LegalAddrId { get; set; }
        public System.DateTime Created { get; set; }
        public System.Guid CreatedBy { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Company> LegalEnitity4Companies { get; set; }
    }
}
