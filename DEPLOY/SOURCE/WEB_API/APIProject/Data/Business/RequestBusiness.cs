using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class RequestBusiness : GenericBusiness
    {
        public RequestBusiness(TichDiemTrieuDo context = null) : base()
        {
        }
        PointBusiness pBus = new PointBusiness();
        NotifyBusiness notiBus = new NotifyBusiness();
        //LoginBusiness loginBusiness = new LoginBusiness();
        public int CreateRequest(Gift gift, int cusID)
        {
            if (gift.ID <= 0)
                return SystemParam.GIFT_REQUEST_NOT_FOUND;
            Customer cus = cnn.Customers.Find(cusID);
            if (cus.Point < gift.Point)
                return SystemParam.DIEM_DOI_QUA_LON_HON_DIEM_MINH;
            if ((cus.Point - gift.Point) < Util.minPointAccount())
                return SystemParam.KHONG_DU_DIEM_DOI_QUA;
            if ((pBus.getTotalPointPerDay(cusID) + gift.Point) > Util.maxPointPerDay())
                return SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY;

            Request rq = new Request();
            string code = Util.RandomString(SystemParam.SIZE_CODE, false);
            rq.GiftID = gift.ID;
            rq.Point = gift.Point;
            rq.Type = gift.Type;
            rq.CreateDate = DateTime.Now;
            rq.IsActive = SystemParam.ACTIVE;
            rq.CustomerID = cusID;
            rq.Code = code;
            rq.Status = SystemParam.STATUS_REQUEST_WAITING;
            cnn.Requests.Add(rq);
            // sửa lại điểm của khách hàng
            cus.Point -= gift.Point;
            cnn.SaveChanges();
            pBus.CreateHistoryes(cusID, gift.Point, SystemParam.TYPE_POINT_RECEIVE_GIFT, SystemParam.HISTORY_TYPE_ADD_ANOTHER, code, "Yêu cầu đổi quà '" + gift.Name + "'", 0);
            EmailBusiness email = new EmailBusiness();
            email.configClient(SystemParam.EMAIL_CONFIG, "[TÍCH ĐIỂM V99]", "Tài khoản " + cus.Name + " đã yêu cầu đổi quà " + gift.Name);
            return SystemParam.SUCCESS;
        }
        //từ chối yêu cầu rút điểm
        public int UpdateRequest(int Id, int Select,string Note)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {
                PointBusiness pointBus = new PointBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();

                var req = cnn.Requests.Find(Id);
                var statusCurrent = req.Status;
                req.Status = Select;
                req.Note = Note;

                var customer = cnn.Customers.Find(req.CustomerID);
                double point = customer.Point += req.Point.Value;
                customer.Point = point;
                
                string titleNotify = "";
                string contentNoti = "";
                int typeNotify = 0;

                if (Select == SystemParam.STATUS_ORDER_REFUSE)
                {
                    titleNotify = "đã bị hủy";
                    typeNotify = SystemParam.NOTIFY_NAVIGATE_REQUEST;
                    contentNoti = "Yêu cầu rút điểm của bạn " + titleNotify + " và bạn được hoàn trả " + req.Point + " điểm "+ req.Note;
                    
                    var history = cnn.MembersPointHistories.Where(c => c.CustomerID == req.CustomerID && c.AddPointCode==req.Code && c.IsActive == SystemParam.ACTIVE).FirstOrDefault();
                    history.Point = req.Point.Value;
                    history.Type = SystemParam.TYPE_REQUEST_DRAW_POINT;
                    history.TypeAdd = SystemParam.TYPE_POINT;
                    history.Comment = contentNoti;
                    history.Status = SystemParam.STATUS_ORDER_REFUSE;
                    notifyBusiness.CreateNoti(req.CustomerID, SystemParam.TYPE_ADD_POINT, contentNoti, contentNoti, history.ID);
                    var nt = cnn.Notifications.Where(n => n.CustomerID == req.CustomerID).OrderByDescending(n => n.ID).FirstOrDefault();
                    if (customer.DeviceID != null && customer.DeviceID.Length > 0)
                    {
                       NotifyDataModel notifyData = new NotifyDataModel();
                        notifyData.id = history.ID;
                        notifyData.type = SystemParam.ONESIGNAL_NOTIFY_REQUEST_DETAIL;
                        notifyData.code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                        notifyData.Point = (long)point;
                        List<string> listDevice = new List<string>();
                        listDevice.Add(customer.DeviceID);
                        string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                       packageBusiness.PushOneSignals(value);
                    }
                }
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
               

            }
            catch(Exception ex)
            {
                
                return SystemParam.RETURN_FALSE;
            }
        }
        //xác nhận yêu cầu rút điểm
        public int UpdateConfirm(int Id,int Select)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {
                PointBusiness pointBus = new PointBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();

                var req = cnn.Requests.Find(Id);
                var statusCurrent = req.Status;

                var customer = cnn.Customers.Find(req.CustomerID);
                
                req.Status = Select;

                //Chỗ này chỉ để test có push được thông báo đi hay không
                string titleNotify = "";
                string contentNoti = "";
                titleNotify = "đã được xác nhận";
                contentNoti = "Yêu cầu rút " + req.Point + " điểm của bạn " + titleNotify;
               
                var history = cnn.MembersPointHistories.Where(c => c.CustomerID == req.CustomerID  && c.AddPointCode == req.Code && c.IsActive == SystemParam.ACTIVE).FirstOrDefault();
                history.Status = SystemParam.STATUS_ORDER_CONFIRM;
                history.Point = req.Point.Value;
                history.Type = SystemParam.TYPE_REQUEST_DRAW_POINT;
                history.TypeAdd = SystemParam.TYPE_POINT;
                history.Comment = "Yêu cầu rút " + req.Point + " điểm của bạn " + titleNotify;
                notifyBusiness.CreateNoti(req.CustomerID, SystemParam.NOTIFY_NAVIGATE_REQUEST, contentNoti, contentNoti, history.ID);
                var nt = cnn.Notifications.Where(n => n.CustomerID== req.CustomerID).OrderByDescending(n => n.ID).FirstOrDefault();
                if (customer.DeviceID != null && customer.DeviceID.Length > 0)
                {
                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.id = history.ID;
                    notifyData.type = SystemParam.ONESIGNAL_NOTIFY_REQUEST_DETAIL;
                    notifyData.code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                    List<string> listDevice = new List<string>();
                    listDevice.Add(customer.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                   // string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                    packageBusiness.PushOneSignals(value);
                }

                //if (statusCurrent != Select && Select != SystemParam.STATUS_ORDER_CONFIRM)
                //{
                //    titleNotify = "đã được xác nhận";
                //    typeNotify = SystemParam.NOTIFY_TYPE_ORDER_CONFIRM;
                //    contentNoti = "Yêu cầu rút "+req.Point+ " điểm của bạn " + titleNotify;
                //    notifyBusiness.CreateNoti(req.CustomerID, typeNotify, contentNoti, contentNoti, null);
                //    pointBus.CreateHistoryes(req.CustomerID, req.Point, SystemParam.HISPOINT_DOI_QUA, SystemParam.TYPE_POINT, Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6), "Yêu cầu rút " + req.Point + " điểm của bạn " + titleNotify, 0);
                //    if (customer.DeviceID != null && customer.DeviceID.Length > 0)
                //    {
                //        NotifyDataModel notifyData = new NotifyDataModel();
                //        notifyData.id = 0;
                //        notifyData.type = SystemParam.TYPE_REQUEST_DRAW_POINT;
                //        notifyData.code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                //        List<string> listDevice = new List<string>();
                //        listDevice.Add(customer.DeviceID);
                //        string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                //        string a = packageBusiness.PushOneSignals();
                //    }
                //}
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;

            }
            catch (Exception ex)
            {

                return SystemParam.RETURN_FALSE;
            }
        }
        public CardOutputModel RequestCard(Gift gift, int cusID)
        {
            Customer cus = cnn.Customers.Find(cusID);
            CardOutputModel output = new CardOutputModel();
            if ((cus.Point - gift.Point) < Util.minPointAccount())
            {
                output.price = SystemParam.KHONG_DU_DIEM_DOI_QUA;
                return output;
            }
            if ((pBus.getTotalPointPerDay(cusID) + gift.Point) > Util.maxPointPerDay())
            {
                output.price = SystemParam.DA_DUNG_HET_SO_DIEM_TRONG_NGAY;
                return output;
            }
            output.name = gift.Name;
            output.price = gift.Price;
            var lscard = cnn.Cards.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TelecomType.Equals(gift.TelecomType.Value) && u.CardType.Equals(gift.Price) && u.Status.Equals(SystemParam.NO_ACTIVE));
            if (lscard != null && lscard.Count() > 0)
            {
                List<Card> lsc = lscard.ToList();
                cus.Point -= gift.Point;
                Card card = lscard.FirstOrDefault();
                output.seri = card.Seri;
                output.code = Util.EnCode(card.Code.ToString());
                string content = output.name + "/" + output.price + "/" + output.seri + "/" + output.code;
                card.Status = SystemParam.ACTIVE;
                card.CustomerActiveID = cusID;
                card.ActiveDate = DateTime.Now;
                cnn.SaveChanges();
                pBus.CreateHistoryes(cusID, gift.Point, SystemParam.TYPE_CARD, SystemParam.HISTORY_TYPE_ADD_ANOTHER, content, "", 0);
                return output;
            }
            else
                return output;
        }


        // tìm kiếm
        public List<RequestDetailWebOutputModel> Search(int? Status, string Name, string Phone, string FromDate, string ToDate)
        {
            try
            {
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);

                var query = from r in cnn.Requests
                            orderby r.ID descending
                            select new RequestDetailWebOutputModel
                            {
                                ID = r.ID,
                                Point = (int)r.Customer.Point,
                                Name = r.Customer.Name,
                                Phone = r.Customer.Phone,                             
                                CreateDate = r.CreateDate
                            };
                int count = query.Count();
                if (endDate.HasValue)
                {
                    query = query.Where(u => u.CreateDate.Value.Day <= endDate.Value.Day && u.CreateDate.Value.Month <= endDate.Value.Month && u.CreateDate.Value.Year <= endDate.Value.Year);
                }
                if (query != null && query.Count() > 0)
                {
                    List<RequestDetailWebOutputModel> list = query.ToList();
                    return list;

                }
                else
                {
                    return new List<RequestDetailWebOutputModel>();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<RequestDetailWebOutputModel>();
            }
        }
        // Lấy thông tin chi tiết 1 yêu cầu đổi quà
        public List<RequestDetailWebOutputModel> GetRequestDetail()
        {
            try
            {
                List<RequestDetailWebOutputModel> requestdetail = new List<RequestDetailWebOutputModel>();

                var query = from r in cnn.Requests
                             join c in cnn.Customers on r.CustomerID equals c.ID
                             join cb in cnn.CustomerBanks on c.ID equals cb.CustomerID
                             join b in cnn.Banks on cb.BankID equals b.ID
                             select new RequestDetailWebOutputModel
                             {
                                 ID = r.ID,
                                 Point = r.Point.Value,
                                 Name = c.Name,
                                 Phone = c.Phone,
                                 BankAccout = cb.BankAccount,
                                 BankOwner = cb.BankOwner,
                                 ImageUrl = b.ImageUrl,
                                 Status = r.Status,
                                 BankName = b.BankName,
                                 CreateDate = r.CreateDate
                             };
                if (query != null)
                {
                    return requestdetail = query.ToList();
                }
                return requestdetail;
            }
            catch
            {
                return new List<RequestDetailWebOutputModel>();
            }
        }
        //thông tin chi tiết yêu cầu rút điểm
        public RequestDetailWebOutputModel GetRequest(int id)
        {
            try
            {
                RequestDetailWebOutputModel requestdetail = new RequestDetailWebOutputModel();

                var query = (from r in cnn.Requests
                             join c in cnn.Customers on r.CustomerID equals c.ID
                             join cb in cnn.CustomerBanks on r.BankID equals cb.ID
                             join b in cnn.Banks on cb.BankID equals b.ID
                             where r.ID.Equals(id)
                             select new RequestDetailWebOutputModel
                             {
                                 ID = r.ID,
                                 Point = r.Point.Value,
                                 Name = c.Name,
                                 Phone = c.Phone,
                                 BankAccout = cb.BankAccount,
                                 BankOwner = cb.BankOwner,
                                 Note = r.Note,
                                 ImageUrl = b.ImageUrl,
                                 Status = r.Status,
                                 BankName = b.BankName,
                                 CreateDate = r.CreateDate
                             });
                return query.FirstOrDefault();
            }
            catch
            {
                return new RequestDetailWebOutputModel();
            }
        }
       

        PackageBusiness packageBusiness = new PackageBusiness();

        public int DeleteRequest(int RequestID)
        {
            try
            {
                Request request = cnn.Requests.Find(RequestID);
                request.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<RequestDetailWebOutputModel> NameSearch(int Page, string Name, string FromDate, string ToDate,int? StatusR)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            List<RequestDetailWebOutputModel> list = new List<RequestDetailWebOutputModel>();
            try
            {
                DateTime? startDate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                var query = (from r in cnn.Requests
                             join c in cnn.Customers on r.CustomerID equals c.ID
                             join cb in cnn.CustomerBanks on r.BankID equals cb.ID
                             join b in cnn.Banks on cb.BankID equals b.ID
                             where (startDate.HasValue ? r.CreateDate >= startDate.Value : true && r.IsActive == SystemParam.ACTIVE)
                             orderby r.CreateDate descending
                             select new RequestDetailWebOutputModel
                             {
                                 ID = r.ID,
                                 Point = r.Point.Value,
                                 Name = c.Name,
                                 Phone = c.Phone,
                                 BankAccout = cb.BankAccount,
                                 BankOwner = cb.BankOwner,
                                 BankName = b.BankName,
                                 ImageUrl = b.ImageUrl,
                                 Status = r.Status,
                                 CreateDate = r.CreateDate
                             }).ToList();
                if (StatusR != null)
                {
                    query = query.Where(r => r.Status.Equals(StatusR)).ToList();
                }
                query = query.Where(r => Util.Converts(r.Name.ToLower()).Contains(Util.Converts(Name.ToLower())) || r.Phone.Contains(Name)).ToList();
                if (endDate.HasValue)
                {
                    // query = query.Where(u => u.CreateDate.Day >= startDate.Value.Day && u.CreateDate.Month >= startDate.Value.Month && u.CreateDate.Year >= startDate.Value.Year).ToList();
                    query = query.Where(r => r.CreateDate.Value.Day <= endDate.Value.Day && r.CreateDate.Value.Month <= endDate.Value.Month && r.CreateDate.Value.Year <= endDate.Value.Year).ToList();
                }
                if(query != null && query.Count() > 0)
                {
                    foreach(var p in query)
                    {
                        list.Add(p);
                    }
                    return list;
                }
                else
                {
                    return new List<RequestDetailWebOutputModel>();
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
                return new List<RequestDetailWebOutputModel>();
            }
        }
        //// thống kê đổi quà
        //public List<ListStaticGiftModel> SearchStatisticGift(string CusName, int? GiftType, string FromDate, string ToDate)
        //{
        //    try
        //    {
        //        List<ListStaticGiftModel> query = new List<ListStaticGiftModel>();
        //        var listRequest = from r in cnn.Requests
        //                          where r.IsActive.Equals(SystemParam.ACTIVE) && r.Status.Equals(SystemParam.STATUS_REQUEST_ACCEPTED)
        //                          orderby r.CreateDate
        //                          select new ListStaticGiftModel
        //                          {
        //                              CusName = r.Customer.Name,
        //                              Type = r.Type,
        //                              GiftName = r.Gift.Name,
        //                              Point = r.Point.Value,
        //                              Price = r.Gift.Price,
        //                              CreateDate = r.CreateDate
        //                          };

        //        //if (CusName != null)
        //        //    listRequest = listRequest.Where(x => x.CusName.Contains(CusName));

        //        if (GiftType != null)
        //        {
        //            listRequest = listRequest.Where(x => x.Type == GiftType);
        //        }
        //        if (FromDate != null && FromDate != "")
        //        {
        //            DateTime? fd = Util.ConvertDate(FromDate);
        //            listRequest = listRequest.Where(x => x.CreateDate >= fd);
        //        }
        //        if (ToDate != null && ToDate != "")
        //        {
        //            DateTime? td = Util.ConvertDate(ToDate);
        //            td = td.Value.AddDays(1);
        //            listRequest = listRequest.Where(x => x.CreateDate <= td);
        //        }
        //        if (listRequest != null && listRequest.Count() > 0)
        //        {
        //            query = listRequest.ToList();
        //            if (!String.IsNullOrEmpty(CusName))
        //                query = query.Where(u => Util.Converts(u.CusName.ToLower()).Contains(Util.Converts(CusName.ToLower()))).ToList();
        //        }
        //        return query;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return new List<ListStaticGiftModel>();
        //    }
        //}
        public double RevenueRefuse()
        {
            try
            {
                double query=(from c in cnn.Requests
                       where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.STATUS_ORDER_REFUSE)
                       select (double?)c.Point).Sum() ?? 0;
                return query;
            }
            catch (Exception)
            {
                return 0;
            } 
        }
        public double RevenuePending()
        {
            try
            {
                double query = (from c in cnn.Requests
                                where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.STATUS_ORDER_PENDING)
                                select (double?)c.Point).Sum() ?? 0;
                return query;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public double RevenueAccepted()
        {
            try
            {
                double query = (from c in cnn.Requests
                                where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.STATUS_ORDER_CONFIRM)
                                select (double?)c.Point).Sum() ?? 0;
                return query;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public string CountRequest()
        {
            var lsRequest = cnn.Requests.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            int totalRequest = lsRequest.Count();
            int requestSuccess = lsRequest.Where(u => u.Status.Equals(SystemParam.STATUS_REQUEST_PENDING)).Count();
            return totalRequest.ToString();
        }


        // xuất excel
        //public ExcelPackage ExportExcel(string fromDate, string toDate, int? status, int? typeRequest, string codeOrCusName)
        //{
        //    try
        //    {
        //        FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/DS_yeu_cau_doi_qua.xlsx"));
        //        ExcelPackage pack = new ExcelPackage(file);
        //        ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
        //        int row = 2;
        //        int stt = 1;

        //       // var list = Search(status, typeRequest, codeOrCusName, fromDate, toDate);
        //        /*if (fromDate != null && fromDate != "")
        //        {
        //            DateTime? fd = Util.ConvertDate(fromDate);
        //            list = list.Where(r => r.CreateDate >= fd.Value);
        //        }

        //        if(toDate != null && toDate != "")
        //        {
        //            DateTime? td = Util.ConvertDate(toDate);
        //            list = list.Where(r => r.CreateDate <= td.Value);
        //        }

        //        if(status != null)
        //        {
        //            list = list.Where(r => r.Status == status);
        //        }

        //        if(typeRequest != null)
        //        {
        //            list = list.Where(r => r.Type == typeRequest);
        //        }

        //        if(codeOrCusName != null && codeOrCusName != "")
        //        {
        //            list = list.Where(r => r.Code.ToLower().Equals(codeOrCusName.ToLower()) || Util.Converts(r.))
        //        }*/

        //        foreach (var item in list)
        //        {
        //            sheet.Cells[row, 1].Value = stt;
        //            sheet.Cells[row, 2].Value = item.RequestCode;
        //            if (item.TypeGift == SystemParam.TYPE_GIFT_GIFT)
        //            {
        //                sheet.Cells[row, 3].Value = "Quà tặng";
        //            }
        //            else if (item.TypeGift == SystemParam.TYPE_GIFT_VOUCHER)
        //            {
        //                sheet.Cells[row, 3].Value = "Voucher";
        //            }
        //            else if (item.TypeGift == SystemParam.TYPE_GIFT_CARD)
        //            {
        //                sheet.Cells[row, 3].Value = "Thẻ cào";
        //            }
        //            sheet.Cells[row, 4].Value = item.CustomerName;
        //            if (item.Status == SystemParam.STATUS_REQUEST_PENDING)
        //            {
        //                sheet.Cells[row, 5].Value = "Chờ xác nhận";
        //            }
        //            else if (item.Status == SystemParam.STATUS_REQUEST_ACCEPTED)
        //            {
        //                sheet.Cells[row, 5].Value = "Đã xác nhận";
        //            }
        //            else if (item.Status == SystemParam.STATUS_REQUEST_CANCEL)
        //            {
        //                sheet.Cells[row, 5].Value = "Hủy";
        //            }

        //            sheet.Cells[row, 6].Value = item.CreateDate.Value.ToString("dd/MM/yyyy");
        //            row++;
        //            stt++;
        //        }
        //        return pack;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return new ExcelPackage();
        //    }
        //}

        //public ExcelPackage singleRequestExport(int id)
        //{
        //    try
        //    {
        //        var itemExport = GetRequestDetail(id);
        //        FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/Yeu_cau_doi_qua.xlsx"));
        //        ExcelPackage pack = new ExcelPackage(file);
        //        ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
        //        sheet.Cells[2, 3].Value = itemExport.CustomerName;
        //        sheet.Cells[2, 7].Value = itemExport.CustomerPhone;
        //        sheet.Cells[3, 3].Value = itemExport.CustomerAddress;
        //        sheet.Cells[3, 7].Value = itemExport.CreateDate.Value.ToString("dd/MM/yyyy");
        //        sheet.Cells[5, 3].Value = itemExport.RequestCode;
        //        if (itemExport.TypeGift == SystemParam.TYPE_REQUEST_GIFT)
        //            sheet.Cells[5, 7].Value = "Quà tặng";
        //        else if (itemExport.TypeGift == SystemParam.TYPE_REQUEST_VOUCHER)
        //            sheet.Cells[5, 7].Value = "Voucher";
        //        sheet.Cells[6, 3].Value = @String.Format("{0:0,0}", itemExport.Point);
        //        sheet.Cells[6, 7].Value = itemExport.GiftName;
        //        switch (itemExport.Status)
        //        {
        //            case SystemParam.STATUS_REQUEST_PENDING:
        //                sheet.Cells[7, 3].Value = "Chờ";
        //                break;
        //            case SystemParam.STATUS_REQUEST_ACCEPTED:
        //                sheet.Cells[7, 3].Value = "Đã xác nhận";
        //                break;
        //            case SystemParam.STATUS_REQUEST_CANCEL:
        //                sheet.Cells[7, 3].Value = "Hủy";
        //                break;
        //        }
        //        sheet.Cells[7, 7].Value = itemExport.Note;
        //        return pack;

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return null;
        //    }
        //}
    }
}
