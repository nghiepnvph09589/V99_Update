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
using Microsoft.Office.Interop.Excel;
using Spire.Barcode;
using Rectangle = System.Drawing.Rectangle;

namespace Data.Business
{
    public class BatchBusiness : GenericBusiness
    {
        public BatchBusiness(TichDiemTrieuDo context = null) : base()
        {

        }
        //DEKKOEntities cnn = Util.Connect;
        //check trùng tên lô hàng
        public Boolean CheckDuplicateBatchCode(string batchCode)
        {
            try
            {
                var batch = cnn.Batches.Where(u => u.BatchCode.Equals(batchCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (batch != null && batch.Count() > 0)
                {
                    return SystemParam.BOOLEAN_TRUE;
                }
                return SystemParam.BOOLEAN_FALSE;
            }
            catch
            {
                return SystemParam.BOOLEAN_FALSE;
            }
        }

        // tạo lô hàng mới
        public int CreateBatch(CreateBatchInputModel input, int CreateUserID)
        {
            try
            {
                if (CheckDuplicateBatchCode(input.BatchCode))
                {
                    return SystemParam.DUPLICATE_NAME;
                }
                if (input.Note == null)
                {
                    input.Note = "";
                }
                if(input.ProductName == null)
                {
                    input.ProductName = "Dữ liệu chưa được cập nhật";
                }
                Batch batch = new Batch();
                batch.CreateUserID = CreateUserID;
                batch.BatchName = input.BatchName;
                batch.ItemID = input.ProductID;
                batch.BatchCode = input.BatchCode;
                batch.Point = Convert.ToInt32((input.Point).ToString().Replace(",", ""));
                batch.QTY = Convert.ToInt32((input.QTY).ToString().Replace(",", ""));
                batch.Price = Convert.ToInt32((input.Price).ToString().Replace(",", ""));
                batch.Note = input.Note;
                batch.ProductName = input.ProductName;
                batch.IsActive = SystemParam.ACTIVE;
                batch.CreateDate = DateTime.Today;
                batch.Products = CreateProductOfBatch(batch.BatchCode, batch.QTY);
                cnn.Batches.Add(batch);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        //đồng thời tạo ra list sản phẩm của lô hàng vừa được tạo mới
        public List<Product> CreateProductOfBatch(string BatchCode, int QTY)
        {
            try
            {
                List<Product> list = new List<Product>();
                int num0 = QTY.ToString().Length;
                for (int i = 1; i <= QTY; i++)
                {
                    string PC = i.ToString();
                    while (PC.Length < num0)
                    {
                        PC = "0" + PC;
                    }
                    Product product = new Product();
                    product.ProductCode = BatchCode + PC;
                    product.IsActive = SystemParam.ACTIVE;
                    product.Status = SystemParam.STATUS_PRODUCT_NO_ACTIVE;
                    product.CreateDate = DateTime.Today;
                    list.Add(product);
                }
                return list;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<Product>();
            }
        }

        // xóa lô hàng
        public int DeleteBatch(int ID)
        {
            try
            {
                var usedProduct = cnn.Products.Where(u => u.BatchID.Equals(ID) && u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(SystemParam.STATUS_PRODUCT_ACTIVE));
                if (usedProduct != null && usedProduct.Count() > 0)
                {
                    return SystemParam.CAN_NOT_DELETE;
                }

                Batch batch = cnn.Batches.Find(ID);
                batch.IsActive = SystemParam.NO_ACTIVE_DELETE;

                var listProduct = cnn.Products.Where(u => u.BatchID.Equals(ID) && u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals(SystemParam.STATUS_PRODUCT_NO_ACTIVE));
                foreach (var product in listProduct)
                {
                    product.IsActive = SystemParam.NO_ACTIVE_DELETE;
                }
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }


        // tìm kiếm
        public IPagedList<ListBatchOutputModel> Search(int Page, string BatchCode, string FromDate, string ToDate)
        {
            try
            {
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);

                var query = from b in cnn.Batches
                            where b.IsActive.Equals(SystemParam.ACTIVE)
                            && (!String.IsNullOrEmpty(BatchCode) ? b.BatchCode.Contains(BatchCode) : true)
                            && (startdate.HasValue ? b.CreateDate >= startdate.Value : true)
                            && (endDate.HasValue ? b.CreateDate <= endDate.Value : true)
                            orderby b.ID descending
                            select new ListBatchOutputModel
                            {
                                ID = b.ID,
                                ProductName = b.ItemID.HasValue ? b.Item.Name : "",
                                ProductID = b.ItemID.HasValue ? b.ItemID.Value : 0,
                                //ProductID = cnn.Batches.Where(u => u.ID.Equals(b.ItemID.Value)).FirstOrDefault() != null ? cnn.Items.Where(u => u.ID.Equals(b.ItemID)).FirstOrDefault().ID:1,
                                BatchCode = b.BatchCode,
                                BatchName = b.BatchName,
                                Point = b.Point,
                                Note = b.Note,
                                QTY = b.QTY,
                                //UsedQTY = b.Products.Where(p => p.IsActive.Equals(SystemParam.ACTIVE) && p.Status.Equals(SystemParam.ACTIVE)).Count(),
                                CreateUserName = b.User.UserName,
                                CreateDate = b.CreateDate
                            };
                int count = query.Count();
                if (query != null && query.Count() > 0)
                {
                    IPagedList<ListBatchOutputModel> list = query.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                    return list;
                }
                else
                {
                    return new List<ListBatchOutputModel>().ToPagedList(1, 1);
                }
            }
            catch
            {
                return new List<ListBatchOutputModel>().ToPagedList(1, 1);
            }
        }

        // Lấy thông tin chi tiết 1 lô hàng
        public BatchDetailOutputModel GetBatchDetail(int BatchID)
        {
            try
            {
                BatchDetailOutputModel batchDetail = new BatchDetailOutputModel();

                var query = (from b in cnn.Batches
                             join u in cnn.Users on b.CreateUserID equals u.UserID
                             where b.IsActive.Equals(SystemParam.ACTIVE) && b.ID.Equals(BatchID)
                             select new BatchDetailOutputModel
                             {
                                 BatchID = b.ID,
                                 BatchCode = b.BatchCode,
                                 BatchName = b.BatchName,
                                 ListProduct = b.Products.Select(p => new ProductOfBatchModel
                                 {
                                     ProductID = p.ID,
                                     BatchID = p.BatchID,
                                     ProductCode = p.ProductCode,
                                     Status = p.Status
                                 }).ToList(),
                                 Point = b.Point,
                                 ProductName = b.ItemID.HasValue ? b.Item.Name : "",
                                 ProductID = b.ItemID.Value,
                                 Note = b.Note,
                                 Price = b.Price,
                                 QTY = b.QTY,

                                 CreateUserName = u.UserName
                             }).FirstOrDefault();
                if (query != null && query.BatchID > 0)
                {
                    return batchDetail = query;
                }
                return batchDetail;
            }
            catch
            {
                return new BatchDetailOutputModel();
            }
        }

        // Xuất Barcode code sang Excel
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
       
        public ExcelPackage ExportQR(int batchID)
        {
            try
            {
                    List<ProductOfBatchModel> listProduct = new List<ProductOfBatchModel>();
                var query = cnn.Products.Where(p => p.BatchID == batchID).Select(p => new ProductOfBatchModel
                {
                    ProductID = p.ID,
                    BatchID = p.BatchID,
                    Status = p.Status,
                    ProductCode = p.ProductCode
                });
                if (query != null && query.Count() > 0)
                    listProduct = query.ToList();
                string path = HttpContext.Current.Server.MapPath(@"/Template/Batch.xlsx");
                FileInfo file = new FileInfo(path);
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                foreach (var item in listProduct)
                {
                    IBarcodeSettings st=new BarcodeSettings();
                    st.Data = item.ProductCode;
                    BarCodeGenerator generator = new BarCodeGenerator(st);
                    Image bar = generator.GenerateImage();
                    bar = Util.CropImage(bar, 10, 20, bar.Width, bar.Height - 20);
                    //BarcodeLib.Barcode.Linear barcode = new BarcodeLib.Barcode.Linear();

                    //  var bar = BarcodeWriter.CreateBarcode(item.ProductCode, BarcodeWriterEncoding.Code128).ResizeTo(200,50).ToBitmap();
                    //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    // QRCodeData data = qrGenerator.CreateQrCode(item.ProductCode, QRCodeGenerator.ECCLevel.Q);
                    //QRCode qr = new QRCode(data);
                    //var img = byteArrayToImage(bar);
                    //Bitmap bitmapImage = new Bitmap("d://file.jpg");
                    sheet.Cells[row, 1].Value = item.ProductCode;
                    if (item.Status == SystemParam.STATUS_PRODUCT_ACTIVE)
                        sheet.Cells[row, 2].Value = SystemParam.STATUS_PRODUCT_ACTIVE_STRING;
                    else if (item.Status == SystemParam.STATUS_PRODUCT_NO_ACTIVE)
                        sheet.Cells[row, 2].Value = SystemParam.STATUS_PRODUCT_NO_ACTIVE_STRING;
                    ExcelPicture QrImage = sheet.Drawings.AddPicture("QR_CODE" + item.ProductID, bar);
                    QrImage.From.Column = 2;
                    QrImage.From.Row = row - 1;
                    QrImage.To.Column = 3;
                    QrImage.To.Row = row;
                    QrImage.SetSize(300, 60);
                    QrImage.From.ColumnOff = 19050;
                    QrImage.From.RowOff = 19050;
                    sheet.Cells[row, 1].AutoFitColumns();
                    sheet.Cells[row, 2].AutoFitColumns();
                    sheet.Column(3).Width = 45;
                    sheet.Row(row).Height = 50;
                    row++;
                }
                sheet.Cells["A1:C" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:C" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:C" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A1:C" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }


        public string CountBatch()
        {
            return cnn.Batches.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Count().ToString();
        }

        public ListBatchOutputModel ModalEditBatch(int ID)
        {
            try
            {
                ListBatchOutputModel batchDetail = new ListBatchOutputModel();

                var query = (from b in cnn.Batches
                             join u in cnn.Users on b.CreateUserID equals u.UserID
                             where b.IsActive == SystemParam.ACTIVE && b.ID == ID
                             select new ListBatchOutputModel
                             {
                                 ID = b.ID,
                                 BatchCode = b.BatchCode,
                                 BatchName = b.BatchName,
                                 ProductName = b.ItemID.HasValue ? b.Item.Name : "",
                                 ProductID = b.ItemID.HasValue ? b.ItemID.Value : 0,
                                 Price = b.Price,
                                 CreateUserName = u.UserName,
                                 QTY = b.QTY,
                                 Point = b.Point,
                                 Note = b.Note
                             }).FirstOrDefault();
                if (query != null && query.ID > 1)
                {
                    return batchDetail = query;
                }
                return batchDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListBatchOutputModel();
            }


        }
        // Sửa thông tin lô hàng
        public int SaveEditBatch(BatchDetailOutputModel input)
        {
            try
            {
                var obj = cnn.Batches.Find(input.BatchID);
                List<int> check = (from p in cnn.Products
                                   where p.BatchID.Equals(obj.ID) && p.IsActive.Equals(SystemParam.ACTIVE) && p.CustomerActiveID != null
                                   select p.BatchID
                    ).ToList();
                var qtyCheck = Convert.ToInt32((input.QtyStr).ToString().Replace(",", ""));
                if (qtyCheck == obj.QTY)
                {
                    if (input.ProductName == null)
                    {
                        input.ProductName = "Dữ liệu chưa được cập nhật";
                    }
                    obj.ItemID = input.ProductID;
                    obj.BatchName = input.BatchName;
                    obj.ProductName = input.ProductName;
                    obj.QTY = Convert.ToInt32((input.QtyStr).ToString().Replace(",", ""));
                    obj.Price = Convert.ToInt32((input.PriceStr).ToString().Replace(",", ""));
                    obj.Point = Convert.ToInt32((input.PointStr).ToString().Replace(",", ""));
                    if (input.Note == null)
                    {
                        input.Note = "Dữ liệu chưa được cập nhật";
                    }
                    else
                    {
                        obj.Note = input.Note;
                    }

                    //obj.Note = input.Note;
                    cnn.SaveChanges();
                    return SystemParam.RETURN_TRUE;
                }
                if(qtyCheck != obj.QTY && check.Contains(input.BatchID))
                {
                    return 2;
                }
                else
                {
                    if(qtyCheck != obj.QTY)
                    {
                        var prd = cnn.Products.Where(p => p.BatchID.Equals(input.BatchID));
                        if(prd != null && prd.Count() > 0)
                        {
                            cnn.Products.RemoveRange(prd.ToList());
                        }
                        if (input.ProductName == null)
                        {
                            input.ProductName = "Dữ liệu chưa được cập nhật";
                        }
                        obj.ItemID = input.ProductID;
                        obj.BatchName = input.BatchName;
                        obj.ProductName = input.ProductName;
                        obj.QTY = Convert.ToInt32((input.QtyStr).ToString().Replace(",", ""));
                        obj.Price = Convert.ToInt32((input.PriceStr).ToString().Replace(",", ""));
                        obj.Point = Convert.ToInt32((input.PointStr).ToString().Replace(",", ""));

                        //Tạo lại số lượng sản phẩm của lô hàng
                        for (int i = 1; i <= qtyCheck; i++)
                        {
                            Product product = new Product();
                            product.ProductCode = Util.CreateMD5(input.BatchCode + "_" + i).Substring(0, SystemParam.LENGTH_QR_HASH) + SystemParam.PRODUCT;
                            product.IsActive = SystemParam.ACTIVE;
                            product.Status = SystemParam.STATUS_PRODUCT_NO_ACTIVE;
                            product.CreateDate = DateTime.Today;
                            product.BatchID = input.BatchID;
                            cnn.Products.Add(product);
                           // cnn.SaveChanges();
                        }
                        // CreateProductOfBatch(input.BatchCode, qtyCheck);
                        if (input.Note == null)
                        {
                            input.Note = "Dữ liệu chưa được cập nhật";
                        }
                        else
                        {
                            obj.Note = input.Note;
                        }

                        //obj.Note = input.Note;
                        cnn.SaveChanges();
                        return SystemParam.RETURN_TRUE;
                    }
                }
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        //Lấy ra list product
        public List<ListProdcut> ListProduct()
        {
            try
            {
                List<ListProdcut> listProduct = new List<ListProdcut>();
                var query = from i in cnn.Items
                            where i.IsActive == SystemParam.ACTIVE
                            orderby i.Name
                            select new ListProdcut
                            {
                                ID = i.ID,
                                Name = i.Name
                            };
                if(query != null && query.Count() > 0)
                {
                    listProduct = query.ToList();
                    return listProduct;
                }
                else
                {
                    return listProduct;
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
                return new List<ListProdcut>();
            }
        }
    }
}
