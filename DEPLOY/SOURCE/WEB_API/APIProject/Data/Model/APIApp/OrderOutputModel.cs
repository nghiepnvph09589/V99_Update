using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class OrderDetailOutputModel : OrderOutputModel
    {
        public double BasePrice { get { return listOrderItem.Select(u => u.SumPrice).Sum(); } set { } }
        public List<OrderDetailModel> listOrderItem { get; set; }
    }
    public class OrderDetailModel
    {
        public int? OrderItemID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }
        public long SumPrice { get; set; }
        public int Qty { get; set; }
        public int Warranty { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int Status { get; set; }
        public List<int> ListOrderItemID { get; set; }
    }


    public class ListCartOutputModel
    {
        public double TotalPrice { get; set; }
        public List<OrderDetailModel> listCart { get; set; }
    }


    public class OrderOutputModel
    {
        public int OrderID { get; set; }
        public string Code { get; set; }
        public double TotalPrice { get; set; }
        public int Status { get; set; }
        public int Qty { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public double Discount { get; set; }
        public int Type { get; set; }
        public int? ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastRefCode { get; set; }
        public double? Point { get; set; }
        public string Hour
        {
            get
            {
                return CreateDate.ToString("HH:mm");
            }
            set { }
        }
        public string Date
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME);
            }
            set { }
        }
        public DateTime? CancelDate { get; set; }
        public string CancelDateString
        {
            get
            {
                return CancelDate.HasValue ? CancelDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
            set { }
        }
        public DateTime? ConfirmDate { get; set; }
        public string ConfirmDateString
        {
            get
            {
                return ConfirmDate.HasValue ? ConfirmDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
            set { }
        }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateString
        {
            get
            {
                return PaymentDate.HasValue ? PaymentDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
            set { }
        }
    }



}
