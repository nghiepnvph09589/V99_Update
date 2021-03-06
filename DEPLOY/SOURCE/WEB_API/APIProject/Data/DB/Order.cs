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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public int CustomerID { get; set; }
        public long Discount { get; set; }
        public long TotalPrice { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int IsActive { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public Nullable<double> PointAdd { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string LastRefCode { get; set; }
        public Nullable<double> PointRanking { get; set; }
        public Nullable<double> Point { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual District District { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Province Province { get; set; }
    }
}
