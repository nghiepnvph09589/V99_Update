using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListHistoryOutputModel
    {
        public int ID { set; get; }
        public string PointAddCode { set; get; }
        public int Type { set; get; }
        public int CustomerID { set; get; }
        public string CustomerName { set; get;}
        public string CustomerPhone { set; get;}
        public string NameProduct { set; get; }
        public DateTime? ActiveDate { set; get; }
        public DateTime? ExpireDate { set; get; }
        public double Point { set; get; }
        public double Balance { set; get; }
        public DateTime? CreateDate { set; get; }
        public string GetStringCreateDate
        {
            set {}
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string GetStringActiveDate
        {
            set {}
            get
            {
                return ActiveDate.HasValue ? ActiveDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string GetStringExpireDate
        {
            set {}
            get
            {
                return ExpireDate.HasValue ? ExpireDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
    }
}
