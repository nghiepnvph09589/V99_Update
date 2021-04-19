using Data.Business;
using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class BaseAPIController:ApiController
    {
        
        protected TichDiemTrieuDo Context;
        public LoginBusiness lgBus;
        public NewsBusiness newsBus;
        public NotifyBusiness notiBus;
        public GiftBusiness giftBus;
        public RequestBusiness rqBus;
        public CustomerBusiness cusBus;
        public PointBusiness pBus;
        public MessageBusiness mBus;
        public RequestAPIBusiness apiBus;
        public StatisticBusiness statisticBus;
        public ShopBusiness shopBus;
        public ItemBusiness itemBus;
        public OrderBusiness orderBus;
        public PackageBusiness packageBusiness;
        public HistoryPointMemberBusiness historyPoint;
        public TaskBusiness taskBus;


        public BaseAPIController() : base()
        {
            lgBus = new LoginBusiness(this.GetContext());
            newsBus = new NewsBusiness(this.GetContext());
            historyPoint = new HistoryPointMemberBusiness(this.GetContext());
            orderBus = new OrderBusiness(this.GetContext());
            packageBusiness = new PackageBusiness(this.GetContext());
            notiBus = new NotifyBusiness(this.GetContext());
            taskBus = new TaskBusiness(this.GetContext());
        }

        /// <summary>
        /// Create new context if null
        /// </summary>
        public TichDiemTrieuDo GetContext()
        {
            if (Context == null)
            {
                Context = new TichDiemTrieuDo();
            }
            return Context;
        }
    }
}