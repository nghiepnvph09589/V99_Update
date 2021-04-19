using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class BankOutputModel
    {
        public int ID { get; set; }
        public string BankName { get; set; }
        public string ShortName { get; set; }
        public string ImageUrl { get; set; }
        public string CodeBankAccount { get; set; }
        public string UserName { get; set; }
    }
}
