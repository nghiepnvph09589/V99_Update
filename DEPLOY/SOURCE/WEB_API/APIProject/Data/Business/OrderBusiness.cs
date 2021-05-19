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
        PackageBusiness packageBusiness = new PackageBusiness();
        MembersPointHistory hisPoint = new MembersPointHistory();
        PointBusiness pointBus = new PointBusiness();

        public List<OrderItem> CreateOrderItemOrder(List<OrderDetailModel> lsOrderItem)
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
        public OrderOutputModel CreateOrder(OrderDetailOutputModel input, int cusID, string lastRefCode)
        {
            var conect = cnn.Database.BeginTransaction();
            try
            {

                //tạo một đơn hàng mới

                Order od = new Order();
                var cus = cnn.Customers.Where(c => c.ID == cusID).FirstOrDefault();
                var code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                List<OrderItem> listOI = CreateOrderItemOrder(input.listOrderItem);
                var point = Math.Round(Convert.ToDouble(listOI.Select(u => u.SumPrice).Sum()) / 1000, 2);
                //Tính toán lại số dư khi mua hàng
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

                string content = "Hệ thống trừ điểm khi mua hàng";
                var type = SystemParam.NOTIFY_NAVIGATE_ORDER;
                var typeNoti = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                var statusOrder = SystemParam.STATUS_ORDER_PENDING;
                if (cus.PointRanking >= point * 0.1)
                {
                    var PointCus = point * 0.9;
                    var PointRankingCus = point * 0.1;
                    //Lưu lại số dư sau khi đã được tính toán

                    od.Point = PointCus;
                    od.PointRanking = PointRankingCus;

                    MembersPointHistory m = new MembersPointHistory();
                    MembersPointHistory mr = new MembersPointHistory();
                    //Tạo lịch sử mua hàng
                    string title = "Bạn vừa bị trừ " + PointCus + " điểm ví Point từ đơn hàng " + code;
                    string titleRank = "Bạn vừa bị trừ " + PointRankingCus + " điểm ví tích điểm từ đơn hàng " + code;
                    m.CustomerID = cusID;
                    m.Point = PointCus;
                    m.Type = SystemParam.TYPE_MINUS_POINT_ORDER;
                    m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                    m.TypeAdd = SystemParam.TYPE_POINT;
                    m.CraeteDate = DateTime.Now;
                    m.IsActive = SystemParam.ACTIVE;
                    m.Comment = "Hệ thống trừ điểm ví Point khi mua hàng";
                    m.Title = title;
                    m.Balance = cus.Point - PointCus;
                    //Tạo lịch sử mua hàng

                    mr.CustomerID = cusID;
                    mr.Point = PointRankingCus;
                    mr.Type = SystemParam.TYPE_MINUS_POINT_ORDER;
                    mr.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                    mr.TypeAdd = SystemParam.TYPE_POINT_RANKING;
                    mr.CraeteDate = DateTime.Now;
                    mr.IsActive = SystemParam.ACTIVE;
                    mr.Comment = "Hệ thống trừ điểm ví tích điểm khi mua hàng";
                    mr.Title = titleRank;
                    mr.Balance = cus.PointRanking - PointRankingCus;

                    cus.Point -= PointCus;
                    cus.PointRanking -= PointRankingCus;
                    cnn.MembersPointHistories.Add(m);
                    cnn.MembersPointHistories.Add(mr);
                    cnn.Orders.Add(od);
                    cnn.SaveChanges();

                    //Tạo thông báo cho người nhận
                    packageBusiness.PushNotiAppCNN(PointCus, type, statusOrder, typeNoti, od.ID, title, content, cus.ID, cus.DeviceID, cnn);
                    packageBusiness.PushNotiAppCNN(PointRankingCus, type, statusOrder, typeNoti, od.ID, titleRank, content, cus.ID, cus.DeviceID, cnn);
                }
                else
                {
                    cus.Point -= point;
                    od.Point = point;
                    MembersPointHistory m = new MembersPointHistory();
                    //Tạo lịch sử mua hàng
                    string title = "Bạn vừa bị trừ " + point + " điểm ví Point từ đơn hàng " + code;
                    m.CustomerID = cusID;
                    m.Point = point;
                    m.Type = SystemParam.TYPE_MINUS_POINT_ORDER;
                    m.AddPointCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                    m.TypeAdd = SystemParam.TYPE_POINT;
                    m.CraeteDate = DateTime.Now;
                    m.IsActive = SystemParam.ACTIVE;
                    m.Comment = "Hệ thống trừ điểm ví Point khi mua hàng";
                    m.Title = title;
                    m.Balance = cus.Point - point;
                    cnn.MembersPointHistories.Add(m);
                    cnn.Orders.Add(od);
                    cnn.SaveChanges();
                    packageBusiness.PushNotiAppCNN(point, type, statusOrder, typeNoti, od.ID, title, content, cus.ID, cus.DeviceID, cnn);
                }
                conect.Commit();
                conect.Dispose();

                //Tiến hành gửi thông báo đến web admin
                var url = SystemParam.URL_WEB_SOCKET + "?content=" + "Đơn hàng " + code + " đang chờ xác nhận&type=" + SystemParam.TYPE_NOTI_ORDER;
                packageBusiness.GetJson(url);

                int id = cnn.Orders.OrderByDescending(u => u.ID).FirstOrDefault().ID;
                return GetOrderDetail(id, cus.Point, cus.PointRanking);
            }
            catch (Exception ex)
            {
                conect.Rollback();
                conect.Dispose();
                return null;
            }
        }
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

        public OrderDetailOutputModel GetOrderDetail(int orderID, double? balance, double? balanceRanking)
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
                data.ProvinceName = order.Province != null ? order.Province.Name : "";
                data.DistrictName = order.District != null ? order.District.Name : "";
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
                data.PointRanking = balanceRanking;

                //lấy danh dách sản phẩm trong đơn hàng
                var listOdItem = (from oi in cnn.OrderItems
                                  where oi.IsActive.Equals(SystemParam.ACTIVE) && oi.OrderID.Equals(orderID)
                                  select oi).ToList();
                data.listOrderItem = listOdItem.Select(oi => new OrderDetailModel
                {
                    OrderItemID = oi.ID,
                    ItemID = oi.ItemID,
                    ItemName = oi.Item != null ? oi.Item.Name : "",
                    ItemPrice = oi.Item != null ? oi.Item.Price : 0,
                    Qty = oi.QTY,
                    Warranty = oi.Item != null ? oi.Item.Warranty : 0,
                    SumPrice = Convert.ToInt64(oi.SumPrice),
                    Description = oi.Item != null ? oi.Item.Description : "",
                    Image = oi.Item != null ? oi.Item.ImageUrl.Split(',').FirstOrDefault() : ""
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
                List<OrderItem> listOI = CreateOrderItem(lsOrderItem,cus.IsVip);
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
                return GetOrderDetail(id, null, null);
            }
            catch
            {
                return null;
            }
        }
        public List<OrderItem> CreateOrderItem(List<OrderDetailModel> lsOrderItem,int isVip)
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
                    oi.SumPrice = isVip == SystemParam.CUSTOMER_NORMAL ? (item.Price * orderItem.Qty) : (item.PriceVIP * orderItem.Qty);
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
                var returnPoint = Math.Round(Convert.ToDouble(Convert.ToDouble(cnn.Orders.Find(ID).Point.GetValueOrDefault()) ), 2);
                var returnPointRanking = Math.Round(Convert.ToDouble(Convert.ToDouble(cnn.Orders.Find(ID).PointRanking.GetValueOrDefault()) ), 2);
                var totalPoint = returnPoint + returnPointRanking;
                var plusPoint = Math.Round(Convert.ToDouble((Convert.ToDouble(cnn.Orders.Find(ID).TotalPrice) / 1000) * SystemParam.PARAM_PLUS), 2);
                double? cusPoint = customer.Point;
                double? cusPointRanking = customer.PointRanking;
                double? pointUserTake = null;
                double? PointRankingUserTake = null;
                double? AddPointRanking = null;
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
                    AddPointRanking = Convert.ToDouble(returnPointRanking);
                    cusPoint = AddPoint.Value + customer.Point;
                    cusPointRanking = AddPointRanking.GetValueOrDefault() + customer.PointRanking;
                    customer.Point = AddPoint.Value + customer.Point;
                    customer.PointRanking = AddPointRanking.GetValueOrDefault() + customer.PointRanking;
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
                        if (idcr.HasValue && idcr > 0)
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
                            customerRecommend.Point = customerRecommend.Point + Math.Round(Convert.ToDouble(totalPoint * Convert.ToDouble(addPointcr) / 100), 2);
                            //pointUserTake = customerRecommend.PointRanking + (returnPoint * Convert.ToDouble(addPointcr)) / 100;
                            //PointRankingUserTake = customerRecommend.PointRanking;
                            //customerRecommend.Point = pointUserTake;//điểm + thêm cho sđt giới thiệu
                        }

                    }

                }


                cnn.SaveChanges();
                string titleNotify = "";
                string contentNoti = "";
                string contentNotiRanking = "";
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

                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify + " và bạn được hoàn trả " + AddPoint + " điểm vào ví Point";
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.TYPE_ADD_POINT, contentNoti, contentNoti, ID);
                        pointBus.CreateHistoryes(itemEdit.CustomerID, AddPoint.Value, SystemParam.TYPEADD_POINT_FROM_BILL, SystemParam.TYPE_POINT, itemEdit.Code, "Bạn vừa được hoàn trả " + AddPoint + " điểm từ đơn hàng vào ví point" + itemEdit.Code, 0);
                        if (itemEdit.PointRanking.HasValue)
                        {
                            contentNotiRanking = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify + " và bạn được hoàn trả " + AddPointRanking + " điểm vào ví Tích điểm";
                            notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.TYPE_ADD_POINT, contentNotiRanking, contentNotiRanking, ID);
                            pointBus.CreateHistoryes(itemEdit.CustomerID, AddPointRanking.Value, SystemParam.TYPEADD_POINT_FROM_BILL, SystemParam.TYPE_POINT_RANKING, itemEdit.Code, "Bạn vừa được hoàn trả " + AddPointRanking + " điểm từ đơn hàng vào ví tích điểm" + itemEdit.Code, 0);

                        }
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
                        if (itemEdit.PointRanking.HasValue)
                        {
                            string valueRanking = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNotiRanking);
                            packageBusiness.PushOneSignals(valueRanking);
                        }
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
            int isActive = cnn.Orders.Where(x => x.IsActive == SystemParam.ACTIVE && x.Type == SystemParam.TYPE_ORDER).Count();
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
