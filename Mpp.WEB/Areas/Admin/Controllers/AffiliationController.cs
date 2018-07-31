using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using Mpp.WEB.Areas.Admin.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Mpp.WEB.Filters;

namespace Mpp.WEB.Areas.Admin.Controllers
{
    [SessionExpireFilter]
  
    public class AffiliationController : Controller
    {
        #region Class Variables and Constants
        private AdminData adata;
        #endregion

        #region Constructors
        public AffiliationController()
        {
            this.adata = new AdminData();
            //StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"].ToString());
        }

        #endregion

        #region Public Methods
        // GET: Admin/Affiliation
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        [AdminRoleFilter(UserType = "1")]
        public ActionResult AffiliationCode()
        {
            return View();
        }

        [NoCache]
        [AdminRoleFilter(UserType = "2")]
        public ActionResult Dashboard()
        {
            return View("GetCodesByAffiliate");
        }

        #endregion

        #region Service Methods

        [HttpPost]
        [AdminRoleFilter(UserType = "1,2")]
        public JsonResult GetAllCodes(PagingOptions options)
        {
            Int32 start = options.Start;
            var length = options.Length;
            int recordsTotal = 0;
            int pageSize = length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            var searchValue = options.SearchName;
            int skip = start;
            var userType = Convert.ToInt32(Session["UserType"]);
            var userId = Convert.ToInt32(Session["AdminUserID"]);
            //paging parameter
            IEnumerable<AffiliationCode> couponItems = null;
            try
            {
                if (userType == 1)
                    couponItems = adata.GetAffiliationData(searchValue);
                else if (userType == 2)
                    couponItems = adata.GetAffiliationData(userId, searchValue);

                var v = from a in couponItems select a;

                //filter
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    v = v.Where(a =>
                //        a.Code.ToLower().Contains(searchValue.ToLower())
                //        );
                //}

                //sort
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = v.Count();
                couponItems = v.Skip(skip).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                LogFile.WriteLog(msg);
            }
            return Json(new { recordsTotal = recordsTotal, data = couponItems });
        }


        [HttpPost]
        [AdminRoleFilter(UserType = "1,2")]
        public JsonResult AddUpdateCode(AffiliationData cdata)
        {
            String res = "";
            if (ModelState.IsValid)
            {
                if ((cdata.Redeemby >= DateTime.Now && cdata.Redeemby!=null) || cdata.Redeemby==null)
                {
                    try
                    {
                        String Code = cdata.Code.Trim();
                        var couponOptions = new StripeCouponCreateOptions();
                        couponOptions.Id = Code;
                        couponOptions.Duration = cdata.Duration == 0 ? "once" : "forever";
                        couponOptions.MaxRedemptions = cdata.Maxredeem;
                        if (cdata.Redeemby != null)
                            couponOptions.RedeemBy = Convert.ToDateTime(cdata.Redeemby).AddMinutes(-1).AddDays(1);
                        if (cdata.Amount == null || cdata.Amount == 0)
                        {
                            couponOptions.PercentOff = cdata.Percent;
                        }
                        else
                        {
                            couponOptions.AmountOff = (int)cdata.Amount * 100;
                            couponOptions.Currency = "usd";
                        }
                        StripeHelper.CreateCoupon(couponOptions);
                        var userid = Convert.ToInt32(Session["AdminUserID"]);
                        res = adata.AddUpdateCouponData(Code, cdata.Duration, cdata.Amount, cdata.Percent, cdata.Maxredeem, cdata.Redeemby, userid);
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }
                }
                else
                {
                    res = "Redeemby must be greater than current date.";
                }
            }
            else
            {
                res = "Failed!";
            }
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [AdminRoleFilter(UserType = "1,2")]
        public JsonResult DeleteCode(string CodeID)
        {
            String res = "";
            try
            {
                var couponService = new StripeCouponService();
               StripeHelper.DeleteCoupon(CodeID);
                res = adata.DeleteCouponData(CodeID);
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [AdminRoleFilter(UserType = "1,2")]
        public JsonResult UpdateStatus(Int32 CodeID, bool Status)
        {
            String res = adata.UpdateCouponStatus(CodeID, Status);
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [AdminRoleFilter(UserType = "1,2")]
        public JsonResult GetAffiliateDetails(Int32 AffiliateID)
        {
            var salesDetails = new List<AffiliateSales>();
            DataSet ds = adata.GetAffiliateSaleData(AffiliateID);
            var Rows = ds.Tables[0].AsEnumerable().ToList();
            if (Rows.Count > 0)
            {
                DataRow FirstRow = ds.Tables[1].Rows[0];
                var CreatedOn = FirstRow.Field<DateTime>("CreatedOn");
                var Redeemby = FirstRow.Field<DateTime?>("Redeemby").GetValueOrDefault();
                var UsedBy = (Redeemby == null) ? DateTime.Now : Convert.ToDateTime(Redeemby);
                //var coupanAmount = FirstRow.Field<decimal>("Amount");
                //var percentOff = FirstRow.Field<int>("Percentile_off");
                salesDetails = GetAffiliateSales(CreatedOn, Rows, UsedBy);
            }
            return Json(new { data = salesDetails });
        }

        [AdminRoleFilter(UserType = "1,2")]
        public void GetAffiliateReport()
        {
            try
            {
                Int32 AffiliateID = Convert.ToInt32(Request.QueryString["AffiliateID"]);
                DataSet ds = adata.GetAffiliateSaleData(AffiliateID);
                var Rows = ds.Tables[0].AsEnumerable().ToList();
                DataRow FirstRow = ds.Tables[1].Rows[0];
                var CreatedOn = FirstRow.Field<DateTime>("CreatedOn");
                var UsedBy = FirstRow.Field<DateTime>("Redeemby");
                var MonthlyDetails = GetAffiliateSales(CreatedOn, Rows, UsedBy);

                var AffiliateCode = FirstRow.Field<String>("AffiliateCode");
                var fileName = "Affiliate_MPP_" + DateTime.Now.ToShortDateString();
                Document document = new Document(PageSize.A4, 88f, 88f, 50f, 10f);

                Font NormalFont = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
                Font BoldFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);


                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    Phrase phrase = null;
                    PdfPCell cell = null;
                    PdfPTable table = null;
                    BaseColor color = null;

                    document.Open();

                    //row1

                    //Header Table
                    table = new PdfPTable(2);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.SetWidths(new float[] { 0.5f, 0.5f });

                    //Report name
                    phrase = new Phrase();
                    phrase.Add(new Chunk("Affiliate Report", FontFactory.GetFont("Arial", 20, Font.BOLD, BaseColor.BLACK)));
                    cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    table.AddCell(cell);
                    //Company Logo
                    cell = ImageCell("~/content/images/logo.png", 80f, Element.ALIGN_RIGHT);
                    table.AddCell(cell);
                    document.Add(table);
                    // separation line header 
                    color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));
                    //line1
                    DrawLine(writer, 25f, document.Top - 50f, document.PageSize.Width - 25f, document.Top - 50f, color);
                    DrawLine(writer, 25f, document.Top - 51f, document.PageSize.Width - 25f, document.Top - 51f, color);

                    //row2
                    table = new PdfPTable(1);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.SpacingBefore = 40f;

                    phrase = new Phrase();
                    phrase.Add(new Chunk("Affiliate Code: " + AffiliateCode + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                    table.AddCell(cell);
                    document.Add(table);

                    //row3
                    table = new PdfPTable(6);
                    table.TotalWidth = 500f;
                    table.LockedWidth = true;
                    table.SpacingBefore = 20f;
                    table.SetWidths(new float[] { 0.2f, 0.3f, 0.2f, 0.3f, 0.2f, 0.2f });

                    var FontColor = new BaseColor(51, 122, 183);
                    var BorderColor = new BaseColor(221, 221, 221);
                    var headerfont = FontFactory.GetFont("Arial", 10, Font.BOLD, FontColor);

                    //RecordID
                    cell = new PdfPCell(new Phrase("RecordID", headerfont));
                    cell = TableCell(cell);
                    table.AddCell(cell);
                    //Date
                    cell = new PdfPCell(new Phrase("Date", headerfont));
                    cell = TableCell(cell);
                    table.AddCell(cell);
                    //Sales
                    cell = new PdfPCell(new Phrase("Sales", headerfont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell = TableCell(cell);
                    table.AddCell(cell);
                    //PreDiscountSale
                    cell = new PdfPCell(new Phrase("Pre DiscountSale", headerfont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell = TableCell(cell);
                    table.AddCell(cell);
                    //AffiliateCommission
                    cell = new PdfPCell(new Phrase("Commission", headerfont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell = TableCell(cell);
                    table.AddCell(cell);
                    //Subscribers
                    cell = new PdfPCell(new Phrase("Subscribers", headerfont));
                    cell = TableCell(cell);
                    table.AddCell(cell);

                    int i = 0;
                    if (MonthlyDetails.Count > 0)
                    {
                        foreach (var record in MonthlyDetails)
                        {
                            i += 1;
                            //RecordID
                            cell = new PdfPCell(new Phrase(i.ToString(), NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                            //Date
                            cell = new PdfPCell(new Phrase(record.Date, NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                            //Sales
                            cell = new PdfPCell(new Phrase("$" + record.TotalSales.ToString("0.##"), NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                            //PreDiscountSale
                            cell = new PdfPCell(new Phrase("$" + record.PreDiscountSale.ToString(), NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                            //AffiliateCommission
                            cell = new PdfPCell(new Phrase("$"+ record.AffiliateCommission.ToString("0.##"), NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                            //Subscribers
                            cell = new PdfPCell(new Phrase(record.TotalUsers.ToString(), NormalFont));
                            cell = TableCell(cell);
                            table.AddCell(cell);
                        }

                        var TotalSales = FirstRow.Field<Decimal>("TotalSales");
                        var TotalUsers = FirstRow.Field<Int32>("TotalUsers");

                        //Year to date
                        cell = new PdfPCell(new Phrase("Total: ", BoldFont));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.MinimumHeight = 25f;
                        cell.Colspan = 2;
                        cell.BorderColor = BorderColor;
                        cell.BorderWidth = 1f;
                        table.AddCell(cell);
                        //TotalSales
                        cell = new PdfPCell(new Phrase("$ " + TotalSales.ToString(), BoldFont));
                        cell = TableCell(cell);
                        table.AddCell(cell);
                        //TotalSubscribers
                        cell = new PdfPCell(new Phrase(TotalUsers.ToString(), BoldFont));
                        cell = TableCell(cell);
                        table.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("No records found", NormalFont));
                        cell.Colspan = 4;
                        cell = TableCell(cell);
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();
                    byte[] bytes = ms.ToArray();
                    ms.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".pdf");
                    Response.ContentType = "application/pdf";
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                    Response.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

         // get affiliate users to assign affilate code by admin
        [AdminRoleFilter(UserType = "1")]
        public JsonResult GetAffiliateUsers(string serachParam,int code)
        {
            var users = adata.GetAffilateUsers(serachParam, code);
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        //map affiliate and coupon code
        [AdminRoleFilter(UserType = "1")]
        public JsonResult AssignCodeToAffiliate(int codeId,int userId)
        {
            var result = false;
           if (codeId > 0 && userId>0)
            {
                 result = adata.AddAffiliateCode(codeId, userId);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //get all assigend users for code
        [AdminRoleFilter(UserType = "1")]
        public JsonResult GetAssignedAffiliates(int codeId)
        {
            if (codeId > 0)
            {
              var  result = adata.GetAssignedAffliates(codeId);
              return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods
        private static void DrawLine(PdfWriter writer, float x1, float y1, float x2, float y2, BaseColor color)
        {
            PdfContentByte contentByte = writer.DirectContent;
            contentByte.SetColorStroke(color);
            contentByte.MoveTo(x1, y1);
            contentByte.LineTo(x2, y2);
            contentByte.Stroke();
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        private PdfPCell ImageCell(string path, float scale, int align)
        {
            Image image = Image.GetInstance(HttpContext.Server.MapPath(path));
            image.ScalePercent(scale);
            PdfPCell cell = new PdfPCell(image);
            cell.BorderColor = BaseColor.WHITE;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }
        private PdfPCell TableCell(PdfPCell cell)
        {
            var BorderColor = new BaseColor(221, 221, 221);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.MinimumHeight = 25f;
            cell.BorderColor = BorderColor;
            cell.BorderWidth = 1f;
            return cell;
        }
        private List<AffiliateSales> GetAffiliateSales(DateTime CreatedOn, List<DataRow> salesDetails,DateTime UsedBy)
        {
            int year_start = CreatedOn.Year;
            int Year_end = UsedBy.Year;
            int month_start = CreatedOn.Month;

          
            var affiliateList = new List<AffiliateSales>();
            while (year_start <= Year_end)
            {
                int current_month = DateTime.Now.Month;
                int month_end = (UsedBy.Month > current_month) ? current_month : UsedBy.Month;
                if (year_start < Year_end)
                    month_end = 12;

                while (month_start <= month_end)
                {
                    var row = salesDetails.FirstOrDefault(s => s.Field<Int32>("SalesMonth").Equals(month_start) && s.Field<Int32>("SalesYear").Equals(year_start));
                    if (row != null)
                        affiliateList.Add(ConvertToSales(row));
                    else
                        affiliateList.Add(ConvertToSales(month_start, year_start));
                    month_start += 1;
                }
                month_start = 1;
                year_start += 1;
            }
            return affiliateList;
        }
        private AffiliateSales ConvertToSales(DataRow row)
        {
            return new AffiliateSales()
            {

                Date = Enum.GetName(typeof(Statics.MonthName), Convert.ToInt32(row.Field<Int32>("SalesMonth"))) + "," + row.Field<Int32>("SalesYear"),
                TotalSales = (double)row.Field<Decimal>("TotalSales"),
                TotalUsers = row.Field<Int32>("TotalUsers"),
                PreDiscountSale = row.Field<Decimal>("TotalPreSales"),
                AffiliateCommission = row.Field<Decimal>("Commision")
            };
        }
        private AffiliateSales ConvertToSales(int month, int year)
        {
            return new AffiliateSales()
            {
                Date = Enum.GetName(typeof(Statics.MonthName), month) + ", " + year,
                TotalSales = 0,
                TotalUsers = 0,
                PreDiscountSale = 0.00M,
                AffiliateCommission = 0.00M
            };
        }
        #endregion
    }
}