//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SupportContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class TicketGroup
    {
        public int GroupTaskId { get; set; }
        public System.Guid TicketId { get; set; }
        public string AssignedUsers { get; set; }
        public string AssignedGroups { get; set; }
        public string AssignedDepartments { get; set; }
        public string UserStatuses { get; set; }
    
        public virtual Ticket Ticket { get; set; }
    }
}
