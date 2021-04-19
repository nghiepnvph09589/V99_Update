using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class StatisticBusiness : GenericBusiness
    {
        public StatisticBusiness(TichDiemTrieuDo context = null) : base()
        {

        }

        public long Revenue()
        {
            var revenue = (from order in cnn.Orders
                            where order.IsActive.Equals(SystemParam.ACTIVE) && order.Status.Equals(SystemParam.STATUS_ORDER_PAID)
                            select (int?)order.TotalPrice).Sum()??0;
            //var revenue = (cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_PAID && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice));
            return revenue;
        }

        public List<Order> Search(int? obj, string FromDate, string ToDate)
        {
            try
            {
                var query = cnn.Orders.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(2));
                if (obj != null)
                    query = query.Where(x => x.Customer.Role.Equals(obj.Value));
                if (FromDate != "" && FromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(FromDate); ;
                    query = query.Where(x => x.CreateDate >= fd);
                }

                if (ToDate != "" && ToDate != null)
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(x => x.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                    return query.OrderByDescending(x => x.CreateDate).ToList();
                else
                    return new List<Order>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<Order>();
            }
        }
        public string countRevenue()
        {
            var listRevenue = cnn.Orders.Where(u => u.IsActive.Equals(1) && u.Status.Equals(2));
            int activeOrder = listRevenue.Where(u => u.Status.Equals(2)).Count();
            int countOder = listRevenue.Count();
            return "" + activeOrder + "/" + countOder;
        }

        public CountNotiOuputModel CountNoti()
        {
            CountNotiOuputModel data = new CountNotiOuputModel();
            var order = cnn.Orders.Where(o => o.IsActive == SystemParam.ACTIVE && o.Status == SystemParam.STATUS_ORDER_PENDING).Count();
            var request = cnn.Requests.Where(r => r.IsActive == SystemParam.ACTIVE && r.Status == SystemParam.STATUS_ORDER_PENDING).Count();
            data.Request = request;
            data.Order = order;
            data.All = request + order;
            return data;
        }
    }
}
