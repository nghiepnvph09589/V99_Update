using Data.DB;
using Data.Model;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Business;
using System.IO;
using Newtonsoft.Json;

namespace Data.Business
{
    public class TaskBusiness : GenericBusiness
    {
        PackageBusiness packageBusiness;
        public TaskBusiness(TichDiemTrieuDo context = null) : base()
        {
            packageBusiness = new PackageBusiness(this.context);
        }
        public void SaveLog(string content)
        {
            var reportDirectory = string.Format("~/logging/{0}/", DateTime.Now.ToString("yyyy-MM-dd"));
            reportDirectory = System.Web.Hosting.HostingEnvironment.MapPath(reportDirectory);
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }
            var dailyReportFullPath = string.Format("{0}text_{1}.log", reportDirectory, DateTime.Now.Day);
            var logContent = string.Format("{0}-{1}-{2}", DateTime.Now, "Check_Schedule " + content, Environment.NewLine);
            File.AppendAllText(dailyReportFullPath, logContent);
        }
        public void ProcessAddingPoint()
        {
            try
            {
                //chuyển trạng thái có tiến trình đang chạy


                //Lấy danh sách những customer đang có điểm tích lũy lơn hơn 0;

                var getPointCustomer = cnn.Customers.Where(u => u.PointRanking > 0 && u.IsActive == SystemParam.ACTIVE && u.Status == SystemParam.ACTIVE).ToList();


                //Kiểm tra danh sách có dữ liệu hay không và viết tiến trình

                if (getPointCustomer != null && getPointCustomer.Count() > 0)
                {
                    var list = new List<MembersPointHistory>();
                    var listNoti = new List<Notification>();
                    foreach (var task in getPointCustomer)
                    {

                        Notification nt = new Notification();
                        //TH1: Khách hàng được cộng điểm 0,2 % hằng ngày
                        //Kiểm tra điều kiện tài khoản tạo nhỏ hơn 120 ngày
                        if (task.CraeteDate.AddDays(120) >= DateTime.Now)
                        {
                            double pointAdd = Math.Round(Convert.ToDouble(task.PointRanking * 0.002), 2);
                            if (pointAdd > 0)
                            {
                                var point = task.Point + pointAdd;
                                var pointRanking = task.PointRanking - pointAdd;
                                var code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                                task.Point = point;
                                task.PointRanking = pointRanking;
                                MembersPointHistory m = new MembersPointHistory();
                                m.CustomerID = task.ID;
                                m.Point = pointAdd;
                                m.Type = SystemParam.TYPE_SYSTEM_ADD_POINT;
                                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                                m.AddPointCode = code;
                                m.TypeAdd = SystemParam.TYPE_POINT;
                                m.CraeteDate = DateTime.Now;
                                m.IsActive = SystemParam.ACTIVE;
                                m.Comment = "Hệ thống chuyển đổi 0,2 % điểm tích lũy sang điểm ví point hàng ngày";
                                m.Title = "Bạn vừa được hệ thống quy đổi " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                m.Balance = point;
                                m.ProductID = null;
                                m.UserSendID = null;
                                list.Add(m);
                                //Lưu lại lịch sử quy đổi điểm từ ví điểm tích sang ví point
                                MembersPointHistory mb = new MembersPointHistory();
                                mb.CustomerID = task.ID;
                                mb.Point = pointAdd;
                                mb.Type = SystemParam.TYPE_SYSTEM_ADD_POINT;
                                mb.AddPointCode = code;
                                mb.TypeAdd = SystemParam.TYPE_POINT_RANKING;
                                mb.CraeteDate = DateTime.Now;
                                mb.IsActive = SystemParam.ACTIVE;
                                mb.Comment = "Hệ thống chuyển đổi 0,2 % điểm tích lũy sang điểm ví point hàng ngày";
                                mb.Title = "Hệ thống quy đổi " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                mb.Balance = pointRanking;
                                mb.ProductID = null;
                                mb.UserSendID = null;
                                list.Add(mb);

                                // Tạo bản ghi thông báo
                                nt.CustomerID = task.ID;
                                nt.Content = "Hệ thống chuyển đổi 0,2 % điểm tích lũy sang điểm ví point hàng ngày";
                                nt.Viewed = 0;
                                nt.CreateDate = DateTime.Now;
                                nt.IsActive = SystemParam.ACTIVE;
                                nt.Title = "Bạn vừa được cộng " + pointAdd + " từ điểm tích lũy sang điểm ví point";
                                nt.Type = SystemParam.TYPE_AWARDED_POINT;
                                nt.NewsID = null;
                                listNoti.Add(nt);
                                if (task.DeviceID != null && task.DeviceID.Length > 15)
                                {
                                    //Tiến hành gửi thông báo
                                    NotifyDataModel notifyData = new NotifyDataModel();
                                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                                    notifyData.Point = point;
                                    notifyData.PointRaking = pointRanking;
                                    string titleNoti = "Bạn vừa được cộng " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                    List<string> listDevice = new List<string>();
                                    listDevice.Add(task.DeviceID);
                                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống chuyển đổi 0,2 % điểm tích lũy sang điểm ví point hàng ngày");
                                    packageBusiness.PushOneSignals(value);
                                }
                            }

                        }
                        if (task.CraeteDate.AddDays(120) < DateTime.Now)
                        {
                            double pointAdd = Math.Round(Convert.ToDouble(task.PointRanking * 0.001), 2);
                            if (pointAdd > 0)
                            {
                                var pointRanking = task.PointRanking - pointAdd;
                                var point = task.Point + pointAdd;
                                task.Point = point;
                                task.PointRanking = pointRanking;
                                //Lưu lại lịch sử
                                MembersPointHistory m = new MembersPointHistory();
                                m.CustomerID = task.ID;
                                m.Point = pointAdd;
                                m.Type = SystemParam.TYPE_SYSTEM_ADD_POINT;
                                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                                m.TypeAdd = SystemParam.TYPE_POINT;
                                m.CraeteDate = DateTime.Now;
                                m.IsActive = SystemParam.ACTIVE;
                                m.Comment = "Hệ thống chuyển đổi 0,1 % điểm tích lũy sang điểm ví point hàng ngày";
                                m.Title = "Bạn vừa được cộng " + pointAdd + " điểm điểm từ điểm tích lũy sang điểm ví point";
                                m.Balance = point;
                                m.ProductID = null;
                                m.UserSendID = null;
                                list.Add(m);
                                //Lưu lại lịch sử quy đổi điểm từ ví điểm tích sang ví point
                                MembersPointHistory mb = new MembersPointHistory();
                                mb.CustomerID = task.ID;
                                mb.Point = pointAdd;
                                mb.Type = SystemParam.TYPE_SYSTEM_ADD_POINT;
                                mb.TypeAdd = SystemParam.TYPE_POINT_RANKING;
                                mb.CraeteDate = DateTime.Now;
                                mb.IsActive = SystemParam.ACTIVE;
                                mb.Comment = "Hệ thống chuyển đổi 0,1 % điểm tích lũy sang điểm ví point hàng ngày";
                                mb.Title = "Hệ thống quy đổi " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                mb.Balance = pointRanking;
                                mb.ProductID = null;
                                mb.UserSendID = null;
                                list.Add(mb);

                                //Tạo bản ghi thông báo
                                nt.CustomerID = task.ID;
                                nt.Content = "Hệ thống chuyển đổi 0,1 % điểm tích lũy sang điểm ví point hàng ngày";
                                nt.Viewed = 0;
                                nt.CreateDate = DateTime.Now;
                                nt.IsActive = SystemParam.ACTIVE;
                                nt.Title = "Bạn vừa được cộng " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                nt.Type = SystemParam.TYPE_AWARDED_POINT;
                                nt.NewsID = null;
                                listNoti.Add(nt);
                                if (task.DeviceID != null && task.DeviceID.Length > 15)
                                {
                                    // Tiến hành gửi thông báo
                                    NotifyDataModel notifyData = new NotifyDataModel();
                                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                                    notifyData.Point = point;
                                    notifyData.PointRaking = pointRanking;
                                    string titleNoti = "Bạn vừa được cộng " + pointAdd + " điểm từ điểm tích lũy sang điểm ví point";
                                    List<string> listDevice = new List<string>();
                                    listDevice.Add(task.DeviceID);
                                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống chuyển đổi 0,1 % điểm tích lũy sang điểm ví point hàng ngày");
                                    packageBusiness.PushOneSignals(value);
                                }
                            }
                        }
                    }
                    cnn.MembersPointHistories.AddRange(list);
                    cnn.Notifications.AddRange(listNoti);
                    cnn.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                SaveLog(ex.ToString());
                return;
            }
        }
        public void MultiplePushNotiWeb(List<string> deviceID, string title, string contents)
        {
            NotifyDataModel notifyData = new NotifyDataModel();
            notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
            OneSignalInput input = new OneSignalInput();
            TextInput header = new TextInput();
            header.en = contents.Length > 0 ? title : "";
            TextInput content = new TextInput();
            content.en = contents.Length > 0 ? contents : title;
            input.app_id = SystemParam.APP_ID;
            input.data = notifyData;
            input.headings = header;
            input.contents = content;
            input.android_channel_id = SystemParam.ANDROID_CHANNEL_ID;
            input.include_player_ids = deviceID;
            if (deviceID.Count <= 80)
            {
                input.include_player_ids = deviceID.Where(u => u.Length > 10).ToList();
                string value = JsonConvert.SerializeObject(input);
                string res = packageBusiness.PushOneSignals(value);
                SaveLog(res);
            }
            else
            {
                int count = deviceID.Count / 80;
                int remain = deviceID.Count % 80;
                for (int index = 0; index <= count; index++)
                {
                    List<string> listDeviceID = deviceID.GetRange(index * 80, 80);
                    input.include_player_ids = listDeviceID;
                    string values = JsonConvert.SerializeObject(input);
                    string respon = packageBusiness.PushOneSignals(values);
                    SaveLog(respon);
                }
                List<string> lsDeviceID = deviceID.GetRange(count * 80, 80);
                input.include_player_ids = lsDeviceID;
                string value = JsonConvert.SerializeObject(input);
                string res = packageBusiness.PushOneSignals(value);
                SaveLog(res);
            }
        }
        public void AddPointEveryDay()
        {
            try
            {
                var getPointCustomer = cnn.Customers.Where(u => u.PointRanking > 0 && u.IsActive == SystemParam.ACTIVE && u.Status == SystemParam.ACTIVE).ToList()
                    .Where(u => Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2) > 0)
                    .ToList();
                var dateTime = DateTime.Today;
                int hour = DateTime.Now.Hour;
                if (hour >= 8)
                {
                    var listMemberHistory = cnn.MembersPointHistories.Where(u => u.Type.Equals(6) && u.CraeteDate >= dateTime).Count();
                    if (getPointCustomer.Count > 0 && listMemberHistory == 0)
                    {
                        var customerHaveDeviceID = getPointCustomer.Where(u => !String.IsNullOrEmpty(u.DeviceID) && u.DeviceID.Length > 15).ToList();
                        var listaddPoint = getPointCustomer.Select(u => new MembersPointHistory
                        {
                            CustomerID = u.ID,
                            Point = Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2),
                            Type = SystemParam.TYPE_SYSTEM_ADD_POINT,
                            AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6),
                            TypeAdd = SystemParam.TYPE_POINT,
                            CraeteDate = DateTime.Now,
                            IsActive = SystemParam.ACTIVE,
                            Comment = "Hệ thống chuyển đổi " + (u.CraeteDate.AddDays(120) >= DateTime.Now ? "0,2 %" : "0,1 %") + " điểm tích lũy sang điểm ví point hàng ngày",
                            Title = "Bạn vừa được cộng " + Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2) + " điểm điểm từ điểm tích lũy sang điểm ví point",
                            Balance = u.Point + Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2),
                        }).ToList();
                        var listRankPoint = getPointCustomer.Select(u => new MembersPointHistory
                        {
                            CustomerID = u.ID,
                            Point = Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2),
                            Type = SystemParam.TYPE_SYSTEM_ADD_POINT,
                            AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6),
                            TypeAdd = SystemParam.TYPE_POINT_RANKING,
                            CraeteDate = DateTime.Now,
                            IsActive = SystemParam.ACTIVE,
                            Comment = "Hệ thống chuyển đổi " + (u.CraeteDate.AddDays(120) >= DateTime.Now ? "0,2 %" : "0,1 %") + " điểm tích lũy sang điểm ví point hàng ngày",
                            Title = "Bạn vừa được cộng " + Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2) + " điểm điểm từ điểm tích lũy sang điểm ví point",
                            Balance = u.PointRanking - Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2),
                        }).ToList();
                        var listNoti01 = getPointCustomer.Select(u => new Notification
                        {
                            CustomerID = u.ID,
                            Content = "Hệ thống chuyển đổi " + (u.CraeteDate.AddDays(120) >= DateTime.Now ? "0,2 %" : "0,1 %") + " điểm tích lũy sang điểm ví point hàng ngày",
                            Viewed = 0,
                            CreateDate = DateTime.Now,
                            IsActive = SystemParam.ACTIVE,
                            Title = "Bạn vừa được cộng " + Math.Round(Convert.ToDouble(u.PointRanking * (u.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2) + " điểm từ điểm tích lũy sang điểm ví point",
                            Type = SystemParam.TYPE_AWARDED_POINT,
                        }).ToList();
                        var listMuntilNoti = customerHaveDeviceID.Select(u => new
                        {
                            DeviceID = u.DeviceID,
                            Content = "Hệ thống chuyển đổi " + (u.CraeteDate.AddDays(120) >= DateTime.Now ? "0,2 %" : "0,1 %") + " điểm tích lũy sang điểm ví point hàng ngày",
                        }).GroupBy(u => u.Content).Select(u => new { lsDeviceID = u.Select(s => s.DeviceID).ToList(), content = u.FirstOrDefault().Content }).ToList();

                        foreach (var cus in getPointCustomer)
                        {
                            var point = Math.Round(Convert.ToDouble(cus.PointRanking * (cus.CraeteDate.AddDays(120) >= DateTime.Now ? 0.002 : 0.001)), 2);
                            cus.Point += point;
                            cus.PointRanking = cus.PointRanking > point ? cus.PointRanking - point : 0;
                        }
                        cnn.MembersPointHistories.AddRange(listaddPoint);
                        cnn.MembersPointHistories.AddRange(listRankPoint);
                        cnn.Notifications.AddRange(listNoti01);
                        cnn.SaveChanges();
                        foreach (var noti in listMuntilNoti)
                        {
                            MultiplePushNotiWeb(noti.lsDeviceID, "Cộng điểm chuyển đổi hằng ngày", noti.content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.ToString());
            }
        }
        public void MinusPointCustomer()
        {
            try
            {
                //Lấy danh sách những customer đang có điểm lớn hơn 0 và có ngày tạo trước ngày 25;

                var getPointCustomer = cnn.Customers.Where(u => u.Point > 0 && u.IsActive == SystemParam.ACTIVE && (u.CraeteDate.Month == DateTime.Now.Month ? u.CraeteDate.Day < 25 : true)).ToList();
                //Tạo mới Noti
                var listNoti = new List<Notification>();
                Notification nt = new Notification();
                PackageBusiness packageBusiness = new PackageBusiness();
                var list = new List<MembersPointHistory>();

                //Kiểm tra danh sách có dữ liệu hay không và viết tiến trình

                if (getPointCustomer != null && getPointCustomer.Count() > 0)
                {

                    foreach (var task in getPointCustomer)
                    {
                        //Trừ tiền hàng tháng
                        var point = task.Point - 20;
                        if (point < 0)
                            point = 0;
                        task.Point = point;

                        MembersPointHistory m = new MembersPointHistory();
                        m.CustomerID = task.ID;
                        m.Point = 20;
                        m.Type = SystemParam.TYPE_FEE_USE_APP;
                        m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                        m.TypeAdd = SystemParam.TYPE_POINT;
                        m.CraeteDate = DateTime.Now;
                        m.IsActive = SystemParam.ACTIVE;
                        m.Comment = "Hệ thống thu phí sủ dụng app hàng tháng";
                        m.Title = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app";
                        m.Balance = task.Point;
                        m.ProductID = null;
                        m.UserSendID = null;
                        list.Add(m);
                        // Tạo bản ghi thông báo
                        nt.CustomerID = task.ID;
                        nt.Content = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app";
                        nt.Viewed = 0;
                        nt.CreateDate = DateTime.Now;
                        nt.IsActive = SystemParam.ACTIVE;
                        nt.Title = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app";
                        nt.Type = SystemParam.TYPE_FEE_USE_APP;
                        nt.NewsID = null;
                        listNoti.Add(nt);
                        if (task.DeviceID != null && task.DeviceID.Length > 15)
                        {
                            //Tiến hành gửi thông báo
                            NotifyDataModel notifyData = new NotifyDataModel();
                            notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                            notifyData.Point = point;
                            string titleNoti = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app";
                            List<string> listDevice = new List<string>();
                            listDevice.Add(task.DeviceID);
                            string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app");
                            packageBusiness.PushOneSignals(value);
                        }
                        listNoti.Add(nt);

                    }
                    cnn.Notifications.AddRange(listNoti);
                    cnn.MembersPointHistories.AddRange(list);
                    cnn.SaveChanges();
                }

            }
            catch
            {
                return;
            }


        }

        public void MinusPointEveryMonth() {
            var getPointCustomer = cnn.Customers.Where(u => u.Point > 0 && u.IsActive == SystemParam.ACTIVE && (u.CraeteDate.Month == DateTime.Now.Month ? u.CraeteDate.Day < 25 : true)).ToList();
            var customerHaveDeviceID = getPointCustomer.Where(u => !String.IsNullOrEmpty(u.DeviceID) && u.DeviceID.Length > 15).ToList();
            int month = DateTime.Now.Month;
            List<MembersPointHistory> listMember = cnn.MembersPointHistories.Where(u => u.Type.Equals(SystemParam.TYPE_FEE_USE_APP) && u.CraeteDate.Month.Equals(month)).ToList();
            if (listMember.Count == 0)
            {
                var listaddPoint = getPointCustomer.Select(u => new MembersPointHistory
                {
                    CustomerID = u.ID,
                    Point = 20,
                    Type = SystemParam.TYPE_FEE_USE_APP,
                    AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6),
                    TypeAdd = SystemParam.TYPE_POINT,
                    CraeteDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    Comment = "Hệ thống thu phí sủ dụng app hàng tháng",
                    Title = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app",
                    Balance = u.Point > 20 ? u.Point - 20 : 0,
                }).ToList();
                var listNoti = customerHaveDeviceID.Select(u => new Notification
                {
                    CustomerID = u.ID,
                    Content = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app",
                    Viewed = 0,
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    Title = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app",
                    Type = SystemParam.TYPE_AWARDED_POINT,
                }).ToList();
                var listMuntilNoti = customerHaveDeviceID.Select(u => new
                {
                    DeviceID = u.DeviceID,
                    Content = "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app",
                }).ToList();
                foreach (var cus in getPointCustomer)
                {
                    cus.Point = cus.Point > 20 ? cus.Point - 20 : 0;
                }
                cnn.MembersPointHistories.AddRange(listaddPoint);
                cnn.Notifications.AddRange(listNoti);
                cnn.SaveChanges();
                MultiplePushNotiWeb(listMuntilNoti.Select(u => u.DeviceID).ToList(), "Hệ thống thu phí duy trì hàng tháng", "Hệ thống thu phí 20 điểm hàng tháng vào phí sử dụng app");
            }
        }
    }
}
