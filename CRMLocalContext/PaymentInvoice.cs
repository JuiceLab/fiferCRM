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
    
    public partial class PaymentInvoice
    {
        public PaymentInvoice()
        {
            this.PaymentDetails1 = new HashSet<PaymentDetail>();
        }
    
        public int PaymentInvoiceId { get; set; }
        public string PaymentNumber { get; set; }
        public System.DateTime PaymentBefore { get; set; }
        public string PaymentDetails { get; set; }
        public string Comment { get; set; }
        public decimal TotalValue { get; set; }
        public Nullable<decimal> TotalPayed { get; set; }
        public bool IsCash { get; set; }
        public System.Guid FromCompany { get; set; }
        public System.Guid FromEmployee { get; set; }
        public byte StatusId { get; set; }
        public System.DateTime Created { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.Guid> SubmitedBy { get; set; }
        public Nullable<System.DateTime> SubmitedDate { get; set; }
        public Nullable<int> C_FromLegalEntityDetailID { get; set; }
    
        public virtual LegalEnityDetail LegalEnityDetail { get; set; }
        public virtual ICollection<PaymentDetail> PaymentDetails1 { get; set; }
    }
}
