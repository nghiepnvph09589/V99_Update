using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
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
    public class CustomerBusiness : GenericBusiness
    {
        PointBusiness pointBus = new PointBusiness();
        NotifyBusiness notiBus = new NotifyBusiness();
        public CustomerBusiness(TichDiemTrieuDo context = null) : base()
        {
        }



        //public List<CustomerTopOutputModel> GetCustomerTop()
        //{
        //    List<CustomerTopOutputModel> query = new List<CustomerTopOutputModel>();
        //    var listCus = from c in cnn.Customers
        //                  where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.STATUS_RUNNING)
        //                  orderby c.Point descending
        //                  select new CustomerTopOutputModel
        //                  {
        //                      CustomerName = c.Name,
        //                      Point = c.Point,
        //                      UrlImage = c.AvatarUrl
        //                  };
        //    if (listCus != null && listCus.Count() > 0) {
        //        query = listCus.ToList();
        //    }
        //    return query;
        //}
        public List<Province> LoadCityCustomer()
        {
            List<Province> listCity = new List<Province>();
            var query = from p in cnn.Provinces
                        orderby p.Name
                        select p;

            if (query != null && query.Count() > 0)
            {
                listCity = query.ToList();
                return listCity;
            }
            else
                return new List<Province>();
        }



        public List<District> loadDistrict(int ProvinceID)
        {
            List<District> listDistrict = new List<District>();
            var query = from d in cnn.Districts
                        where d.ProvinceCode.Equals(ProvinceID)
                        select d;
            if (query != null && query.Count() > 0)
            {
                //listDistrict = query.ToList();
                return query.ToList();
            }
            else
                return new List<District>();
        }
        public List<ListCustomerOutputModel> Search(string FromDate, string ToDate, int? City, int? District, string Phone, int? Role, int? Rank, int? Status)
        {
            List<ListCustomerOutputModel> listCustomer = new List<ListCustomerOutputModel>();
            var query = from cus in cnn.Customers
                        where cus.IsActive.Equals(SystemParam.ACTIVE)
                        && (Status.HasValue ? cus.Status.Equals(Status.Value) : true) && cus.Password.Length > 0
                        //&&(cus.Password.Length > 10)
                        orderby cus.ID descending
                        select new ListCustomerOutputModel
                        {
                            CustomerID = cus.ID,
                            CustomerName = cus.Name,
                            PhoneNumber = cus.Phone,
                            Role = cus.Role,
                            Point = cus.Point,
                            Status = cus.Status,
                            DOB = cus.DOB,
                            Email = cus.Email,
                            ProvinceCode = cus.ProvinceCode,
                            DistrictCode = cus.DistrictCode,
                            TypeLogin = cus.Type,
                            CreateDate = cus.CraeteDate,
                            Sex = cus.Sex,
                            AgentCode = cus.AgentCode,
                            PointRanking = cus.PointRanking,
                            PointV = cus.PointV,
                            LastRefCode=cus.LastRefCode,
                            Address = cus.Address.Length == 0 || cus.Address == null ? "Chưa cập nhật" : cus.Address
                        };

            if (FromDate != "" && FromDate != null)
            {
                try
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate.Value >= fd);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            if (ToDate != "" && ToDate != null)
            {
                try
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate.Value <= td);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            

            if (Role != null)
                query = query.Where(p => p.Role == Role);

            if (Status != null)
                query = query.Where(p => p.Status == Status);

            if (City != null)
                query = query.Where(p => p.ProvinceCode == City);

            if (District != null)
                query = query.Where(p => p.DistrictCode == District);


            if (query != null && query.Count() > 0)
            {
                listCustomer = query.ToList();
                if (!String.IsNullOrEmpty(Phone))
                    listCustomer = listCustomer.Where(u => Util.Converts(u.CustomerName.ToLower()).Contains(Util.Converts(Phone.ToLower())) || u.PhoneNumber.Contains(Phone) || u.LastRefCode.Contains(Phone)).ToList();

            }
            return listCustomer;
        }

        public int addPoint(string Phone, int Point, string Note)
        {
            try
            {
                var query = cnn.Customers.Where(p => p.Phone.CompareTo(Phone) == 0);
                Customer Cus = query.SingleOrDefault();
                if (Cus == null || Cus.ID < 0)
                {
                    return 3;
                }

                //if (Cus.Status == 0)
                //{
                //    return 2;
                //}
                //Cus.Point += Point;
                cnn.SaveChanges();
                //notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
                pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, SystemParam.HISTORY_TYPE_ADD_PRODUCT, Util.CreateMD5(Cus.Phone), Note, 0);
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public List<Order> searchOrderHistory(int cusID, string fromDate, string toDate)
        {
            try
            {
                var query = cnn.Orders.Where(o => o.IsActive == SystemParam.ACTIVE && o.CustomerID == cusID);
                if (fromDate != "" && fromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(fromDate);
                    query = query.Where(x => x.CreateDate >= fd);
                }
                if (toDate != "" && toDate != null)
                {
                    DateTime? td = Util.ConvertDate(toDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(x => x.CreateDate <= td);
                }
                if (query != null && query.Count() >= 0)
                {
                    return query.OrderByDescending(x => x.CreateDate).ToList();
                }
                else
                {
                    return new List<Order>();
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<Order>();
            }


        }

        // cộng điểm nhiều khách hàng

        public int addPointAll(string listID, string listCusPhone, int Point, string Note)
        {
            try
            {
                // Customer customer = new Customer();
                PackageBusiness packageBusiness = new PackageBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();
                if (listCusPhone != null && listCusPhone != "")
                {
                    List<string> arrListPhone = listCusPhone.Split(',').ToList<string>();
                    foreach (string strPhone in arrListPhone)
                    {
                        var query = cnn.Customers.Where(p => p.IsActive.Equals(SystemParam.ACTIVE) && p.Phone.Equals(strPhone));
                        if (query == null || query.Count() <= 0)
                        {
                            return 3;
                        }
                        Customer Cus = query.FirstOrDefault();
                        Cus.Point += Point;
                        cnn.SaveChanges();
                        string contentNoti = "Bạn vừa được cộng " + Point + " điểm từ hệ thống";
                        int typeNotify = SystemParam.NOTIFY_NAVIGATE_HISTORY;
                        notifyBusiness.CreateNoti(Cus.ID, typeNotify, contentNoti, contentNoti, 0);
                        if (Cus.DeviceID != null && Cus.DeviceID.Length > 15)
                        {
                            NotifyDataModel notifyData = new NotifyDataModel();
                            notifyData.id = Cus.ID;
                            notifyData.type = SystemParam.HISPOINT_HE_THONG_CONG_DIEM;
                            List<string> listDevice = new List<string>();
                            listDevice.Add(Cus.DeviceID);
                            string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, contentNoti);
                            packageBusiness.PushOneSignals(value);
                        }
                        //notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
                        pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.TYPE_ADD_POINT, SystemParam.TYPE_POINT, Util.CreateMD5(DateTime.Now.ToString()), Note, 0);
                    }
                }
                if (listID != null && listID != "")
                {

                    List<string> arrListID = listID.Split(',').ToList<string>();
                    foreach (string strid in arrListID)
                    {
                        int id = int.Parse(strid);
                        var query = cnn.Customers.Where(p => p.IsActive.Equals(SystemParam.ACTIVE) && p.ID.Equals(id));
                        Customer Cus = query.SingleOrDefault();
                        //Cus.Point += Point;
                        cnn.SaveChanges();
                        //notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
                        pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, SystemParam.TYPE_POINT_RANKING, Util.CreateMD5(DateTime.Now.ToString()), Note, 0);
                    }
                    cnn.SaveChanges();
                }
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public Customer getCustomerByPhone(string Phone)
        {
            try
            {
                Customer cusDetail = cnn.Customers.Where(p => p.Phone.Equals(Phone)).SingleOrDefault();
                return cusDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Customer();
            }
        }

        public Customer cusDetail(int? ID)
        {
            try
            {
                Customer cusDetail = cnn.Customers.Where(p => p.ID == ID && p.IsActive == SystemParam.ACTIVE).SingleOrDefault();
                return cusDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Customer();
            }
        }
        public int SaveEditCustomer(int ID, int Status,int IsVip)
        {
            try
            {
                Customer cus = cnn.Customers.Find(ID);
                //cus.Status = Status;
                cus.Status = Status;
                cus.IsVip = IsVip;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public List<GetListHistoryMemberPointInputModel> SearchHistoryPoint(int cusID, string FromDate, string ToDate)
        {
            try
            {
                var cus = cnn.Customers;
                var query = from MBH in cnn.MembersPointHistories
                            where MBH.IsActive.Equals(SystemParam.ACTIVE)
                            && MBH.CustomerID == cusID && (MBH.Type == SystemParam.TYPE_REQUEST_DRAW_POINT || MBH.Type == SystemParam.TYPE_REQUEST_GIFT_POINT ||
                            MBH.Type == SystemParam.TYPE_FEE_USE_APP || MBH.Type == SystemParam.TYPE_ADD_POINT || (MBH.Type == SystemParam.TYPE_SYSTEM_ADD_POINT && MBH.TypeAdd == SystemParam.TYPE_POINT) ||
                            MBH.Type == SystemParam.TYPE_AWARDED_POINT || MBH.Type == SystemParam.TYPE_MINUS_POINT_ORDER || (MBH.Type == SystemParam.TYPEADD_POINT_FROM_BILL && MBH.TypeAdd == SystemParam.TYPE_POINT))
                            select new GetListHistoryMemberPointInputModel
                            {
                                HistoryID = MBH.ID,
                                AddPointCode = MBH.AddPointCode,
                                Type = MBH.Type,
                                TypeAdd = MBH.TypeAdd,
                                Point = MBH.Point,
                                Comment = MBH.Comment,
                                Title = MBH.Title,
                                CreateDate = MBH.CraeteDate,
                                Phone = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Phone).FirstOrDefault() : "Chưa cập nhật",
                                UserSend = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Name).FirstOrDefault() : "Chưa cập nhật",
                                BankOwner = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankOwner : "",
                                BankAccount = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankAccount : "",
                                BankName = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().Bank.BankName : "",
                            };
                if (FromDate != null && FromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate >= fd);
                }
                if (ToDate != null && ToDate != "")



                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                    return query.OrderByDescending(x => x.CreateDate).ToList();
                else
                    return new List<GetListHistoryMemberPointInputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<GetListHistoryMemberPointInputModel>();
            }
        }

        //lich su tich diem

        public List<GetListHistoryMemberPointInputModel> SearchHistoryPointR(int cusID, string FromDate, string ToDate)
        {
            try
            {
                var cus = cnn.Customers;
                var query = from MBH in cnn.MembersPointHistories
                            where MBH.IsActive.Equals(SystemParam.ACTIVE)
                            && MBH.CustomerID == cusID && ((MBH.Type == SystemParam.TYPE_SYSTEM_ADD_POINT && MBH.TypeAdd == SystemParam.TYPE_POINT_RANKING) || (MBH.Type == SystemParam.TYPEADD_POINT_FROM_BILL && MBH.TypeAdd == SystemParam.TYPE_POINT_RANKING))
                            select new GetListHistoryMemberPointInputModel
                            {
                                HistoryID = MBH.ID,
                                AddPointCode = MBH.AddPointCode,
                                Type = MBH.Type,
                                TypeAdd = MBH.TypeAdd,
                                Point = MBH.Point,
                                Comment = MBH.Comment,
                                Title = MBH.Title,
                                CreateDate = MBH.CraeteDate,
                                Phone = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Phone).FirstOrDefault() : "Chưa cập nhật",
                                UserSend = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Name).FirstOrDefault() : "Chưa cập nhật",
                                BankOwner = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankOwner : "",
                                BankAccount = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankAccount : "",
                                
                            };
                if (FromDate != null && FromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate >= fd);
                }
                if (ToDate != null && ToDate != "")



                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                {
                    var s = query.OrderByDescending(x => x.CreateDate).ToList();
                    return s;
                }
                else
                    return new List<GetListHistoryMemberPointInputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<GetListHistoryMemberPointInputModel>();
            }
        }
        //lịch sử ví V

        public List<GetListHistoryMemberPointInputModel> SearchHistoryPointV(int cusID, string FromDate, string ToDate)
        {
            try
            {
                var cus = cnn.Customers;
                var query = from MBH in cnn.MembersPointHistories
                            where MBH.IsActive.Equals(SystemParam.ACTIVE)
                            && MBH.CustomerID == cusID && MBH.TypeAdd == SystemParam.TYPE_POINT_V
                            select new GetListHistoryMemberPointInputModel
                            {
                                HistoryID = MBH.ID,
                                AddPointCode = MBH.AddPointCode,
                                Type = MBH.Type,
                                TypeAdd = MBH.TypeAdd,
                                Point = MBH.Point,
                                Comment = MBH.Comment,
                                Title = MBH.Title,
                                CreateDate = MBH.CraeteDate,
                                Phone = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Phone).FirstOrDefault() : "Chưa cập nhật",
                                UserSend = MBH.UserSendID.HasValue ? cus.Where(c => c.ID == MBH.UserSendID.Value).Select(c => c.Name).FirstOrDefault() : "Chưa cập nhật",
                                BankOwner = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankOwner : "",
                                BankAccount = MBH.BankID.HasValue ? cnn.CustomerBanks.Where(b => b.ID == MBH.BankID.Value).FirstOrDefault().BankAccount : "",
                                
                            };
                if (FromDate != null && FromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate >= fd);
                }
                if (ToDate != null && ToDate != "")



                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                {
                    var s = query.OrderByDescending(x => x.CreateDate).ToList();
                    return s;
                }
                else
                    return new List<GetListHistoryMemberPointInputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<GetListHistoryMemberPointInputModel>();
            }
        }
        public List<ListRequestOutputModel> SearchReQuest(int cusID, string FromDate, string ToDate)
        {
            try
            {
                var query = from RQ in cnn.Orders
                            where RQ.IsActive.Equals(SystemParam.ACTIVE)
                            && RQ.CustomerID == cusID
                            select new ListRequestOutputModel
                            {
                                RequestID = RQ.ID,
                                Type = RQ.Type,
                                Code = RQ.Code,
                                PointAdd = (int)RQ.PointAdd,
                                Status = RQ.Status,
                                CreateDate = RQ.CreateDate
                            };
                if (FromDate != null && FromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate >= fd);
                }
                if (ToDate != null && ToDate != "")
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                    return query.OrderByDescending(r => r.CreateDate).ToList();
                else
                    return new List<ListRequestOutputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListRequestOutputModel>();
                throw;
            }
        }

        public int DeleteCustomer(int ID)
        {
            try
            {
                var cusDelete = cnn.Customers.Find(ID);
                //if (cusDelete.Status == 1)
                //{
                //    return 2;
                //}
                cusDelete.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public List<Customer> TopPoint()
        {
            Customer p = new Customer();
            var topPoint = (from tp in cnn.Customers
                            where tp.IsActive.Equals(SystemParam.ACTIVE)
                            orderby tp.Point descending
                            select tp).Take(10).ToList();
            return topPoint;
            //var p = (from c in cnn.Customers
            //        orderby c.Point descending
            //        select c).ToList();
            // return cnn.Customers.Where(x => x.IsActive.Equals(SystemParam.ACTIVE)).Take(10).ToList();
        }
        public string countCustomer()
        {

            return cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)&& u.Password.Length > 0).Count().ToString();
        }
        //Select list image agent
        public List<ListImageAgentModel> ListCusImage(int? ID)
        {
            try
            {
                List<ListImageAgentModel> lstIMG = new List<ListImageAgentModel>();
                //Sử dụng linq
                //lstIMG = (from c in cnn.CustomerImages
                //             where c.IsActive.Equals(1) && c.CustomerID.Equals(ID)
                //             select new ListImageAgentModel
                //             {
                //                 ID = c.ID,
                //                 ImgUrl = c.Images
                //             }).ToList();
                //sử dụng lambda
                var query = cnn.CustomerImages.Where(c => c.IsActive == 1 && c.CustomerID == ID).ToList();
                foreach (var item in query)
                {
                    ListImageAgentModel img = new ListImageAgentModel();
                    img.ID = item.ID;
                    img.ImgUrl = item.Images;
                    lstIMG.Add(img);
                }
                if (lstIMG != null)
                {
                    return lstIMG;
                }
                else
                {
                    return lstIMG;
                }
            }
            catch
            {
                return null;
            }
        }

        public ExcelPackage ExportExcelCustomer(string FromDate, string ToDate, string Phone, int? Status)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Customer.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;

                var list = Search(FromDate, ToDate, null, null, Phone, null, null, Status);

                foreach (var item in list)
                {
                    sheet.Cells[row, 1].Value = stt;
                    sheet.Cells[row, 2].Value = item.CustomerName;
                    sheet.Cells[row, 3].Value = item.PhoneNumber;
                    sheet.Cells[row, 4].Value = item.Email;
                    sheet.Cells[row, 5].Value = item.Point;
                    sheet.Cells[row, 6].Value = item.PointRanking;
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
                    sheet.Cells[row, 7].Value = item.Status == SystemParam.ACTIVE ? "Hoạt động" : "Dừng hoạt động";


                    sheet.Cells[row, 8].Value = item.CreateDate.Value.ToString("dd/MM/yyyy");
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


        // lấy Thông tin tài khoản ngân hàng
        public List<ListCustomerBank> ListCustomerBank(int? ID)
        {
            var data = new List<ListCustomerBank>();
            var query = cnn.CustomerBanks.Where(c => c.CustomerID == ID.Value && c.IsActive == SystemParam.ACTIVE)

                .Select(c => new ListCustomerBank
                {
                    BankName = c.Bank.BankName,
                    ShortName = c.Bank.ShortName,
                    BankAcount = c.BankAccount,
                    BankOwer = c.BankOwner,
                    ImgUrl = c.Bank.ImageUrl,
                    CreateDate = c.CreatedDate

                }).ToList();
            if (query != null && query.Count() > 0)

                return data = query;
            return data;
        }

        // Reset MK khách hàng
        public int RefreshCustomer(int id)
        {
            try
            {
                Customer cu = cnn.Customers.Find(id);
                cu.Password = Util.GenPass(SystemParam.PASS_DEFAULT);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        //Get Revenue
        public double RevenuePoint()
        {
            var revenue = (from c in cnn.Customers
                           where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.ACTIVE)
                           select (double?)c.Point).Sum() ?? 0;
            //var revenue = (cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_PAID && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice));
            return revenue;
        }
        //Get Revenue
        public double RevenuePointRanking()
        {
            var revenue = (from c in cnn.Customers
                           where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.ACTIVE)
                           select (double?)c.PointRanking).Sum() ?? 0;
            //var revenue = (cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_PAID && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice));
            return revenue;
        }
        //Get Revenue
        public double RevenuePointV()
        {
            var revenue = (from c in cnn.Customers
                           where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.ACTIVE)
                           select (double?)c.PointV).Sum() ?? 0;
            //var revenue = (cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_PAID && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice));
            return revenue;
        }
    }
}
