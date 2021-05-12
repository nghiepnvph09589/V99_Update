using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CreateItemInputModel
    {
        public int ID { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public int Status { set; get; }
        public int StockStatus { set; get; }
        public string Price { set; get; }
        public string PriceVip { set; get; }
        public int CategoryID { set; get; }
        public string ImageUrl { set; get; }
        public string Technical { set; get; }
        public int Warranty { set; get; }
        public string Note { set; get; }
    }
}
