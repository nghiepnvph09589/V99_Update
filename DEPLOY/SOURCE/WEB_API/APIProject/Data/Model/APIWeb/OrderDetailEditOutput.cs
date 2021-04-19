using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class OrderDetailEditOutput
    {
        public class OrderItemEdit
        {
            public int ItemID { get; set; }
            public string ItemName { get; set; }
            public int ItemQTY { get; set; }
            public double ItemPrice { get; set; }
            public double ItemTotalPrice { get; set; }
            public string ItemCode { get; set; }

        }
        public List<OrderItemEdit> ListItem { get; set; }
        public int OrderID { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public string AgentCode { get; set; }
        public int Status { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public string Code { get; set; }
        public double? addPoint { get; set; }
        public double? PointNow { get; set; }
        public string LastRefCode { get; set; }

    }
}
