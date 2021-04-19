using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class GetListHistoryMemberPointInputModel
    {
        public int HistoryID { set; get; }
        public string AddPointCode { get; set; }
        public int Type{ set; get; }
        public int TypeAdd { set; get; }
        public double Point { set; get; }
        public string Comment { get; set; }
        public DateTime? CreateDate { set; get; }
        public string UserSend { get; set; }
        public string BankAccount { get; set; }
        public string BankOwner { get; set; }
        public string BankName { get; set; }
        public string Phone { get;set; }
        public string Title { get; set; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
    }
}
