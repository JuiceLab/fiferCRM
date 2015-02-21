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
    
    public partial class GeoLocation
    {
        public GeoLocation()
        {
            this.LegalEnityDetails = new HashSet<LegalEnityDetail>();
            this.LegalEntities = new HashSet<LegalEntity>();
            this.LegalEntities1 = new HashSet<LegalEntity>();
        }
    
        public int GeoLocationId { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public System.Guid CityGuid { get; set; }
    
        public virtual ICollection<LegalEnityDetail> LegalEnityDetails { get; set; }
        public virtual ICollection<LegalEntity> LegalEntities { get; set; }
        public virtual ICollection<LegalEntity> LegalEntities1 { get; set; }
    }
}
