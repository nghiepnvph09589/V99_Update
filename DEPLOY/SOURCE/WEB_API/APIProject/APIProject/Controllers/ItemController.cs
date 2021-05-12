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

namespace APIProject.Controllers
{
    public class ItemController : BaseController
    {
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.listCategory = itemBusiness.getListCategory();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string fromDate, string toDate, string itemName, int? Status, string category,int? StockStatus)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.ItemName = itemName;
                ViewBag.Status = Status;
                ViewBag.Category = category;
                ViewBag.StockStatus = StockStatus;
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                List<ListItemOutputModel> lstProduct = itemBusiness.Search(Page, startDate, endDate, itemName, Status, category, StockStatus);
                return PartialView("_TableItem", lstProduct.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("TableItem", new List<ListItemOutputModel>().ToPagedList(1, 1));
            }
        }
        [UserAuthenticationFilter]
        public ActionResult AddItem()
        {
            ViewBag.listCategory = itemBusiness.getListCategory();
            return View();/*PartialView("AddItem");*/
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int CreateItem(int? CategoryID,int? Special, string Code, string Name, int? Status, int? StockStatus, string Price,string PriceVip, int? Warranty, string ImageUrl, string Technical, string Note)
        {
            try
            {
                return itemBusiness.CreateItem(CategoryID.Value,Special, Code, Name, Status.Value, StockStatus.Value, Price, PriceVip, Warranty.Value, ImageUrl, Technical, Note);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult LoadItem(int ID)
        {
            try
            {
                ViewBag.listCategory = itemBusiness.getListCategory();
                var Item = itemBusiness.LoadItem(ID);
                return PartialView("_EditItem", Item);
            }
            catch
            {
                return PartialView("_EditItem", new CreateItemInputModel());
            }
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        // Lưu lại cập nhật sản phẩm
        public int SaveEditItem(int ID,int? Special, string Code, string Name, int? Status, int? StockStatus, int? CategoryID, string ImageUrl, int? Warranty, string Technical, string Note, string Price,string PriceVip)
        {
            try
            {
                return itemBusiness.SaveEditItem(ID,Special, Code, Name, Status.Value,StockStatus.Value, CategoryID.Value, ImageUrl, Warranty.Value, Technical, Note, Price,PriceVip);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteItem(int ID)
        {
            try
            {
                return itemBusiness.DeleteItem(ID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
    }
}