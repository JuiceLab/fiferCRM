//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UserContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class Application
    {
        public Application()
        {
            this.AppCookies = new HashSet<AppCookie>();
        }
    
        public int AppId { get; set; }
        public string Description { get; set; }
        public string AppUrl { get; set; }
        public Nullable<System.Guid> AppGuid { get; set; }
    
        public virtual ICollection<AppCookie> AppCookies { get; set; }
    }
}
