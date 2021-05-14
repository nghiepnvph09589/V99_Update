using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class PackageBusiness : GenericBusiness
    {
        public PackageBusiness(TichDiemTrieuDo context = null) : base()
        {
        }

        public void PushNotiApp(double point,int type,int statusOrder,int typeNoti,int orderID,string title,string content,int cusID,string deviceID)
        {
            Notification ntf = new Notification();
            ntf.CustomerID = cusID;
            ntf.Content = title;
            ntf.Viewed = 0;
            ntf.CreateDate = DateTime.Now;
            ntf.IsActive = SystemParam.ACTIVE;
            ntf.Title = title;
            ntf.Type = typeNoti;
            cnn.Notifications.Add(ntf);
            cnn.SaveChanges();
            if (deviceID != null && deviceID.Length > 15)
            {
                //Tiến hành gửi thông báo
                NotifyDataModel notifyData = new NotifyDataModel();
                notifyData.Point = point;
                notifyData.type = type;
                notifyData.StatusOrder = statusOrder;
                notifyData.id = orderID;
                List<string> listDevice = new List<string>();
                listDevice.Add(deviceID);
                string value = StartPushNoti(notifyData, listDevice, title, content);
                PushOneSignals(value);
            }
        }
        public string StartPushNoti(object obj, List<string> deviceID, string title, string contents)
        {
            OneSignalInput input = new OneSignalInput();
            TextInput header = new TextInput();
            header.en = contents.Length > 0 ? title : "";
            TextInput content = new TextInput();
            content.en = contents.Length > 0 ? contents : title;
            input.app_id = SystemParam.APP_ID;
            input.data = obj;
            input.headings = header;
            input.contents = content;
            input.android_channel_id = SystemParam.ANDROID_CHANNEL_ID;
            input.include_player_ids = deviceID;
            return  JsonConvert.SerializeObject(input);
        }


        public string PushOneSignals(string value)
        {
            string url = SystemParam.URL_ONESIGNAL;

            var request = HttpWebRequest.Create(string.Format(url));

            request.Headers["Authorization"] = SystemParam.Authorization;
            request.Headers["https"] = SystemParam.URL_BASE_https;
            var byteData = Encoding.UTF8.GetBytes(value);
            request.ContentType = "application/json";
            request.Method = "POST";
            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }

        public string GetJson(string url)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(url));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            Console.WriteLine(WebResp.StatusCode);
            Console.WriteLine(WebResp.Server);

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            return jsonString;
        }
    }
}
