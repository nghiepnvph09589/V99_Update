using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class NotifyBusiness : GenericBusiness
    {
        public NotifyBusiness(TichDiemTrieuDo context = null) : base()
        {
        }
        public List<NotifiedByCustomerIDOutputModel> GetListNotify(int CusID)
        {
            List<NotifiedByCustomerIDOutputModel> query = new List<NotifiedByCustomerIDOutputModel>();
            var listnotify = from n in cnn.Notifications
                             where n.CustomerID.HasValue ? n.CustomerID.Value.Equals(CusID) : true && n.IsActive.Equals(SystemParam.ACTIVE)
                             orderby n.CreateDate descending
                             select new NotifiedByCustomerIDOutputModel
                             {
                                 NotifyID = n.ID,
                                 ObjectID = n.NewsID,
                                 Content = n.Content,
                                 CreatedDate = n.CreateDate,
                                 Viewed = n.Viewed,
                                 Title = n.Title,
                                 Type = n.Type.Value,
                             };
            if (listnotify != null && listnotify.Count() > 0)
            {
                query = listnotify.Take(50).ToList(); 
            }
            return query;
        }

        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        public void CreateNoti(int cusID, int type, string title, string content, int? objectID)
        {
            Notification noti = new Notification();
            noti.CustomerID = cusID;
            noti.Viewed = 0;
            noti.CreateDate = DateTime.Now;
            noti.IsActive = SystemParam.ACTIVE;
            noti.Type = type;
            noti.NewsID = objectID;
            noti.Title = title;
            noti.Content = content;
            cnn.Notifications.Add(noti);
            cnn.SaveChanges();
        }


        public void CreateNoties(int cusID, int type, int newsID, string content, string title, TichDiemTrieuDo cnns)
        {
            Notification noti = new Notification();
            noti.CustomerID = cusID;
            noti.Viewed = 0;
            noti.CreateDate = DateTime.Now;
            noti.IsActive = SystemParam.ACTIVE;
            noti.Type = type;
            if (type == SystemParam.NOTIFY_NAVIGATE_NEWS)
            {
                noti.NewsID = newsID;
            }
            noti.Title = title;
            noti.Content = content;
            cnns.Notifications.Add(noti);
        }


    }
}
