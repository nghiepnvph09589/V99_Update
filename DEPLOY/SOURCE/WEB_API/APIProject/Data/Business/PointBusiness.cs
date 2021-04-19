using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Business
{
    public class PointBusiness : GenericBusiness
    {
        public PointBusiness(TichDiemTrieuDo context = null) : base()
        {
        }
        NotifyBusiness notiBus = new NotifyBusiness();
        //LoginBusiness loginBusiness = new LoginBusiness();

        // tặng điểm
        public int CreateGivePoint(GetGiftPointInputModel item, int cusID)
        {
            try
            {
                Customer cusRe = cnn.Customers.Where(u => u.Phone.Equals(item.Phone) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                Customer cusGi = cnn.Customers.Find(cusID);
                if (cusGi.Point < int.Parse(item.Point) + Util.minPointAccount())
                {
                    return SystemParam.KHONG_DU_DIEM_DE_TANG;
                }
                if ((getTotalPointPerDay(cusID) + int.Parse(item.Point)) > Util.maxPointPerDay())
                {
                    return SystemParam.VUOT_QUA_HAN_MUC_QUY_DINH;
                }

                // sửa lại tổng điểm
                cusGi.Point -= int.Parse(item.Point);
                cusRe.Point += int.Parse(item.Point);
                cnn.SaveChanges();

                // người gửi
                CreateHistoryes(cusID, int.Parse(item.Point), SystemParam.TYPE_POINT_GIVE, SystemParam.HISTORY_TYPE_ADD_ANOTHER, Util.RandomString(SystemParam.SIZE_CODE, false), item.Comment, cusRe.ID);
                //notiBus.CreateNoti(cusID, SystemParam.NOTIFY_TYPE_POINT_GIVE, "Bạn đã tặng " + item.Point + " điểm cho " + cusRe.Name + "(" + cusRe.Phone + ")");
                // người nhận
                CreateHistoryes(cusRe.ID, int.Parse(item.Point), SystemParam.TYPE_POINT_RECEIVE, SystemParam.HISTORY_TYPE_ADD_ANOTHER, Util.RandomString(SystemParam.SIZE_CODE, false), item.Comment, cusID);
                notiBus.CreateNoti(cusRe.ID, SystemParam.NOTIFY_TYPE_POINT_RECEIVE, "Bạn đã được tặng " + item.Point + " điểm từ " + cusGi.Name + (cusGi.Phone.Length > 0 ? "(" + cusGi.Phone + ")" : ""), "", 0);
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }


        // tích điểm
        public int CreateAddPointByWarranty(AddPointIntPutModel item, int cusID)
        {
            var items = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductCode.Equals(item.code.Trim())).FirstOrDefault();
            //int type = 0;
            //try
            //{
            //    type = int.Parse(item.code.Substring(item.code.Length - 1));
            //}
            //catch
            //{
            //}
            //if (type == SystemParam.WARRANTY)
            if (items == null)
            {
                WarrantyCard wc = cnn.WarrantyCards.Where(u => u.WarrantyCardCode.Equals(item.code) && u.Status.Equals(SystemParam.NO_ACTIVE) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                if (wc != null)
                {
                    wc.CustomerActiveID = cusID;
                    wc.ActiveDate = DateTime.Now;
                    wc.Status = SystemParam.ACTIVE;

                    // thêm điểm cho customer
                    Customer cus = cnn.Customers.Find(cusID);
                    cus.Point += wc.Warranty.Point;
                    int maxPointRanking = Util.maxPointRanking();
                    if ((cus.PointRanking + wc.Warranty.Point) >= maxPointRanking)
                    {
                        cus.PointRanking = maxPointRanking;
                    }
                    else
                    {
                        cus.PointRanking += wc.Warranty.Point;
                    }
                    cnn.SaveChanges();

                    CreateHistoryes(cusID, wc.Warranty.Point, SystemParam.TYPE_POINT_SAVE, SystemParam.HISTORY_TYPE_ADD_WARRANTY, item.code, SystemParam.COMMENT_HISTORY_ADD_POINT, 0);
                    return wc.Warranty.Point;
                }
            }
            else
            {
                Product pd = cnn.Products.Where(u => u.ProductCode.Equals(item.code.Trim()) && u.Status.Equals(SystemParam.NO_ACTIVE) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                if (pd != null)
                {
                    pd.CustomerActiveID = cusID;
                    pd.ActiveDate = DateTime.Now;
                    pd.Status = SystemParam.ACTIVE;
                    pd.ExpireDate = DateTime.Now.AddMonths(pd.Batch.Item.Warranty);

                    // thêm điểm cho customer
                    Customer cus = cnn.Customers.Find(cusID);
                    cus.Point += pd.Batch.Point;
                    int maxPointRanking = Util.maxPointRanking();
                    if ((cus.PointRanking + pd.Batch.Point) >= maxPointRanking)
                    {
                        cus.PointRanking = maxPointRanking;
                    }
                    else
                    {
                        cus.PointRanking += pd.Batch.Point;
                    }
                    cnn.SaveChanges();
                    CreateHistoryes(cusID, pd.Batch.Point, SystemParam.TYPE_POINT_SAVE, SystemParam.HISTORY_TYPE_ADD_PRODUCT, item.code, SystemParam.COMMENT_HISTORY_SAVE_POINT_PRODUCT, 0);
                    return pd.Batch.Point;
                }
            }
            return -1;
        }

        public List<HistoryGivePointWebOutputModel> ListHistoty(DateTime? time, int loginID, int type)
        {
            List<HistoryGivePointWebOutputModel> query = new List<HistoryGivePointWebOutputModel>();
            var listHistory = from h in cnn.MembersPointHistories
                              where h.IsActive.Equals(SystemParam.ACTIVE)
                              orderby h.CraeteDate descending
                              select new HistoryGivePointWebOutputModel
                              {
                                  HistoryID = h.ID,
                                  CustomerID = h.CustomerID.Value,
                                  Code = h.AddPointCode,
                                  CreateDate = h.CraeteDate,
                                  CustomerName = h.Customer.Name,
                                  Point = h.Point,
                                  Type = h.Type,
                                  TypeAdd = h.TypeAdd,
                                  Title = h.Title,
                                  Balance = h.Balance,
                              };
            if (listHistory != null && listHistory.Count() > 0)
            {
                query = listHistory.ToList();
                if (type == SystemParam.ROLL_CUSTOMER)
                {
                    query = query.Where(u => u.CustomerID.Equals(loginID)).ToList();
                }
                if (time != null)
                {
                    query = query.Where(u => u.CreateDate >= time).ToList();
                }
            }
            return query;
        }
        // tạo lịch sử 
        public void CreateHistoryes(int cusID, double point, int type, int typeadd, string code, string comment, int recusID)
        {
            try
            {
                MembersPointHistory mph = new MembersPointHistory();
                Customer cus = cnn.Customers.Find(cusID);
                Customer Recus = cnn.Customers.Find(recusID);
                mph.CustomerID = cusID;
                mph.Point = point;
                mph.Type = type;
                mph.TypeAdd = typeadd;
                mph.AddPointCode = code;
                mph.CraeteDate = DateTime.Now;
                mph.Balance = cus.Point;
                mph.IsActive = SystemParam.ACTIVE;
                switch (type)
                {
                    case SystemParam.TYPEADD_POINT_FROM_BILL:
                        //mph.Title = "Bạn đã được tích thêm " + point + " điểm";
                        var product = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductCode.Equals(code));
                        if (product != null && product.Count() > 0)
                        {
                            int productID = product.FirstOrDefault().ID;
                            mph.ProductID = productID;
                        }
                        break;
                    case SystemParam.TYPE_REQUEST_GIFT_POINT:
                        mph.Title = "Bạn đã tặng " + point + " điểm cho " + Recus.Name + "(" + Recus.Phone + ")";
                        break;
                    case SystemParam.TYPE_AWARDED_POINT:
                        mph.Title = "Bạn đã được tặng " + point + " điểm từ " + Recus.Name + (Recus.Phone.Length > 0 ? "(" + Recus.Phone + ")" : "");
                        break;
                    //case 4:
                    //    mph.Title = comment;
                    //    mph.Status = SystemParam.STATUS_ORDER_CONFIRM;
                    //    break;
                    case SystemParam.TYPE_ADD_POINT:
                        mph.Comment = comment;
                        mph.Title = "Bạn vừa được cộng " + point + " điểm từ hệ thống";
                        // mph.Title = "Bạn đã được cộng " + point + " điểm từ hệ thống";
                        break;
                    case 6:
                        mph.Title = "Đổi thẻ";
                        break;
                    case 7:
                        mph.Title = comment;
                   //     mph.Status = SystemParam.STATUS_ORDER_REFUSE;
                        break;
                }
                mph.Comment = comment;
                cnn.MembersPointHistories.Add(mph);
                cnn.SaveChanges();
            }
            catch
            {
            }

        }
        // tỉnh tổng tiền đã gưi trong ngày
        public double getTotalPointPerDay(int cusID)
        {
            List<double> listpoint = cnn.MembersPointHistories.Where(u => u.CustomerID.Value.Equals(cusID)
            && (u.CraeteDate.Day).Equals(DateTime.Now.Day)
            && (u.CraeteDate.Month).Equals(DateTime.Now.Month)
            && (u.CraeteDate.Year).Equals(DateTime.Now.Year)
            && (u.Type.Equals(SystemParam.TYPE_POINT_GIVE) || u.Type.Equals(SystemParam.TYPE_POINT_RECEIVE_GIFT) || u.Type.Equals(SystemParam.TYPE_CARD))).Select(u => u.Point).ToList();
            return listpoint.Sum();
        }

        public List<ListHistoryOutputModel> Search(int Page, int Type, string KeySearch, string fromDate, string toDate)
        {
            try
            {
                List<ListHistoryOutputModel> listHistories = new List<ListHistoryOutputModel>();
                var query = from p in cnn.MembersPointHistories
                            where p.IsActive.Equals(SystemParam.ACTIVE)
                            orderby p.ID descending
                            select new ListHistoryOutputModel
                            {
                                ID = p.ID,
                                PointAddCode = p.AddPointCode,
                                Type = p.Type,
                                CustomerID = p.CustomerID.Value,
                                CustomerName = p.Customer.Name,
                                CustomerPhone = p.Customer.Phone,
                                ActiveDate = p.Product != null ? p.Product.ActiveDate : null,
                                ExpireDate = p.Product != null ? p.Product.ExpireDate : null,
                                NameProduct = p.Product != null && p.Product.Batch != null && p.Product.Batch.Item != null ? p.Product.Batch.Item.Name : null,
                                Point = p.Point,
                                Balance = p.Balance,
                                CreateDate = p.CraeteDate
                            };
                var query2 = (from p in cnn.MembersPointHistories
                              where p.IsActive.Equals(SystemParam.ACTIVE)
                              orderby p.ID descending
                              select p
                            ).ToList();
                if (Type != 0)
                {
                    query = query.Where(u => u.Type.Equals(Type));
                }
                if (fromDate != null && fromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(fromDate);
                    query = query.Where(u => u.CreateDate >= fd.Value);
                }
                /* if (toDate.HasValue)
                 {
                 (  u.CreateDate <= td.Value)
                     query = query.Where(u => u.CreateDate.Value.Day <= toDate.Value.Day && u.CreateDate.Value.Month <= toDate.Value.Month && u.CreateDate.Value.Year <= toDate.Value.Year);
                 }*/
                if (toDate != null && toDate != "")
                {
                    DateTime? td = Util.ConvertDate(toDate);
                    query = query.Where(u => (u.CreateDate.Value.Day <= td.Value.Day) && (u.CreateDate.Value.Month <= td.Value.Month) && (u.CreateDate.Value.Year <= td.Value.Year));
                }
                if (query != null && query.Count() > 0)
                {
                    listHistories = query.ToList();
                    if (!String.IsNullOrEmpty(KeySearch))
                        listHistories = listHistories.Where(u => Util.Converts(u.CustomerName.ToLower()).Contains(Util.Converts(KeySearch.ToLower())) || u.CustomerPhone.Contains(KeySearch) || Util.Converts(u.PointAddCode.ToLower()).Contains(Util.Converts(KeySearch.ToLower()))).ToList();
                }
                return listHistories;
            }
            catch
            {
                return new List<ListHistoryOutputModel>();
            }
        }

        public PointDetailOutputModel GetPointDetail(int ID)
        {
            try
            {
                var query = (from mp in cnn.MembersPointHistories
                             join c in cnn.Customers
                             on mp.CustomerID equals c.ID
                             where mp.ID.Equals(ID) && mp.IsActive.Equals(SystemParam.ACTIVE)
                             select new PointDetailOutputModel
                             {
                                 AddPointCode = mp.AddPointCode,
                                 Type = mp.Type,
                                 Balance = (int)mp.Balance,
                                 ActiveDate = mp.Product != null ? mp.Product.ActiveDate : null,
                                 ExpireDate = mp.Product != null ? mp.Product.ExpireDate : null,
                                 ProductName = mp.Product != null && mp.Product.Batch != null && mp.Product.Batch.Item != null ? mp.Product.Batch.Item.Name : null,
                                 CustomerName = c.Name,
                                 Address = c.Address,
                                 Point = mp.Point,
                                 CreatDate = mp.CraeteDate,
                                 Phone = c.Phone
                             }).FirstOrDefault();
                return query;
            }
            catch
            {
                return new PointDetailOutputModel();
            }
        }


        public int activeWarrantyByPhone(AddPointIntPutModel item, Customer customer)
        {
            var items = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductCode.Equals(item.code.Trim())).FirstOrDefault();
            if (items != null && items.ID > 0)
            {
                //nếu mã đã được kích hoạt
                if (items.Status == SystemParam.ACTIVE)
                    return -2;

                items.CustomerActiveID = customer.ID;
                items.ActiveDate = DateTime.Now;
                items.Status = SystemParam.ACTIVE;
                items.ExpireDate = DateTime.Now.AddMonths(items.Batch.Item.Warranty);

                // thêm điểm cho customer
                customer.Point += items.Batch.Point;
                int maxPointRanking = Util.maxPointRanking();
                //nếu bảo hành xong mà điểm rank cao hơn mức quy định => điểm rank của khách = điểm admin quy định
                if ((customer.PointRanking + items.Batch.Point) >= maxPointRanking)
                {
                    customer.PointRanking = maxPointRanking;
                }
                else
                {
                    customer.PointRanking += items.Batch.Point;
                }
                cnn.SaveChanges();
                CreateHistoryes(customer.ID, items.Batch.Point, SystemParam.TYPE_POINT_SAVE, SystemParam.HISTORY_TYPE_ADD_PRODUCT, item.code, SystemParam.COMMENT_HISTORY_SAVE_POINT_PRODUCT, 0);
                return items.Batch.Point;
            }
            //nếu không tìm thấy mã
            return -1;
        }



    }
}
