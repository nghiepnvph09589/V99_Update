using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ItemOutputModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public long ItemPrice { get; set; }
        public string ImageUrl { get; set; }
        public int CateID { get; set; }
    }
}
