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
    
    public partial class Message
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string Content { get; set; }
        public int Viewed { get; set; }
        public int Type { get; set; }
        public System.DateTime CraeteDate { get; set; }
        public int IsActive { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
    }
}
