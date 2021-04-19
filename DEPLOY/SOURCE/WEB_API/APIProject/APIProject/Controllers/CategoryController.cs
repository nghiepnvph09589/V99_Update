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
    public class CategoryController : BaseController
    {
        // GET: Category
        [UserAuthenticationFilter]

        public ActionResult Index()
        {
            ViewBag.listItems = categoryBusiness.loadGroupItems();
            ViewBag.LoadListCategory = categoryBusiness.loadCategorys();
            return View();
        }

        public PartialViewResult Search(int Page, string CateName, string FromDate, string ToDate, int? Category)
        {
            try
            {
                ViewBag.CateName = CateName;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.CateID = Category;
                DateTime? StartDate = Util.ConvertDate(FromDate);
                DateTime? EndDate = Util.ConvertDate(ToDate);
                /* IPagedList<GetListGroupItemModels> list = categoryBusiness.Search( FromDate, CateName).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);*/
                List<ListCategoryOutputModel> listCategory = categoryBusiness.Search(Page, CateName, FromDate, ToDate, Category);
                return PartialView("_TableCategory", listCategory.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableCategory", new List<ListCategoryOutputModel>().ToPagedList(1, 1));
            }
        }

        public int CreateCategory(string Name)
        {
            try
            {
                return categoryBusiness.createCategory(Name);
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }

        public PartialViewResult ModalEditCategory(int ID)
        {
            try
            {
                ViewBag.listItems = categoryBusiness.loadGroupItems();
                ListCategoryOutputModel CategoryDetail = categoryBusiness.ModalEditCategory(ID);
                return PartialView("_EditCategory", CategoryDetail);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_EditCategory", new ListCategoryOutputModel());
            }
        }
        public int EditCategory(ListCategoryOutputModel data)
        {
            try
            {
                return categoryBusiness.EditCategory(data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteCategory(int ID)
        {
            try
            {
                return categoryBusiness.DeleteCategory(ID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
    }
}