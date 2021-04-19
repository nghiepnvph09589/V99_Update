using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class GroupItemChild
    {
        public Nullable<int> ParentID { get; set; }
        public string ParentName { get; set; }
        public int ChildID { get; set; }
        public string ChildName { get; set; }
        public List<ListItemModel> listProduct { get; set; }
    }
}
