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
    
    public partial class PageBlock
    {
        public int PageBlockId { get; set; }
        public string BlobUrl { get; set; }
        public Nullable<System.DateTime> AvailableTo { get; set; }
        public int C_LandingBlockId { get; set; }
        public int C_LandingPageId { get; set; }
    
        public virtual LandingBlock LandingBlock { get; set; }
        public virtual LandingPage LandingPage { get; set; }
    }
}
