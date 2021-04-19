using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class NewsDetailModel
    {
        public NewsAppOutputModel newsDetail { get; set; }
        public List<NewsAppOutputModel> newsRelative { get; set; }
    }
}
