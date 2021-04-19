using APIProject.App_Start;
using Data.Business;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.Revenue = statisticBus.Revenue();
            return View();
        }
        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, int? Status, string FromDate, string ToDate, string Phone)
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.Role = UserLogins.Role;
            ViewBag.Tus = Status;
            ViewBag.fd = FromDate;
            ViewBag.td = ToDate;
            ViewBag.Phone = Phone;
            return PartialView("_List", orderBus.Search(Status, FromDate, ToDate, Phone).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
        }

        [UserAuthenticationFilter]
        // ShowEdit
        public PartialViewResult ShowEditOrder(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_USER)
            {
                Session[Sessions.LOGIN] = null;
                return PartialView("_EditForm", new OrderDetailEditOutput());
            }
            else
                return PartialView("_EditForm", orderBus.ItemEdit(ID));
        }

        //Save Edit
        [UserAuthenticationFilter]
        public int SaveEditOrder(int ID, int Status, double? AddPoint, string BuyerName, string BuyerPhone, string BuyerAddress, long TotalPrice, int Discount, string LastRefCode)
        {
            try
            {
                //UserDetailOutputModel userLogin = UserLogins;
                   
                
                return orderBus.SaveEdit(ID, Status, AddPoint, BuyerName, BuyerPhone, BuyerAddress, TotalPrice, Discount, LastRefCode);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }

        }

        // Delete Order
        [UserAuthenticationFilter]
        public int DeleteOrder(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_USER)
            {
                Session[Sessions.LOGIN] = null;
                return SystemParam.ERROR;
            }
            return orderBus.DeleteOrder(ID);
        }

        [UserAuthenticationFilter]
        public FileResult ExportBill(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return File(orderBus.ExportBill(ID, userLogin.UserName).GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BillForm.xlsx");

            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }
        //export file excel
        [UserAuthenticationFilter]
        public FileResult ExportOrder(int? Status, string FromDate, string ToDate, string Phone)
        {
            return File(orderBus.ExportExcelOrder(Status, FromDate, ToDate, Phone).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Order.xlsx");
        }
    }
}