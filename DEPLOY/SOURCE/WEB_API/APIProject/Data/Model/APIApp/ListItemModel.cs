using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListItemModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<string> Image { get; set; }
        public long Price { get; set; }
        public string Description { get; set; }
        public string Technical { get; set; }
        public int Warranty { get; set; }
        public int Special { get; set; }
        public int? StockStatus { get; set; }
    }

    public class ActiveWarrantyModel : ListItemModel
    {
        public ActiveCustomerInfo activeCustomerInfo { get; set; }
        public DateTime? activeDate { get; set; }
        public string active
        {
            get
            {
                return activeDate.HasValue ? activeDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public DateTime? expireDate { get; set; }
        public string expire
        {
            get
            {
                return expireDate.HasValue ? expireDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
    }

    public class ActiveCustomerInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }


}
