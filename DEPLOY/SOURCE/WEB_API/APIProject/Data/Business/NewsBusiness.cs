using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Business
{
    public class NewsBusiness : GenericBusiness
    {
        public NewsBusiness(TichDiemTrieuDo context = null) : base()
        {
        }


        public NewsAppOutputModel getNewsDetailApp(int newsID)
        {
            NewsAppOutputModel query = new NewsAppOutputModel();
            var news = (from n in cnn.News
                        where n.IsActive.Equals(SystemParam.ACTIVE) && n.ID.Equals(newsID)
                        orderby n.DisplayOrder
                        orderby n.ID
                        select new NewsAppOutputModel
                        {
                            NewsID = n.ID,
                            Content = n.Content,
                            CreateDate = n.CreateDate,
                            Title = n.Title,
                            Type = n.Type,
                            UrlImage = n.UrlImage,
                            Description = n.Description
                        }).FirstOrDefault();
            if (news != null && news.NewsID > 0)
            {
                query = news;
            }
            return query;
        }

        public List<NewsAppOutputModel> GetListNews(int type)
        {
            try
            {
                List<NewsAppOutputModel> query = new List<NewsAppOutputModel>();
                var ListNews = (from n in cnn.News
                               where n.Type.Equals(type) && n.IsActive.Equals(SystemParam.ACTIVE) && n.Status.Equals(SystemParam.ACTIVE)
                               orderby n.DisplayOrder, n.ID descending
                               select new NewsAppOutputModel
                               {
                                   NewsID = n.ID,
                                   Content = n.Content,
                                   CreateDate = n.CreateDate,
                                   Title = n.Title,
                                   Type = n.Type,
                                   UrlImage = n.UrlImage,
                                   Description = n.Description
                               }).ToList();
                if (ListNews != null && ListNews.Count() > 0)
                {
                    return query = ListNews; 
                }
                return query;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return new List<NewsAppOutputModel>();
            }
           
        }



        // tạo bài viết tin tức
        public int CreateNewsDekko(string Content, string Title, int Type, int TypeSend, string UrlImage, int Status, int Display, int SendNotify, int CreateUserID)
        {
            try
            {
                News news = new News();
                news.CreateUserID = CreateUserID;
                news.CategoryID = GetCategoryByName(Type).ID;
                news.Title = Title;
                news.Description = "";
                news.Content = Content;
                news.Type = Type;
                news.DisplayOrder = Display;
                /*news.TypeSend = SystemParam.TYPE_SEND_ALL;*/
                /* news.TypeSend = GetTypeSend(TypeSend).ID;*/
                news.TypeSend = TypeSend;
                news.Status = Status;
                news.UrlImage = UrlImage;
                news.CreateDate = DateTime.Now;
                news.IsActive = SystemParam.ACTIVE;
                cnn.News.Add(news);
                cnn.SaveChanges();
                if (Status == SystemParam.STATUS_NEWS_ACTIVE && SendNotify == SystemParam.SEND_NOTIFY)
                {
                    int newsID = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.ID).FirstOrDefault().ID;
                    sendNotifyNews(Title, "", newsID, TypeSend);
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        NotifyBusiness notifyBusiness = new NotifyBusiness();
        PackageBusiness packageBusiness = new PackageBusiness();

        public void sendNotifyNews(string title, string Description, int newsID, int TypeSend)
        {
            var news = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(newsID)).FirstOrDefault();
            NotifyDataModel notifyData = new NotifyDataModel();
            notifyData.id = newsID;
            notifyData.type = SystemParam.NOTIFY_NAVIGATE_NEWS;
            List<Customer> listCustomer;
            List<List<Customer>> listCustomers = new List<List<Customer>>();
            if (TypeSend == SystemParam.TYPE_SEND_CUSTOMER)
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Role.Equals(SystemParam.ROLE_CUSTOMER)).OrderBy(u => u.ID).ToList();
            }
            else if (TypeSend == SystemParam.TYPE_SEND_AGENCY)
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Role.Equals(SystemParam.ROLE_AGENT)).OrderBy(u => u.ID).ToList();
            }
            else
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderBy(u => u.ID).ToList();
            }
            List<Customer> listCusNoti = listCustomer.Where(u => !String.IsNullOrEmpty(u.DeviceID)).ToList();
            int max = SystemParam.MAX_PEOPLE;
            List<Customer> list = new List<Customer>();
            for (int i = 0; i < listCusNoti.Count(); i++)
            {
                list.Add(listCusNoti[i]);
                if ((i % max == 0 && i > 0) || i == listCusNoti.Count() - 1)
                {
                    listCustomers.Add(list);
                    list = new List<Customer>();
                }
            }
            foreach (var customer in listCustomer)
            {
                notifyBusiness.CreateNoties(customer.ID, SystemParam.NOTIFY_NAVIGATE_NEWS, news.ID, Description, title, cnn);
            }
            cnn.SaveChanges();
            foreach (List<Customer> lists in listCustomers)
            {
                List<string> listDeviceId = new List<string>();
                foreach (var customer in lists)
                {
                    if (customer.DeviceID.Length > 10)
                    {
                        listDeviceId.Add(customer.DeviceID);
                    }
                }
                string value = packageBusiness.StartPushNoti(notifyData, listDeviceId, title, Description);
                 packageBusiness.PushOneSignals(value);
                //apiBus.SendNotification(listDeviceId, new NotifyTitle(title), new NotifyContent(Description), new object());
            }
        }
        public News GetTypeSend(int TypeSend)
        {
            switch (TypeSend)
            {
                case SystemParam.TYPE_SEND_ALL:
                    TypeSend = SystemParam.TYPE_SEND_ALL;

                    break;
                case SystemParam.TYPE_SEND_CUSTOMER:
                    TypeSend = SystemParam.TYPE_SEND_CUSTOMER;
                    break;
                case SystemParam.TYPE_SEND_AGENCY:
                    TypeSend = SystemParam.TYPE_SEND_AGENCY;
                    break;
            }
            var typeSend = cnn.News.Where(u => u.TypeSend == TypeSend).FirstOrDefault();
            return typeSend;
        }


        public CategoryNew GetCategoryByName(int Type)
        {
            try
            {
                string categoryName = "";
                switch (Type)
                {
                    case SystemParam.NEWS_TYPE_EVENT:
                        categoryName = SystemParam.NEWS_TYPE_EVENT_STRING;
                        break;
                    case SystemParam.NEWS_TYPE_BANNER:
                        categoryName = SystemParam.NEWS_TYPE_BANNER_STRING;
                        break;
                    case SystemParam.NEWS_TYPE_NEWS:
                        categoryName = SystemParam.NEWS_TYPE_NEWS_STRING;
                        break;
                    case SystemParam.NEWS_TYPE_ADS:
                        categoryName = SystemParam.NEWS_TYPE_ADS_STRING;
                        break;
                    default:
                        break;
                }
                var categoryNews = cnn.CategoryNews.Where(u => u.Name.Equals(categoryName)).FirstOrDefault();
                return categoryNews;
            }
            catch
            {
                return new CategoryNew();
            }
        }

        //chỉnh sửa bài viết
        public int UpdateNewsDekko(int ID, string Content, string Title, int Type, string UrlImage, int Status,int SendNotify)
        {
            try
            {
                News news = cnn.News.Find(ID);
                news.CategoryID = GetCategoryByName(Type).ID;
                news.Title = Title;
                news.Description = "";
                news.Content = Content;
                news.Type = Type;
                news.DisplayOrder = 1;
                if (Status == SystemParam.UPDATE_NEWS_POST)
                {
                    news.Status = SystemParam.STATUS_NEWS_ACTIVE;
                }
                news.UrlImage = UrlImage;
                cnn.SaveChanges();
                if (news.Status == SystemParam.STATUS_NEWS_ACTIVE && SendNotify == SystemParam.SEND_NOTIFY)
                {
                    int newsID = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.ID).FirstOrDefault().ID;
                    sendNotifyNews(Title, "", newsID, 1);
                }
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        //xóa bài viết
        public int DeleteNews(int ID)
        {
            try
            {
                News news = cnn.News.Find(ID);
                news.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        // tìm kiếm
        public List<ListNewsWebOutputModel> Search(int Page, string Title, int? CreateUserID, int? Type, int? Status, string FromDate, string ToDate)
        {
            try
            {
                List<ListNewsWebOutputModel> listNews = new List<ListNewsWebOutputModel>();
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);

                var query = (from n in cnn.News
                            where n.IsActive.Equals(SystemParam.ACTIVE)
                            //&& (!String.IsNullOrEmpty(Title) ? n.Title.Contains(Title) : true)
                            && (CreateUserID.HasValue ? n.CreateUserID == CreateUserID.Value : true)
                            && (Type.HasValue ? n.Type == Type.Value : true)
                            && (Status.HasValue ? n.Status == Status.Value : true)
                            && (startdate.HasValue ? n.CreateDate >= startdate.Value : true)
                            && (endDate.HasValue ? n.CreateDate <= endDate.Value : true)
                            orderby n.ID descending
                            select new ListNewsWebOutputModel
                            {
                                ID = n.ID,
                                CategoryName = n.CategoryNew.Name,
                                Title = n.Title,
                                Status = n.Status,
                                Type = n.Type,
                                Display = n.DisplayOrder.Value,
                                CreateUserID = n.User.UserID,
                                CreateUserName = n.User.UserName,
                                CreateDate = n.CreateDate
                            }).ToList();
                if(Title!= null&& Title != "")
                {
                    query = query.Where(x => Util.Converts(x.Title.ToLower()).Contains(Util.Converts(Title.ToLower()))).ToList();
                }
                if (query != null && query.Count() > 0)
                {
                    listNews = query.ToList();
                    if (!String.IsNullOrEmpty(Title))
                        listNews = listNews.Where(u => Util.Converts(u.Title.ToLower()).Contains(Util.Converts(Title.ToLower())) || Util.Converts(u.CreateUserName.ToLower()).Contains(Util.Converts(Title.ToLower()))).ToList();
                }
                return listNews;
            }
            catch
            {
                return new List<ListNewsWebOutputModel>();
            }
        }


        //lấy danh sách tác giả
        public List<ListUserWebOutputModel> GetListAuthor()
        {
            try
            {
                var ListUser = from news in cnn.News
                               join user in cnn.Users on news.CreateUserID equals user.UserID
                               where news.IsActive == SystemParam.ACTIVE
                               select new ListUserWebOutputModel()
                               {
                                   ID = user.UserID,
                                   Name = user.UserName,
                                   CreateDate = user.CreateDate
                               };
                if (ListUser != null && ListUser.Count() > 0)
                {
                    ListUser = ListUser.Distinct();
                    return ListUser.ToList();
                }
                else
                {
                    return new List<ListUserWebOutputModel>();
                }
            }
            catch
            {
                return new List<ListUserWebOutputModel>();
            }
        }

        //Test----------------------------------------------------------------
        public List<DropdownOuputModel> GetListCategoryNews()
        {
            try
            {
                var ListName = from news in cnn.CategoryNews
                               where news.IsActive == SystemParam.ACTIVE && news.Type <= SystemParam.NEWS_TYPE_BANNER
                               select new DropdownOuputModel()
                               {
                                   type=news.Type,
                                   value=news.Name
                               };
                return ListName.ToList();
            }
            catch
            {
                return new List<DropdownOuputModel>();
            }
        }

        //lấy thông tin chi tiết 1 bài viết
        public ListNewsWebOutputModel GetNewsDetail(int ID)
        {
            try
            {
                ListNewsWebOutputModel newsDetail = new ListNewsWebOutputModel();
                var query = (from n in cnn.News
                             join u in cnn.Users on n.CreateUserID equals u.UserID
                             where n.IsActive.Equals(SystemParam.ACTIVE) && n.ID.Equals(ID)
                             select new ListNewsWebOutputModel
                             {
                                 ID = n.ID,
                                 Type = n.Type,
                                 CategoryID = n.CategoryID,
                                 Display = n.DisplayOrder.Value,
                                 TypeSend = n.TypeSend,
                                 Status = n.Status,
                                 Title = n.Title,
                                 Depcription = n.Description,
                                 Content = n.Content,
                                 UrlImage = n.UrlImage,
                                 CreateUserID = n.CreateUserID,
                                 CreateUserName = u.UserName
                             }).FirstOrDefault();
                if (query != null && query.ID > 0)
                {
                    return newsDetail = query;
                }
                return newsDetail;
            }
            catch
            {
                return new ListNewsWebOutputModel();
            }
        }



    }
}
