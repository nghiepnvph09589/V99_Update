using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CartOutputModel
    {
        public int CountCart { get; set; }
        public OrderDetailModel Cart { get; set; }
    }
}
