using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class RequestPointModel
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public int? Status { get; set; }
        public double? Point { get; set; }
        public string Note { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public double? Balance { get; set; }
        public int? BankID { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public List<BankOutputModel> Listbank { get; set; }

        public long MinPoint { get; set; }
        public long Maxpoint { get; set; }
        public DateTime? CreatedDate { set; get; }
        public double? TotalMoney { get; set; }
        public BankInfo BankInfo { get; set; }
    }

    public class BankInfo
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string ShortName { get; set; }
        public string BankOwner { get; set; }
        public string UrlImg { get; set; }
    }
}
