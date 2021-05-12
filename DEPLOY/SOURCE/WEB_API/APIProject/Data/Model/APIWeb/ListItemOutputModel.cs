using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;
namespace Data.Model.APIWeb
{
    public class ListItemOutputModel
    {
        public int ID { set; get; }
        public string ItemCode { set; get; }
        public string Category { set; get; }
        public string ItemName { set; get; }
        public int ItemStatus { set; get; }
        public int StockStatus { set; get; }
        public double Price { set; get; }
        public double PriceVip { get; set; }
        public string ImgUrl { set; get; }
        public string Technical { set; get; }
        public string Description { set; get; }
        public DateTime? CreateDate { set; get; }
        public string GetStringCreateDate
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string GetNameStatus
        {
            get
            {
                return ItemStatus.Equals(SystemParam.ACTIVE_FALSE) ? "Ngừng hoạt động" : "Đang hoạt động";
            }
        }
        public string GetStockStatus
        {
            get
            {
                return StockStatus.Equals(SystemParam.OUT_OF_STOCK) ? "Hết hàng" : "Còn hàng";
            }
        }
    }
}
