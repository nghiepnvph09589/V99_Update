using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListHistoriesPointAppOutputModel
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int totalPage { get; set; }
        public IPagedList<HistoryPointMemberModel> listHistoriesPointMember { get; set; }
    }
}
