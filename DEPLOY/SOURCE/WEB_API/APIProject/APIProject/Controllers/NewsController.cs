using APIProject.App_Start;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class NewsController : BaseController
    {
        // GET: News


        [UserAuthenticationFilter]
        public ActionResult CreateNews()
        {
            ViewBag.listcaterogy = newsBusiness.GetListCategoryNews();
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult UpdateNews(int id)
        {
            return View(newsBusiness.GetNewsDetail(id));
        }


        [ValidateInput(false)]
        [HttpPost]
        [UserAuthenticationFilter]
        public int CreateNewsDekko(string Content, string Title, int Type,int TypeSend, string UrlImage, int Status, int Display, int SendNotify)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return newsBusiness.CreateNewsDekko(Content, Title, Type, TypeSend, UrlImage, Status,Display,SendNotify, userLogin.UserID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        /*   public int CreateNewsDekko(string Content, string Title, int Type,int TypeSend, string UrlImage, int Status, int Display, int SendNotify)
           {
               try
               {
                   UserDetailOutputModel userLogin = UserLogins;
                   return newsBusiness.CreateNewsDekko(Content, Title, Type, TypeSend, UrlImage, Status, Display, SendNotify, userLogin.UserID);
               }
               catch
               {
                   return SystemParam.ERROR;
               }
           }*/

        [ValidateInput(false)]
        [HttpPost]
        [UserAuthenticationFilter]
        public int UpdateNewsDekko(int ID, string Content, string Title, int Type, string UrlImage, int Status,int SendNotify)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return newsBusiness.UpdateNewsDekko(ID, Content, Title, Type, UrlImage, Status, SendNotify);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string Title, int? CreateUserID, int? Type, int? Status, string FromDate, string ToDate)
        {
            try
            {
                ViewBag.CreateUserIDNews = CreateUserID;
                ViewBag.TitleNews = Title;
                ViewBag.PageCurrentNews = Page;
                ViewBag.StatusNews = Status;
                ViewBag.TypeNews = Type;
                ViewBag.FromDateNews = FromDate;
                ViewBag.ToDateNews = ToDate;
                List<ListNewsWebOutputModel> listNews = newsBusiness.Search(Page, Title, CreateUserID, Type, Status, FromDate, ToDate);
                return PartialView("_TableNews", listNews.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch
            {
                return PartialView("_TableNews", new List<ListNewsWebOutputModel>().ToPagedList(1, 1));
            }
        }


        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.listcaterogy = newsBusiness.GetListCategoryNews();
            List<ListUserWebOutputModel> listAuthor = newsBusiness.GetListAuthor();
            ViewBag.ListAuthor = listAuthor;
            return View();
        }


        [UserAuthenticationFilter]
        public int DeleteNews(int ID)
        {
            try
            {
                return newsBusiness.DeleteNews(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult GetNewsDetail(int ID)
        {
            try
            {
                ListNewsWebOutputModel newsDetail = newsBusiness.GetNewsDetail(ID);
                return PartialView("_UpdateNews", newsDetail);
            }
            catch
            {
                return PartialView("_UpdateNews", new ListNewsWebOutputModel());
            }
        }
        
    }
}