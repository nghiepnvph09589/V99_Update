using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using Data.Utils;
using Data.Model;
using Data.Model.APIWeb;
using Data.DB;
using Data.Business;

namespace APIProject.Controllers
{
    public class CustomerController : BaseController
    {

        // GET: Customer
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.RevenuePoint = cusBusiness.RevenuePoint();
            ViewBag.RevenuePointRanking = cusBusiness.RevenuePointRanking();
            ViewBag.listCity = cusBusiness.LoadCityCustomer();
            return View();

        }
        [UserAuthenticationFilter]
        public PartialViewResult LoadDistrict(int ProvinceID)
        {
            ViewBag.listDistrict = cusBusiness.loadDistrict(ProvinceID);
            return PartialView("_ListDistrict");
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string FromDate, string ToDate, int? City, int? District, string Phone, int? Role, int? Rank, int? Status)
        {
            ViewBag.FromDateCus = FromDate;
            ViewBag.ToDateCus = ToDate;
            ViewBag.PhoneSearch = Phone;
            ViewBag.City = City;
            ViewBag.District = District;
            ViewBag.Role = Role;
            ViewBag.Status = Status;
            IPagedList<ListCustomerOutputModel> list = cusBusiness.Search(FromDate, ToDate, City, District, Phone, Role, Rank, Status).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            //ViewBag.listCustomer = cusBusiness.Search(FromDate, ToDate, City, District, Phone).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListCustomer", list);
        }
        [UserAuthenticationFilter]
        public int AddPoint(string Phone, int Point, string Note)
        {
            return cusBusiness.addPoint(Phone, Point, Note);

        }
        [UserAuthenticationFilter]
        public int addPointAll(string listID, string listCusPhone, int Point, string Note)
        {
            return cusBusiness.addPointAll(listID, listCusPhone, Point, Note);
        }

        [UserAuthenticationFilter]
        public PartialViewResult CustomerDetail(int? ID)
        {
            ViewBag.CusDetail = cusBusiness.cusDetail(ID);
            ViewBag.listCusImage = cusBusiness.ListCusImage(ID);
            ViewBag.MemberRank = rankBusiness.getRankByLever(1);
            ViewBag.SliverRank = rankBusiness.getRankByLever(2);
            ViewBag.GoldRank = rankBusiness.getRankByLever(3);
            ViewBag.PlatinumRank = rankBusiness.getRankByLever(4);
            ViewBag.CustomerBank = cusBusiness.ListCustomerBank(ID);
            return PartialView("_CustomerDetail");
        }

        public PartialViewResult Detail(int Id)
        {
            var cus = cusBusiness.cusDetail(Id);
            return PartialView("AddPoint",cus);
        }

        [UserAuthenticationFilter]
        public int SaveEditCustomer(int ID,  int Status)
        {
            //List<int> lstIdImg = new List<int>();
            //List<string> lstUrlImg = new List<string>();
            //if (LstUrlImg != "")
            //{
            //    lstUrlImg = LstUrlImg.Split(',').ToList();
            //}
            //if (LstID == "")
            //{
            //    lstIdImg = null;
            //}
            //if (LstID != "")
            //{
            //    lstIdImg = LstID.Split(',').Select(int.Parse).ToList();
            //}

            try
            {
                return cusBusiness.SaveEditCustomer( ID,Status);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchHistoryPoint(int Page, int cusID, string FromDate, string ToDate)
        {
            ViewBag.cusID = cusID;
            ViewBag.FromDateHis = FromDate;
            ViewBag.ToDateHis = ToDate;
            IPagedList<GetListHistoryMemberPointInputModel> list = cusBusiness.SearchHistoryPoint(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListHistoryPoint", list);
        }

        public PartialViewResult SearchHistoryPointR(int Page, int cusID, string FromDate, string ToDate)
        {
            ViewBag.cusID = cusID;
            ViewBag.FromDateHis = FromDate;
            ViewBag.ToDateHis = ToDate;
            IPagedList<GetListHistoryMemberPointInputModel> list = cusBusiness.SearchHistoryPointR(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListRequest", list);
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchReQuest(int Page, int cusID, string FromDate, string ToDate)
        {
            ViewBag.cusID = cusID;
            ViewBag.FromDateRQ = FromDate;
            ViewBag.ToDateRQ = ToDate;
            IPagedList<ListRequestOutputModel> list = cusBusiness.SearchReQuest(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListRequest", list);
        }

        [UserAuthenticationFilter]
        public int DeleteCustomer(int ID)
        {
            return cusBusiness.DeleteCustomer(ID);
        }

        [UserAuthenticationFilter]
        public PartialViewResult searchCustomerbank(int Page, int? cusID)
        {
            ViewBag.cusID = cusID;
            IPagedList<ListCustomerBank> list = cusBusiness.ListCustomerBank(cusID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListCustomerbank", list);
        }

        public ActionResult GoHome()
        {
            return Json(Url.Action("Index", "Customer"));
        }
        public FileResult ExportCustomer(string FromDate, string ToDate, string Phone, int? Status)
        {
            return File(cusBusiness.ExportExcelCustomer(FromDate, ToDate, Phone, Status).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Customer.xlsx");
        }
        // Reset mật khẩu
        public int RefreshCustomer(int id)
        {
            try
            {
                return cusBusiness.RefreshCustomer(id);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
    }
}