using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
   public class ListCategoryOutputModel
    {
        public int ID { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string ParentName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public string getStrCreateDate
        {
            get
            {
                try
                {
                    return this.CreateDate.ToString(Utils.SystemParam.CONVERT_DATETIME);
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
