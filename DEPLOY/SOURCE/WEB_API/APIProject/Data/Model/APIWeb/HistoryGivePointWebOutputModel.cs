using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class HistoryGivePointWebOutputModel
    {
        public int HistoryID { set; get; }
        public int CustomerID { get; set; }
        public string CustomerName { set; get; }
        public double Point { set; get; }
        public string AddpointCode
        {
            set { }
            get
            {
                if (Type == SystemParam.TYPE_CARD)
                {
                    string[] str = Code.Split('/');
                    return str[3];
                }
                else
                    return Code;
            }
        }
        public string Code { get; set; }
        public DateTime? CreateDate { set; get; }
        //public string CreateDateStr
        //{
        //    set { }
        //    get
        //    {
        //        return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
        //    }
        //}
        public string Date
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string Hour
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString("HH:mm") : "";
            }
        }

        public int TypeAdd { set; get; }
        public int Type { set; get; }
        public string Title { set; get; }
        public double Balance { set; get; }
        public string icon
        {
            get
            {
                switch (Type)
                {
                    case SystemParam.TYPE_POINT_SAVE:
                        return SystemParam.TYPE_POINT_SAVE_ICON;
                    case SystemParam.TYPE_POINT_GIVE:
                        return SystemParam.TYPE_POINT_GIVE_ICON;
                    case SystemParam.TYPE_POINT_RECEIVE:
                        return SystemParam.TYPE_POINT_RECEIVE_ICON;
                    case SystemParam.TYPE_POINT_RECEIVE_GIFT:
                        return SystemParam.TYPE_POINT_RECEIVE_GIFT_ICON;
                    //case SystemParam.TYPE_ADD_POINT:
                    //    return SystemParam.TYPE_ADD_POINT_ICON;
                    case SystemParam.TYPE_CARD:
                        return SystemParam.TYPE_CARD_ICON;
                    case SystemParam.HISTORY_POINT_CANCEL_REQUEST:
                        return SystemParam.ICON_NOTIFY_TYPE_REQUEST;
                    default:
                        return SystemParam.ICON_BULLHORN;
                }
            }
            set { }
        }
        public object cardDetail
        {
            get
            {
                CardOutputModel output = new CardOutputModel();
                if (Type == SystemParam.TYPE_CARD)
                {
                    string[] str = Code.Split('/');
                    output.name = str[0];
                    output.price = int.Parse(str[1]);
                    output.seri = str[2];
                    output.code = str[3];
                    return output;
                }
                return new object();
            }
            set { }
        }
    }
}
