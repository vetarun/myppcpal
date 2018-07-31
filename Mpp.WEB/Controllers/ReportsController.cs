using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Configuration;
using System.IO;
using System.Data;

namespace Mpp.WEB.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        #region Class Variables and Constants
        ReportsData rdata;
        #endregion

        #region Constructors
        public ReportsController()
        {
            this.rdata = new ReportsData();
        }

        #endregion

        #region Public Methods 
        public ActionResult Index()
        {
            return View();
        }

        //Origional Reports
        [SessionTimeOutFilter]
        [NoCache]
        [AuthorizeSellerAccess]
        [AuthorizePlan]
        public ActionResult FileIn()
        {
            return View();
        }

        //Upload Reports
        [SessionTimeOutFilter]
        [NoCache]
        [AuthorizeSellerAccess]
        [AuthorizePlan]
        public ActionResult FileOut()
        { 
            return View();
        }

        [HttpGet]
        public JsonResult GetReportDates()
        {
            object data=null;
            if (Session["UserID"] != null)
            {
                 data = rdata.GetReportDates(Convert.ToInt32(Session["UserID"]));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Service Methods

        [HttpPost]
        public JsonResult GetReportsIn(PagingOptions options)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            //var sortColumn = options.ColumnName;
            //var sortColumnDir = options.Direction ? "ascending" : "descending";
            //filter parameter
            //var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;

            var reportsData = rdata.GetReportsInData(SessionData.UserID);
            
            //Database query
            var v = from a in reportsData select a;

            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    v = v.Where(a =>
            //        a.CampaignName.Contains(searchValue)
            //        );
            //}

            //sort
            //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            //{
            //    //for make sort simpler we will add Syste.Linq.Dynamic reference
            //    v = v.OrderBy(sortColumn + " " + sortColumnDir);
            //}

            recordsTotal = v.Count();
            reportsData = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = reportsData });
        }

        [HttpPost]
        public JsonResult GetReportsOut(PagingOptions options)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            //var sortColumn = options.ColumnName;
            //var sortColumnDir = options.Direction ? "ascending" : "descending";
            //filter parameter
            //var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;

            var reportsData = rdata.GetReportsOutData(SessionData.UserID);

            //Database query
            var v = from a in reportsData select a;

            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    v = v.Where(a =>
            //        a.CampaignName.Contains(searchValue)
            //        );
            //}

            //sort
            //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            //{
            //    //for make sort simpler we will add Syste.Linq.Dynamic reference
            //    v = v.OrderBy(sortColumn + " " + sortColumnDir);
            //}

            recordsTotal = v.Count();
            reportsData = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = reportsData });
        }

        //public JsonResult Downloadfile(DateTime startDate, DateTime endDate)
        //{
        //    var res = rdata.DownloadFileData(startDate, endDate);
        //    return Json(res, JsonRequestBehavior.AllowGet);

        //}

        [HttpGet]
        public JsonResult Uploadfile(DateTime startDate, DateTime endDate)
        {
            var res = rdata.UploadFileData(MppUtility.DateFormat(startDate, 2), MppUtility.DateFormat(endDate, 2));
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public void DownloadFileData(DateTime startDate, DateTime endDate)
        {
            String msg = "";
            try
            {
                Int32 userid = SessionData.UserID;
                String downloaddir = ConfigurationManager.AppSettings["DownloadFolderPath"];
                string uploadfilepath = MppUtility.GetFilelocation(userid, downloaddir, "bulk");
                string fileName = ConfigurationManager.AppSettings["filename"];
                string fileName1 = fileName.PadRight(29) + MppUtility.DateFormat(startDate, 2).PadRight(11) + "-" + MppUtility.DateFormat(endDate, 2).PadLeft(11) + ".csv";
                string filePath = Path.Combine(uploadfilepath, fileName1);
                bool test = System.IO.File.Exists(filePath);
                if (System.IO.File.Exists(filePath))
                {
                    Response.ClearHeaders();
                    Response.Clear();
                    //context.Response.ContentType = "application/octet-stream";
                    //context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName1);
                    //context.Response.WriteFile(filePath);
                    //context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    byte[] Content = System.IO.File.ReadAllBytes(filePath);
                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName1 + ".csv");
                    Response.BufferOutput = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.OutputStream.Write(Content, 0, Content.Length);
                    Response.End();
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }

        #endregion

        #region Private Methods
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
        #endregion
    }
}