using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class ItemBusiness : GenericBusiness
    {
        public ItemBusiness(TichDiemTrieuDo context = null) : base()
        {

        }

        public List<ListItemOutputModel> Search(int Page, DateTime? fromDate, DateTime? toDate, string itemName, int? Status, string category, int? StockStatus)
        {
            try
            {
                if (category == "1")
                {
                    category = null;
                }
                List<ListItemOutputModel> query = new List<ListItemOutputModel>();
                var lstItem = from i in cnn.Items
                              where i.IsActive.Equals(SystemParam.ACTIVE)
                              //&& (i.Status.Equals(Status))
                              && (fromDate.HasValue ? i.CreateDate >= fromDate.Value : true)
                              && (toDate.HasValue ? i.CreateDate <= toDate.Value : true)
                              orderby i.ID descending
                              select new ListItemOutputModel
                              {
                                  StockStatus= (int)i.StockStatus,
                                  ID = i.ID,
                                  ItemCode = i.Code,
                                  ItemName = i.Name,
                                  ImgUrl = i.ImageUrl,
                                  Technical = i.Technical,
                                  Description = i.Description,
                                  Category = i.GroupItem != null ? i.GroupItem.Name : "Chưa được cập nhật",
                                  ItemStatus = i.Status,
                                  Price = i.Price,
                                  PriceVip = i.PriceVIP,
                                  CreateDate = i.CreateDate
                              };
                if (Status != null)
                {
                    lstItem = lstItem.Where(x => x.ItemStatus.Equals(Status.Value));
                }
                if (StockStatus != null)
                {
                    lstItem = lstItem.Where(x => x.StockStatus.Equals(StockStatus.Value));
                }
                //if(category != null)
                //{
                //    lstItem = lstItem.Where(u => u.Category.Equals(category.Value));
                //}

                if (lstItem != null && lstItem.Count() > 0 || category == null)
                {
                    query = lstItem.ToList();
                    if (!String.IsNullOrEmpty(itemName))
                        query = query.Where(u => Util.Converts(u.ItemName.ToLower()).Contains(Util.Converts(itemName.ToLower()))).ToList();
                    if (category != null)
                    {
                        query = query.Where(u => u.Category.Contains(category)).ToList();
                    }
                }
                return query;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListItemOutputModel>();
            }
        }

        public List<CategoryModel> getListCategory()
        {
            try
            {
                List<CategoryModel> listCategory = new List<CategoryModel>();
                var query = from c in cnn.GroupItems
//<<<<<<< HEAD
//                            where c.IsActive == SystemParam.ACTIVE && c.ID != null
//=======
                            where c.IsActive == SystemParam.ACTIVE && c.Status == SystemParam.STATUS_ACTIVE
//>>>>>>> 8f7b16ce695273159a7e3a17633194a7edf6856e
                            select new CategoryModel
                            {
                                CategoryID = c.ID,
                                Name = c.Name,
                                //ParentID = c.ParentID
                            };

                if (query != null && query.Count() > 0)
                {
                    listCategory = query.ToList();
                    return listCategory;
                }
                else
                    return listCategory;
            }
            catch (Exception)
            {
                return new List<CategoryModel>();
            }

        }


        //public List<ItemOutputModel> GetListItem()
        //{
        //    List<ItemOutputModel> data = new List<ItemOutputModel>();
        //    var query = from i in cnn.Items
        //                where i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE)
        //                orderby i.ID descending
        //                select new ItemOutputModel
        //                {
        //                    ItemID = i.ID,
        //                    ImageUrl = i.ImageUrl,
        //                    Description = i.Description,
        //                    ItemName = i.Name,
        //                    NewPrice = i.Price
        //                };
        //    if (query != null && query.Count() > 0)
        //    {
        //        foreach (var output in query)
        //        {
        //            if (!output.ImageUrl.ToLower().Contains("http"))
        //                output.ImageUrl = "http://" + HttpContext.Current.Request.Url.Authority + "/" + output.ImageUrl;
        //            data.Add(output);
        //        }
        //    }
        //    return data;
        //}


        public Boolean CheckExistingItem(string itemCode)
        {
            try
            {
                var item = cnn.Items.Where(u => u.Code.Equals(itemCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (item != null && item.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public int CreateItem(int CategoryID,int? Special, string Code, string Name, int Status, int StockStatus, string Price,string PriceVip, int Warranty, string ImageUrl, string Technical, string Note)
        {
            //kiểm tra mã sản phẩm
            var q = cnn.Items.Where(x => x.Code.Equals(Code) && x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            if (q.Count > 0)
                return SystemParam.EXISTING;
            try
            {
                Item item = new Item();
                item.GroupItemID = CategoryID;
                item.Code = Code;
                item.Name = Name;
                item.Status = Status;
                item.StockStatus = StockStatus;
                item.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                item.PriceVIP = Convert.ToInt32((Price).ToString().Replace(",", ""));
                item.Warranty = Warranty;
                item.ImageUrl = ImageUrl;
                item.Special = Special.Value;
                item.Technical = Technical;
                item.Description = Note;
                item.CreateDate = DateTime.Today;
                item.IsActive = SystemParam.ACTIVE;
                cnn.Items.Add(item);
                cnn.SaveChanges();


                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public CreateItemInputModel LoadItem(int ID)
        {
            try
            {
                var obj = cnn.Items.Find(ID);
                CreateItemInputModel Item = new CreateItemInputModel
                {
                    ID = obj.ID,
                    Code = obj.Code,
                    Name = obj.Name,
                    CategoryID = obj.GroupItemID,
                    Status = obj.Status,
                    Price = obj.Price.ToString(),
                    PriceVip = obj.PriceVIP.ToString(),
                    ImageUrl = obj.ImageUrl,
                    Note = obj.Description,
                    StockStatus = (int)obj.StockStatus,
                    Technical = obj.Technical,
                    Warranty = obj.Warranty
                };
                return Item;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new CreateItemInputModel();
            }
        }

        public int DeleteItem(int ID)
        {
            try
            {
                List<int> check = (from b in cnn.Batches
                                   where b.IsActive.Equals(SystemParam.ACTIVE)
                                   select b.ItemID.Value).ToList();

                var obj = cnn.Items.Find(ID);
                if (check.Contains(obj.ID))
                {
                    return 3;
                }
                else
                {
                    obj.IsActive = SystemParam.ACTIVE_FALSE;
                    cnn.SaveChanges();
                    return SystemParam.RETURN_TRUE;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        //Lưu lại cập nhật sản phẩm
        public int SaveEditItem(int ID,int? Special, string Code, string Name, int? Status, int? StockStatus, int? CategoryID, string ImageUrl, int? Warranty, string Technical, string Note, string Price,string PriceVip)
        {
            try
            {
                Item item = cnn.Items.Find(ID);
                item.Code = Code;
                item.Name = Name.Trim();
                item.Special = Special.Value;
                item.GroupItemID = CategoryID.Value;
                item.Technical = Technical;
                item.Warranty = Warranty.Value;
                item.Status = Status.Value;
                item.StockStatus = StockStatus.Value;
                item.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                item.PriceVIP = Convert.ToInt32((PriceVip).ToString().Replace(",", ""));
                item.ImageUrl = ImageUrl;
                item.Description = Note.Trim();
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public string countItem()
        {
            var listItem = cnn.Items.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            int activeItem = listItem.Where(u => u.Status.Equals(1)).Count();
            int countItem = listItem.Count();
            return countItem.ToString();
        }
    }
}