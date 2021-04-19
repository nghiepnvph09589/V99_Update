using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class HistoryPointMemberModel
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int HistoryID { get; set; }
        public int CusID { get; set; }
        public double Point { get; set; }
        public int Type { get; set; }
        public int TypeAdd { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public double Balance { get; set; }
        public DateTime CreateDate { get; set; }
        public double PointUser { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserSendName { get; set; }
        public string UserSendPhone { get; set; }

    }
}
