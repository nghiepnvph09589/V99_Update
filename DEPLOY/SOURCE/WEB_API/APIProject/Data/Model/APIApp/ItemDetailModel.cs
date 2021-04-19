using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ItemDetailModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long Price { get; set; }
        public string Image { get; set; }
        // mô tả sản phẩm
        public string Description { get; set; }
        // thông số kỹ thuật
        public string Technical { get; set; }
        public int? StockStatus { get; set; }
    }
}
