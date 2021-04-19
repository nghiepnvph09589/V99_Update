using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using PagedList.Mvc;
using Data.Business;
using Data.DB;

namespace APIProject.Controllers
{
    public class RequestController : BaseController
    {
        // GET: Request
        [UserAuthenticationFilter]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.RevenueRefuse = requestBusiness.RevenueRefuse();
            ViewBag.RevenueAccepted = requestBusiness.RevenueAccepted();
            ViewBag.RevenuePending = requestBusiness.RevenuePending();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, int? Status, string Name, string Phone, string FromDate, string ToDate)
        {
            try
            {
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Name = Name;
                ViewBag.Status = Status;
                ViewBag.Phone = Phone;
                ViewBag.PageCurrent = Page;
                return PartialView("_TableRequest", requestBusiness.Search( Status, Name, Phone, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch
            {
                return PartialView("_TableRequest", new List<RequestDetailWebOutputModel>().ToPagedList(1, 1));
            }
        }

        public ActionResult GoRequest()
        {
            return Json(Url.Action("Index", "Request"));
        }


        //Yeu cau rut diem
        public PartialViewResult SearchRequest(int ?id)
        {
            try
            {
                List<RequestDetailWebOutputModel> listRequest = requestBusiness.GetRequestDetail();
                return PartialView("_TableRequest",listRequest.ToList());
            }
            catch
            {
                return PartialView("_TableRequest");
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult GetRequestDetail(int ID)
        {
            try
            {
                RequestDetailWebOutputModel requestDetail = requestBusiness.GetRequest(ID);
                return PartialView("_RequestDetail", requestDetail);
            }
            catch
            {
                return PartialView("_RequestDetail" , new RequestDetailWebOutputModel());
            }
        }
        //public int AcceptRequest(int StatusRequest, int RequestID, int CustomerID, string RequestGiftName, string Note)
        //{
        //    try
        //    {
        //        UserDetailOutputModel userLogin = UserLogins;
        //        return requestBusiness.AcceptRequest(StatusRequest, RequestID, CustomerID, RequestGiftName, Note, userLogin.UserID);
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}

        [UserAuthenticationFilter]
        public int DeleteRequest(int RequestID)
        {
            try
            {
                return requestBusiness.DeleteRequest(RequestID);
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }

        public int UpdateRequest(int Id, int Select, string Note)
        {
            try
            {
                if(Select == SystemParam.STATUS_ORDER_REFUSE)
                {
                    return requestBusiness.UpdateRequest(Id, Select, Note);
                }
                if(Select == SystemParam.STATUS_ORDER_CONFIRM)
                {
                    return requestBusiness.UpdateConfirm(Id, Select);
                }
                return SystemParam.RETURN_TRUE;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        //Xuất Excel 
        //public FileResult ExportRequest(string fromDate, string toDate, int? status, int? typeRequest, string codeOrCusName)
        //{
        //    return File(requestBusiness.ExportExcel(fromDate, toDate, status, typeRequest, codeOrCusName).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "DS_yeu_cau_doi_qua.xlsx");
        //}

        // xuaats 1 yeeu caauf
        //public FileResult singleRequestExport(int id)
        //{
        //    try
        //    {
        //        return File(requestBusiness.singleRequestExport(id).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "Yeu_cau_doi_qua.xlsx");
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return null;
        //    }
        //}

        // Xuat QR to Excel
        //[UserAuthenticationFilter]
        //public FileResult ExportRequestDetail(int requestID)
        //{
        //    try
        //    {
        //        return File(requestBusiness.ExportRequestDetail(requestID).GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Batch.xlsx");

        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        public ActionResult NameSearch(int Page, int? StatusR,  string Name = "", string FromDate = "", string ToDate = "")
        {
            
            ViewBag.Name = Name;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.Status = StatusR;
            List<RequestDetailWebOutputModel> listRequest = new List<RequestDetailWebOutputModel>();
            listRequest = requestBusiness.NameSearch(Page,Name,FromDate,ToDate, StatusR);
            return PartialView("_TableRequest", listRequest.ToPagedList(Page,SystemParam.MAX_ROW_IN_LIST_WEB));
        }

    }
}