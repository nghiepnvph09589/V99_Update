using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Animation;
using static Data.Model.APIWeb.OrderDetailEditOutput;
namespace Data.Business
{
    public class OrderBusiness : GenericBusiness
    {
        public OrderBusiness(TichDiemTrieuDo context = null) : base()
        {

        }
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        AgentBusiness agentBusiness = new AgentBusiness();

        MembersPointHistory hisPoint = new MembersPointHistory();
        PointBusiness pointBus = new PointBusiness();
        // tìm kiếm đơn hàng
        public List<Order> Search(int? Status, string FromDate, string ToDate, string Phone)
        {
            try
            {
                var query = cnn.Orders.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.Type.Equals(SystemParam.TYPE_ORDER)).ToList();
                if (Status != null)
                {
                    query = query.Where(x => x.Status.Equals(Status.Value)).ToList();
                }
                if (FromDate != "" && FromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(x => x.CreateDate >= fd).ToList();
                }
                if (ToDate != "" && ToDate != null)
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(x => x.CreateDate <= td).ToList();
                }
                if (Phone != "" && Phone != null)
                {
                    query = query.Where(x => x.BuyerPhone.Contains(Phone) || Util.Converts(x.BuyerName.ToLower()).Contains(Util.Converts(Phone.ToLower())) || x.Code.Contains(Phone)).ToList();
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

        public OrderDetailOutputModel GetOrderDetail(int orderID, double? balance)
        {
            OrderDetailOutputModel data = new OrderDetailOutputModel();
            var order = cnn.Orders.Find(orderID);
            if (order != null)
            {
                data.OrderID = orderID;
                data.TotalPrice = order.TotalPrice;
                data.Discount = order.Discount;
                data.Status = order.Status;
                data.Code = order.Code;
                data.ProvinceID = order.ProvinceID;
                data.DistrictID = order.DistrictID;
                data.ProvinceName = order.Province.Name;
                data.DistrictName = order.District.Name;
                data.Address = order.BuyerAddress;
                data.Note = order.Note;
                data.BuyerName = order.BuyerName;
                data.BuyerPhone = order.BuyerPhone;
                data.CancelDate = order.CancelDate;
                data.ConfirmDate = order.ConfirmDate;
                data.PaymentDate = order.PaymentDate;
                data.CreateDate = order.CreateDate;
                data.LastRefCode = order.LastRefCode;
                data.Hour = order.CreateDate.ToString("HH:mm");
                data.Date = order.CreateDate.ToString("dd/MM/yyyy");
                //Lấy ra số dư người dùng cho app tiện sử dụng
                data.Point = balance;

                //lấy danh dách sản phẩm trong đơn hàng
                var listOdItem = (from oi in cnn.OrderItems
                                  where oi.IsActive.Equals(SystemParam.ACTIVE) && oi.OrderID.Equals(orderID)
                                  select oi).ToList();
                data.listOrderItem = listOdItem.Select(oi => new OrderDetailModel
                {
                    OrderItemID = oi.ID,
                    ItemID = oi.ItemID,
                    ItemName = oi.Item.Name,
                    ItemPrice = oi.Item.Price,
                    Qty = oi.QTY,
                    Warranty = oi.Item.Warranty,
                    SumPrice = Convert.ToInt64(oi.SumPrice),
                    Description = oi.Item.Description,
                    Image = oi.Item.ImageUrl.Split(',').FirstOrDefault()
                }).ToList();
                //data.listOrderItem = (from oi in cnn.OrderItems
                //                      where oi.IsActive.Equals(SystemParam.ACTIVE) && oi.OrderID.Equals(orderID)
                //                      select new OrderDetailModel
                //                      {
                //                          OrderItemID = oi.ID,
                //                          ItemID = oi.ItemID,
                //                          ItemName = oi.Item.Name,
                //                          ItemPrice = oi.Item.Price,
                //                          Qty = oi.QTY,
                //                          Warranty = oi.Item.Warranty,
                //                          SumPrice = oi.SumPrice,
                //                          Description = oi.Item.Description,
                //                          Image = oi.Item.ImageUrl
                //                      }).ToList();
                data.Qty = data.listOrderItem.Select(u => u.Qty).ToList().Sum();
                return data;
            }
            else return null;
        }

        public OrderOutputModel CreateOrder(List<OrderDetailModel> lsOrderItem, int cusID)
        {
            try
            {
                Order od = new Order();
                Customer cus = cnn.Customers.Find(cusID);
                Province pro = cnn.Provinces.Find(cus.ProvinceCode);
                District dist = cnn.Districts.Find(cus.DistrictCode);
                List<OrderItem> listOI = CreateOrderItem(lsOrderItem);
                od.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                od.Status = 0;
                od.PointAdd = 0;
                od.IsActive = SystemParam.ACTIVE;
                od.CreateDate = DateTime.Now;
                od.CustomerID = cusID;
                od.TotalPrice = Convert.ToInt64(listOI.Select(u => u.SumPrice).Sum());
                od.Discount = 0;
                od.OrderItems = listOI;
                od.BuyerName = cus.Name;
                od.BuyerPhone = cus.Phone;
                od.BuyerAddress = cus.Address + " , " + dist.Name + " , " + pro.Name;
                cnn.Orders.Add(od);
                cnn.SaveChanges();
                int id = cnn.Orders.OrderByDescending(u => u.ID).FirstOrDefault().ID;
                return GetOrderDetail(id, null);
            }
            catch
            {
                return null;
            }
        }
        public List<OrderItem> CreateOrderItem(List<OrderDetailModel> lsOrderItem)
        {
            List<OrderItem> lsOI = new List<OrderItem>();
            foreach (var orderItem in lsOrderItem)
            {
                Item item = cnn.Items.Find(orderItem.ItemID);
                if (item != null && item.Status.Equals(SystemParam.ACTIVE))
                {
                    OrderItem oi = new OrderItem();
                    oi.ItemID = orderItem.ItemID;
                    oi.QTY = orderItem.Qty;
                    oi.SumPrice = item.Price * orderItem.Qty;
                    oi.Status = SystemParam.ACTIVE;
                    oi.Discount = 0;
                    oi.IsActive = SystemParam.ACTIVE;
                    oi.CreateDate = DateTime.Now;
                    lsOI.Add(oi);
                }
            }
            return lsOI;


        }



        public List<ListOrderModel> GetListOrderByStatus(int? status, int cusID, string fromdate, string todate)
        {
            try
            {
                List<ListOrderModel> listOutput = new List<ListOrderModel>();
                DateTime? fromD = null;
                DateTime? toD = null;
                try
                {
                    fromD = DateTime.ParseExact(fromdate, "dd/MM/yyyy", null);
                }
                catch { }
                try
                {
                    toD = DateTime.ParseExact(todate, "dd/MM/yyyy", null);
                }
                catch { }
                var data = (from o in cnn.Orders
                            where o.IsActive.Equals(SystemParam.ACTIVE)
                            && o.Type.Equals(SystemParam.TYPE_ORDER)
                            && o.CustomerID.Equals(cusID)
                            && (status.HasValue ? o.Status == status.Value : o.Status == SystemParam.STATUS_ORDER_CANCEL || o.Status == SystemParam.STATUS_ORDER_PAID || o.Status == SystemParam.STATUS_ORDER_REFUSE)
                            orderby o.ID descending
                            select o).ToList();
                var query = data.Select(o => new ListOrderModel
                {
                    orderID = o.ID,
                    totalPrice = o.TotalPrice,
                    status = o.Status,
                    qty = o.OrderItems.Count(),
                    code = o.Code,
                    image = !String.IsNullOrEmpty(o.OrderItems.Select(i => i.Item.ImageUrl).FirstOrDefault()) ? o.OrderItems.Select(i => i.Item.ImageUrl).FirstOrDefault().Split(',').FirstOrDefault() : "",
                    name = o.OrderItems.Select(i => i.Item.Name).FirstOrDefault(),
                    CreateDate = o.CreateDate,
                }).ToList();
                if (query != null && query.Count() > 0)
                {
                    listOutput = query.ToList();
                    if (fromD != null)
                        listOutput = listOutput.Where(u => u.CreateDate.Date >= fromD.Value.Date).ToList();
                    if (toD != null)
                        listOutput = listOutput.Where(u => u.CreateDate.Date <= toD.Value.Date).ToList();
                    //if(text.Length > 0)
                    //    listOutput = listOutput.Where(u => Util.Converts(u.code.ToLower()).Contains(Util.Converts(text.ToLower()))).ToList();
                    return listOutput;
                }
                else
                    return new List<ListOrderModel>();
            }
            catch (Exception e)
            {
                return new List<ListOrderModel>();
            }
        }





        public OrderDetailEditOutput ItemEdit(int ID)
        {
            try
            {
                var query = cnn.Orders.Find(ID);
                string codeOrder = query.Code;
                OrderDetailEditOutput edit = new OrderDetailEditOutput();
                edit.OrderID = query.ID;
                edit.Code = query.Code;
                edit.CusName = query.Customer.Name;
                edit.Phone = query.Customer.Phone;
                edit.CreateDate = query.CreateDate;
                edit.AgentCode = query.Customer.AgentCode;
                edit.Status = query.Status;
                edit.TotalPrice = query.TotalPrice + query.Discount;
                edit.Discount = query.Discount * 100 / (query.TotalPrice + query.Discount);
                edit.BuyerName = query.BuyerName;
                edit.BuyerPhone = query.BuyerPhone;
                edit.BuyerAddress = query.BuyerAddress;
                edit.LastRefCode = query.LastRefCode;


                var queryFindPoint = cnn.MembersPointHistories.Where(x => x.AddPointCode.Equals(codeOrder));

                if (queryFindPoint != null && queryFindPoint.Count() > 0)
                {
                    edit.addPoint = queryFindPoint.ToList().LastOrDefault().Point;
                }


                edit.ListItem = (from oi in cnn.OrderItems
                                 where oi.IsActive.Equals(SystemParam.ACTIVE) && oi.OrderID.Equals(ID)
                                 select new OrderItemEdit
                                 {
                                     ItemID = oi.ItemID,
                                     ItemName = oi.Item.Name,
                                     ItemCode = oi.Item.Code,
                                     ItemQTY = oi.QTY,
                                     ItemPrice = cnn.Items.Where(u => u.ID.Equals(oi.ItemID)).FirstOrDefault().Price,
                                     ItemTotalPrice = oi.SumPrice,
                                 }).ToList();
                return edit;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new OrderDetailEditOutput();
            }
        }
        public int SaveEdit(int ID, int Status, double? AddPoint, string BuyerName, string BuyerPhone, string BuyerAddress, double TotalPrice, int Discount, string LastRefCode)
        {

            try
            {
                PackageBusiness packageBusiness = new PackageBusiness();
                PointBusiness pointBus = new PointBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();
                MembersPointHistory pointHistory = new MembersPointHistory();
                var itemEdit = cnn.Orders.Find(ID);
                var customer = cnn.Customers.Find(itemEdit.CustomerID);
                //var PointNow = cnn.Customers.Find(itemEdit.CustomerID).Point;
                var statusCurrent = itemEdit.Status;
                var returnPoint = Math.Round(Convert.ToDouble(Convert.ToDouble(cnn.Orders.Find(ID).TotalPrice) / 1000),2);
                var plusPoint = Math.Round(Convert.ToDouble((Convert.ToDouble(cnn.Orders.Find(ID).TotalPrice) / 1000) * SystemParam.PARAM_PLUS),2);
                double? cusPoint = customer.Point;
                double? cusPointRanking = customer.PointRanking;
                double? pointUserTake = null;
                double? PointRankingUserTake = null;
                //if (AddPoint == null)
                //{
                //    AddPoint = 0;
                //}
                if (Status == SystemParam.STATUS_ORDER_CANCEL)
                {
                    itemEdit.CancelDate = DateTime.Now;
                }
                else
               if (Status == SystemParam.STATUS_ORDER_PAID)
                {
                    itemEdit.PaymentDate = DateTime.Now;

                }
                else
                {
                    if (Status == SystemParam.STATUS_ORDER_CONFIRM)
                    {
                        itemEdit.ConfirmDate = DateTime.Now;
                    }
                }

                TotalPrice = itemEdit.TotalPrice;
                BuyerName = itemEdit.BuyerName;
                BuyerPhone = itemEdit.BuyerPhone;
                itemEdit.BuyerAddress = BuyerAddress;
                itemEdit.Status = Status;
                LastRefCode = itemEdit.LastRefCode;
                //if (itemEdit.Status == SystemParam.STATUS_ORDER_PENDING || itemEdit.Status == SystemParam.STATUS_ORDER_CONFIRM)
                //{
                //    customer.Point = 0 + PointNow;
                //}
                //else 
                if (itemEdit.Status == SystemParam.STATUS_ORDER_CANCEL || itemEdit.Status == SystemParam.STATUS_ORDER_REFUSE)
                {
                    AddPoint = Convert.ToDouble(returnPoint);
                    cusPointRanking = AddPoint.Value + customer.Point;
                    customer.Point = AddPoint.Value + customer.Point;
                }
                else if (itemEdit.Status == SystemParam.STATUS_ORDER_PAID)
                {
                    AddPoint = Convert.ToDouble(plusPoint);
                    cusPointRanking = AddPoint.Value + customer.PointRanking;
                    customer.PointRanking = AddPoint.Value + customer.PointRanking;
                    itemEdit.PointAdd = AddPoint.Value;
                    //Kiểm tra có mã giới thiệu không
                    if (LastRefCode != null && LastRefCode != "")
                    {
                        //Lấy id của mã giới thiệu
                        Customer customerRecommend = null;
                        int? idcr = (from c in cnn.Customers
                                    where c.Phone.Equals(LastRefCode) && c.IsActive.Equals(1) && c.Status.Equals(SystemParam.ACTIVE)
                                    select c.ID).FirstOrDefault();
                        if(idcr.HasValue && idcr > 0 )
                        {
                            customerRecommend = cnn.Customers.Find(idcr.Value);
                            //var PointNowRecommend = cnn.Customers.Find(from c in cnn.Customers
                            //                                           where c.Phone.Equals(LastRefCode) && c.IsActive.Equals(SystemParam.ACTIVE)
                            //                                           select c.ID).Point;
                            //lấy điểm + trong config
                            var addPointcr = (from config in cnn.Configs
                                              where config.ID.Equals(SystemParam.TYPE_POINT_RANKING)
                                              select config.Value).FirstOrDefault();
                            //Cộng điểm cho khách giới thiệu
                            customerRecommend.Point = customerRecommend.Point + Math.Round(Convert.ToDouble(returnPoint * Convert.ToDouble(addPointcr) / 100),2);
                            //pointUserTake = customerRecommend.PointRanking + (returnPoint * Convert.ToDouble(addPointcr)) / 100;
                            //PointRankingUserTake = customerRecommend.PointRanking;
                            //customerRecommend.Point = pointUserTake;//điểm + thêm cho sđt giới thiệu
                        }

                    }

                }


                cnn.SaveChanges();
                string titleNotify = "";
                string contentNoti = "";
                int typeNotify = 0;
                //thông báo cho khách giới thiệu
                string titleNotifyR = "";
                string contentNotiR = "";
                int typeNotifyR = 0;

                if (statusCurrent != Status && Status != SystemParam.STATUS_ORDER_PENDING)
                {
                    switch (Status)
                    {
                        case SystemParam.STATUS_ORDER_CANCEL:
                            titleNotify = "đã bị hủy";
                            typeNotify = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                            break;
                        case SystemParam.STATUS_ORDER_CONFIRM:
                            titleNotify = "đã được xác nhận";
                            typeNotify = SystemParam.NOTIFY_NAVIGATE_ORDER;
                            break;
                        case SystemParam.STATUS_ORDER_PAID:
                            titleNotify = "đã được hoàn thành";
                            typeNotify = SystemParam.NOTIFY_NAVIGATE_HISTORY; ;
                            if (LastRefCode != null && LastRefCode != "")
                            {
                                titleNotifyR = "đã được cộng";
                                typeNotifyR = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                            }
                            break;
                        case SystemParam.STATUS_ORDER_REFUSE:
                            titleNotify = "đã bị từ chối";
                            typeNotify = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                            break;
                        default:
                            break;
                    }
                    if (Status == SystemParam.STATUS_ORDER_PAID)
                    {
                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify + " và bạn được cộng thêm " + AddPoint + " điểm vào ví điểm tích";
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.TYPE_ADD_POINT_RANKING, contentNoti, contentNoti, ID);
                        pointBus.CreateHistoryes(itemEdit.CustomerID, AddPoint.Value, SystemParam.TYPEADD_POINT_FROM_BILL, SystemParam.TYPE_POINT_RANKING, itemEdit.Code, "Bạn vừa được cộng " + AddPoint + " điểm từ đơn hàng vào ví điểm tích" + itemEdit.Code, 0);
                        if (LastRefCode != null && LastRefCode != "")
                        {
                            //Lấy id của mã giới thiệu
                            Customer customerRecommend = null;
                            int? idcr = (from c in cnn.Customers
                                        where c.Phone.Equals(LastRefCode) && c.IsActive.Equals(1) && c.Status.Equals(SystemParam.ACTIVE)
                                        select c.ID).FirstOrDefault();
                            if (idcr.HasValue && idcr > 0)
                            {
                                customerRecommend = cnn.Customers.Find(idcr.Value);
                                //lấy điểm + trong config
                                var addPointcr = (from config in cnn.Configs
                                                  where config.ID.Equals(SystemParam.TYPE_POINT_RANKING)
                                                  select config.Value).FirstOrDefault();
                                //Thông báo cho khách giới thiệu
                                var addPointcrplus = Math.Round(Convert.ToDouble(returnPoint * Convert.ToDouble(addPointcr) / 100), 2);//điểm + thêm cho sđt giới thiệu
                                contentNotiR = "Bạn được " + titleNotifyR + " " + addPointcrplus + " điểm giới thiệu sản phẩm vào ví Point";
                                notifyBusiness.CreateNoti(idcr.Value, SystemParam.TYPE_ADD_POINT, contentNotiR, contentNotiR, ID);
                                pointBus.CreateHistoryes(idcr.Value, Convert.ToDouble(addPointcrplus), SystemParam.TYPE_ADD_POINT_PRODUCT_INTRODUCTION, SystemParam.TYPE_POINT, itemEdit.Code, "Bạn vừa được cộng " + addPointcrplus + " điểm từ đơn hàng giới thiệu vào ví Point " + itemEdit.Code, 0);
                            }
                            
                        }
                    }
                    else if (Status == SystemParam.STATUS_ORDER_REFUSE || Status == SystemParam.STATUS_ORDER_CANCEL)
                    {

                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify + " và bạn được hoàn trả " + AddPoint + " điểm ";
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.TYPE_ADD_POINT, contentNoti, contentNoti, ID);
                        pointBus.CreateHistoryes(itemEdit.CustomerID, AddPoint.Value, SystemParam.TYPEADD_POINT_FROM_BILL, SystemParam.TYPE_POINT, itemEdit.Code, "Bạn vừa được hoàn trả " + AddPoint + " điểm từ đơn hàng vào ví point" + itemEdit.Code, 0);
                    }

                    else
                    {
                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify;
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, typeNotify, contentNoti, contentNoti, ID);
                    }
                    if (customer.DeviceID != null && customer.DeviceID.Length > 0)
                    {
                        NotifyDataModel notifyData = new NotifyDataModel();
                        notifyData.id = ID;
                        notifyData.type = SystemParam.NOTIFY_NAVIGATE_ORDER;
                        notifyData.code = itemEdit.Code;
                        notifyData.StatusOrder = Status;
                        notifyData.Point = (double)cusPoint.Value;
                        notifyData.PointRaking = (double)cusPointRanking.Value;
                        List<string> listDevice = new List<string>();
                        listDevice.Add(customer.DeviceID);
                        string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                       packageBusiness.PushOneSignals(value);
                        if (Status == SystemParam.STATUS_ORDER_PAID)
                        {

                            if (LastRefCode != null && LastRefCode != "")
                            {
                                //Lấy id của mã giới thiệu
                                Customer customerRecommend = null;
                                int? idcr = (from c in cnn.Customers
                                            where c.Phone.Equals(LastRefCode) && c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.ACTIVE)
                                            select c.ID).FirstOrDefault();
                                if (idcr.HasValue && idcr > 0)
                                {
                                    customerRecommend = cnn.Customers.Find(idcr.Value);
                                    //lấy điểm + trong config
                                    var addPointcr = (from config in cnn.Configs
                                                      where config.ID.Equals(SystemParam.TYPE_POINT_RANKING)
                                                      select config.Value).FirstOrDefault();
                                    //Thông báo cho khách giới thiệu
                                    if (customerRecommend.DeviceID != null && customerRecommend.DeviceID.Length > 0)
                                    {
                                        NotifyDataModel notifyDataR = new NotifyDataModel();
                                        notifyDataR.id = idcr.Value;
                                        notifyDataR.type = SystemParam.TYPEADD_POINT_FROM_BILL;
                                        notifyDataR.code = itemEdit.Code;
                                        //notifyData.Point = pointUserTake == null ? 0 : pointUserTake.Value;
                                        //notifyData.PointRaking = PointRankingUserTake == null ? 0 : PointRankingUserTake.Value;
                                        List<string> listDeviceR = new List<string>();
                                        listDeviceR.Add(customerRecommend.DeviceID);
                                        string valueR = packageBusiness.StartPushNoti(notifyDataR, listDeviceR, SystemParam.TICHDIEM_NOTI, contentNotiR);
                                        packageBusiness.PushOneSignals(valueR);
                                    }
                                }
                                
                            }
                        }
                    }
                }


                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int DeleteOrder(int ID)
        {
            try
            {
                var ItemDel = cnn.Orders.Find(ID);
                ItemDel.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public string countOrder()
        {
            int isDone = cnn.Orders.Count(x => x.Status == 0 && x.IsActive == 1);
            int orderCount = cnn.Orders.Where(x => x.IsActive == 1).Count();
            int isActive = cnn.Orders.Where(x => x.IsActive == SystemParam.ACTIVE&& x.Type==SystemParam.TYPE_ORDER).Count();
            //return "" + isDone + " / " + orderCount;
            return "" + isActive;
        }

        //xuất Excel
        public ExcelPackage ExportBill(int ID, string userName)
        {
            try
            {
                OrderDetailEditOutput Bill = ItemEdit(ID);
                string path = HttpContext.Current.Server.MapPath(@"/Template/BillForm.xlsx");
                FileInfo file = new FileInfo(path);
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                sheet.Cells[3, 3].Value = Bill.Code;
                sheet.Cells[3, 6].Value = Bill.CreateDate.ToString("dd/MM/yyyy hh:mm");
                sheet.Cells[5, 3].Value = Bill.BuyerName;
                sheet.Cells[6, 3].Value = Bill.BuyerPhone;
                sheet.Cells[7, 3].Value = Bill.BuyerAddress;

                //if (Bill.AgentCode != null)
                //    sheet.Cells[10, 3].Value = Bill.AgentCode;

                //if (Bill.Status == SystemParam.STATUS_ORDER_PAID)
                //{
                //    sheet.Cells[10, 3].Value = "Đã Thanh Toán";
                //}
                //if (Bill.Status == SystemParam.STATUS_ORDER_CONFIRM)
                //{
                //    sheet.Cells[10, 3].Value = "Đã xác Nhận";
                //}
                //if (Bill.Status == SystemParam.STATUS_ORDER_CANCEL)
                //{
                //    sheet.Cells[10, 3].Value = "Đã hủy";
                //}
                //if (Bill.Status == SystemParam.STATUS_ORDER_PENDING)
                //{
                //    sheet.Cells[10, 3].Value = "Chờ xác nhận";
                //}
                int row = 10;
                int stt = 1;
                foreach (var item in Bill.ListItem)
                {
                    sheet.Cells[row, 1].Value = stt.ToString();
                    sheet.Cells[row, 2].Value = item.ItemCode;
                    sheet.Cells[row, 3].Value = item.ItemName;
                    sheet.Cells[row, 3].AutoFitColumns();
                    sheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[row, 4].Value = item.ItemQTY.ToString();
                    sheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[row, 5].Value = @String.Format("{0:0,0}", item.ItemPrice);
                    sheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[row, 6].Value = @String.Format("{0:0,0}", item.ItemTotalPrice);
                    row++;
                    stt++;
                }
                //sheet.Cells[10, 2].AutoFitColumns();

                sheet.Cells[row + 1, 5].Style.Font.Bold = true;
                sheet.Cells[row + 1, 5].Value = "Tổng Tiền:";
                sheet.Cells[row + 1, 6].Value = @String.Format("{0:0,0}", Bill.TotalPrice);
                sheet.Cells[row + 1, 6].Style.Font.Bold = true;
                //sheet.Cells[row + 2, 5].Value = "Chiết Khấu:";
                //sheet.Cells[row + 2, 6].Value = @String.Format("{0:0,0}", Bill.Discount * Bill.TotalPrice / 100);
                //sheet.Cells[row + 3, 5].Value = "Tiền Thanh Toán:";
                //sheet.Cells[row + 3, 6].Value = @String.Format("{0:0,0}", Bill.TotalPrice - (Bill.Discount * Bill.TotalPrice / 100));

                //if (Bill.Status == 2)
                //{
                //    sheet.Cells[row + 4, 5].Value = "Điểm Tích:";
                //    sheet.Cells[row + 4, 5].AutoFitColumns();
                //    sheet.Cells[row + 4, 6].Value = @String.Format("{0:0,0}", Bill.addPoint.Value);
                //}
                int rangeLast = row;
                sheet.Cells["E" + rangeLast + ":F" + rangeLast].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //sheet.Cells[ row + ":F" + rangeLast].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //sheet.Cells[ row + ":F" + rangeLast].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //sheet.Cells[ row + ":F" + rangeLast].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                //border
                //sheet.Cells["A1:F" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //sheet.Cells["A1:F" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //sheet.Cells["A1:F" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //sheet.Cells["A1:F" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                return pack;
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }

        public ExcelPackage ExportExcelOrder(int? Status, string FromDate, string ToDate, string Phone)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Order.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 2;
                int stt = 1;

                var list = Search(Status, FromDate, ToDate, Phone);

                foreach (var item in list)
                {
                    sheet.Cells[row, 1].Value = stt;
                    sheet.Cells[row, 2].Value = item.Code;
                    sheet.Cells[row, 3].Value = item.BuyerName;
                    sheet.Cells[row, 4].Value = item.BuyerPhone;
                    sheet.Cells[row, 5].Value = item.TotalPrice;
                    //if (item.TypeGift == SystemParam.TYPE_GIFT_GIFT)
                    //{
                    //    sheet.Cells[row, 3].Value = "Quà tặng";
                    //}
                    //else if (item.TypeGift == SystemParam.TYPE_GIFT_VOUCHER)
                    //{
                    //    sheet.Cells[row, 3].Value = "Voucher";
                    //}
                    //else if (item.TypeGift == SystemParam.TYPE_GIFT_CARD)
                    //{
                    //    sheet.Cells[row, 3].Value = "Thẻ cào";
                    //}
                    if (item.Status == SystemParam.STATUS_ORDER_PENDING)
                    {
                        sheet.Cells[row, 6].Value = "Chờ xác nhận";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_CONFIRM)
                    {
                        sheet.Cells[row, 6].Value = "Đang thực hiện";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_CANCEL)
                    {
                        sheet.Cells[row, 6].Value = "Hủy";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_PAID)
                    {
                        sheet.Cells[row, 6].Value = "Đã hoàn thành";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_REFUSE)
                    {
                        sheet.Cells[row, 6].Value = "Từ chối";
                    }

                    sheet.Cells[row, 7].Value = item.CreateDate.ToString("dd/MM/yyyy");
                    row++;
                    stt++;
                }
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }
        }
        //Get Revenue
        public double Revenue()
        {
            return cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_PAID && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice);
        }
    }
}
