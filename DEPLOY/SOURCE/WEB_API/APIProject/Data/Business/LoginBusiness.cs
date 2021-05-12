using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class LoginBusiness : GenericBusiness
    {
        public LoginBusiness(TichDiemTrieuDo context = null) : base()
        {

        }
        //PointBusiness pBus = new PointBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        /// <summary>
        /// đăng nhập vào app
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public LoginAppOutputModel CheckLoginApp(string Phone, string passWord, string deviceID)
        {
            try
            {
                LoginAppOutputModel data = new LoginAppOutputModel();
                var cus = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone == Phone).FirstOrDefault();
                if (Util.CheckPass(passWord, cus.Password))
                {
                    cus.Token = Util.CreateMD5(DateTime.Now.ToString());
                    cus.DeviceID = deviceID;
                    cus.ExpireTocken = DateTime.Now.AddYears(1);
                    cnn.SaveChanges();
                    data.UserID = cus.ID;
                    data.Type = SystemParam.SUCCESS_CODE;
                }
                else
                {
                    data.Type = SystemParam.PROCESS_ERROR;
                }
                return data;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new LoginAppOutputModel();
            }

        }


        public UserInforOutputModel GetuserInfor(int cusID)
        {
            UserInforOutputModel query = new UserInforOutputModel();
            Customer cus = cnn.Customers.Find(cusID);
            DateTime date = DateTime.Now.AddDays(2);

            Province pr = cnn.Provinces.Find(cus.ProvinceCode);
            District dt = cnn.Districts.Find(cus.DistrictCode);
            if (pr != null)
            {
                query.ProvinceID = cus.ProvinceCode;
                query.ProvinceName = pr.Name;
            }
            else
            {
                query.ProvinceID = cus.ProvinceCode;
                query.ProvinceName = "";
            }
            if (dt != null)
            {
                query.DistrictID = cus.DistrictCode;
                query.DistrictName = dt.Name;
            }
            else
            {
                query.DistrictID = cus.DistrictCode;
                query.DistrictName = "";
            }
            query.UserID = cusID;
            query.Address = cus.Address;
            query.DOB = cus.DOB;
            query.DOBStr = cus.DOB.ToString(SystemParam.CONVERT_DATETIME);
            query.Email = cus.Email;
            query.TypeLogin = cus.Type;
            query.CustomerName = cus.Name;
            query.Point = Math.Round(cus.Point,2);
            query.Phone = cus.Phone;
            query.Sex = cus.Sex;
            query.IsAgent = cus.Role.Equals(SystemParam.ROLE_AGENT) ? 1 : 0;
            query.UrlAvatar = cus.AvatarUrl;
            query.Token = cus.Token;
            double p = cus.PointRanking;
            query.PointRanking =Math.Round(p,2);
            query.Status = cus.Status;
            query.LastRefCode = cus.LastRefCode;
            query.IsVip = cus.IsVip;
            query.PointV = cus.PointV;

            return query;
        }



        /// <summary>
        /// Tạo Khách hàng mới
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void CreateCustomer(string phone)
        {
            try
            {
                Customer cus = new Customer();

                cus.Code = "";
                cus.Phone = phone;
                cus.Token = "";
                cus.IsActive = SystemParam.ACTIVE;
                cus.Address = "";
                cus.DOB = DateTime.Now;
                cus.Name = "";
                cus.Email = "";
                cus.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
                cus.DistrictCode = SystemParam.DISTRICT_DEFAULT;
                cus.Sex = SystemParam.GENDER_MEN;
                cus.Role = SystemParam.ROLE_CUSTOMER;
                cus.AvatarUrl = "https://st.quantrimang.com/photos/image/072015/22/avatar.jpg";
                cus.LastLoginDate = DateTime.Now;
                cus.Point = 0;
                cus.PointRanking = SystemParam.POINT_RANKING_START;
                cus.DeviceID = "";
                cus.Type = SystemParam.TYPE_LOGIN_DEFAULT;
                cus.Status = SystemParam.ACTIVE_FALSE;
                cus.ExpireTocken = DateTime.Now.AddSeconds(SystemParam.TIME_EXPIRE_OTP);
                cus.CraeteDate = DateTime.Now;
                cnn.Customers.Add(cus);
                cnn.SaveChanges();
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
            }
        }
        /// <summary>
        /// Tạo Khách hàng mới
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateUser(UserInforOutputModel item, int CusID)
        {
            try
            {
                item.Phone = Util.ConvertPhone(item.Phone);
                if (!Util.validPhone(item.Phone))
                    return SystemParam.ERROR_INVALID_PHONE;

                int res = checkExistPhoneEmail(CusID, item.Phone, item.Email);
                if (res != SystemParam.SUCCESS)
                    return res;

                Customer cus = cnn.Customers.Find(CusID);
                cus.Name = item.CustomerName;
                cus.Address = item.Address;
                try
                {
                    cus.DOB = DateTime.ParseExact(item.DOBStr, SystemParam.CONVERT_DATETIME, null);
                }
                catch { }
                cus.Email = item.Email;
                cus.ProvinceCode = item.ProvinceID;
                cus.DistrictCode = item.DistrictID;
                cus.Sex = item.Sex;
                cus.Phone = item.Phone;
                cus.LastLoginDate = DateTime.Now;
                cus.ExpireTocken = DateTime.Now.AddYears(1);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }


        public int checkExistPhoneEmail(int cusID, string phone, string email)
        {
            var customerUpdate = cnn.Customers.Find(cusID);
            var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            var customerPhone = customer.Where(u => u.Phone.Equals(phone));
            if (customerPhone != null && customerPhone.Count() > 0)
            {
                if (customerPhone.FirstOrDefault().ID != cusID)
                {
                    return SystemParam.ERROR_EXIST_PHONE;
                }
            }
            if (email.Length > 0 && customerUpdate.Email != email)
            {
                var customerEmail = customer.Where(u => u.Email.Equals(email));
                if (customerEmail != null && customerEmail.Count() > 0)
                {
                    if (customerEmail.FirstOrDefault().ID != cusID)
                    {
                        return SystemParam.ERROR_EXIST_EMAIL;
                    }
                }
            }
            return SystemParam.SUCCESS;
        }



        public UserDetailOutputModel CheckLoginWeb(string phone, string password)
        {
            UserDetailOutputModel query = new UserDetailOutputModel();
            var passUser = cnn.Users.Where(u => u.IsActive == SystemParam.ACTIVE && u.Phone == phone ).FirstOrDefault();
            if (passUser == null)
                return query;
            if (Util.CheckPass(password, passUser.PassWord))
            {
                query = cnn.Users.Where(u => u.IsActive == SystemParam.ACTIVE && u.Phone == phone ).Select(u => new UserDetailOutputModel { UserID = u.UserID, UserName = u.UserName, Role = u.Role, Phone = u.Phone }).FirstOrDefault();
            }
            else
            {
                query = null;
            }
            return query;
        }

        public CustomerDetailOutputModel CusDetail(int cusID)
        {
            Customer cus = cnn.Customers.Find(cusID);
            CustomerDetailOutputModel query = new CustomerDetailOutputModel();
            //query.Code = Util.CheckNullString(cus.Code);
            query.DistrictName = cus.District.Name;
            query.ProvinceName = cus.Province.Name;
            query.ProvinceID = cus.ProvinceCode;
            query.DistrictID = cus.DistrictCode;
            query.Address = cus.Address;
            query.DOB = cus.DOB;
            query.DOBStr = cus.DOB.ToString(SystemParam.CONVERT_DATETIME);
            query.Email = cus.Email;
            query.TypeLogin = cus.Type;
            query.CustomerName = cus.Name;
            query.Phone = cus.Phone;
            query.Sex = cus.Sex;
            //query.ConfirmCode = Util.CheckNullString(cus.ConfirmCode);
            //query.Point = cus.Point;
            query.UrlAvatar = cus.AvatarUrl;
            return query;
        }


        //public int RemainPoint(Customer cus)
        //{
        //    int totalPointPerDay = pBus.getTotalPointPerDay(cus.ID);
        //    int maxpoint = maxPointPerDay();
        //    int minpoint = minPointAccount();
        //    return cus.Point >= minpoint ? (cus.Point > minpoint + maxpoint ? maxpoint - totalPointPerDay : cus.Point - minpoint) : 0;
        //    //return cus.Point >= minpoint ? (cus.Point > minpoint + maxpoint ? maxpoint - totalPointPerDay : cus.Point - totalPointPerDay - minpoint) : 0;
        //}

        public UserInforOutputModel ActiveAgent(int cusID, int agentID)
        {
            Agent agent = cnn.Agents.Find(agentID);
            Customer cus = cnn.Customers.Find(cusID);
            cus.AgentCode = agent.Code;
            cus.Role = SystemParam.ROLE_AGENT;
            agent.CustomerActiveID = cusID;
            agent.ActiveDate = DateTime.Now;
            agent.Name = cus.Name;
            agent.Phone = cus.Phone;
            agent.Address = cus.Address.Length > 0 ? cus.Address.Trim() + ", " : "" + cus.District.Name + ", " + cus.Province.Name;
            cnn.SaveChanges();
            return GetuserInfor(cusID);
        }

        public LoginAppOutputModel CheckPhone(string phone)
        {
            try
            {
                LoginAppOutputModel query = new LoginAppOutputModel();
                var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Role <= 2 && u.Phone == phone).FirstOrDefault();

                if (customer != null)
                {
                    query.Type = SystemParam.TYPE_LOGIN_DEFAULT;
                    customer.ExpireTocken = DateTime.Now.AddSeconds(SystemParam.TIME_EXPIRE_OTP);
                    cnn.SaveChanges();
                }
                else
                {
                    query.Type = SystemParam.TYPE_REGISTER_ACCOUNT;
                    CreateCustomer(phone);
                }
                return query;
            }
            catch (Exception e)
            {
                LoginAppOutputModel query = new LoginAppOutputModel();
                query.Token = e.ToString();
                return query;
            }
        }


        public LoginAppOutputModel CheckPhoneRegister(string Email)
        {
            try
            {
                EmailBusiness email = new EmailBusiness();
                string code = Util.RandomNumber(100000,999999).ToString();
                LoginAppOutputModel data = new LoginAppOutputModel();
                var check = cnn.Customers.Where(c => c.Email == Email && c.IsActive == SystemParam.ACTIVE).FirstOrDefault();
                if (check != null)
                {
                    if (check.Password == null && check.Type.Equals(SystemParam.CHECK_OTP_TRUE))
                    {
                      
                        data.Type = SystemParam.TYPE_ERROR_UPDATE_PHONE;
                        data.IsUpdate = SystemParam.TYPE_ERROR_UPDATE_PHONE;
                        return data;
                    }else if (check.Password == null && !check.Type.Equals(SystemParam.CHECK_OTP_TRUE))
                    {
                        string contentMessage = string.Format(SystemParam.CONTENT_MESS, code);
                        log.Info("Gui den mail: " + Email);
                        log.Info("Noi dung: " + contentMessage);
                        check.Code = code;
                        check.ExpireTocken = DateTime.Now.AddSeconds(SystemParam.TIME_EXPIRE_OTP);
                        cnn.SaveChanges();
                        email.configClient(Email, "[TÍCH ĐIỂM V99]", contentMessage + code);
                        return data;
                    }
                    else
                    {
                        data.Type = SystemParam.TYPE_ERROR_PHONE_EXIST;
                        data.IsUpdate = SystemParam.ERROR;
                        return data;
                    }
                }
                else
                {
                    Customer cus = new Customer();
                    cus.Code = code;
                    cus.Name = "";
                    cus.Phone = "";
                    cus.Email = Email;
                    cus.CraeteDate = DateTime.Now;
                    cus.ExpireTocken = DateTime.Now.AddSeconds(SystemParam.TIME_EXPIRE_OTP);
                    cus.Token = "";
                    cus.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
                    cus.DistrictCode = SystemParam.DISTRICT_DEFAULT;
                    cus.DOB = DateTime.Now;
                    cus.Sex = SystemParam.GENDER_MEN;
                    cus.Role = SystemParam.ROLE_CUSTOMER;
                    cus.AvatarUrl = "https://st.quantrimang.com/photos/image/072015/22/avatar.jpg";
                    cus.LastLoginDate = DateTime.Now;
                    cus.Point = SystemParam.POINT_RANKING_START;
                    cus.PointRanking = SystemParam.POINT_RANKING_START;
                    cus.DeviceID = "";
                    cus.Address = "";
                    cus.Type = SystemParam.CHECK_OTP_FAIL;
                    cus.Status = SystemParam.ACTIVE_FALSE;
                    cus.PointV = SystemParam.POINT_V_START;
                    cus.IsVip = SystemParam.CUSTOMER_NORMAL;
                    cus.IsActive = SystemParam.ACTIVE;
                    cnn.Customers.Add(cus);
                    cnn.SaveChanges();
                    string contentMessage = string.Format(SystemParam.CONTENT_MESS, code);
                    log.Info("Gui den mail: " + Email);
                    log.Info("Noi dung: " + contentMessage);
                    email.configClient(Email, "[TÍCH ĐIỂM V99]", contentMessage + code);
                }
                return data;
            }
            catch (Exception e)
            {
                e.ToString();
                log.Info("Loi kiem tra email khi dang ky tai khoan: " + e.ToString());
                return new LoginAppOutputModel();
            }

        }

        //Check mã otp
        public int CheckOtpCode(string otp, string Email)
        {
            var cus = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Email == Email).FirstOrDefault();
            if (cus.Code != otp)
                return SystemParam.ERROR;
            if (cus.ExpireTocken < DateTime.Now)
                return SystemParam.ERROR_EXPIRE_OTP;
            cus.Type = SystemParam.CHECK_OTP_TRUE;
            cnn.SaveChanges();
            return SystemParam.SUCCESS;
        }

        //Đăng ký tài khoản
        public LoginAppOutputModel Register(LoginAppInputModel input)
        {
            try
            {
                LoginAppOutputModel data = new LoginAppOutputModel();
                var customer = cnn.Customers;
                var cus = customer.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Email.Equals(input.Email)).FirstOrDefault();
                var checkPhone = customer.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Phone.Equals(input.Phone)).FirstOrDefault();
               //Check tồn tại số điện thoại
                if(checkPhone != null)
                {
                    data.UserID = -1;
                    return data;
                }

                if (cus != null)
                {
                    cus.Name = input.Name;
                    cus.Password = Util.GenPass(input.Password);
                    cus.ExpireTocken = DateTime.Now.AddYears(1);
                    cus.Token = Util.CreateMD5(DateTime.Now.ToString());
                    cus.IsVip = SystemParam.CUSTOMER_NORMAL;
                    cus.PointV = 500;
                    cus.LastRefCode = input.LastRefCode;
                    cus.Phone = input.Phone;
                    cnn.SaveChanges();
                    data.UserID = cus.ID;
                    return data;
                }
                var customerRef = cnn.Customers.FirstOrDefault(x => x.Phone == input.LastRefCode);
                customerRef.PointV += 1000;
                cnn.SaveChanges();
                data.UserID = 0;
                return data;

            }
            catch (Exception e)
            {
                LoginAppOutputModel data = new LoginAppOutputModel();
                data.UserID = 0;
                e.ToString();
                log.Info("Loi dang ky tai khoan: " + e.ToString());
                return data;
            }
        }

        //Check số điện thoại quên mật khẩu
        public int CheckPhoneForgotPassword(string Email)
        {
            string code = Util.RandomNumber(100000, 999999).ToString();
            var check = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Email == Email).FirstOrDefault();
            if (check != null)
            {
                EmailBusiness email = new EmailBusiness();
                email.configClient(Email, "[TÍCH ĐIỂM V99]", "Mã xác thực của bạn là: " + code);
                check.ExpireTocken = DateTime.Now.AddSeconds(SystemParam.TIME_EXPIRE_OTP);
                check.Code = code;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            return SystemParam.RETURN_FALSE;
        }

        //Cập nhật lại mật khẩu của chức năng quên mật khẩu
        public int UpdatePassword(string Email, string password)
        {
            var cus = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && c.Email == Email).FirstOrDefault();
            if (cus != null)
            {
                cus.Password = Util.GenPass(password);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }

            return SystemParam.ERROR;
        }
    }
}
