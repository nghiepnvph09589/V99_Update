using APIProject.App_Start;
using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class AgentController : BaseController
    {
        // GET: Agent
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string Code, int? Status, string FromDate, string ToDate)
        {
            ViewBag.Code = Code;
            ViewBag.Status = Status;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.UserCreate = userLogin.UserName;
            return PartialView("_ListAgent", agentBusiness.Search(Page, Code, Status, FromDate, ToDate));
        }

        // Thêm Đại Lý
        [UserAuthenticationFilter]
        public int CreateAgent(Agent agent)
        {
            return agentBusiness.CreateAgent(agent);
        }

        // show edit Agent
        [UserAuthenticationFilter]
        public PartialViewResult ShowEditForm(int ID)
        {         
            return PartialView("_EditAgent", agentBusiness.ShowEdit(ID));
        }
        // Save Edit Agent
        [UserAuthenticationFilter]
        public int SaveEdit(int ID, string Name, string Phone, string Address)
        {
            return agentBusiness.SaveEdit(ID, Name, Address);
        }

        // Delete Agent
        [UserAuthenticationFilter]
        public int DeleteAgent(int ID)
        {
            return agentBusiness.DeleteAgent(ID);
        }

        // hủy kích hoạt đại lý
        [UserAuthenticationFilter]
        public int cancelActive(int ID)
        {
            try
            {
                return agentBusiness.cancelActive(ID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        // Import Data
        [UserAuthenticationFilter]
        public int ImportData(HttpPostedFileBase ExcelFile)
        {
            return agentBusiness.ImportExcel(ExcelFile);
        }

        // mẫu import
        [UserAuthenticationFilter]
        public FileResult exportFormImport()
        {
            FileInfo file = new FileInfo(Server.MapPath(@"/Template/FormAgent.xlsx"));
            ExcelPackage pack = new ExcelPackage(file);
            ExcelWorksheet workSheet = pack.Workbook.Worksheets[1];
            return File(pack.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FormAgent_template.xlsx");
        }
    }
}