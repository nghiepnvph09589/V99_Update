using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class LoginAppOutputModel
    {
        public int UserID { get; set; }
        public int Type { get; set; }
        public string Token { get; set; }
        public int Role { get; set; }
        public int IsUpdate { get; set; }
    }
}
