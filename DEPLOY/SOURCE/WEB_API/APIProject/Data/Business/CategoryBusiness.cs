using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.IO;
using System.Web;
using OfficeOpenXml;
using QRCoder;
using System.Drawing;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;

namespace Data.Business
{
    public class CategoryBusiness
    {
        public CategoryBusiness(TichDiemTrieuDo context = null) : base()
        {
        }

        public List<ListCategoryOutputModel> loadGroupItems()
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            var query = from p in cnn.GroupItems
                        where p.IsActive == 1
                        orderby p.Name
                        select new ListCategoryOutputModel()
                        {
                            ID = p.ID,
                            ParentId = p.Status,
                            Name = p.Name,
                            CreateDate = p.CreatedDate.Value,
                            ImageUrl = p.ImageUrl,
                            Description = p.Description
                        };
            if (query != null && query.Count() > 0)
            {
                return query.ToList();
            }
            else
            {
                return new List<ListCategoryOutputModel>();
            }
        }
        public List<ListCategoryOutputModel> loadCategorys()
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();

            List<ListCategoryOutputModel> list = new List<ListCategoryOutputModel>();
            var query = (from c in cnn.GroupItems
                         orderby c.Name
                         where c.IsActive == 1
                         select new ListCategoryOutputModel()
                         {
                             ID = c.ID,
                             Name = c.Name,
                             ParentId = c.ParentID,
                             CreateDate = c.CreatedDate.Value,
                             ImageUrl = c.ImageUrl
                         }).ToList();
            if (query != null && query.Count() > 0)
            {

                foreach (var value in query)
                {
                    if (value.ParentId != null)
                    {
                        // lấy thông tin của danh mục cha 
                        var parent = query.Where(u => u.ID == value.ParentId);
                        if (parent != null && parent.Count() > 0)
                        {
                            var p = parent.SingleOrDefault();
                            value.ParentName = p.Name;
                        }
                        else
                        {
                            value.ParentName = "";
                        }
                    }
                    else
                    {
                        value.ParentName = "";
                    }

                    list.Add(value);
                }
                return list;
            }
            else
            {
                return new List<ListCategoryOutputModel>();
            }
        }

        public List<ListCategoryOutputModel> Search(int Page, string CateName, string FromDate, string ToDate, int? Category)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {

                DateTime? startDate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);

                List<ListCategoryOutputModel> list = new List<ListCategoryOutputModel>();
                var query = (from c in cnn.GroupItems
                             orderby c.CreatedDate descending
                             where c.IsActive == 1 
                            && (startDate.HasValue ? c.CreatedDate >= startDate.Value : true)
                             select new ListCategoryOutputModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 ParentId = c.Status,
                                 CreateDate = c.CreatedDate.Value,
                                 ImageUrl = c.ImageUrl
                             }).ToList();
                if( CateName!=null && CateName != "")
                {
                    query = query.Where(u => Util.Converts(u.Name.ToLower()).Contains(Util.Converts(CateName.ToLower()))).ToList();
                }
                if (endDate.HasValue)
                {
                    // query = query.Where(u => u.CreateDate.Day >= startDate.Value.Day && u.CreateDate.Month >= startDate.Value.Month && u.CreateDate.Year >= startDate.Value.Year).ToList();
                    query = query.Where(u => u.CreateDate.Day <= endDate.Value.Day && u.CreateDate.Month <= endDate.Value.Month && u.CreateDate.Year <= endDate.Value.Year).ToList();
                }
                if (Category != null && Category == 0)
                {
                    //query = query.Where(u => u.ParentId == 0).ToList();
                    foreach (var value in query)
                    {
                        if (value.ParentId == 0)
                        {
                            value.ParentName = "Không hoạt động";
                        }

                        list.Add(value);
                    }

                    return query.Where(u => u.ParentId == 0).ToList();
                }
                if (Category != null && Category == 1)
                {
                    // query = query.Where(u => u.ParentId != null).ToList();
                    foreach (var value in query)
                    {
                        if (value.ParentId == 1)
                        {
                            value.ParentName = "Hoạt động";
                        }

                        list.Add(value);
                    }

                    return query.Where(u => u.ParentId == 1).ToList();
                }
                if (query != null && query.Count() > 0)
                {
                    foreach (var value in query)
                    {
                        if (value.ParentId == 1)
                        {
                            // lấy thông tin của danh mục cha 
                            value.ParentName = "Hoạt động";
                        }
                        else
                        {
                            value.ParentName = "Không hoạt động";
                        }

                        list.Add(value);
                    }

                    return list;
                }

                else
                {
                    return new List<ListCategoryOutputModel>();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListCategoryOutputModel>();
            }

        }

        public int createCategory(string Name)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {
                
                GroupItem obj = new GroupItem();
                var check = cnn.GroupItems.Where(x => x.Name.Trim()==Name.Trim()&&x.IsActive==SystemParam.ACTIVE).ToList();
                if (check.Count() > 0)
                    return SystemParam.EXISTING;
                //if (ParentID == 0)
                //{
                //    obj.ParentID = null;
                //}
                //else
                //{
                //    obj.ParentID = ParentID;
                //}
                // obj.ParentID = ParentID;
                obj.ParentID = null;
                obj.Name = Name;
                //obj.ImageUrl = Url;
                obj.CreatedDate = DateTime.Now;
                obj.IsActive = SystemParam.ACTIVE;
                obj.Type = 1;
                //obj.Description = Description;
                obj.Status = 1;
                obj.OrderIndex = 1;
                cnn.GroupItems.Add(obj);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public ListCategoryOutputModel ModalEditCategory(int ID)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {
                ListCategoryOutputModel categoryDetail = new ListCategoryOutputModel();
                var query = (from c in cnn.GroupItems
                             where c.IsActive == SystemParam.ACTIVE
                             && c.ID == ID
                             select new ListCategoryOutputModel
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 ParentId = c.ParentID,
                                 ImageUrl = c.ImageUrl,
                                 Description = c.Description,
                                 Status = c.Status.Value
                             }).FirstOrDefault();
                if (query != null && query.ID > 0)
                {
                    return categoryDetail = query;
                }
                else
                {
                    return categoryDetail;
                }
                
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListCategoryOutputModel();
            }
        }
        public int EditCategory(ListCategoryOutputModel data)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {
                var check = cnn.GroupItems.Where(x => x.Name.Trim() == data.Name.Trim() && x.IsActive==SystemParam.ACTIVE).ToList();
                if (check.Count() > 0)
                    return SystemParam.EXISTING;
                var obj = cnn.GroupItems.Find(data.ID);
                if (data.ParentId == 0)
                {
                    obj.Status = 0;
                }
                else
                {
                    obj.Status = 1;
                }
                obj.Name = data.Name;
                //obj.ImageUrl = data.ImageUrl;
                //obj.Description = data.Description;

                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteCategory(int ID)
        {
            TichDiemTrieuDo cnn = new TichDiemTrieuDo();
            try
            {

                GroupItem category = cnn.GroupItems.Find(ID);
                List<int> item = (from i in cnn.Items
                                  where i.IsActive.Equals(SystemParam.ACTIVE)
                                  select i.GroupItemID).ToList();
                //List<int> check = (from gi in cnn.GroupItems
                //                   where gi.IsActive.Value.Equals(SystemParam.ACTIVE) && (gi.ParentID.HasValue ? gi.ParentID.Value.Equals(ID) : false)
                //                   select gi.ParentID.Value).ToList();

                //if (check.Count > 0)
                //{
                //    return 3;
                //}
                if (item.Contains(category.ID))
                {
                    return 4;
                }
                else
                {
                    if (category != null && category.IsActive.Equals(SystemParam.ACTIVE))
                    {
                        category.IsActive = SystemParam.ACTIVE_FALSE;
                        cnn.SaveChanges();
                        return SystemParam.RETURN_TRUE;
                    }
                    return SystemParam.RETURN_FALSE;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
    }
}
