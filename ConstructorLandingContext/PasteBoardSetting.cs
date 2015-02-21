//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConstructorLandingContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class PasteBoardSetting
    {
        public PasteBoardSetting()
        {
            this.PasteBoardPages = new HashSet<PasteBoardPage>();
        }
    
        public int SiteSettingsId { get; set; }
        public System.Guid SiteGuid { get; set; }
        public System.Guid TemplateGuid { get; set; }
        public string IconPath { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string HomePageUrl { get; set; }
        public string CompanyTesis { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CssCustom { get; set; }
        public string BackgroundImage { get; set; }
        public string HeaderImagePath { get; set; }
        public string Mail { get; set; }
        public string TemplateName { get; set; }
        public Nullable<double> Latitiude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string CityName { get; set; }
    
        public virtual ICollection<PasteBoardPage> PasteBoardPages { get; set; }
    }
}
