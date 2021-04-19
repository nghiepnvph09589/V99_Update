using APIProject.App_Start;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class StatisticRevenueController : BaseController
    {
        // GET: StatisticRevenue
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.Revenue = statisticBus.Revenue();
            return View();

        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, int? obj, string FromDate, string ToDate)
        {
            ViewBag.obj = obj;
            ViewBag.fd = FromDate;
            ViewBag.td = ToDate;         
            return PartialView("_List", statisticBus.Search(obj, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
        }
    }
}