using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using Mpp.WEB.Filters;
using Mpp.WEB.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Mpp.UTILITIES.Enums.StatusEnums;
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Areas.Admin.Controllers
{
    [AdminRoleFilter(UserType = "1")]
    [SessionExpireFilter]
    public class PreferencesController : Controller
    {
        #region Class Variables and Constants
        private AdminData adata;
        #endregion

        #region Constructors
        public PreferencesController()
        {
            this.adata = new AdminData();
        }

        #endregion

        #region Public Methods
        // GET: Admin/Index
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult AmazonCode()
        {
            return View();
        }

        [NoCache]
        public ActionResult Log()
        {
            return View();
        }
        [NoCache]
        public ActionResult Report()
        {
            return View();
        }

        #endregion

        #region AngularServices
        [HttpGet]
        public JsonResult GetReportDates(int userId)
        {
            object data = null;
             data = adata.GetReportDates(Convert.ToInt32(userId),2);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLoginCodes()
        {
            IEnumerable<object> res = null;
            res = adata.Get2FAData();
            return Json(res.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateLoginCode(Int32 CodeID, String LoginCode)
        {
            String msg = "";
            if (!String.IsNullOrWhiteSpace(LoginCode))
            {
                msg = adata.Update2FACodeData(CodeID, LoginCode);
            }
            else
            {
                msg = "Code was not found, Please contact admin";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetClients(string srchTerm)
        {
            var clientList = adata.GetAllClients(srchTerm);

            return Json(clientList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// method to get report logs
        /// </summary>
        /// <param name="pageOptions">params to sort,sort dir,filter,paging</param>
        /// <returns>it returns headers and data with key value so we can bind headers dynamicaly in html.</returns>
        [HttpPost]
        public JsonResult GetReportLogs(PagingOptions pageOptions)
        {
            DataTableResponse reportLogTable = null;
            if (pageOptions.ColumnName == null)
                pageOptions.ColumnName = "ReportReqDate";

            var total = 0;
            var clientList = adata.GetReportLogs(pageOptions, out total);
            if (clientList != null && clientList.Rows.Count > 0)
            {
                reportLogTable = new DataTableResponse();

                var headers = new List<DataTableHeaders>();
                headers.Add(new DataTableHeaders { Title = "Client", Key = "Name" });
                headers.Add(new DataTableHeaders { Title = "Report Type", Key = "RecordName" });
                headers.Add(new DataTableHeaders { Title = "Report Date", Key = "ReportDate" });
                headers.Add(new DataTableHeaders { Title = "Optimize Date", Key = "OptimizeDate" });
                headers.Add(new DataTableHeaders { Title = "Refresh Count", Key = "RefStatusCount" });
                headers.Add(new DataTableHeaders { Title = "Report Status", Key = "ReportStatus" });
                headers.Add(new DataTableHeaders { Title = "Refresh Status", Key = "RefreshStatus" });
                headers.Add(new DataTableHeaders { Title = "Update Bid Status", Key = "UpdtBidStatus" });
                headers.Add(new DataTableHeaders { Title = "Update Neg Status", Key = "UpdtNegStatus" });
                headers.Add(new DataTableHeaders { Title = "RequestedDate", Key = "ReportReqDate" });
                var dataList = new List<object>();
                foreach (DataRow item in clientList.Rows)
                {
                    dataList.Add(new
                    {
                        Name = item["Name"],
                        RecordName = item["RecordName"],
                        ReportDate = Convert.ToDateTime(item["ReportDate"]).ToString("MMM dd, yyyy"),
                        OptimizeDate = item["OptimizeDate"] is DBNull?"N/A":Convert.ToDateTime(item["OptimizeDate"]).ToString("MMM dd, yyyy"),
                        ReportStatus = Enum.GetName(typeof(RefreshStatus), Convert.ToInt32(item["ReportStatus"])),
                        RefreshStatus = Enum.GetName(typeof(RefreshStatus),Convert.ToInt32(item["RefreshStatus"])),
                        UpdtBidStatus =Convert.ToInt32(item["UpdtBidStatus"])==0?"Not Set":"Set",
                        UpdtNegStatus = Convert.ToInt32(item["UpdtNegStatus"])==0?"Not Set":"Set",
                        RefStatusCount = Convert.ToInt32(item["RefStatusCount"]),
                        ReportReqDate = Convert.ToDateTime(item["ReportReqDate"]).ToString("MMM dd, yyyy")

                    });
                }

                //headers and table data
                reportLogTable.Data = dataList;
                reportLogTable.Headers = headers;

                //pagingOptions
                reportLogTable.ColumnName = pageOptions.ColumnName;
                reportLogTable.Direction = pageOptions.Direction;
                reportLogTable.Total = total;
                reportLogTable.Start = pageOptions.Start;
                reportLogTable.Length = pageOptions.Length;


            }
            return Json(reportLogTable, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// method to get report logs
        /// </summary>
        /// <param name="pageOptions">params to sort,sort dir,filter,paging</param>
        /// <returns>it returns headers and data with key value so we can bind headers dynamicaly in html.</returns>
        [HttpPost]
        public JsonResult GetEmailLog(PagingOptions pageOptions)
        {
            DataTableResponse reportLogTable = null;
            if (pageOptions.ColumnName == null)
                pageOptions.ColumnName = "SellerName";
            var total = 0;
            var clientList = adata.GetEmailLOg(pageOptions, out total);
            if (clientList != null && clientList.Rows.Count > 0)
            {
                reportLogTable = new DataTableResponse();

                var headers = new List<DataTableHeaders>();
                headers.Add(new DataTableHeaders { Title = "Client", Key = "Name" });
             //   headers.Add(new DataTableHeaders { Title = "Seller Name", Key = "SellerName" });
                headers.Add(new DataTableHeaders { Title = "Data Import", Key = "DataImportAlert" });
                headers.Add(new DataTableHeaders { Title = "Access Limit", Key = "AccessLimitAlert" });
                headers.Add(new DataTableHeaders { Title = "SevenDay Trial", Key = "SevenDayTrialAlert" });
                headers.Add(new DataTableHeaders { Title = "Trial End", Key = "TrialEndAlert" });
                headers.Add(new DataTableHeaders { Title = "Activation Email", Key = "ActivationEmailAlert" });
                //headers.Add(new DataTableHeaders { Title = "ModifiedOn", Key = "ModifiedOn" });
                headers.Add(new DataTableHeaders { Title = "PaymentAlert", Key = "PayAlert" });

                var dataList = new List<object>();
                foreach (DataRow item in clientList.Rows)
                {
                    var accessAlert = Convert.ToInt32(item["AccessLimitAlert"]);
                    dataList.Add(new
                    {
                        Name = item["Name"],
                       // SellerName = item["SellerName"],
                        DataImportAlert = Convert.ToInt32(item["DataImportAlert"]) == 0 ? "Not Sent" : "Sent",
                        AccessLimitAlert =(accessAlert==1?"Not Sent":(accessAlert==0?"Sent(3)":"Sent("+(accessAlert-1)+")")),
                        SevenDayTrialAlert = Convert.ToInt32(item["SevenDayTrialAlert"]) == 0 ? "Not Sent" : "Sent",
                        TrialEndAlert = Convert.ToInt32(item["TrialEndAlert"]) == 0 ? "Not Sent" : "Sent",
                        ActivationEmailAlert = Convert.ToInt32(item["ActivationEmailAlert"]) == 0 ? "Not Sent" : "Sent:(" + Convert.ToInt32(item["ActivationEmailAlert"]) + ")",
                        //ModifiedOn = Convert.ToDateTime(item["ModifiedOn"]).ToString("MMM dd, yyyy"),
                        PayAlert = Convert.ToInt32(item["PayAlert"]) == 0 ? "Not Sent" : (Convert.ToInt32(item["PayAlert"]) == 1 ? "Sent(Paid)" : "Sent(Pay. Failed)")


                    });
                }

                //headers and table data
                reportLogTable.Data = dataList;
                reportLogTable.Headers = headers;

                //pagingOptions
                reportLogTable.ColumnName = pageOptions.ColumnName;
                reportLogTable.Direction = pageOptions.Direction;
                reportLogTable.Total = total;
                reportLogTable.Start = pageOptions.Start;
                reportLogTable.Length = pageOptions.Length;


            }
            return Json(reportLogTable, JsonRequestBehavior.AllowGet);
        }
     

        #endregion
    }
}