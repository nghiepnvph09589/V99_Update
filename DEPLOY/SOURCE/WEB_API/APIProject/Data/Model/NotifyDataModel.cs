using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class NotifyDataModel
    {
        public int id { get; set; }
        public int type { get; set; }
        public string code { get; set; }
        public double? Point { get; set; }
        public double? PointRaking { get; set; }
        public int StatusOrder { get; set; }
    }
}
