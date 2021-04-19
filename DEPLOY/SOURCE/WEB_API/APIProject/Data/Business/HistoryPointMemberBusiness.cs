using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Data.Business
{
    public class HistoryPointMemberBusiness : GenericBusiness
    {
        public HistoryPointMemberBusiness(TichDiemTrieuDo context = null) : base()
        {

        }

        //Lịch sử yêu cầu , rút điểm, chuyển điểm của customer
        //public List<HistoryPointMemberModel> GetListHistory(int page, int limit, int Type, int ID)
        //{

        //    try
        //    {
        //        List<HistoryPointMemberModel> data = new List<HistoryPointMemberModel>();
        //        var cus = cnn.Customers;
        //        var query = from m in cnn.MembersPointHistories
        //                        where m.IsActive == SystemParam.ACTIVE && m.Type == Type && m.CustomerID == ID
        //                        orderby m.CraeteDate, m.ID descending
        //                        select new HistoryPointMemberModel
        //                        {
        //                            HistoryID = m.ID,
        //                            CusID = m.CustomerID.Value,
        //                            CreateDate = m.CraeteDate,
        //                            Title = m.Title,
        //                            Type = m.Type,
        //                            Point = m.Point,
        //                            Comment = m.Comment,
        //                            Balance = m.Balance.Value,
        //                            PointUser = m.Customer.Point.Value,
        //                            UserName = m.UserSendID.HasValue ? cus.Where(c => c.ID == m.UserSendID.Value).FirstOrDefault().Name:"",
        //                            UserPhone = m.UserSendID.HasValue ? cus.Where(c => c.ID == m.UserSendID.Value).FirstOrDefault().Phone:""

        //                        };

        //        if (query != null && query.Count() > 0)
        //            data = (List<HistoryPointMemberModel>)query;
        //        int count = data.Count();
        //        double totalPage = (double)count / (double)limit;
        //        ListHistoriesPointAppOutputModel lstHistories = new ListHistoriesPointAppOutputModel();
        //        lstHistories.limit = limit;
        //        lstHistories.totalCount = count;
        //        lstHistories.page = page;
        //        lstHistories.totalPage = (int)Math.Ceiling(totalPage);
        //        lstHistories.listHistoriesPointMember = data.ToPagedList(page, limit);
        //        return lstHistories;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return new List<HistoryPointMemberModel>();
        //    }

        //}
        //Vào màn hình rút điểm: gồm điểm và list các tài khoản ngân hàng
        public RequestPointModel GetWithrawPoints(int ID, double? point)
        {
            try
            {
                var data = cnn.Customers.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID == ID).Select(r => new RequestPointModel
                {

                    CustomerID = r.ID,
                    Status = r.Status,
                    Code = r.Code,
                    Point = r.Point,
                    TotalMoney = point.Value,
                    Maxpoint = 0,
                    MinPoint = 0,
                    Listbank = cnn.CustomerBanks.Where(c => c.IsActive == SystemParam.ACTIVE && c.CustomerID == ID).Select(u => new BankOutputModel
                    {
                        ID = u.ID,
                        BankName = u.Bank.BankName,
                        ShortName = u.Bank.ShortName,
                        ImageUrl = u.Bank.ImageUrl,
                        CodeBankAccount = u.BankOwner
                    }).ToList()
                }).FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new RequestPointModel();
            }
        }

        //Tiến hành rút điểm sau khi chọn đầy đủ thông tin
        public ResultRequestOuptputModel SuccessDrawPoint(int ID, double point, int bankID)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            ResultRequestOuptputModel data = new ResultRequestOuptputModel();
            PackageBusiness packageBusiness = new PackageBusiness();
            var conect = cnn.Database.BeginTransaction();
            try
            {
                var cus = cnn.Customers.Where(c => c.ID == ID).FirstOrDefault();
                double balance = cus.Point - point;
                cus.Point = balance;
                Request obj = new Request();
                obj.CustomerID = ID;
                obj.Point = point;
                obj.Status = SystemParam.STATUS_ORDER_PENDING;
                obj.Note = "";
                obj.Type = SystemParam.TYPE_REQUEST_DRAW_POINT;
                obj.BankID = bankID;
                obj.GiftID = 0;
                obj.CreateDate = DateTime.Now;
                obj.IsActive = SystemParam.ACTIVE;
                obj.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);


                //Tiến hành gửi thông báo đến web admin
                 var url = SystemParam.URL_WEB_SOCKET + "?content=" + "Một yêu cầu rút điểm mới đang chờ xác nhận&type=" + SystemParam.TYPE_NOTI_REQUEST_DRAW_POINT;
                  packageBusiness.GetJson(url);

                // lấy số điểm hiện có của khách hàng và trừ đi số điểm yêu cầu rút

                //Tạo lịch sử rút điểm
                MembersPointHistory m = new MembersPointHistory();
                m.CustomerID = ID;
                m.Point = point;
                m.Type = SystemParam.TYPE_REQUEST_DRAW_POINT;
                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                m.TypeAdd = SystemParam.TYPE_POINT;
                m.CraeteDate = DateTime.Now;
                m.Status = SystemParam.STATUS_ORDER_PENDING;
                m.IsActive = SystemParam.ACTIVE;
                m.Comment = "Chúng tôi sẽ xử lý yêu cầu sớm nhất có thể";
                m.Title = "Bạn vừa yêu cầu rút " + point + " thành công";
                m.Balance = balance;
                m.ProductID = null;
                m.UserSendID = null;
                m.BankID = bankID;

                //Tạo thông báo cho người nhận
                Notification ntf = new Notification();
                ntf.CustomerID = ID;
                ntf.Content = "Chúng tôi sẽ xử lý yêu cầu sớm nhất có thể";
                ntf.Viewed = 0;
                ntf.CreateDate = DateTime.Now;
                ntf.IsActive = SystemParam.ACTIVE;
                ntf.Title = "Bạn vừa yêu cầu rút " + point + " thành công";
                ntf.Type = SystemParam.NOTIFY_NAVIGATE_REQUEST;

                cnn.Notifications.Add(ntf);
                cnn.Requests.Add(obj);
                cnn.MembersPointHistories.Add(m);
                cnn.SaveChanges();
                conect.Commit();
                conect.Dispose();
                data.Status = SystemParam.RETURN_TRUE;
                data.RequestID = m.ID;
                var nt = cnn.Notifications.Find(ntf.ID);
                nt.NewsID = m.ID;
                cnn.SaveChanges();
                if (cus.DeviceID != null  && cus.DeviceID.Length > 15)
                {
                    //Tiến hành gửi thông báo
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_REQUEST_DETAIL;
                    notifyData.id = m.ID;
                    string titleNoti = "Bạn vừa bị trừ " + point + " khi yêu cầu rút điểm thành công ";
                    List<string> listDevice = new List<string>();
                    listDevice.Add(cus.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống trừ điểm khi yêu cầu rút điểm");
                    packageBusiness.PushOneSignals(value);
                }

                return data;
            }
            catch (Exception ex)
            {
                conect.Rollback();
                conect.Dispose();
                ex.ToString();
                return new ResultRequestOuptputModel();
            }
        }
    }
}
