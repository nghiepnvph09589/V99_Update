using APIProject.Models;
using Data.Business;
using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIProject.Controllers
{
    public class ServiceController : BaseAPIController
    {
        TichDiemTrieuDo cnn;
        public ServiceController()
        {
            if (cnn == null)
            {
                this.cnn = new TichDiemTrieuDo();
            }
        }
        //Check đăng nhập app
        public JsonResultModel CheckLoginApp([FromBody] LoginAppInputModel item)
        {
            try
            {
                if (String.IsNullOrEmpty(item.Phone) || String.IsNullOrEmpty(item.Password))
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                var Phone = Util.ConvertPhone(item.Phone.Trim());
                if (!Util.validPhone(Phone))
                    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_PHONE, "");


                var query = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Phone == item.Phone).FirstOrDefault();
                if (query != null)
                {
                    var data = lgBus.CheckLoginApp(item.Phone, item.Password, item.DeviceID);

                    //Sai thông tin đăng nhập
                    if (data.Type == SystemParam.PROCESS_ERROR)
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_LOGIN_ACCOUNT_FAIL, "");
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lgBus.GetuserInfor(data.UserID));
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_PHONE_NOT_FOUND, SystemParam.MESSAGE_EMAIL_ERROR_LOGIN, "");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }

        //Check số điện thoại đăng ký tài khoản
        [HttpPost]
        public JsonResultModel CheckPhoneRegister([FromBody] LoginAppInputModel item)
        {
            if (item.Email.Length == 0)
                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
            if (!Util.ValidateEmail(item.Email))
                return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");

            //if (String.IsNullOrEmpty(item.Phone))

            //    return response(SystemParam.ERROR, SystemParam.ERROR_PHONE_NOT_FOUND, SystemParam.USERID_NOT_FOUND, "");

            //var phone = Util.ConvertPhone(item.Phone.Trim());
            //if (!Util.validPhone(phone))
            //    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_PHONE, "");



            var check = lgBus.CheckPhoneRegister(item.Email);

            if (check.Type == SystemParam.TYPE_ERROR_PHONE_EXIST)
                return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_EXIST_EMAIL, check);
            if (check.Type == SystemParam.TYPE_ERROR_UPDATE_PHONE)
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_PLEASE_UPDATE_PHONE, check);
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }


        //Check mã otp
        public JsonResultModel CheckOTP([FromBody] CheckOtpOutputModel dt)
        {
            if (String.IsNullOrEmpty(dt.Email) || String.IsNullOrEmpty(dt.otp))

                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

            if (!Util.ValidateEmail(dt.Email))
                return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");

            var result = lgBus.CheckOtpCode(dt.otp, dt.Email);
            if (result == SystemParam.ERROR)
                return response(SystemParam.ERROR, SystemParam.LOGIN_APP_CUSTOMER_FALSE, SystemParam.MESSAGE_LOGIN_APP_CUSTOMER_FALSE, "");
            if (result == SystemParam.ERROR_EXPIRE_OTP)
                return response(SystemParam.ERROR, SystemParam.ERROR_EXPIRE_OTP, SystemParam.MESSAGE_ERROR_EXPIRE_OTP, "");

            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }

        // Đăng ký tài khoản
        public JsonResultModel Register([FromBody] LoginAppInputModel input)
        {
            if (input.Name.Length == 0 || input.Name.Length == 0 || input.Password.Length == 0 || input.Email.Length == 0)
                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
            var Phone = Util.ConvertPhone(input.Phone.Trim());
            var lastRefCode = Util.ConvertPhone(input.LastRefCode.Trim());
            if (!Util.validPhone(Phone))
                return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_PHONE, "");
            if (!Util.ValidateEmail(input.Email))
                return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");
            var checkPhone = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Phone == lastRefCode && c.Status.Equals(SystemParam.ACTIVE) && c.Phone != input.Phone).Select(c => c.Phone).FirstOrDefault();
            if (!Util.validPhone(lastRefCode) || checkPhone == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_LASTREF_CODE, SystemParam.MESSAGE_ERROR_LASTREF_CODE, "");

            var result = lgBus.Register(input);
            if (result.UserID == 0)
                return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_EMAIL_NOT_FOUND, "");
            if (result.UserID == -1)
                return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_PHONE, SystemParam.MESSAGE_ERROR_EXIST_PHONE, "");
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lgBus.GetuserInfor(result.UserID));
        }
        // lấy token request
        public string getTokenApp()
        {
            if (Request.Headers.Contains("token"))
            {
                return Request.Headers.GetValues("token").FirstOrDefault();
            }
            return "";
        }


        // Lấy ra danh sách tin tức liên quan
        [HttpGet]
        public JsonResultModel GetListNewsRelative(int type, int newsID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<NewsAppOutputModel> data = new List<NewsAppOutputModel>();
                var query = from n in cnn.News
                            where n.Type.Equals(type)
                            && n.IsActive.Equals(SystemParam.ACTIVE)
                            && n.Status.Equals(SystemParam.ACTIVE)
                            && n.ID != newsID
                            orderby n.ID descending
                            select new NewsAppOutputModel
                            {
                                NewsID = n.ID,
                                Content = n.Content,
                                CreateDate = n.CreateDate,
                                Title = n.Title,
                                Type = n.Type,
                                UrlImage = n.UrlImage,
                                Description = n.Description
                            };
                if (query != null && query.Count() > 0)
                {
                    data = query.Take(3).ToList();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // Lấy ra danh sách tin tức
        [HttpGet]
        public JsonResultModel GetNews(int type)
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<NewsAppOutputModel> query = new List<NewsAppOutputModel>();
                query = newsBus.GetListNews(type);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        public JsonResultModel response(int status, int code, string message, object data)
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = status;
            result.Code = code;
            result.Message = message;
            result.Data = data;
            return result;
        }


        public JsonResultModel serverError()
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = SystemParam.ERROR;
            result.Code = SystemParam.ERROR;
            result.Message = SystemParam.SERVER_ERROR;
            result.Data = "";
            return result;
        }


        // thông tin người đăng nhập
        [HttpGet]
        public JsonResultModel GetUserInfor()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                //UserInforOutputModel query = new UserInforOutputModel();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, "", lgBus.GetuserInfor(cusID.Value));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        // Lịch sử rút điểm, chuyển điểm,....
        [HttpGet]
        public JsonResultModel ListHistoryPointMember(int page, int limit, int Type, int? TypePoint)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                List<HistoryPointMemberModel> data = new List<HistoryPointMemberModel>();
                var cus = cnn.Customers;
                var query = from m in cnn.MembersPointHistories
                            where m.IsActive == SystemParam.ACTIVE &&
                            m.TypeAdd == Type && m.CustomerID == cusID.Value &&
                            (TypePoint.HasValue && TypePoint > 0 ? m.Type == TypePoint.Value : true)
                             && (m.Status.HasValue && !TypePoint.HasValue ? m.Status.Value.Equals(SystemParam.STATUS_ORDER_PENDING) : true)

                            orderby m.ID descending
                            select new HistoryPointMemberModel
                            {
                                HistoryID = m.ID,
                                CusID = m.CustomerID.Value,
                                CreateDate = m.CraeteDate,
                                Title = m.Title,
                                Type = m.Type,
                                Point = m.Point,
                                Comment = m.Comment,
                                Balance = Math.Round(m.Balance, 2),
                                PointUser = Math.Round(m.Customer.Point, 2),
                                UserName = m.UserSendID.HasValue ? m.Customer1.Name : "",
                                UserPhone = m.UserSendID.HasValue ? m.Customer1.Phone : ""
                            };

                if (query != null && query.Count() > 0)
                    data = query.ToList();
                int count = data.Count();
                double totalPage = (double)count / (double)limit;
                ListHistoriesPointAppOutputModel lstHistories = new ListHistoriesPointAppOutputModel();
                lstHistories.limit = limit;
                lstHistories.totalCount = count;
                lstHistories.page = page;
                lstHistories.totalPage = (int)Math.Ceiling(totalPage);
                lstHistories.listHistoriesPointMember = data.ToPagedList(page, limit);

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, "", lstHistories);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }
        [HttpPost]
        public JsonResultModel ConvertPointVtoPointRanking([FromBody] ConvertPointInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var cus = cnn.Customers.FirstOrDefault(x => x.ID == cusID);
                if (input.point > cus.PointV || input.point <= 0)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_INVALID_INPUT_POINT, "");
                }
                var balanceV = cus.PointV - input.point;
                var balancePoint = cus.PointRanking + input.point;
                cus.PointV -= input.point;
                cus.PointRanking += input.point;
                //Tạo lịch sử chuyển điểm ví V
                MembersPointHistory mv = new MembersPointHistory();
                mv.CustomerID = cus.ID;
                mv.Point = input.point;
                mv.Type = SystemParam.TYPE_CONVERT_POINT_V_TO_POINT_RANKING;
                mv.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                mv.TypeAdd = SystemParam.TYPE_POINT_V;
                mv.CraeteDate = DateTime.Now;
                mv.IsActive = SystemParam.ACTIVE;
                mv.Comment = "Chuyển điểm từ ví V sang ví điểm tích lũy";
                mv.Title = "Ví V đã bị trừ " + input.point + "điểm";
                mv.Balance = balanceV;

                //Tạo lịch sử rút điểm
                MembersPointHistory m = new MembersPointHistory();
                m.CustomerID = cus.ID;
                m.Point = input.point;
                m.Type = SystemParam.TYPE_CONVERT_POINT_V_TO_POINT_RANKING;
                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                m.TypeAdd = SystemParam.TYPE_POINT_RANKING;
                m.CraeteDate = DateTime.Now;
                m.IsActive = SystemParam.ACTIVE;
                m.Comment = "Chuyển điểm từ ví V sang ví tích điểm";
                m.Title = "Ví tích điểm đã được cộng" + input.point + "điểm";
                m.Balance = balancePoint;

                //Tạo thông báo cho người nhận
                Notification ntf = new Notification();
                ntf.CustomerID = cus.ID;
                ntf.Content = "Bạn vừa chuyển điểm từ ví V sang ví điểm tích lũy";
                ntf.Viewed = 0;
                ntf.CreateDate = DateTime.Now;
                ntf.IsActive = SystemParam.ACTIVE;
                ntf.Title = "Bạn vừa chuyển " + input.point + " điểm từ ví V sang ví điểm tích lũy thành công";
                ntf.Type = SystemParam.NOTIFY_NAVIGATE_REQUEST;


                cnn.Notifications.Add(ntf);
                cnn.MembersPointHistories.Add(mv);
                cnn.MembersPointHistories.Add(m);
                cnn.SaveChanges();
                if (cus.DeviceID != null && cus.DeviceID.Length > 15)
                {
                    //Tiến hành gửi thông báo
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_REQUEST_DETAIL;
                    notifyData.id = m.ID;
                    string titleNoti = "Bạn vừa chuyển " + input.point + " điểm từ ví V sang ví điểm tích lũy thành công";
                    List<string> listDevice = new List<string>();
                    listDevice.Add(cus.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, titleNoti);
                    packageBusiness.PushOneSignals(value);
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        [HttpPost]
        public JsonResultModel ConvertPointRankingtoPointV([FromBody] ConvertPointInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var cus = cnn.Customers.FirstOrDefault(x => x.ID == cusID);
                if (input.point > cus.PointV || input.point <= 0)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_INVALID_INPUT_POINT, "");
                }
                var balanceV = cus.PointV + input.point;
                var balancePoint = cus.PointRanking - input.point;
                cus.PointV += input.point;
                cus.PointRanking -= input.point;
                //Tạo lịch sử chuyển điểm ví V
                MembersPointHistory mv = new MembersPointHistory();
                mv.CustomerID = cus.ID;
                mv.Point = input.point;
                mv.Type = SystemParam.TYPE_CONVERT_POINT_RANKING_TO_POINT_V;
                mv.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                mv.TypeAdd = SystemParam.TYPE_POINT_V;
                mv.CraeteDate = DateTime.Now;
                mv.IsActive = SystemParam.ACTIVE;
                mv.Comment = "Chuyển điểm từ ví điểm tích lũy sang ví V";
                mv.Title = "Ví V đã được cộng " + input.point + "điểm";
                mv.Balance = balanceV;

                //Tạo lịch sử rút điểm
                MembersPointHistory m = new MembersPointHistory();
                m.CustomerID = cus.ID;
                m.Point = input.point;
                m.Type = SystemParam.TYPE_CONVERT_POINT_RANKING_TO_POINT_V;
                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                m.TypeAdd = SystemParam.TYPE_POINT_RANKING;
                m.CraeteDate = DateTime.Now;
                m.IsActive = SystemParam.ACTIVE;
                m.Comment = "Chuyển điểm từ ví điểm tích lũy sang ví V";
                m.Title = "Ví tích điểm đã bị trừ" + input.point + "điểm";
                m.Balance = balancePoint;

                //Tạo thông báo cho người nhận
                Notification ntf = new Notification();
                ntf.CustomerID = cus.ID;
                ntf.Content = "Bạn vừa chuyển điểm từ ví V sang ví điểm tích lũy";
                ntf.Viewed = 0;
                ntf.CreateDate = DateTime.Now;
                ntf.IsActive = SystemParam.ACTIVE;
                ntf.Title = "Bạn vừa chuyển " + input.point + " điểm từ ví V sang ví điểm tích lũy thành công";
                ntf.Type = SystemParam.NOTIFY_NAVIGATE_REQUEST;


                cnn.Notifications.Add(ntf);
                cnn.MembersPointHistories.Add(mv);
                cnn.MembersPointHistories.Add(m);
                cnn.SaveChanges();
                if (cus.DeviceID != null && cus.DeviceID.Length > 15)
                {
                    //Tiến hành gửi thông báo
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_REQUEST_DETAIL;
                    notifyData.id = m.ID;
                    string titleNoti = "Bạn vừa chuyển " + input.point + " điểm từ ví V sang ví điểm tích lũy thành công";
                    List<string> listDevice = new List<string>();
                    listDevice.Add(cus.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, titleNoti);
                    packageBusiness.PushOneSignals(value);
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        //// Rút điểm
        //public JsonResultModel WithdraPoints([FromBody] PointInputModel input)
        //{
        //    try
        //    {
        //        string token = getTokenApp();
        //        if (token.Length == 0)
        //            return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
        //        int? cusID = checkTokenApp(token);
        //        if (cusID == null)
        //            return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

        //        var pointOfCus = cnn.Customers.Where(c => c.ID == cusID.Value).FirstOrDefault().Point;
        //        var pointConfig = cnn.Configs.Where(c => c.Key == SystemParam.MinPoint).FirstOrDefault().Value;
        //        if (pointOfCus < (dou)pointConfig)
        //        {
        //            return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_NOT_ENOUGH_CONDITION_DRAW_POINT, "");
        //        }

        //        if (input.point > pointOfCus)
        //            return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_MAX_POINT_DRAW, "");
        //        var data = historyPoint.GetWithrawPoints(cusID.Value, input.point.HasValue ? input.point : 0);
        //        if (data != null)
        //        {
        //            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
        //        }
        //        else
        //        {
        //            return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return serverError();
        //    }
        //}

        // Tiến hành rút điểm Key Value  MaxPointPerDay MinPoint Config 
        public JsonResultModel DrawPoint([FromBody] PointInputModel dt)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var check = cnn.Customers.Where(c => c.ID.Equals(cusID.Value)).FirstOrDefault();
                //Kiểm tra tài khoản đã được active chưa
                if (check.Status == SystemParam.ACTIVE_FALSE)
                    return response(SystemParam.ERROR, SystemParam.ERROR_ACOUNT_DEACTIVE, SystemParam.MESSAGE_ERROR_CONDITION_STATUS_ACCOUNT_DEACTIVE, "");
                if (check.Point < dt.point)
                    return response(SystemParam.ERROR, SystemParam.ERROR_CONDITION_POINT, SystemParam.MESSAGE_ERROR_MAX_POINT_DRAW, "");
                var checkMinPoint = cnn.Configs.Where(c => c.Key == SystemParam.MinPoint).FirstOrDefault().Value;
                if (dt.point < checkMinPoint)
                    return response(SystemParam.ERROR, SystemParam.ERROR_MINPOINT_DRAW, "Số điểm tối thiểu trên một lần rút phải lớn hơn " + checkMinPoint, "");
                var data = historyPoint.SuccessDrawPoint(cusID.Value, dt.point.Value, dt.BankID);
                if (data != null)
                {
                    return GetHistyoriesDetail(data.RequestID);
                }
                return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_DRAW_POINT_ERROR, "");
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // Lấy danh sách thông báo
        [HttpGet]
        public JsonResultModel GetNotify()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<NotifiedByCustomerIDOutputModel> query = new List<NotifiedByCustomerIDOutputModel>();
                query = notiBus.GetListNotify(cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }



        //[HttpGet]
        //public JsonResultModel GetHomeScreen()
        //{
        //    return response(SystemParam.ERROR, SystemParam.DEVICE_ID_NOT_FOUND, SystemParam.DEVICE_ID_NOT_FOUND_MESSAGE, "");
        //}


        // thông tin màn trang chủ
        [HttpGet]
        public JsonResultModel GetHomeScreen()
        {
            try
            {
                string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //if (String.IsNullOrEmpty(deviceID))
                //    return response(SystemParam.ERROR, SystemParam.DEVICE_ID_NOT_FOUND, SystemParam.DEVICE_ID_NOT_FOUND_MESSAGE, "");

                int? customerID = null;
                if (token.Length > 0)
                {
                    customerID = checkTokenApp(token);
                }

                //int? customerID = Util.checkTokenApp(token);
                //if (customerID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                HomeScreenOutPutModel query = new HomeScreenOutPutModel();

                if (customerID != null)
                {
                    Customer cust = cnn.Customers.Find(customerID.Value);

                    CustomerLogin customerLogin = new CustomerLogin();
                    customerLogin.CustomerName = cust.Name;
                    customerLogin.Point = cust.Point;

                    query.UserInfo = customerLogin;
                    query.customerInfo = lgBus.GetuserInfor(customerID.Value);
                }
                else
                {
                    query.customerInfo = null;
                }

                query.listNews = newsBus.GetListNews(SystemParam.NEWS_TYPE_NEWS).Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                query.listBanner = newsBus.GetListNews(SystemParam.NEWS_TYPE_BANNER).Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                query.listEnvent = newsBus.GetListNews(SystemParam.NEWS_TYPE_EVENT).Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                //query.listProduct = getListProductHomeScreen().Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // cập nhật thông tin người dùng
        [HttpPost]
        public JsonResultModel UpdateUser(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                UserInforOutputModel item = JsonConvert.DeserializeObject<UserInforOutputModel>(input.ToString());
                if (item.Phone.Length == 0 || item.CustomerName.Length == 0 || item.Email.Length == 0 || item.ProvinceID <= 0 || item.DistrictID <= 0)
                    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_USER_INFO, "");
                if (!Util.ValidateEmail(item.Email))
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");
                if (!Util.checkDistrictInProvince(item.ProvinceID, item.DistrictID))
                    return response(SystemParam.ERROR, SystemParam.ERROR_DISTRICT_NOT_IN_PROVINCE, SystemParam.MESSAGE_ERROR_DISTRICT_NOT_IN_PROVINCE, "");

                int res = lgBus.UpdateUser(item, cusID.Value);
                switch (res)
                {
                    case SystemParam.ERROR_INVALID_PHONE:
                        return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_PHONE, SystemParam.MESSAGE_ERROR_INVALID_PHONE, "");
                    case SystemParam.ERROR_EXIST_PHONE:
                        return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_PHONE, SystemParam.MESSAGE_ERROR_EXIST_PHONE, "");
                    case SystemParam.ERROR_EXIST_EMAIL:
                        return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_EXIST_EMAIL, "");
                    case SystemParam.ERROR:
                        return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR, "");
                    default:
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lgBus.GetuserInfor(cusID.Value));
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // danh sách tỉnh thành, quận huyện
        [HttpGet]
        public JsonResultModel GetProvinceAndDistric(int? proID)
        {
            try
            {
                ProvinceAndDistrictModel query = new ProvinceAndDistrictModel();
                var listProvice = from p in cnn.Provinces
                                  orderby p.Code
                                  select new
                                  {
                                      ProvinceID = p.Code,
                                      ProvinceName = p.Name,
                                      ProvinceType = p.Type
                                  };
                var listDistrict = from d in cnn.Districts
                                   orderby d.ProvinceCode
                                   select new
                                   {
                                       DistrictID = d.Code,
                                       DistrictName = d.Name,
                                       DistrictType = d.Type,
                                       ProvinceID = d.ProvinceCode
                                   };

                if (proID == null || proID == 0)
                {
                    query.province = listProvice.ToList();
                    query.listDistrict = listDistrict.ToList();
                }
                else
                {
                    var Province = listProvice.Where(u => u.ProvinceID.Equals(proID.Value) || u.ProvinceID.Equals(0)).FirstOrDefault();
                    if (Province != null)
                        query.province = Province;
                    else
                        query.province = new object();
                    query.listDistrict = listDistrict.Where(u => u.ProvinceID.Equals(proID.Value));
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }



        [HttpPost]
        public JsonResultModel uploadImage()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? customerID = checkTokenApp(token);
                if (customerID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                Customer customer = cnn.Customers.Find(customerID.Value);
                if (customer.Role != SystemParam.ROLE_AGENT)
                    return response(SystemParam.ERROR, SystemParam.ERROR, "Tài khoản của bạn không phải đại lý. Không thể tải ảnh lên.", "");

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    string rootFolder = HttpContext.Current.Server.MapPath(@"\Uploads\agent\");
                    var docfiles = new List<string>();
                    string urlFile = "";
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        string name = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".jpg";
                        var filePath = HttpContext.Current.Server.MapPath(@"\Uploads\agent\" + name);
                        urlFile = "http://" + HttpContext.Current.Request.Url.Authority + "/Uploads/agent/" + name;
                        postedFile.SaveAs(filePath);
                        docfiles.Add(urlFile);

                        CustomerImage customerImage = new CustomerImage();
                        customerImage.CustomerID = customerID.Value;
                        customerImage.Images = urlFile;
                        customerImage.IsActive = SystemParam.ACTIVE;
                        cnn.CustomerImages.Add(customerImage);
                        //cnn.SaveChanges();
                    }
                    cnn.SaveChanges();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, docfiles);
                }
                else
                {
                    return serverError();
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Maximum request length exceeded.")
                    return response(SystemParam.ERROR, SystemParam.ERROR, "Kích thước ảnh quá lớn. Tối đa 4MB", "");
                return serverError();
            }
        }



        [HttpPost]
        public JsonResultModel deleteImage(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? customerID = checkTokenApp(token);
                if (customerID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ListICustomerImageIDModel data = JsonConvert.DeserializeObject<ListICustomerImageIDModel>(input.ToString());
                string rootFolder = HttpContext.Current.Server.MapPath(@"\Uploads\agent");

                foreach (int id in data.listID)
                {
                    var img = cnn.CustomerImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(id));
                    if (img != null && img.Count() > 0)
                    {
                        if (img.FirstOrDefault().CustomerID != customerID.Value)
                            return response(SystemParam.ERROR, SystemParam.ERROR, "Chỉ có thể xóa ảnh của mình", "");

                        img.FirstOrDefault().IsActive = SystemParam.NO_ACTIVE_DELETE;

                        string[] str = img.FirstOrDefault().Images.Split('/');
                        string fileName = rootFolder + "\\" + str[str.Length - 1];
                        // xóa file cũ
                        string[] files = Directory.GetFiles(rootFolder);
                        foreach (string file in files)
                        {
                            //string fileName = rootFolder + str[str.Length - 1];
                            if (file.Equals(fileName))
                            {
                                File.Delete(file);
                                Console.WriteLine($"{file} da xoa.");
                            }
                        }
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.ERROR, "Ảnh không tồn tại", "");
                    }
                }
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception e)
            {
                return serverError();
            }
        }



        [HttpGet]
        public JsonResultModel getImages()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? customerID = checkTokenApp(token);
                if (customerID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<ListCustomerImageModel> data = new List<ListCustomerImageModel>();
                var query = (from g in cnn.CustomerImages
                             where g.CustomerID == customerID.Value && g.IsActive == SystemParam.ACTIVE
                             orderby g.ID descending
                             select new ListCustomerImageModel
                             {
                                 ID = g.ID,
                                 image = g.Images
                             });

                if (query != null && query.Count() > 0)
                {
                    data = query.ToList();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }



        [HttpGet]
        public object PushNotify(string deviceId, string title, string content, int type, int id, string code, int qty)
        {
            try
            {
                NotifyDataModel notifyData = new NotifyDataModel();
                notifyData.type = type;
                notifyData.id = id;
                notifyData.code = code;
                List<string> listDevice = new List<string>();
                listDevice.Add(deviceId);
                for (int i = 1; i <= qty; i++)
                {
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, title, content + i);
                    packageBusiness.PushOneSignals(value);
                    System.Threading.Thread.Sleep(1000);
                }
                return notifyData;
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        //[HttpGet]
        //public object PushNotifyChat(int id, string message)
        //{
        //    try
        //    {
        //        Customer customer = cnn.Customers.Find(id);
        //        NotifyDataModel notifyData = new NotifyDataModel();
        //        notifyData.type = SystemParam.NOTIFY_NAVIGATE_CHAT;
        //        if (customer.DeviceID.Length > 20 && !customer.DeviceID.Contains(" "))
        //        {
        //            List<string> listDevice = new List<string>();
        //            listDevice.Add(customer.DeviceID);
        //            string value = packageBusiness.StartPushNoti(notifyData, listDevice, "Bạn có tin nhắn mới", message);
        //            string a = packageBusiness.PushOneSignals();
        //        }
        //        return notifyData;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}



        [HttpGet]
        public JsonResultModel ChangeViewed(int notiID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                }
                if (checkTokenApp(token) != null)
                {
                    Notification noti = cnn.Notifications.Find(notiID);
                    if (noti != null && noti.IsActive.Equals(SystemParam.ACTIVE))
                    {
                        noti.Viewed = 1;
                        cnn.SaveChanges();
                        NotifiedByCustomerIDOutputModel notidetail = new NotifiedByCustomerIDOutputModel();
                        notidetail.NotifyID = noti.ID;
                        notidetail.Content = noti.Content;
                        notidetail.CreatedDate = noti.CreateDate;
                        notidetail.Viewed = 1;
                        notidetail.Title = noti.Title;
                        notidetail.Type = noti.Type.Value;

                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, notidetail);
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_NOTIFY_NOT_FOUND, "");
                    }
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // đăng xuất
        [HttpGet]
        public JsonResultModel Logout()
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

            Customer cust = cnn.Customers.Find(cusID.Value);
            cust.Token = "";
            cust.DeviceID = "";
            cnn.SaveChanges();
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }


        // danh sách đơn hàng
        [HttpGet]
        public JsonResultModel GetListOrder(int? status, int page, int limit)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                //if (String.IsNullOrEmpty(text))
                //    text = "";

                var lsOrder = orderBus.GetListOrderByStatus(status, checkTokenApp(token).Value, null, null);
                int count = lsOrder.Count();
                double totalPage = (double)count / (double)limit;
                ListOrderOutputModel listOrderOutputModel = new ListOrderOutputModel();
                listOrderOutputModel.limit = limit;
                listOrderOutputModel.totalCount = count;
                listOrderOutputModel.page = page;
                listOrderOutputModel.lastPage = (int)Math.Ceiling(totalPage);
                listOrderOutputModel.listOrder = lsOrder.ToPagedList(page, limit);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, listOrderOutputModel);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // chi tiết đơn hàng
        [HttpGet]
        public JsonResultModel GetOrderDetail(int orderID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                var query = orderBus.GetOrderDetail(orderID, null);
                return response(query != null ? SystemParam.SUCCESS : SystemParam.ERROR, query != null ? SystemParam.SUCCESS_CODE : SystemParam.PROCESS_ERROR, query != null ? "Thành công" : "Đơn hàng không tồn tại", query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // tạo đơn hàng
        [HttpPost]
        public JsonResultModel CreateOrder(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                OrderDetailOutputModel data = JsonConvert.DeserializeObject<OrderDetailOutputModel>(input.ToString());

                //Kiểm tra điều kiện tài khoản có đủ để mua hàng hay không
                var checkCus = cnn.Customers.Where(c => c.ID == cusID).FirstOrDefault();
                if (data.BuyerName.Length == 0 || data.BuyerPhone.Length == 0 || data.Address.Length == 0 || data.DistrictID == null || data.ProvinceID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_USER_INFO, SystemParam.MESSAGE_ERROR_USER_INFO, "");


                //Kiểm tra tài khoản đã được active chưa
                if (checkCus.Status == SystemParam.ACTIVE_FALSE)
                    return response(SystemParam.ERROR, SystemParam.ERROR_ACOUNT_DEACTIVE, SystemParam.MESSAGE_ERROR_CONDITION_STATUS_ACCOUNT_DEACTIVE, "");

                //Kiểm tra số điểm hiện có có đủ để mua hàng hay không
                var totalPrice = data.listOrderItem.Select(p => p.SumPrice).Sum();
                if (checkCus.Point < Math.Round(Convert.ToDouble(totalPrice / 1000), 2))
                    return response(SystemParam.ERROR, SystemParam.ERROR_CONDITION_POINT, SystemParam.MESSAGE_ERROR_CONDITION_POINT_CREATE_ORDER_FAIL, "");

                ////Kiểm  tra mã giới thiệu có hợp lệ hay không
                //if (!String.IsNullOrEmpty(data.LastRefCode))
                //{
                //    var checkPhone = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Phone == data.LastRefCode && c.Status.Equals(SystemParam.ACTIVE) && c.Phone != checkCus.Phone).Select(c => c.Phone).FirstOrDefault();
                //    if (String.IsNullOrEmpty(checkPhone) || checkPhone.Length == 0)
                //        return response(SystemParam.ERROR, SystemParam.ERROR_LASTREF_CODE, SystemParam.MESSAGE_ERROR_LASTREF_CODE, "");

                //}
                if (!Util.checkDistrictInProvince(data.ProvinceID.Value, data.DistrictID.Value))
                    return response(SystemParam.ERROR, SystemParam.ERROR_DISTRICT_NOT_IN_PROVINCE, SystemParam.MESSAGE_ERROR_DISTRICT_NOT_IN_PROVINCE, "");

                data.BuyerPhone = Util.ConvertPhone(data.BuyerPhone);
                if (!Util.validPhone(data.BuyerPhone))
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_PHONE, SystemParam.MESSAGE_ERROR_INVALID_PHONE, "");

                if (data.listOrderItem.Count() == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_CREATE_ORDER_NO_DATA, SystemParam.MESSAGE_ERROR_CREATE_ORDER_NO_DATA, "");

                Customer customer = cnn.Customers.Find(cusID.Value);
                UserInforOutputModel updateUser = new UserInforOutputModel();
                updateUser.CustomerName = data.BuyerName.Trim();
                updateUser.Phone = data.BuyerPhone;
                updateUser.Address = data.Address.Trim();
                updateUser.Email = customer.Email;
                updateUser.ProvinceID = data.ProvinceID.HasValue ? data.ProvinceID.Value : customer.ProvinceCode;
                updateUser.DistrictID = data.DistrictID.HasValue ? data.DistrictID.Value : customer.DistrictCode;
                updateUser.Sex = customer.Sex;
                updateUser.DOBStr = customer.DOB.ToString(SystemParam.CONVERT_DATETIME);

                int res = lgBus.UpdateUser(updateUser, cusID.Value);
                if (res == SystemParam.ERROR_EXIST_PHONE)
                    return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_PHONE, SystemParam.MESSAGE_ERROR_EXIST_PHONE, "");
                if (res == SystemParam.ERROR_EXIST_EMAIL)
                    return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_EXIST_EMAIL, "");
                if (res == SystemParam.ERROR)
                    return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR, "");

                var order = CreateOrder(data, cusID.Value, data.LastRefCode);
                if (order != null && order.OrderID > 0)
                {
                    //EmailBusiness email = new EmailBusiness();
                    //email.configClient(SystemParam.EMAIL_CONFIG, "[DAIICHI CÓ ĐƠN HÀNG MỚI]", "Đã có khách đặt đơn hàng với mã " + order.Code + ". Giá trị: " + String.Format("{0:0,0 VND}", order.TotalPrice));
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, order);
                }
                return response(SystemParam.ERROR, SystemParam.CREATE_ORDER_FAIL, SystemParam.MESSAGE_CREATE_ORDER_FAIL, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }

        public OrderOutputModel CreateOrder(OrderDetailOutputModel input, int cusID, string lastRefCode)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            var conect = cnn.Database.BeginTransaction();
            try
            {

                //tạo một đơn hàng mới
                Order od = new Order();
                var cus = cnn.Customers.Where(c => c.ID == cusID).FirstOrDefault();
                var code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                List<OrderItem> listOI = CreateOrderItem(input.listOrderItem);
                var point = Math.Round(Convert.ToDouble(listOI.Select(u => u.SumPrice).Sum()) / 1000, 2);
                //Tính toán lại số dư ki mua hàng
                var balance = cus.Point - point;
                od.Code = code;
                od.Status = SystemParam.STATUS_ORDER_PENDING;
                od.Type = SystemParam.TYPE_ORDER;
                od.PointAdd = 0;
                od.IsActive = SystemParam.ACTIVE;
                od.CreateDate = DateTime.Now;
                od.CustomerID = cusID;
                od.ProvinceID = input.ProvinceID;
                od.DistrictID = input.DistrictID;
                od.Note = input.Note;
                od.TotalPrice = Convert.ToInt64(listOI.Select(u => u.SumPrice).Sum());
                od.Discount = 0;
                od.OrderItems = listOI;
                od.BuyerName = input.BuyerName.Trim();
                od.BuyerPhone = input.BuyerPhone;
                od.BuyerAddress = input.Address.Trim();
                od.LastRefCode = lastRefCode;

                //Lưu lại số dư sau khi đã được tính toán
                cus.Point = balance;

                //Tạo lịch sử mua hàng
                MembersPointHistory m = new MembersPointHistory();
                m.CustomerID = cusID;
                m.Point = point;
                m.Type = SystemParam.TYPE_MINUS_POINT_ORDER;
                m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                m.TypeAdd = SystemParam.TYPE_POINT;
                m.CraeteDate = DateTime.Now;
                m.IsActive = SystemParam.ACTIVE;
                m.Comment = "Hệ thống trừ điểm khi mua hàng";
                m.Title = "Bạn vừa bị trừ " + point + " điểm từ đơn hàng " + code;
                m.Balance = balance;
                m.ProductID = null;
                m.UserSendID = null;

                //Tạo thông báo cho người nhận
                Notification ntf = new Notification();
                ntf.CustomerID = cusID;
                ntf.Content = "Bạn vừa bị trừ " + point + " điểm từ đơn hàng " + code;
                ntf.Viewed = 0;
                ntf.CreateDate = DateTime.Now;
                ntf.IsActive = SystemParam.ACTIVE;
                ntf.Title = "Bạn vừa bị trừ " + point + " điểm từ đơn hàng " + code;
                ntf.Type = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                ntf.NewsID = null;

                cnn.Notifications.Add(ntf);
                cnn.MembersPointHistories.Add(m);
                cnn.Orders.Add(od);
                cnn.SaveChanges();
                conect.Commit();
                conect.Dispose();

                //Tiến hành gửi thông báo đến web admin
                var url = SystemParam.URL_WEB_SOCKET + "?content=" + "Đơn hàng " + code + " đang chờ xác nhận&type=" + SystemParam.TYPE_NOTI_ORDER;
                packageBusiness.GetJson(url);
                if (cus.DeviceID != null && cus.DeviceID.Length > 15)
                {
                    //Tiến hành gửi thông báo
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.Point = balance;
                    notifyData.type = SystemParam.NOTIFY_NAVIGATE_ORDER;
                    notifyData.StatusOrder = SystemParam.STATUS_ORDER_PENDING;
                    notifyData.id = od.ID;
                    string titleNoti = "Bạn vừa bị trừ " + point + " điểm từ đơn hàng " + code;
                    List<string> listDevice = new List<string>();
                    listDevice.Add(cus.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống trừ điểm khi mua hàng");
                    packageBusiness.PushOneSignals(value);
                }
                int id = cnn.Orders.OrderByDescending(u => u.ID).FirstOrDefault().ID;
                return orderBus.GetOrderDetail(id, balance);
            }
            catch
            {
                conect.Rollback();
                conect.Dispose();
                return null;
            }
        }

        public List<OrderItem> CreateOrderItem(List<OrderDetailModel> lsOrderItem)
        {
            List<OrderItem> lsOI = new List<OrderItem>();
            foreach (var orderItem in lsOrderItem)
            {
                // cập nhật những sản phẩm đc đặt mua thì xóa khỏi giỏ hàng
                // nếu orderItemID == null là trường hợp đặt mua ngay, không có trong giỏ hàng
                if (orderItem.OrderItemID != null)
                {
                    var currentCart = cnn.OrderItems.Find(orderItem.OrderItemID);
                    currentCart.QTY = 0;
                    currentCart.SumPrice = 0;
                }

                OrderItem oi = new OrderItem();
                oi.ItemID = orderItem.ItemID;
                oi.QTY = orderItem.Qty;
                oi.SumPrice = orderItem.SumPrice;
                oi.Status = 1;
                oi.Type = SystemParam.TYPE_ORDER;
                oi.Discount = 0;
                oi.IsActive = SystemParam.ACTIVE;
                oi.CreateDate = DateTime.Now;
                oi.UpdateAt = DateTime.Now;
                lsOI.Add(oi);
            }
            cnn.SaveChanges();
            return lsOI;
        }


        // hủy đơn hàng
        [HttpGet]
        public JsonResultModel CancelOrder(int orderID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                }
                int? customerID = checkTokenApp(token);
                if (customerID != null)
                {
                    double point = 0;
                    Order order = cnn.Orders.Find(orderID);
                    var cus = cnn.Customers.Find(customerID.Value);
                    if (order == null)
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, "Hệ thống không tìm thấy thông tin đơn hàng", "");
                    if (order.CustomerID != customerID.Value)
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, "Bạn không thể hủy đơn hàng không có trong danh sách của mình", "");

                    if (order.Status == SystemParam.STATUS_ORDER_PENDING)
                    {
                        point = (double)order.TotalPrice / 1000;
                        order.Status = SystemParam.STATUS_ORDER_CANCEL;
                        order.CancelDate = DateTime.Now;
                        //Hoàn lại điểm cho người mua
                        var balance = Math.Round(Convert.ToDouble(cus.Point + point), 2);
                        cus.Point = balance;
                        //Tạo lịch sử hoàn tiền từ đơn hàng
                        MembersPointHistory m = new MembersPointHistory();
                        m.CustomerID = customerID;
                        m.Point = point;
                        m.Type = SystemParam.TYPEADD_POINT_FROM_BILL;
                        m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                        m.TypeAdd = SystemParam.TYPE_POINT;
                        m.CraeteDate = DateTime.Now;
                        m.IsActive = SystemParam.ACTIVE;
                        m.Comment = "Hệ thống hoàn điểm khi đơn hàng bị hủy";
                        m.Title = "Bạn vừa được hoàn " + point + " điểm từ đơn hàng " + order.Code;
                        m.Balance = balance;
                        m.ProductID = null;
                        m.UserSendID = null;
                        //Tạo thông báo cho người nhận
                        Notification ntf = new Notification();
                        ntf.CustomerID = cus.ID;
                        ntf.Content = "Hệ thống hoàn điểm khi đơn hàng bị hủy";
                        ntf.Viewed = 0;
                        ntf.CreateDate = DateTime.Now;
                        ntf.IsActive = SystemParam.ACTIVE;
                        ntf.Title = "Bạn vừa được hoàn " + point + " điểm từ đơn hàng " + order.Code;
                        ntf.Type = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                        ntf.NewsID = null;
                        cnn.Notifications.Add(ntf);
                        cnn.MembersPointHistories.Add(m);
                        cnn.SaveChanges();
                        var query = orderBus.GetOrderDetail(orderID, balance);
                        if (cus.DeviceID.Length > 15 && cus.DeviceID != null)
                        {
                            //Tiến hành gửi thông báo
                            NotifyDataModel notifyData = new NotifyDataModel();
                            notifyData.type = SystemParam.NOTIFY_NAVIGATE_ORDER;
                            notifyData.StatusOrder = SystemParam.STATUS_ORDER_CANCEL;
                            notifyData.Point = balance;
                            notifyData.id = orderID;
                            string titleNoti = "Bạn vừa được hoàn " + point + " điểm từ đơn hàng " + order.Code;
                            List<string> listDevice = new List<string>();
                            listDevice.Add(cus.DeviceID);
                            string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống hoàn điểm khi đơn hàng bị hủy");
                            packageBusiness.PushOneSignals(value);
                        }
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, "Bạn đã hủy thành công đơn hàng.", query);
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, "Bạn không thể hủy đơn hàng này", "");
                    }
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // danh sách cửa hàng
        [HttpGet]
        public JsonResultModel GetListShop(int? provinceID, double latitude, double longitude)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                var query = shopBus.GetListShop(provinceID, latitude, longitude);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        //lấy tổng số sản phẩm trong giỏ hàng
        [HttpGet]
        public JsonResultModel GetCountCart()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, getCountCart(cusID.Value));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        public int getCountCart(int customerID)
        {
            return cnn.OrderItems.Where(u => u.QTY > 0 && u.Order.CustomerID.Equals(customerID) && u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(SystemParam.TYPE_CART)).Count();
        }


        // danh mục sản phẩm
        [HttpGet]
        public JsonResultModel getListCategory()
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<GroupItemModel> data = new List<GroupItemModel>();
                var query = cnn.GroupItems.Where(g => g.IsActive == SystemParam.ACTIVE && !g.ParentID.HasValue && g.Status.Value.Equals(SystemParam.ACTIVE))
                    .Select(g => new GroupItemModel()
                    {
                        ID = g.ID,
                        Name = g.Name,
                        Description = g.Description
                    });

                if (query != null && query.Count() > 0)
                {
                    data = query.ToList();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }


        // danh sách sản phẩm
        //[HttpGet]
        //public JsonResultModel getListProduct()
        //{
        //    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
        //}

        //[HttpGet]
        //public JsonResultModel getListProduct(int page, int limit, int ParentID)
        //{
        //    return getListProduct(page, limit, ParentID, null);
        //}

        [HttpGet]
        public JsonResultModel getListProduct(int page = 1, int limit = 10, int? CateID = null, string text = "")
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<ListItemModel> data = new List<ListItemModel>();
                var query = (from i in cnn.Items
                             where i.IsActive == SystemParam.ACTIVE && i.Status == SystemParam.ACTIVE && (CateID.HasValue ? i.GroupItemID == CateID.Value : true)
                             orderby i.ID descending
                             select i).ToList();
                var listProduct = query.Select(i => new ListItemModel
                {
                    ID = i.ID,
                    Code = i.Code,
                    Name = i.Name,
                    Image = i.ImageUrl.Split(',').ToList(),
                    Price = i.Price,
                    Description = i.Description,
                    Technical = i.Technical,
                    Warranty = i.Warranty,
                    StockStatus = i.StockStatus.Value
                }).ToList();

                if (listProduct != null && listProduct.Count() > 0)
                {
                    data = listProduct;
                    if (!String.IsNullOrEmpty(text))
                    {
                        string[] str = Util.Converts(text.ToLower()).Split(' ');
                        foreach (string key in str)
                        {
                            data = data.Where(u => Util.Converts(u.Name.ToLower()).Contains(key) || Util.Converts(u.Code.ToLower()).Contains(key)).ToList();
                        }
                    }
                }
                int count = data.Count();
                double totalPage = (double)count / (double)limit;
                ListProductAppModel listProductApp = new ListProductAppModel();
                listProductApp.limit = limit;
                listProductApp.totalCount = count;
                listProductApp.page = page;
                listProductApp.totalPage = (int)Math.Ceiling(totalPage);
                listProductApp.listProduct = data.ToPagedList(page, limit);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, listProductApp);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // thông tin chi tiết sản phẩm
        [HttpGet]
        public JsonResultModel getProductDetail(int ProductID)
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                var products = from i in cnn.Items
                               where i.IsActive == SystemParam.ACTIVE && i.ID == ProductID
                               orderby i.ID descending
                               select new ItemDetailModel
                               {
                                   ProductID = i.ID,
                                   Code = i.Code,
                                   Name = i.Name,
                                   Image = i.ImageUrl,
                                   Price = i.Price,
                                   Description = i.Description,
                                   Technical = i.Technical,
                                   StockStatus = i.StockStatus.Value
                               };
                if (products != null && products.Count() > 0)
                {
                    var product = products.FirstOrDefault();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, product);
                }
                return response(SystemParam.ERROR, SystemParam.NOT_EXIST_PRODUCT, SystemParam.MESSAGE_NOT_EXIST_PRODUCT, new object());
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // lấy danh sách sản phẩm mới nhất hiện ra trang chủ
        public List<ListItemModel> getListProductHomeen()
        {
            try
            {
                List<ListItemModel> data = new List<ListItemModel>();
                var listProduct = from i in cnn.Items
                                  where i.IsActive == SystemParam.ACTIVE && i.Special == SystemParam.SHOW_HOME_SCREEN
                                  orderby i.ID descending
                                  select new ListItemModel
                                  {
                                      ID = i.ID,
                                      Code = i.Code,
                                      Name = i.Name,
                                      Image = i.ImageUrl.Split(',').ToList(),
                                      Price = i.Price,
                                      Description = i.Description,
                                      Technical = i.Technical,
                                      Warranty = i.Warranty,
                                      Special = i.Special
                                  };

                if (listProduct != null && listProduct.Count() > 0)
                {
                    data = listProduct.ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                return new List<ListItemModel>();
            }
        }


        // lấy danh sách giỏ hàng giỏ hàng
        [HttpGet]
        public JsonResultModel getCarts()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, getListCart(cusID.Value));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        public ListCartOutputModel getListCart(int customerID)
        {
            ListCartOutputModel data = new ListCartOutputModel();
            var cart = cnn.Orders.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(SystemParam.TYPE_CART) && u.CustomerID.Equals(customerID));
            if (cart != null && cart.Count() > 0)
            {
                int orderID = cart.FirstOrDefault().ID;
                var listOrderItem = (from c in cnn.OrderItems
                                     where c.IsActive == SystemParam.ACTIVE
                                     && c.OrderID == orderID
                                     && c.QTY > 0
                                     && c.Status == SystemParam.STATUS_CART_PENDING
                                     && (c.Item.IsActive == SystemParam.ACTIVE && c.Item.Status == SystemParam.STATUS_PRODUCT_ACTIVE)
                                     orderby c.UpdateAt descending
                                     select c).ToList();
                var query = listOrderItem.Select(c => new OrderDetailModel
                {
                    OrderItemID = c.ID,
                    ItemID = c.ItemID,
                    ItemName = c.Item.Name,
                    ItemPrice = c.Item.Price,
                    Image = c.Item.ImageUrl.Split(',').FirstOrDefault(),
                    Qty = c.QTY,
                    Warranty = c.Item.Warranty,
                    SumPrice = Convert.ToInt64(c.SumPrice),
                    Description = c.Item.Description,
                    UpdateAt = c.UpdateAt,
                    Status = c.Item.IsActive.Equals(SystemParam.ACTIVE) && c.Item.Status.Equals(SystemParam.ACTIVE) && c.Item.StockStatus.Value.Equals(SystemParam.ACTIVE) ? SystemParam.ACTIVE : SystemParam.ACTIVE_FALSE
                }).ToList();
                if (query != null && query.Count() > 0)
                {
                    data.TotalPrice = listOrderItem.Sum(o => o.SumPrice);
                    data.listCart = query;
                }
                else
                {
                    data.TotalPrice = 0;
                    data.listCart = new List<OrderDetailModel>();
                }
            }
            return data;
        }


        // thêm sp vào giỏ hàng
        [HttpPost]
        public JsonResultModel AddToCart(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                OrderDetailModel orderItem = JsonConvert.DeserializeObject<OrderDetailModel>(input.ToString());
                int res = AddToCart(orderItem, cusID.Value);
                if (res == SystemParam.ERROR)
                    return response(SystemParam.ERROR, SystemParam.ERROR_ADD_TO_CART, SystemParam.ERROR_ADD_TO_CART_MESSAGE, "");
                if (res == SystemParam.ADD_TO_CART_FAIL)
                    return response(SystemParam.ERROR, SystemParam.ADD_TO_CART_FAIL, SystemParam.MESSAGE_ADD_TO_CART_FAIL, "");
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_ADD_TO_CART_SUCCESS, getCountCart(cusID.Value));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        public int AddToCart(OrderDetailModel orderItem, int customerID)
        {
            try
            {
                Order order = cnn.Orders.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.Equals(customerID) && u.Type.Equals(SystemParam.TYPE_CART)).FirstOrDefault();
                Customer cus = cnn.Customers.Find(customerID);
                if (order != null && order.ID > 0)
                {
                    //Tiến hành cập nhật giỏ hàng
                    int result = CreateOrderItem(orderItem.ListOrderItemID, order.ID);
                    if (result == SystemParam.ERROR)
                        return SystemParam.ERROR;
                    //Cập nhật thông tin người mua
                    order.BuyerName = cus.Name;
                    order.BuyerPhone = cus.Phone;
                }
                else
                {
                    Order od = new Order();
                    //lấy danh sách sản phẩm được chọn
                    List<Item> lstItem = cnn.Items.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE) && i.StockStatus.Value.Equals(SystemParam.ACTIVE) && orderItem.ListOrderItemID.Contains(i.ID)).ToList();
                    if (lstItem.Count() == 0)
                        return SystemParam.ADD_TO_CART_FAIL;
                    List<OrderItem> odItem = new List<OrderItem>();
                    foreach (var dt in lstItem)
                    {
                        OrderItem oi = new OrderItem();
                        oi.ItemID = dt.ID;
                        oi.QTY = SystemParam.QTY_DEFAULT_ADD_TO_CART;
                        oi.SumPrice = dt.Price;
                        oi.Status = SystemParam.STATUS_CART_PENDING;
                        oi.Type = SystemParam.TYPE_CART;
                        oi.Discount = 0;
                        oi.IsActive = SystemParam.ACTIVE;
                        oi.CreateDate = DateTime.Now;
                        oi.UpdateAt = DateTime.Now;
                        odItem.Add(oi);

                    }

                    od.Type = SystemParam.TYPE_CART;
                    od.IsActive = SystemParam.ACTIVE;
                    od.CreateDate = DateTime.Now;
                    od.CustomerID = customerID;
                    od.TotalPrice = lstItem.Select(i => i.Price).Sum();
                    od.Discount = 0;
                    od.OrderItems = odItem;
                    od.BuyerName = cus.Name;
                    od.BuyerPhone = cus.Phone;
                    cnn.Orders.Add(od);
                }
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }

        public int CreateOrderItem(List<int> lstOrderIdtem, int? orderID)
        {
            try
            {
                List<OrderItem> lsOI = new List<OrderItem>();
                //lấy danh sách sản phẩm được chọn
                List<int> lstItemID = cnn.Items.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE) && i.StockStatus.Value.Equals(SystemParam.ACTIVE) && lstOrderIdtem.Contains(i.ID)).Select(i => i.ID).ToList();


                if (lstItemID.Count() == 0)
                    return SystemParam.ADD_TO_CART_FAIL;
                var item = cnn.Items.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                var orderItems = cnn.OrderItems.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                foreach (var dt in lstItemID)
                {

                    Item it = item.Where(i => i.ID.Equals(dt)).FirstOrDefault();
                    OrderItem odit = orderItems.Where(i => i.ItemID.Equals(dt) && (orderID.HasValue ? i.OrderID.Equals(orderID.Value) : false)).FirstOrDefault();
                    if (odit == null)
                    {
                        OrderItem oi = new OrderItem();
                        oi.ItemID = dt;
                        oi.QTY = SystemParam.QTY_DEFAULT_ADD_TO_CART;
                        oi.SumPrice = it.Price * oi.QTY;
                        oi.Status = SystemParam.STATUS_CART_PENDING;
                        oi.Type = SystemParam.TYPE_CART;
                        oi.Discount = 0;
                        oi.IsActive = SystemParam.ACTIVE;
                        oi.CreateDate = DateTime.Now;
                        oi.UpdateAt = DateTime.Now;
                        oi.OrderID = orderID.Value;
                        lsOI.Add(oi);
                    }
                    else
                    {
                        odit.QTY += SystemParam.QTY_DEFAULT_ADD_TO_CART;
                        odit.SumPrice = it.Price * odit.QTY;
                        odit.UpdateAt = DateTime.Now;
                        odit.Order.TotalPrice += Convert.ToInt64(odit.SumPrice);
                    }
                }
                if (lsOI.Count() > 0)
                {
                    Order odit = cnn.Orders.Find(orderID.Value);
                    odit.TotalPrice = Convert.ToInt64(lsOI.Select(o => o.SumPrice).Sum());
                }

                cnn.OrderItems.AddRange(lsOI);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }


        // cập nhật lại giỏ hàng
        [HttpPost]
        public JsonResultModel UpdateCart(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ListCartUpdateModel data = JsonConvert.DeserializeObject<ListCartUpdateModel>(input.ToString());
                if (data.listCartUpdate.Count() == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_UPDATE_CART_NO_DATA, SystemParam.MESSAGE_ERROR_UPDATE_CART_NO_DATA, "");
                int res = updateCarts(data.listCartUpdate);
                if (res == SystemParam.SUCCESS)
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE_UPDATE_CART, getListCart(cusID.Value));
                if (res == SystemParam.UPDATE_CART_FAIL)
                    return response(SystemParam.ERROR, SystemParam.UPDATE_CART_FAIL, SystemParam.MESSAGE_UPDATE_CART_FAIL, "");
                return serverError();
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        public int updateCarts(List<OrderDetailModel> listCart)
        {
            try
            {
                foreach (var updateCart in listCart)
                {
                    var currentCart = cnn.OrderItems.Find(updateCart.OrderItemID);
                    if (currentCart != null && currentCart.ID > 0)
                    {
                        if (updateCart.Qty != currentCart.QTY || updateCart.SumPrice != currentCart.SumPrice)
                        {
                            currentCart.QTY = updateCart.Qty;
                            currentCart.SumPrice = updateCart.SumPrice;
                            currentCart.UpdateAt = DateTime.Now;
                            cnn.SaveChanges();
                        }
                    }
                    else
                        return SystemParam.UPDATE_CART_FAIL;
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }


        // xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public JsonResultModel RemoveCart(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ListOrderItemID data = JsonConvert.DeserializeObject<ListOrderItemID>(input.ToString());
                List<int> listOrderItemID = data.listOrderItemID;
                if (listOrderItemID.Count() == 0)
                    return response(SystemParam.ERROR, SystemParam.REMOVE_CART_NO_ID, SystemParam.MESSAGE_REMOVE_CART_NO_ID, "");
                var listCart = cnn.OrderItems.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(SystemParam.TYPE_CART));
                int? price = null;
                foreach (var id in listOrderItemID)
                {
                    OrderItem orderItem = listCart.Where(u => u.ID.Equals(id)).FirstOrDefault();
                    if (orderItem != null && orderItem.QTY > 0)
                    {
                        orderItem.Order.TotalPrice = Convert.ToInt64(orderItem.Order.TotalPrice - orderItem.SumPrice);
                        orderItem.QTY = 0;
                        orderItem.SumPrice = 0;
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.ERROR_REMOVE_CART_FAIL, SystemParam.MESSAGE_ERROR_REMOVE_CART_FAIL, "");
                    }
                }
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, getListCart(cusID.Value));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }
        // lấy danh sách quà, thẻ cào
        [HttpGet]
        public JsonResultModel GetListGifts(int? Type, int? GiftID)
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                if (GiftID != null && GiftID > 0)
                {
                    var giftDetail = giftBus.GetGiftDetail(GiftID.Value);
                    if (giftDetail == null)
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_NOT_EXIST_INFO, new object());
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, giftDetail);
                }

                if (Type == null)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
                if (Type == SystemParam.TYPE_REQUEST_CARD)
                {
                    var listCard = giftBus.GetListCard();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, listCard.Count() > 0 ? SystemParam.SUCCESS_MESSAGE : SystemParam.MESSAGE_LIST_EMPTY, listCard);
                }
                var query = giftBus.GetListGift(Type.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, query.Count() > 0 ? SystemParam.SUCCESS_MESSAGE : SystemParam.MESSAGE_LIST_EMPTY, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // Gửi yêu cầu đổi quà
        [HttpPost]
        public JsonResultModel CreateRequest(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                CreateRequestAppInputModel item = JsonConvert.DeserializeObject<CreateRequestAppInputModel>(input.ToString());

                Gift gift = new Gift();
                try
                {
                    gift = cnn.Gifts.Find(item.GiftID);
                }
                catch
                {
                }
                if (item.type == SystemParam.TYPE_REQUEST_CARD)
                {
                    CardOutputModel card = rqBus.RequestCard(gift, cusID.Value);
                    if (card.price == SystemParam.KHONG_DU_DIEM_DOI_QUA)
                        return response(SystemParam.ERROR, SystemParam.KHONG_DU_DIEM_DOI_QUA, SystemParam.KHONG_DU_DIEM_DOI_QUA_MESSAGE + Util.minPointAccount(), "");
                    if (card.price == SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY)
                        return response(SystemParam.ERROR, SystemParam.KHONG_DU_DIEM_DOI_QUA, SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY_MESSAGE + Util.maxPointPerDay(), "");

                    if (!String.IsNullOrEmpty(card.seri) && card.seri.Length > 0)
                    {
                        Customer customer = cnn.Customers.Find(cusID.Value);
                        ChangePointOutputModel data = new ChangePointOutputModel();
                        data.CurrentPoint = customer.Point;
                        data.RankingPoint = customer.PointRanking;
                        CardOutput list = new CardOutput();
                        list.cardDetail = card;
                        list.changePoint = data;

                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, list);
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.CONFIRM_FAIL, "");
                    }
                }

                int output = rqBus.CreateRequest(gift, cusID.Value);
                switch (output)
                {
                    case SystemParam.GIFT_REQUEST_NOT_FOUND:
                        return response(SystemParam.ERROR, SystemParam.GIFT_REQUEST_NOT_FOUND, SystemParam.GIFT_REQUEST_NOT_FOUND_MESSAGE, "");
                    case SystemParam.KHONG_DU_DIEM_DOI_QUA:
                        return response(SystemParam.ERROR, SystemParam.KHONG_DU_DIEM_DOI_QUA, SystemParam.KHONG_DU_DIEM_DOI_QUA_MESSAGE + Util.minPointAccount(), "");
                    case SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY:
                        return response(SystemParam.ERROR, SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY, SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY_MESSAGE + Util.maxPointPerDay(), "");
                    case SystemParam.DIEM_DOI_QUA_LON_HON_DIEM_MINH:
                        return response(SystemParam.ERROR, SystemParam.DIEM_DOI_QUA_LON_HON_DIEM_MINH, SystemParam.DIEM_DOI_QUA_LON_HON_DIEM_MINH_MESSAGE, "");
                    default:
                        Customer cus = cnn.Customers.Find(cusID.Value);
                        ChangePointOutputModel query = new ChangePointOutputModel();
                        query.CurrentPoint = cus.Point;
                        query.RankingPoint = cus.PointRanking;
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.CREATE_REQUEST_SUCCESS_MESSAGE, query);
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // tặng điểm bằng số điện thoại
        [HttpPost]
        public JsonResultModel GivePoint(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                GetGiftPointInputModel item = JsonConvert.DeserializeObject<GetGiftPointInputModel>(input.ToString());
                if (String.IsNullOrEmpty(item.Phone) || item.Phone.Length == 0 || String.IsNullOrEmpty(item.Point) || item.Point.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                item.Phone = Util.ConvertPhone(item.Phone);
                if (!Util.validPhone(item.Phone))
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_PHONE, SystemParam.MESSAGE_ERROR_INVALID_PHONE, "");

                if (!Util.validNumber(item.Point))
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_POINT, SystemParam.MESSAGE_ERROR_INVALID_POINT, "");

                Customer cusRe = cnn.Customers.Where(u => u.Phone.Equals(item.Phone) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                if (cusRe != null && cusRe.ID != cusID.Value)
                {
                    if (int.Parse(item.Point) > 0)
                    {
                        // ví dụ: 00010 => 10 hjhj
                        while (item.Point.IndexOf('0') == 0)
                        {
                            item.Point = item.Point.Substring(1);
                        }
                        int res = pBus.CreateGivePoint(item, cusID.Value);
                        if (res == SystemParam.ERROR)
                            return serverError();
                        if (res == SystemParam.KHONG_DU_DIEM_DE_TANG)
                            return response(SystemParam.ERROR, SystemParam.KHONG_DU_DIEM_DE_TANG, SystemParam.MESSAGE_KHONG_DU_DIEM_DE_TANG + Util.minPointAccount(), "");
                        if (res == SystemParam.VUOT_QUA_HAN_MUC_QUY_DINH)
                            return response(SystemParam.ERROR, SystemParam.VUOT_QUA_HAN_MUC_QUY_DINH, SystemParam.MESSAGE_VUOT_QUA_HAN_MUC_QUY_DINH + Util.maxPointPerDay(), "");

                        Customer cus = cnn.Customers.Find(cusID.Value);
                        ChangePointOutputModel query = new ChangePointOutputModel();
                        query.CurrentPoint = cus.Point;
                        query.RankingPoint = cus.PointRanking;

                        if (cusRe.DeviceID.Length > 20 && !cusRe.DeviceID.Contains(" "))
                        {
                            NotifyDataModel notifyData = new NotifyDataModel();
                            notifyData.type = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                            string titleNoti = "Bạn đã được tặng " + item.Point + " điểm từ " + cus.Name + (cus.Phone.Length > 0 ? "(" + cus.Phone + ")" : "");
                            List<string> listDevice = new List<string>();
                            listDevice.Add(cusRe.DeviceID);
                            string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, item.Comment);
                            packageBusiness.PushOneSignals(value);
                        }
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_GIVE_POINT_SUCCESS, query);
                    }
                    else
                    {
                        return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR_GIVE_POINT_NO_POINT, "");
                    }
                }
                else if (cusRe != null && cusRe.ID == cusID.Value)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_CANT_GIVE_POINT_FOR_ME, "");
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_NOT_EXIST_USER, "");
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // đổi mật khẩu
        [HttpPost]
        public JsonResultModel changePassword(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ChangePasswordModel item = JsonConvert.DeserializeObject<ChangePasswordModel>(input.ToString());
                if (String.IsNullOrEmpty(item.password) || String.IsNullOrEmpty(item.newPassword) || item.password.Length == 0 || item.newPassword.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
                if (item.password == item.newPassword)
                    return response(SystemParam.ERROR, SystemParam.ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD, SystemParam.MESSAGE_ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD, "");
                Customer customer = cnn.Customers.Find(cusID.Value);
                if (Util.CheckPass(item.password, customer.Password))
                {
                    customer.Password = Util.GenPass(item.newPassword);
                    cnn.SaveChanges();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_CHANGE_PASSWORD_SUCCESS, "");
                }

                return response(SystemParam.ERROR, SystemParam.ERROR_WRONG_PASSWORD, SystemParam.MESSAGE_ERROR_WRONG_PASSWORD, "");


            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // kích hoạt đại lý
        [HttpPost]
        public JsonResultModel ActiveAgent(JObject input)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ActiveAgentCodeModel item = JsonConvert.DeserializeObject<ActiveAgentCodeModel>(input.ToString());
                if (String.IsNullOrEmpty(item.AgentCode) || item.AgentCode.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                var listAgent = cnn.Agents.Where(u => u.Code.Equals(item.AgentCode) && u.IsActive.Equals(SystemParam.ACTIVE));
                if (listAgent == null || listAgent.Count() == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_NOT_EXIST_AGENT_CODE, SystemParam.MESSAGE_ERROR_NOT_EXIST_AGENT_CODE, "");

                Agent agent = listAgent.FirstOrDefault();
                if (agent.CustomerActiveID != null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_AGENT_CODE_USED, SystemParam.MESSAGE_ERROR_AGENT_CODE_USED, "");

                Customer customer = cnn.Customers.Find(cusID.Value);
                if (customer.Role == SystemParam.ROLE_AGENT)
                    return response(SystemParam.ERROR, SystemParam.ERROR_CUSTOMER_WAS_AGENT, SystemParam.MESSAGE_ERROR_CUSTOMER_WAS_AGENT, "");

                var query = lgBus.ActiveAgent(cusID.Value, agent.ID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_ACTIVE_AGENT_SUCCESS, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // lịch sử giao dịch
        [HttpGet]
        public JsonResultModel GetPointHistory(string FromDate)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                List<HistoryGivePointWebOutputModel> query = new List<HistoryGivePointWebOutputModel>();
                DateTime? time;
                try
                {
                    time = DateTime.ParseExact(FromDate, SystemParam.CONVERT_DATETIME, null);
                }
                catch
                {
                    time = null;
                }
                query = pBus.ListHistoty(time, cusID.Value, SystemParam.ROLL_CUSTOMER);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // kích hoạt bảo hành bằng số điện thoại web khách hàng
        [HttpPost]
        public JsonResultModel activeWarrantyByPhone(JObject input)
        {
            try
            {
                AddPointIntPutModel data = JsonConvert.DeserializeObject<AddPointIntPutModel>(input.ToString());
                if (String.IsNullOrEmpty(data.code) || String.IsNullOrEmpty(data.phone))
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                data.phone = Util.ConvertPhone(data.phone.Trim());
                if (!Util.validPhone(data.phone))
                    return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR_INVALID_PHONE, "");

                var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone.Equals(data.phone)).FirstOrDefault();
                if (customer == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR, "Không tìm thấy thông tin khách hàng từ số điện thoại", "");

                int status = pBus.activeWarrantyByPhone(data, customer);
                if (status == -1)
                    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, "Mã bảo hành không tồn tại", "");
                if (status == -2)
                    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, "Mã bảo hành đã được sử dụng", "");

                //khi đã kích hoạt thành công
                var productWarranty = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductCode.Equals(data.code)).FirstOrDefault();
                var itemWarranty = (from i in cnn.Items
                                    where i.IsActive.Equals(SystemParam.ACTIVE) && i.ID.Equals(productWarranty.Batch.ItemID.Value)
                                    select new ActiveWarrantyModel
                                    {
                                        Code = i.Code,
                                        Name = i.Name,
                                        Image = i.ImageUrl.Split(',').ToList(),
                                        Warranty = i.Warranty,
                                    }).FirstOrDefault();
                ActiveCustomerInfo activedCustomer = new ActiveCustomerInfo();
                activedCustomer.ID = customer.ID;
                activedCustomer.Name = customer.Name;
                activedCustomer.Phone = customer.Phone;
                activedCustomer.Email = customer.Email;
                activedCustomer.Province = customer.Province.Name;
                activedCustomer.District = customer.District.Name;
                activedCustomer.Address = customer.Address;

                itemWarranty.activeDate = productWarranty.ActiveDate;
                itemWarranty.expireDate = productWarranty.ExpireDate;
                itemWarranty.activeCustomerInfo = activedCustomer;

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_ACTIVE_WARRANTY_SUCCESS, itemWarranty);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }



        // lấy thông tin sản phẩm bảo hành
        [HttpGet]
        public JsonResultModel getItemByCode(string code)
        {
            try
            {
                //string token = getTokenApp();
                //if (token.Length == 0)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                //int? cusID = Util.checkTokenApp(token);
                //if (cusID == null)
                //    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                if (String.IsNullOrEmpty(code))
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                var item = getItemWarranty(code.Trim());
                if (item == null)
                    return response(SystemParam.ERROR, SystemParam.WARRANTY_CODE_NOT_EXIST, SystemParam.MESSAGE_WARRANTY_CODE_NOT_EXIST, "");

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, item);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        public ActiveWarrantyModel getItemWarranty(string code)
        {
            var product = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductCode.Equals(code));
            if (product != null && product.Count() > 0)
            {
                var productWarranty = product.FirstOrDefault();
                var item = from i in cnn.Items
                           where i.IsActive.Equals(SystemParam.ACTIVE) && i.ID.Equals(productWarranty.Batch.ItemID.Value)
                           select new ActiveWarrantyModel
                           {
                               ID = i.ID,
                               Code = i.Code,
                               Name = i.Name,
                               Description = i.Description,
                               Warranty = i.Warranty,
                               Price = i.Price,
                               Technical = i.Technical,
                               Image = i.ImageUrl.Split(',').ToList()
                           };
                if (item != null && item.Count() > 0)
                {
                    var itemWarranty = item.FirstOrDefault();
                    if (productWarranty.CustomerActiveID != null)
                    {
                        ActiveCustomerInfo customer = new ActiveCustomerInfo();
                        var activeCustomer = cnn.Customers.Find(productWarranty.CustomerActiveID.Value);
                        customer.ID = activeCustomer.ID;
                        customer.Name = activeCustomer.Name;
                        customer.Phone = activeCustomer.Phone;
                        customer.Email = activeCustomer.Email;
                        customer.Province = activeCustomer.Province.Name;
                        customer.District = activeCustomer.District.Name;
                        customer.Address = activeCustomer.Address;

                        itemWarranty.activeDate = productWarranty.ActiveDate;
                        itemWarranty.expireDate = productWarranty.ExpireDate;
                        itemWarranty.activeCustomerInfo = customer;
                    }
                    return itemWarranty;
                }
            }
            return null;
        }




        // kích hoạt bảo hành app daiichi
        [HttpPost]
        public JsonResultModel activeWarranty(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                AddPointIntPutModel data = JsonConvert.DeserializeObject<AddPointIntPutModel>(input.ToString());
                if (String.IsNullOrEmpty(data.code) || data.code.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

                //int itemID = getItemIDByCode(data.code);
                //if (itemID == SystemParam.WARRANTY_CODE_USED)
                //    return response(SystemParam.ERROR, SystemParam.WARRANTY_CODE_USED, SystemParam.MESSAGE_WARRANTY_CODE_USED, "");
                //if (itemID == SystemParam.WARRANTY_CODE_NOT_EXIST)
                //    return response(SystemParam.ERROR, SystemParam.WARRANTY_CODE_NOT_EXIST, SystemParam.MESSAGE_WARRANTY_CODE_NOT_EXIST, "");

                if (pBus.CreateAddPointByWarranty(data, cusID.Value) == -1)
                    return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_SCAN_QR_FAIL, "");

                Customer cus = cnn.Customers.Find(cusID.Value);
                ChangePointOutputModel query = new ChangePointOutputModel();
                query.CurrentPoint = cus.Point;
                query.RankingPoint = cus.PointRanking;

                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_ACTIVE_WARRANTY_SUCCESS, query);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        // lấy danh sách bảo hành
        [HttpGet]
        public JsonResultModel getListWarranty(int page, int limit, string text)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                string customerName = cnn.Customers.Find(cusID.Value).Name;
                List<ListWarrantyModel> data = new List<ListWarrantyModel>();
                var listWarranty = from p in cnn.Products
                                   where p.IsActive.Equals(SystemParam.ACTIVE) && p.CustomerActiveID.Value.Equals(cusID.Value)
                                   orderby p.ActiveDate descending
                                   select new ListWarrantyModel
                                   {
                                       id = p.Batch.ItemID.Value,
                                       customerName = customerName,
                                       warrantyCode = p.ProductCode,
                                       //product = (from i in cnn.Items
                                       //           where i.IsActive.Equals(SystemParam.ACTIVE) && i.ID.Equals(p.Batch.ItemID.Value)
                                       //           select new ListItemModel
                                       //           {
                                       //               ID = i.ID,
                                       //               Code = i.Code,
                                       //               Name = i.Name,
                                       //               Description = i.Description,
                                       //               Warranty = i.Warranty,
                                       //               Price = i.Price,
                                       //               Technical = i.Technical,
                                       //               Image = i.ImageUrl
                                       //           }).FirstOrDefault(),
                                       activeDate = p.ActiveDate.Value,
                                       expireDate = p.ExpireDate.Value
                                   };
                if (listWarranty != null && listWarranty.Count() > 0)
                {
                    data = listWarranty.ToList();
                    foreach (var warranty in data)
                    {
                        Item item = cnn.Items.Find(warranty.id);
                        //var parentName = cnn.GroupItems.Where(u => u.IsActive == SystemParam.ACTIVE && u.ParentID == null && u.ID == item.GroupItem.ParentID);
                        warranty.Code = item.Code;
                        warranty.Name = item.Name;
                        warranty.Description = item.Description;
                        warranty.Warranty = item.Warranty;
                        warranty.Price = item.Price;
                        warranty.Technical = item.Technical;
                        warranty.Image = item.ImageUrl.Split(',').ToList();
                        warranty.categoryName = item.GroupItem.Name;
                        //warranty.parentCategoryName = parentName != null ? parentName.FirstOrDefault().Name : "";
                    }

                    if (!String.IsNullOrEmpty(text))
                    {
                        string[] str = Util.Converts(text.ToLower()).Split(' ');
                        foreach (string key in str)
                        {
                            data = data.Where(u => Util.Converts(u.Name.ToLower()).Contains(key) || Util.Converts(u.Code.ToLower()).Contains(key) || Util.Converts(u.categoryName.ToLower()).Contains(key) || Util.Converts(u.warrantyCode.ToLower()).Contains(key)).ToList();
                        }
                    }
                }
                int count = data.Count();
                double totalPage = (double)count / (double)limit;
                ListWarrantyAppModel listWarrantyApp = new ListWarrantyAppModel();
                listWarrantyApp.limit = limit;
                listWarrantyApp.totalCount = count;
                listWarrantyApp.page = page;
                listWarrantyApp.totalPage = (int)Math.Ceiling(totalPage);
                listWarrantyApp.listWarranty = data.ToPagedList(page, limit);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, listWarrantyApp);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        //Check đổi mật khẩu
        //[HttpPost]
        //public JsonResultModel CheckChangePass([FromBody] LoginAppInputModel dt)
        //{
        //    string token = getTokenApp();
        //    if (token.Length == 0)
        //        return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
        //    int? cusID = checkTokenApp(token);
        //    if (cusID == null)
        //        return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

        //    var cus = cnn.Customers.Where(c => c.ID.Equals(cusID.Value)).FirstOrDefault();
        //    if(Util.CheckPass(dt.Password,cus.Password))
        //        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        //    return response(SystemParam.ERROR, SystemParam.ERROR_WRONG_PASSWORD, SystemParam.MESSAGE_ERROR_WRONG_PASSWORD, "");
        //}


        // quên mật khẩu
        [HttpPost]
        public JsonResultModel forgotPassword([FromBody] LoginAppInputModel item)
        {
            if (String.IsNullOrEmpty(item.Email))
                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

            if (!Util.ValidateEmail(item.Email))
                return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");

            var result = lgBus.CheckPhoneForgotPassword(item.Email);
            if (result != SystemParam.SUCCESS)
                return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR_EMAIL_NOT_FOUND, "");
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }

        //Cập nhật lại mật khẩu
        public JsonResultModel UpdatePassword(LoginAppInputModel dt)
        {
            if (String.IsNullOrEmpty(dt.Email) || String.IsNullOrEmpty(dt.Password))
                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");

            if (!Util.ValidateEmail(dt.Email))
                return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");

            var result = lgBus.UpdatePassword(dt.Email, dt.Password);
            if (result != SystemParam.SUCCESS)
                return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.MESSAGE_ERROR, "");
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }
        //public JsonResultModel forgotPassword(JObject input)
        //{
        //    try
        //    {
        //        UserInforOutputModel item = JsonConvert.DeserializeObject<UserInforOutputModel>(input.ToString());
        //        if (!item.Email.Contains('@') || item.Email.Length < 6)
        //            return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");
        //        Customer cus = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(SystemParam.TYPE_LOGIN_ACCOUNT) && u.Email.Equals(item.Email)).FirstOrDefault();
        //        if (cus == null)
        //            return response(SystemParam.ERROR, SystemParam.ERROR_NOT_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_NOT_EXIST_EMAIL, "");
        //        EmailBusiness email = new EmailBusiness();
        //        email.configClient(item.Email, "[THAY ĐỔI MẬT KHẨU]", "Mật khẩu ứng dụng DAIICHI của bạn đã được thay đổi. Mật khẩu mới là: " + SystemParam.PASS_DEFAULT);
        //        cus.DeviceID = "";
        //        cus.Password = Util.GenPass(SystemParam.PASS_DEFAULT);
        //        cus.Token = "";
        //        cnn.SaveChanges();
        //        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_NEW_PASSWORD_SENT, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        return serverError();
        //    }
        //}


        // chi tiết bài viết
        [HttpGet]
        public JsonResultModel getNewsDetail(int newsID)
        {
            try
            {
                NewsDetailModel news = new NewsDetailModel();
                var query = newsBus.getNewsDetailApp(newsID);
                if (query != null && query.NewsID > 0)
                {
                    List<NewsAppOutputModel> data = new List<NewsAppOutputModel>();
                    var newsRelative = from n in cnn.News
                                       where n.Type.Equals(query.Type)
                                       && n.IsActive.Equals(SystemParam.ACTIVE)
                                       && n.Status.Equals(SystemParam.ACTIVE)
                                       && n.ID != newsID
                                       orderby n.ID descending
                                       select new NewsAppOutputModel
                                       {
                                           NewsID = n.ID,
                                           Content = n.Content,
                                           CreateDate = n.CreateDate,
                                           Title = n.Title,
                                           Type = n.Type,
                                           UrlImage = n.UrlImage,
                                           Description = n.Description
                                       };
                    if (newsRelative != null && newsRelative.Count() > 0)
                    {
                        data = newsRelative.Take(3).ToList();
                    }
                    news.newsDetail = query;
                    news.newsRelative = data;
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, news);
                }
                return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR_NOT_EXIST_NEWS, "");
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }


        public static int? checkTokenApp(string tocken)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            Customer customer = cnn.Customers.Where(u => u.Token.Equals(tocken) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (customer != null)
            {
                return customer.ID;
            }
            else
                return null;
        }

        //Chi tiết request
        [HttpGet]
        public JsonResultModel RequestDetail(int ID)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, GetRequestDetail(ID));

        }

        //Lấy chi tiết request

        public RequestPointModel GetRequestDetail(int ID)
        {
            var Data = cnn.Requests.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID == ID).Select(r => new RequestPointModel
            {
                ID = r.ID,
                CustomerID = r.CustomerID,
                Status = r.Status,
                CreatedDate = r.CreateDate,
                Balance = r.Customer.Point,
                //Số điểm đã rút
                Point = r.Point,
                TotalMoney = r.Point * 1000,
                BankInfo = cnn.CustomerBanks.Where(b => b.ID == r.BankID).Select(b => new BankInfo
                {
                    BankID = b.ID,
                    BankName = b.Bank.BankName,
                    UrlImg = b.Bank.ImageUrl,
                    ShortName = b.Bank.ShortName,
                    BankOwner = b.BankOwner
                }).FirstOrDefault()
            }).FirstOrDefault();
            if (Data != null)
                return Data;
            return new RequestPointModel();
        }

        //Lấy danh sách khách hàng
        public JsonResultModel GetListMember(string SearchKey = "")
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            try
            {
                var query = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Status == SystemParam.ACTIVE).Select(c => new ListMemberOutputModel()
                {
                    ID = c.ID,
                    Name = c.Name,
                    Phone = c.Phone,
                    NameAndPhone = c.Name + "-" + c.Phone
                }).OrderByDescending(c => c.ID).ToList();
                if (!String.IsNullOrEmpty(SearchKey))
                    query = query.Where(c => Util.Converts(c.Name.ToLower()).Contains(Util.Converts(SearchKey.ToLower())) || c.Phone.Contains(SearchKey)).ToList();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch
            {
                return serverError();
            }

        }
        public JsonResultModel MovePoint([FromBody] PointInputModel dt)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

            if (dt.point < 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_CONDITION_POINT, SystemParam.MESSAGE_ERROR_CONDITION_MOVE_POINT, "");
            var userSend = cnn.Customers.Where(c => c.ID == cusID).FirstOrDefault();
            if (userSend.Point <= 0 || userSend.Point < dt.point)
                return response(SystemParam.ERROR, SystemParam.KHONG_DU_DIEM_DE_TANG, SystemParam.MESSAGE_KHONG_DU_DIEM_DE_TANG, "");
            var userTake = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Phone == dt.phone && c.Status == SystemParam.ACTIVE && c.Phone != userSend.Phone).FirstOrDefault();
            if (userSend.Status != SystemParam.ACTIVE)
                return response(SystemParam.ERROR, SystemParam.ERROR_ACOUNT_DEACTIVE, SystemParam.MESSAGE_ERROR_CONDITION_STATUS_ACCOUNT_DEACTIVE, "");
            if (userTake == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_USER_INFO, SystemParam.MESSAGE_ERROR_USER_NOT_FOUND, "");
            var conect = cnn.Database.BeginTransaction();
            try
            {
                var code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                var list = new List<MembersPointHistory>();
                var balanceUserTake = userTake.Point + dt.point;
                var balanceUserSend = userSend.Point - dt.point;
                //Lưu lại lịch sử chuyển điểm của người gửi
                MembersPointHistory m = new MembersPointHistory();
                m.CustomerID = cusID.Value;
                m.Point = dt.point.Value;
                m.Type = SystemParam.TYPE_REQUEST_GIFT_POINT;
                m.AddPointCode = code;
                m.TypeAdd = SystemParam.TYPE_POINT;
                m.CraeteDate = DateTime.Now;
                m.IsActive = SystemParam.ACTIVE;
                m.Comment = dt.note;
                m.Title = "Bạn vừa chuyển " + dt.point + " đến số điện thoại " + dt.phone;
                m.Balance = balanceUserSend.Value;
                m.ProductID = null;
                m.UserSendID = userTake.ID;
                list.Add(m);
                //Lưu lại lịch sử cộng điểm của người nhận điểm

                MembersPointHistory mb = new MembersPointHistory();
                mb.CustomerID = userTake.ID;
                mb.Point = dt.point.Value;
                mb.Type = SystemParam.TYPE_AWARDED_POINT;
                mb.AddPointCode = code;
                mb.TypeAdd = SystemParam.TYPE_POINT;
                mb.CraeteDate = DateTime.Now;
                mb.IsActive = SystemParam.ACTIVE;
                mb.Comment = dt.note;
                mb.Title = "Bạn vừa nhận được " + dt.point + " từ số điện thoại " + userSend.Phone;
                mb.Balance = balanceUserTake.Value;
                mb.ProductID = null;
                mb.UserSendID = cusID.Value;
                list.Add(mb);
                //Trừ điểm người gửi
                userSend.Point = balanceUserSend.Value;
                //Cộng điểm người nhận
                userTake.Point = balanceUserTake.Value;

                //Tạo thông báo cho người nhận 
                List<Notification> listNt = new List<Notification>();
                Notification nt = new Notification();
                nt.CustomerID = userTake.ID;
                nt.Content = "Bạn vừa nhận được " + dt.point + " từ số điện thoại " + userSend.Phone;
                nt.Viewed = 0;
                nt.CreateDate = DateTime.Now;
                nt.IsActive = SystemParam.ACTIVE;
                nt.Title = "Bạn vừa nhận được " + dt.point + " từ số điện thoại " + userSend.Phone;
                nt.Type = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                nt.NewsID = null;
                listNt.Add(nt);
                //Tạo thông báo cho người gửi
                Notification ntf = new Notification();
                ntf.CustomerID = userSend.ID;
                ntf.Content = "Bạn vừa chuyển " + dt.point + " sang số điện thoại " + userTake.Phone;
                ntf.Viewed = 0;
                ntf.CreateDate = DateTime.Now;
                ntf.IsActive = SystemParam.ACTIVE;
                ntf.Title = "Bạn vừa chuyển " + dt.point + " sang số điện thoại " + userTake.Phone;
                ntf.Type = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                ntf.NewsID = null;
                listNt.Add(ntf);

                cnn.Notifications.AddRange(listNt);
                cnn.MembersPointHistories.AddRange(list);
                cnn.SaveChanges();
                conect.Commit();
                conect.Dispose();
                //Tiến hành gửi thông báo
                if (userTake.DeviceID != null && userTake.DeviceID.Length > 15)
                {
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.Point = balanceUserTake;
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                    string titleNoti = "Bạn vừa nhận được " + dt.point + " từ số điện thoại " + userSend.Phone;
                    List<string> listDevice = new List<string>();
                    listDevice.Add(userTake.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, dt.note);
                    packageBusiness.PushOneSignals(value);
                }
                if (userSend.DeviceID != null && userSend.DeviceID.Length > 15)
                {
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.Point = balanceUserSend;
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                    string titleNoti = "Bạn vừa chuyển " + dt.point + " sang số điện thoại " + userTake.Phone;
                    List<string> listDevice = new List<string>();
                    listDevice.Add(userSend.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống trừ điểm khi chuyển điểm thành công");
                    packageBusiness.PushOneSignals(value);
                }
                var ID = cnn.MembersPointHistories.Where(u => u.CustomerID == cusID).OrderByDescending(u => u.ID).FirstOrDefault().ID;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, GetDetailHistoryPointMember(ID));
            }
            catch (Exception e)
            {
                e.ToString();
                conect.Rollback();
                conect.Dispose();
                return serverError();
            }
        }

        //Lấy chi tết lịch sử về cộng điểm hoặc rút điểm,tặng điểm
        public HistoryPointMemberModel GetDetailHistoryPointMember(int ID)
        {
            var data = cnn.MembersPointHistories.Where(m => m.ID == ID).Select(m => new HistoryPointMemberModel
            {
                HistoryID = m.ID,
                CreateDate = m.CraeteDate,
                UserName = m.Type == SystemParam.TYPE_REQUEST_GIFT_POINT ? cnn.Customers.Where(c => c.ID == m.CustomerID).FirstOrDefault().Name : "",
                UserPhone = m.Type == SystemParam.TYPE_REQUEST_GIFT_POINT ? cnn.Customers.Where(c => c.ID == m.CustomerID).FirstOrDefault().Phone : "",
                CusID = m.CustomerID.Value,
                Point = m.Point,
                Comment = m.Comment,
                Title = m.Title,
                Balance = m.Balance

            }).FirstOrDefault();
            if (data != null)
                return data;
            return new HistoryPointMemberModel();
        }

        //Lấy danh sách tất cả các ngân hàng có trên hệ thống
        public JsonResultModel GetListBank()
        {

            List<BankOutputModel> data = new List<BankOutputModel>();
            var query = cnn.Banks.Select(b => new BankOutputModel
            {
                BankName = b.BankName,
                ShortName = b.ShortName,
                ImageUrl = b.ImageUrl,
                ID = b.ID
            }).ToList();
            if (query.Count() > 0 && query != null)
                data = query;
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
        }

        //Thêm tài khoản ngân hàng 

        public JsonResultModel AddBankAccount([FromBody] BankOutputModel data)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            if (data.CodeBankAccount.Length == 0 || data.UserName.Length == 0)
                return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
            var check = cnn.CustomerBanks.Where(c => c.IsActive == SystemParam.ACTIVE && c.CustomerID == cusID.Value && c.BankID == data.ID).FirstOrDefault();
            if (check != null)
                return response(SystemParam.ERROR, SystemParam.EXIST_ACCOUNT, SystemParam.MESSAGE_ERROR_DUPLICATE_BANK_ACOUNT, "");

            try
            {
                CustomerBank c = new CustomerBank();
                c.CustomerID = cusID.Value;
                c.BankID = data.ID;
                c.ActiveDate = DateTime.Now;
                c.BankAccount = data.UserName;
                c.BankOwner = data.CodeBankAccount;
                c.IsActive = SystemParam.ACTIVE;
                c.CreatedDate = DateTime.Now;
                cnn.CustomerBanks.Add(c);
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, GetBankOfCus(cusID.Value));

            }
            catch (Exception e)
            {
                e.ToString();
                return serverError();
            }
        }

        //Lấy danh sách thẻ ngân hàng của khách hàng
        public List<BankOutputModel> GetBankOfCus(int ID)
        {
            List<BankOutputModel> data = new List<BankOutputModel>();
            var query = cnn.CustomerBanks.Where(c => c.CustomerID == ID && c.IsActive == SystemParam.ACTIVE).OrderByDescending(c => c.ID)
                .Select(c => new BankOutputModel
                {
                    ID = c.ID,
                    BankName = c.Bank.BankName,
                    ShortName = c.Bank.ShortName,
                    CodeBankAccount = c.BankOwner,
                    ImageUrl = c.Bank.ImageUrl
                }).ToList();
            if (query != null && query.Count() > 0)
                return data = query;
            return data;
        }

        //Xóa tài khoản ngân hàng
        [HttpGet]
        public JsonResultModel DelBankAccount(int ID)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            try
            {
                var bank = cnn.CustomerBanks.Find(ID);
                bank.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, GetBankOfCus(cusID.Value));

            }
            catch (Exception e)
            {
                e.ToString();
                return serverError();
            }
        }

        [HttpGet]
        public JsonResultModel GetListRequest(int? status)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            try
            {
                var data = new List<RequestPointModel>();
                var query = cnn.Requests.Where(r => r.IsActive == SystemParam.ACTIVE && (status.HasValue ? r.Status == status.Value : true) && r.CustomerID == cusID.Value)
                    .Select(r => new RequestPointModel
                    {
                        ID = r.ID,
                        CustomerID = r.CustomerID,
                        Status = r.Status,
                        CreatedDate = r.CreateDate,
                        //Số điểm đã rút
                        Point = r.Point,
                        TotalMoney = r.Point * 1000,
                        Balance = r.Customer.Point,
                        BankInfo = cnn.CustomerBanks.Where(b => b.ID == r.BankID).Select(b => new BankInfo
                        {
                            BankID = b.ID,
                            BankName = b.Bank.BankName,
                            UrlImg = b.Bank.ImageUrl,
                            ShortName = b.Bank.ShortName,
                            BankOwner = b.BankOwner
                        }).FirstOrDefault()
                    }).ToList();
                if (query != null && query.Count() > 0)
                    data = query;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch
            {
                return serverError();
            }
        }

        //Lấy danh sách tài khoản ngân hàng của khách hàng
        public JsonResultModel GetListBankOfCus()
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            try
            {
                List<BankOutputModel> data = new List<BankOutputModel>();
                var query = cnn.CustomerBanks.Where(c => c.CustomerID == cusID.Value && c.IsActive == SystemParam.ACTIVE).OrderByDescending(c => c.ID)
                    .Select(c => new BankOutputModel
                    {
                        ID = c.ID,
                        BankName = c.Bank.BankName,
                        ShortName = c.Bank.ShortName,
                        CodeBankAccount = c.BankOwner,
                        ImageUrl = c.Bank.ImageUrl,
                        UserName = c.BankAccount
                    }).ToList();
                if (query.Count() > 0 && query != null)
                    data = query;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch
            {
                return serverError();
            }
        }

        //Chi tiết lịch sử

        [HttpGet]
        public JsonResultModel GetHistyoriesDetail(int ID)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            RequestPointModel data = new RequestPointModel();
            var cus = cnn.Customers;
            var query = cnn.MembersPointHistories.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID.Equals(ID)).Select(r => new RequestPointModel
            {
                ID = r.ID,
                CustomerID = r.CustomerID,
                CreatedDate = r.CraeteDate,
                Balance = r.Customer.Point,
                Status = r.Status,
                BankID = r.BankID,
                Comment = r.Comment,
                Title = r.Title,
                //Số điểm đã rút
                Point = r.Point,
                TotalMoney = r.Point * 1000,
                UserName = r.UserSendID.HasValue ? cus.Where(c => c.ID.Equals(r.UserSendID.Value)).FirstOrDefault().Name : "",
                UserPhone = r.UserSendID.HasValue ? cus.Where(c => c.ID.Equals(r.UserSendID.Value)).FirstOrDefault().Phone : ""
            }).FirstOrDefault();

            query.BankInfo = query.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID.Equals(query.BankID.Value)).Select(b => new BankInfo
            {
                BankID = b.ID,
                BankName = b.Bank.BankName,
                ShortName = b.Bank.ShortName,
                UrlImg = b.Bank.ImageUrl,
                BankOwner = b.BankOwner
            }).FirstOrDefault() : null;
            if (query != null)
                data = query;
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
        }

        //Check version app
        [HttpGet]
        public object CreateOrUpdate(string versionApp, int typeOS, string codePushVersion, int forceUpdate)
        {
            var appVersion = cnn.AppVersions.Where(u => u.VersionApp.Equals(versionApp) && u.TypeOS.Value.Equals(typeOS)).FirstOrDefault();
            if (appVersion != null)
            {
                appVersion.CodePushVersion = codePushVersion;
                appVersion.Is_Force_Update = forceUpdate;
            }
            else
            {
                appVersion = new AppVersion
                {
                    VersionApp = versionApp,
                    CodePushVersion = codePushVersion,
                    Is_Force_Update = forceUpdate,
                    TypeOS = typeOS
                };
                cnn.AppVersions.Add(appVersion);
            }
            cnn.SaveChanges();
            return getAppVersion(typeOS);
        }
        [HttpGet]
        public object getAppVersion(int typeOS)
        {
            var lsappVersion = cnn.AppVersions.Where(u => u.TypeOS.Value.Equals(typeOS)).Select(u => new
            {
                versionApp = u.VersionApp,
                codePushVersion = u.CodePushVersion,
                forceUpdate = u.Is_Force_Update,
                typeOS = u.TypeOS
            }).ToList();
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lsappVersion);
        }

        //cập nhật trạng thái viewed của noti
        [HttpGet]
        public JsonResultModel UpdateNoti(int ID)
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            try
            {
                var nt = cnn.Notifications.Find(ID);
                nt.Viewed = SystemParam.RETURN_TRUE;
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch
            {
                return serverError();
            }

        }

        //Lấy danh sách những người đã nhập mã giới thiệu của mình

        public JsonResultModel GetListLastRefCode()
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            var cus = cnn.Customers.Where(c => c.ID.Equals(cusID.Value)).FirstOrDefault();
            try
            {
                List<ListMemberOutputModel> data = new List<ListMemberOutputModel>();

                data = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.LastRefCode == cus.Phone).Select(c => new ListMemberOutputModel()
                {
                    ID = c.ID,
                    Name = c.Name,
                    Phone = c.Phone,
                    NameAndPhone = c.Name + "-" + c.Phone
                }).OrderByDescending(c => c.ID).ToList();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch
            {
                return serverError();
            }

        }

        //Import khách hàng(Tích Điểm TD)
        //[HttpGet]
        //public string ImportCus()
        //{
        //    string path = HttpContext.Current.Server.MapPath("~/Import/dskh.xlsx");
        //    FileInfo file = new FileInfo(path);
        //    ExcelPackage pack = new ExcelPackage(file);
        //    ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
        //    List<Customer> list = new List<Customer>();
        //    int row = 6;
        //    object data = 0;
        //    try
        //    {
        //        while (data != null)
        //        {
        //            Customer c = new Customer();
        //            data = sheet.Cells[row, 2].Value;
        //            if (data == null)
        //                break;
        //            c.Code = "";
        //            c.Name = sheet.Cells[row, 2].Value.ToString();
        //            c.Phone = sheet.Cells[row, 3].Value.ToString();
        //            c.Address = sheet.Cells[row, 4].Value == null ? "" : sheet.Cells[row, 4].Value.ToString();

        //            DateTime date = (DateTime)sheet.Cells[row, 5].Value;
        //            c.CraeteDate = date;
        //            var point = sheet.Cells[row, 7].Value == null ? 0 : sheet.Cells[row, 7].Value;
        //            var pointRanking = sheet.Cells[row, 6].Value == null ? 0 : sheet.Cells[row, 6].Value;
        //            c.Point = Math.Round(Convert.ToDouble(point), 2);
        //            c.PointRanking = Math.Round(Convert.ToDouble(pointRanking), 2);
        //            c.LastRefCode = sheet.Cells[row, 8].Value == null ? "" : sheet.Cells[row, 8].Value.ToString();
        //            c.Email = "";
        //            c.ExpireTocken = DateTime.Now;
        //            c.Token = "";
        //            c.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
        //            c.DistrictCode = SystemParam.DISTRICT_DEFAULT;
        //            c.DOB = DateTime.Now;
        //            c.Sex = SystemParam.GENDER_MEN;
        //            c.Role = SystemParam.ROLE_CUSTOMER;
        //            c.AvatarUrl = "https://st.quantrimang.com/photos/image/072015/22/avatar.jpg";
        //            c.LastLoginDate = DateTime.Now;
        //            c.DeviceID = "";
        //            c.Type = 10;
        //            c.Status = SystemParam.ACTIVE_FALSE;
        //            c.Password = Util.GenPass("123456");
        //            c.IsActive = SystemParam.ACTIVE;
        //            list.Add(c);
        //            System.Diagnostics.Debug.WriteLine(row);
        //            row++;
        //        }
        //        if (list.Count() == 0)
        //            return "Không có bản ghi nào";
        //        cnn.Customers.AddRange(list);
        //        cnn.SaveChanges();
        //        return "Thành công";
        //    }
        //    catch (Exception e)
        //    {
        //        return "Lỗi dòng thứ " + row + " với nội dung :" + e.ToString();
        //    }
        //}

        //Cập nhật deviceID
        [HttpPost]
        public JsonResultModel UpdateDeviceID([FromBody] LoginAppInputModel dt)
        {
            try
            {
                string token = getTokenApp();
                int? cusID = checkTokenApp(token);
                Customer cus = cnn.Customers.Find(cusID.Value);
                cus.DeviceID = dt.DeviceID;
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch
            {
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
        }

        [HttpGet]
        public JsonResultModel Testt()
        {
            try
            {
                taskBus.MinusPointEveryMonth();
                //NotifyDataModel notifyData = new NotifyDataModel();
                //notifyData.type = SystemParam.ONESIGNAL_NOTIFY_POINT_HISTORY;
                //notifyData.Point = 1;
                //notifyData.PointRaking = 1;
                //string titleNoti = "Cộng điểm chuyển đổi hằng ngày";
                //List<string> listDevice = new List<string>();
                //listDevice.Add("aa1dceab-78f1-4d9b-9578-75a33017fcc0");
                //string value = packageBusiness.StartPushNoti(notifyData, listDevice, titleNoti, "Hệ thống chuyển đổi 0,2 % điểm tích lũy sang điểm ví point hàng ngày");
                //packageBusiness.PushOneSignals(value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch
            {
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
        }
    }
}