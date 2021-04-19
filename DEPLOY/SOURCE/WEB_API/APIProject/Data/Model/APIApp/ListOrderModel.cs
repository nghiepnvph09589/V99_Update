using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListOrderModel
    {
        public int orderID { get; set; }
        public double totalPrice { get; set; }
        public string code { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public int qty { get; set; }
        public string Date
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME);
            }
            set { }
        }
        public DateTime CreateDate { get; set; }
    }
}
