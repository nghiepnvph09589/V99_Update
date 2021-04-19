using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class BatchDetailOutputModel
    {
        public int BatchID { get; set; }
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public int ProductID { get; set; }
        public string CreateUserName { get; set; }
        public int QTY { set; get; }
        public string Note { set; get; }
        public int Point { set; get; }
        public string ProductName { get; set; }
        public long Price { get; set; }
        public string PriceStr { get; set; }
        public string PointStr { get; set; }
        public string QtyStr { get; set; }
        public List<ProductOfBatchModel> ListProduct { set; get; }
    }
}
