using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;


namespace Data.Model.APIApp
{
    public class ListOrderOutputModel
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int lastPage { get; set; }
        public IPagedList<ListOrderModel> listOrder { get; set; }
    }
}
