//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerBank
    {
        public int ID { get; set; }
        public string BankOwner { get; set; }
        public int CustomerID { get; set; }
        public int BankID { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public string BankAccount { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual Customer Customer { get; set; }
    }
}