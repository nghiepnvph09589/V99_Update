using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class NotifiedByCustomerIDOutputModel
    {
        public int NotifyID { set; get; }
        public int? ObjectID { set; get; }
        public string Content { set; get; }
        public int Viewed { set; get; }
        public int Type { get; set; }
        public DateTime? CreatedDate { set; get; }
        public string CreatedDateStr
        {
            set { }
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string Title { get; set; }
        public string icon
        {
            get
            {
                switch (Type)
                {
                    case SystemParam.NOTIFY_TYPE_ORDER_CANCEL:
                        return SystemParam.ICON_ORDER_CANCEL;
                    case SystemParam.NOTIFY_TYPE_ORDER_CONFIRM:
                        return SystemParam.ICON_ORDER_CONFIRM;
                    case SystemParam.NOTIFY_TYPE_ORDER_PAID:
                        return SystemParam.ICON_ORDER_PAID;
                    case SystemParam.TYPE_REQUEST_NOTIFY:
                        return SystemParam.ICON_NOTIFY_TYPE_REQUEST;
                    case SystemParam.NOTIFY_TYPE_POINT_RECEIVE:
                        return SystemParam.ICON_NOTIFY_TYPE_POINT_RECEIVE;
                    case SystemParam.NOTIFY_TYPE_CREATE_NEWS:
                        return SystemParam.ICON_NOTIFY_TYPE_CREATE_NEWS;
                    default:
                        return SystemParam.ICON_BULLHORN;
                }
            }
            set { }
        }
        //public object cardDetail
        //{
        //    get
        //    {
        //        CardOutputModel output = new CardOutputModel();
        //        if (Type == SystemParam.TYPE_CARD)
        //        {
        //            string[] str = Content.Split('/');
        //            output.name = str[0];
        //            output.price = int.Parse(str[1]);
        //            output.seri = str[2];
        //            output.code = str[3];
        //            return output;
        //        }
        //        return new object();
        //    }
        //    set { }
        //}
    }
}