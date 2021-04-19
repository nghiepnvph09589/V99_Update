using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class HomeScreenOutPutModel
    {
        public UserInforOutputModel customerInfo { get; set; }
        public List<NewsAppOutputModel> listNews { get; set; }
        public List<NewsAppOutputModel> listBanner { get; set; }
        public List<NewsAppOutputModel> listEnvent { get; set; }
        public  CustomerLogin UserInfo { get; set; }
        // public List<ListItemModel> listProduct { get; set; }
    }
    public class CustomerLogin
    {
        public string CustomerName { get; set; }
        public double Point { get; set; }
    }
}
