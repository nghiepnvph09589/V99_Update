using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Data.Business.VNPAY_CS_ASPX;

namespace Data.Business
{
    public class VNPayBusiness : GenericBusiness
    {
        public VNPayBusiness(TichDiemTrieuDo context = null) : base()
        {

        }
        VnPayLibrary vnpay = new VnPayLibrary();
        PackageBusiness oneSignalBus = new PackageBusiness();
        public string GetUrl(int TransactionID)
        {
            //Get Config Info
            //Get payment input
            //Build URL for VNPAY
            MembersPointHistory transaction = cnn.MembersPointHistories.Find(TransactionID);

            vnpay.AddRequestData("vnp_Version", "2.0.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", SystemParam.vnp_TmnCode);
            string locale = "vn";//"en"
            if (!string.IsNullOrEmpty(locale))
            {
                vnpay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", transaction.ID.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", transaction.Title);
            vnpay.AddRequestData("vnp_OrderType", "insurance"); //default value: other
            vnpay.AddRequestData("vnp_Amount", (transaction.Point * 100).ToString());
            vnpay.AddRequestData("vnp_ReturnUrl", SystemParam.vnp_Return_url);
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //if (bank.SelectedItem != null && !string.IsNullOrEmpty(bank.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", bank.SelectedItem.Value);
            //}
            string paymentUrl = vnpay.CreateRequestUrl(SystemParam.vnp_Url, SystemParam.vnp_HashSecret);
            oneSignalBus.SaveLog("url", paymentUrl);
            return paymentUrl;
        }
        public static string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
        public VnpViewModel GetVnpReturn(VnpOutputModel vnp)
        {
            string json = JsonConvert.SerializeObject(vnp);
            oneSignalBus.SaveLog("url_return", json);
            VnpViewModel vnpOutput = new VnpViewModel();
            string lang = "vi";
            string Transaction_False = SystemParam.Transaction_False;
            string Transaction_Success = SystemParam.Transaction_Success;
            try
            {
                MembersPointHistory transaction = cnn.MembersPointHistories.Find(int.Parse(vnp.vnp_TxnRef));
                int money;
                try
                {
                    money = int.Parse(vnp.vnp_Amount) / 100;
                    if (money != transaction.Point)
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", money), transaction.CraeteDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                        return vnpOutput;
                    }
                }
                catch (Exception ex)
                {
                    string jsonEx = JsonConvert.SerializeObject(ex);
                    oneSignalBus.SaveLog("Exepcion", jsonEx);
                    vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CraeteDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                    return vnpOutput;
                }

                if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSuccess)
                {
                    if (transaction != null)
                    {

                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CraeteDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_Success, SystemParam.customer_success);
                    }
                    else
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CraeteDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                    }
                }
                else
                {
                    vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", transaction.Point), transaction.CraeteDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                }
            }
            catch (Exception ex)
            {
                string jsonEx = JsonConvert.SerializeObject(ex);
                oneSignalBus.SaveLog("Exepcion", jsonEx);
                vnpOutput.getVnpModel(vnp.vnp_TxnRef, "", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
            }
            return vnpOutput;
        }
        public VNPayOutputModel GetVnpIpn(VnpOutputModel vnp)
        {

            VNPayOutputModel output = new VNPayOutputModel();
            try
            {
                string json = JsonConvert.SerializeObject(vnp);
                oneSignalBus.SaveLog("url_ipn", json);
                PackageBusiness noti = new PackageBusiness();
                int appID = 0;
                appID = int.Parse(vnp.vnp_TxnRef);
                MembersPointHistory transaction = cnn.MembersPointHistories.Find(int.Parse(vnp.vnp_TxnRef));
                if (transaction == null)
                {
                    output = output.GetPayOutputModel("Order not found", "01");
                    return output;
                }
                if (transaction != null)
                {
                    int money = 0;
                    try
                    {
                        money = int.Parse(vnp.vnp_Amount) / 100;
                        if (money != transaction.Point)
                        {
                            output = output.GetPayOutputModel("Invalid amount", "04");
                            string Transaction_False = SystemParam.Transaction_False + output.RspCode;
                            return output;
                        }

                    }
                    catch
                    {
                        output = output.GetPayOutputModel("Invalid amount", "04");
                        string Transaction_False = SystemParam.Transaction_False + output.RspCode;
                        return output;
                    }
                    bool checkSignature = vnpay.ValidateSignature(vnp.vnp_SecureHash, SystemParam.vnp_HashSecret, vnp);

                    if (checkSignature)
                    {
                        try
                        {
                            if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSuccess)
                            {

                                if (transaction.Status == SystemParam.STATUS_TRANSACTION_FALSE)
                                {

                                    var cus = cnn.Customers.FirstOrDefault(x => x.ID == transaction.CustomerID);
                                    cus.Point += transaction.Point;
                                    var title = "Bạn vừa nạp thành công " + Util.ConvertCurrency(transaction.Point) + " điểm vào ví Point";
                                    var type = SystemParam.NOTIFY_ADD_POINT_VNPAY;
                                    var typeNoti = SystemParam.TYPE_ADD_POINT_VNPAY;
                                    var contentNoti = "Nạp tiền ví Point VNPAY";
                                    oneSignalBus.PushNotiApp(transaction.Point,type,transaction.Status.GetValueOrDefault(), typeNoti, transaction.ID, title, contentNoti, cus.ID,cus.DeviceID);
                                    transaction.Status = SystemParam.STATUS_TRANSACTION_SUCCESS;
                                    cnn.SaveChanges();
                                    string content = "Giao dịch thành công: \n + Mã đơn hàng qua VNPAY: " + vnp.vnp_TxnRef + "\n + Số tiền: " + string.Format("{0:#,0}", transaction.Point) + " đ\n + Thời gian: " + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                                    output = output.GetPayOutputModel("Confirm Success", vnp.vnp_ResponseCode);
                                    return output;
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }

                            }
                            else
                            {
                                if (transaction.Status == SystemParam.STATUS_TRANSACTION_WAITING)
                                {
                                    transaction.Status = SystemParam.STATUS_TRANSACTION_FALSE;
                                    cnn.SaveChanges();
                                    output = output.GetPayOutputModel("Confirm Success", "00");
                                    string Transaction_False = SystemParam.Transaction_False + output.RspCode;
                                    return output;
                                }
                                else if (transaction.Status == SystemParam.STATUS_TRANSACTION_FALSE)
                                {
                                    output = output.GetPayOutputModel("Confirm Success", "00");
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            output = output.GetPayOutputModel("Unknow error", "97");
                            string Transaction_False = SystemParam.Transaction_False + output.RspCode;
                        }
                    }
                    else
                    {
                        output = output.GetPayOutputModel("Invalid signature", "97");
                        string Transaction_False = SystemParam.Transaction_False + output.RspCode;
                    }
                }
                else
                    output = output.GetPayOutputModel("Order not found", "01");
            }
            catch (Exception ex)
            {
                output = output.GetPayOutputModel("Unknow error", "99");
                string Transaction_False = SystemParam.Transaction_False + output.RspCode;
            }
            oneSignalBus.SaveLog(output.RspCode, output.Message);
            return output;
        }
    }
}
