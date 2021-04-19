using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class UserInforOutputModel : CustomerDetailOutputModel
    {
        public int UserID { get; set; }
        //public int IsNeedUpdate { get; set; }
        public int IsAgent { get; set; }
        public string Token { get; set; }
        public double PointRanking { get; set; }
        public string RankName { get; set; }
        public string Description { get; set; }
        public string NoteNextLevel { get; set; }
        public int RankLevel { get; set; }
        public int Status { get; set; }
        public string LastRefCode { get; set; }

    }
}
