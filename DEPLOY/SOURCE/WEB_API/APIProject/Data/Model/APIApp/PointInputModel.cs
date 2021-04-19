using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class PointInputModel
    {
        public double? point { get; set; }
        public int BankID { get; set; }
        public string note { get; set; }
        public string phone { get; set; }
    }
}
