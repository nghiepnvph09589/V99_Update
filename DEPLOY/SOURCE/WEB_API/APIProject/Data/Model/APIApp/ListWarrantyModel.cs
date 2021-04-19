using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListWarrantyModel : ListItemModel
    {
        public int id { get; set; }
        public string warrantyCode { get; set; }
        public string customerName { get; set; }
        //public ListItemModel product { get; set; }
        public DateTime activeDate { get; set; }
        public string active
        {
            get
            {
                return activeDate.ToString(SystemParam.CONVERT_DATETIME);
            }
            set { }
        }
        public DateTime expireDate { get; set; }
        public string expire
        {
            get
            {
                return expireDate.ToString(SystemParam.CONVERT_DATETIME);
            }
            set { }
        }
        public string categoryName { get; set; }
        public string parentCategoryName { get; set; }
    }
}
