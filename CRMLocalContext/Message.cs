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
    
    public partial class Message
    {
        public Message()
        {
            this.Message1 = new HashSet<Message>();
        }
    
        public int MessageId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public System.Guid SendFrom { get; set; }
        public System.Guid Recipient { get; set; }
        public bool IsChat { get; set; }
        public bool IsDraft { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<System.DateTime> Readed { get; set; }
        public Nullable<int> C_PrevMsgId { get; set; }
    
        public virtual ICollection<Message> Message1 { get; set; }
        public virtual Message Message2 { get; set; }
    }
}
