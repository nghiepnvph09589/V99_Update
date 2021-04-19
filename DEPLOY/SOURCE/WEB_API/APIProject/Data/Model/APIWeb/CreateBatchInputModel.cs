using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CreateBatchInputModel
    {
        public int ID { set; get; }
        public string BatchCode { get; set; }
        public string BatchName { set; get; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Price { set; get; }
        public string QTY { get; set; }
        public string Point { get; set; }
        public string Note { set; get; }
        public int CreateUserID { set; get; }

    }
}
