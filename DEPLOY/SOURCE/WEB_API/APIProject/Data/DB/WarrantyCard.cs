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
    
    public partial class WarrantyCard
    {
        public int ID { get; set; }
        public Nullable<int> CustomerActiveID { get; set; }
        public string WarrantyCardCode { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int IsActive { get; set; }
        public Nullable<int> WarrantyID { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Warranty Warranty { get; set; }
    }
}
