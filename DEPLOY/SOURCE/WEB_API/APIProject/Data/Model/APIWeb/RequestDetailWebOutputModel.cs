using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class RequestDetailWebOutputModel
    {
        public int ID { set; get; }
        public double Point { set; get; }
        public string Name { set; get; }
        public string Note { set; get; }
        public string Phone { set; get; }
        public string ImageUrl { set; get; }
        public string BankAccout { set; get; }
        public string BankOwner { set; get; }
        public int? Status { set; get; }
        public string BankName { set; get; }
        public double TotalPoint { set { } get {
                return (Point * 1000);
                    } }
        public DateTime? CreateDate { set; get; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string UserActiveName { set; get; }

    }
}
