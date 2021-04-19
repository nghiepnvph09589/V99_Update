using Data.DB;
using Data.Model;
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
    public class AgentBusiness : GenericBusiness
    {
        public AgentBusiness(TichDiemTrieuDo context = null) : base()
        {

        }
        RequestAPIBusiness apiBus = new RequestAPIBusiness();

        // Tìm Kiếm Đại Lý
        public IPagedList<Agent> Search(int Page, string Code, int? Status, string FromDate, string ToDate)
        {
            try
            {
                var list = cnn.Agents.Where(x => x.IsActive.Equals(SystemParam.ACTIVE));

                if( Code != null && Code != "")
                {
                    list = list.Where(x => x.Code.Contains(Code));
                }
                if(Status != null && Status == SystemParam.STATUS_ACTIVE)
                {
                    list = list.Where(x => x.CustomerActiveID != null);
                }
                else if(Status != null && Status == 0)
                {
                    list = list.Where(x => x.CustomerActiveID == null);
                }
                if (FromDate != "" && FromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    list = list.Where(x => x.CreateDate >= fd);
                }

                if (ToDate != "" && ToDate != null)
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    list = list.Where(x => x.CreateDate <= td);
                }

                if (list != null && list.Count() > 0)
                    return list.OrderByDescending(x => x.ID).ToList().ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                else
                    return new List<Agent>().ToPagedList(1, 5);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<Agent>().ToPagedList(1,5);
            }
        }

        public Boolean CheckExistingAgent(string agentCode)
        {
            try
            {
                var agent = cnn.Agents.Where(u => u.Code.Equals(agentCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (agent != null && agent.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public int CreateAgent(Agent agent)
        {
            try
            {
                if (CheckExistingAgent(agent.Code))
                {
                    return SystemParam.EXISTING;
                }
                agent.CustomerActiveID = null;
                agent.Phone = null;
                agent.ActiveDate = null;
                agent.CreateDate = DateTime.Today;
                agent.IsActive = SystemParam.ACTIVE;
                cnn.Agents.Add(agent);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        public Agent ShowEdit(int ID)
        {
            try
            {
                Agent edit = cnn.Agents.Find(ID);
                return edit;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Agent();
            }
        }

        public int SaveEdit(int ID, string Name, string Address)
        {
            try
            {
                Agent agEdit = cnn.Agents.Find(ID);
                agEdit.Name = Name;
                agEdit.Address = Address;              
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int cancelActive(int ID)
        {
            try
            {
                Agent agentCancel = cnn.Agents.Find(ID);
                Customer cusAvtive = cnn.Customers.Find(agentCancel.CustomerActiveID);
                PointBusiness pointBus = new PointBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();
                PackageBusiness packageBusiness = new PackageBusiness();
                NotifyDataModel notifyData = new NotifyDataModel();
                agentCancel.CustomerActiveID = null;
                agentCancel.ActiveDate = null;
                agentCancel.Phone = null;             
                cusAvtive.AgentCode = null;
                agentCancel.Name = null;
                agentCancel.Address = null;
                agentCancel.Code = "";
                agentCancel.Phone = null;
                cusAvtive.Role = 1;
                agentCancel.IsActive = 0;
                cusAvtive.AgentCode = null;
                cnn.SaveChanges();
                notifyBusiness.CreateNoti(cusAvtive.ID, SystemParam.NOTIFY_TYPE_CANCEL_AGENT, "Bạn đã bị hủy kích hoạt đại lý" , "Bạn đã bị hủy kích hoạt đại lý", 0);
                if (cusAvtive.DeviceID != null && cusAvtive.DeviceID.Length > 0)
                {
                    notifyData.id = cusAvtive.ID;
                    List<string> listDevice = new List<string>();
                    listDevice.Add(cusAvtive.DeviceID);
                    string value = packageBusiness.StartPushNoti(notifyData, listDevice, SystemParam.TICHDIEM_NOTI, "Bạn đã bị hủy kích hoạt đại lý");
                    packageBusiness.PushOneSignals(value);
                }
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteAgent(int ID)
        {
            try
            {
                Agent delAgent = cnn.Agents.Find(ID);
                delAgent.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int ImportExcel(HttpPostedFileBase ExcelFile)
        {
            try
            {
                if (ExcelFile == null || ExcelFile.ContentLength == 0)
                {
                    //ModelState.AddModelError("", "Hãy chọn một file Excel");
                    return SystemParam.FILE_NOT_FOUND;
                }
                else if (true==true)
                {
                    string path = HttpContext.Current.Server.MapPath("~/Import/" + ExcelFile.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    ExcelFile.SaveAs(path);
                    FileInfo file = new FileInfo(path);
                    ExcelPackage pack = new ExcelPackage(file);
                    ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                    int row = 3;
                    int col = 1;
                    if(sheet.Cells[row, col].Value == null)
                    {
                        return SystemParam.FILE_EMPTY;
                    }
                    object data = 0;
                    while (data != null)
                    {
                        data = sheet.Cells[row, col].Value;
                        if (data == null)
                            break;
                        string CheckCode = sheet.Cells[row, col].Value.ToString();
                        if (cnn.Agents.Where(a => a.Code.Equals(CheckCode) && a.IsActive.Equals(SystemParam.ACTIVE)).Count() > 0)
                        {
                            return SystemParam.FILE_DATA_DUPLICATE;
                        }
                        Agent item = new Agent();
                        item.Code = sheet.Cells[row, col].Value.ToString();
                       string n  = sheet.Cells[row, 2].Value.ToString();
                        if(n == null || n == "")
                        {
                            item.Name = "";
                        }
                        else
                        {
                            item.Name = n;
                        }
                        item.Address = "";
                        item.CreateDate = DateTime.Now;
                        item.IsActive = SystemParam.ACTIVE;
                        cnn.Agents.Add(item);
                        row++;
                        cnn.SaveChanges();
                    }                  
                    return SystemParam.FILE_IMPORT_SUCCESS;
                }
                else
                {
                    return SystemParam.FILE_FORMAT_ERROR;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.FILE_FORMAT_ERROR;
            }
        }


        public void sendNotiOnebody(string deviceId, string contentNoti)
        {
            List<string> listDeviceId = new List<string>();
            listDeviceId.Add(deviceId);
            //apiBus.SendNotification(listDeviceId, new NotifyTitle(SystemParam.DAIICHI_NOTI), new NotifyContent(contentNoti), new object());
        }
        //
        //public void sendNotiOnebody(string deviceId, string contentNoti)
        //{
        //    List<string> listDeviceId = new List<string>();
        //    listDeviceId.Add(deviceId);
        //    apiBus.SendNotification(listDeviceId, new NotifyTitle(SystemParam.DAIICHI_NOTI), new NotifyContent(contentNoti), new object());
        //}

    }
}
