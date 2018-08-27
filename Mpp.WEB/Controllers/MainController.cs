using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mpp.UTILITIES;
using Mpp.BUSINESS.DataLibrary;
using System.Linq.Dynamic;
using Mpp.BUSINESS;
using Mpp.WEB.Filters;

namespace Mpp.WEB.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        #region Class Variables and Constants
        private DashboardData dashboardData;
        private OptimizeData optimizeData;
        #endregion

        #region Constructors

        public MainController()
        {

            this.dashboardData = new DashboardData();
            this.optimizeData = new OptimizeData();

        }
        #endregion

        #region Public Methods

        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        [SessionTimeOutFilter]
        [NoCache]
        [AuthorizeSellerAccess]
        public ActionResult Dashboard()
        {
            ViewBag.TrilEnd = SessionData.TrialEndOn;
            ViewBag.plan = SessionData.PlanID;
            return View();
        }

        #endregion

        #region Angular Services

        [HttpPost]
        public JsonResult GetCampData(PagingOptions options, Int32 range)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var listCamp = new List<Campaign>();
            DateTime enddate = DateTime.Today.AddDays(-3);
            DateTime startdate = enddate;
            GetStartDate(range, ref startdate);

            switch (range)
            {
                case 0:
                    listCamp = dashboardData.GetCampaignData(SessionData.UserID);
                    break;
                default:
                    listCamp = dashboardData.GetCustCampaignData(SessionData.UserID, startdate, enddate);
                    break;
            }

            //Database query
            var v = from a in listCamp select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.CampaignName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if(options.IsIgnoreZero)
                v = SkipZeroCampaigns(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            listCamp = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = listCamp });
        }
        [HttpPost]
        public JsonResult GetOptimization(PagingOptions options, Int32 range, int type, int id, String KeyName)
        {
            //paging parameter
            var start = options.Start;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int skip = start;
            int recordsTotal = 0;
            var OptimizedCamps = new List<Log>();

                if (sortColumnDir == "ascending")
                {

                OptimizedCamps = dashboardData.GetOptimization(type, id, KeyName, 1, SessionData.UserID);
                }
                else
                {
                OptimizedCamps = dashboardData.GetOptimization(type, id, KeyName, 0, SessionData.UserID);
                }
            //Database query
            var v = from a in OptimizedCamps select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.KeywordName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (sortColumn == "ModifiedOn")
            {
                recordsTotal = v.Count();
                int pageSize = recordsTotal;

                OptimizedCamps = v.Skip(skip).Take(pageSize).ToList();
            }
            else
            {
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = SkipZeroOptimize(sortColumn, v);
                    //for make sort simpler we will add Syste.Linq.Dynamic reference
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                    recordsTotal = v.Count();
                    int pageSize = recordsTotal;
                    OptimizedCamps = v.Skip(skip).Take(pageSize).ToList();
                }
            }
            return Json(new { recordsTotal = recordsTotal, data = OptimizedCamps });
        }
        //[HttpPost]
        //public JsonResult GetOptimizationData(PagingOptions options, Int32 range, int type, int id, String KeyName)
        //{
        //    //paging parameter
        //    var start = options.Start;
        //    var length = options.Length;
        //    //sorting parameter
        //    var sortColumn = options.ColumnName;
        //    var sortColumnDir = options.Direction ? "descending" : "ascending";
        //    //filter parameter
        //    var searchValue = options.SearchName;
        //    int pageSize = length;
        //    int skip = start;
        //    int recordsTotal = 0;
        //    var allKeyword = new List<Log>();

        //    allKeyword = dashboardData.GetOptimization(type, id, KeyName,0, SessionData.UserID);

        //    //Database query
        //    var k = from a in allKeyword select a;

        //    if (!string.IsNullOrEmpty(searchValue))
        //    {
        //        k = k.Where(a =>
        //            a.KeywordName.ToLower().Contains(searchValue.ToLower())
        //            );
        //    }

        //    //sort
        //    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
        //    {

        //        //for make sort simpler we will add Syste.Linq.Dynamic reference
        //        k = k.OrderBy(sortColumn + " " + sortColumnDir);
        //    }

        //    recordsTotal = k.Count();
        //    allKeyword = k.Skip(skip).Take(pageSize).ToList();

        //    return Json(new { recordsTotal = recordsTotal, data = allKeyword });
        //}
        [HttpPost]
        public JsonResult GetKeywordLog(PagingOptions options, long CampID, Int32 FilterArg)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;

            var keylog = optimizeData.GetKeywordLogData(CampID, SessionData.UserID); // all data taken 

            //Database query
            var v = from a in keylog  select a;
            if (FilterArg == 0)
            {
                recordsTotal = v.Count();
                keylog = v.Skip(skip).Take(pageSize).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(searchValue))
                {
                   v = v.Where(a =>a.KeywordName.ToLower().Contains(searchValue.ToLower()));
                }
                recordsTotal = v.Count();
                keylog = v.Skip(skip).Take(pageSize).ToList(); 
            }

            return Json(new { recordsTotal = recordsTotal, data = keylog });
        }
        [HttpPost]
        public JsonResult GetKeyData(PagingOptions options, Int32 range)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var allKeyword = new List<Keyword>();

            DateTime enddate = DateTime.Today.AddDays(-3);
            DateTime startdate = enddate;
            GetStartDate(range, ref startdate);

            switch (range)
            {
                case 0:
                    allKeyword = dashboardData.GetKeywordData(SessionData.UserID);
                    break;
                default:
                    allKeyword = dashboardData.GetCustKeywordData(SessionData.UserID, startdate, enddate);
                    break;

            }

            //Database query
            var v = from a in allKeyword select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.KeywordName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (options.IsIgnoreZero)
                    v = SkipZeroKeywords(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            allKeyword = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = allKeyword });
        }

        [HttpPost]
        public JsonResult GetAdGroupData(PagingOptions options, Int32 range)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var allAdGroups = new List<AdGroup>();

            DateTime enddate = DateTime.Today.AddDays(-2);
            DateTime startdate = enddate;
            GetStartDate(range, ref startdate);

            switch (range)
            {
                case 0:
                    allAdGroups = dashboardData.GetAdGroupData(SessionData.UserID);
                    break;
                default:
                    allAdGroups = dashboardData.GetCustAdGroupData(SessionData.UserID, startdate, enddate);
                    break;

            }

            //Database query
            var v = from a in allAdGroups select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.AdGroupName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (options.IsIgnoreZero)
                    v = SkipZeroAdGroups(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            allAdGroups = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = allAdGroups });
        }

        [HttpPost]
        public JsonResult GetCustomCampData(PagingOptions options, DateTime startDate, DateTime endDate)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            DateTime today = DateTime.Today.AddDays(-1);
            var listCamp = dashboardData.GetCustCampaignData(SessionData.UserID, startDate, endDate);
            //Database query
            var v = from a in listCamp select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.CampaignName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if(startDate != today && endDate != today)
                    v = SkipZeroCampaigns(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            listCamp = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = listCamp });
        }

        [HttpPost]
        public JsonResult GetCustomKeyData(PagingOptions options, DateTime startDate, DateTime endDate)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var listCamp = dashboardData.GetCustKeywordData(SessionData.UserID, startDate, endDate);
            //Database query
            var v = from a in listCamp select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.KeywordName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = SkipZeroKeywords(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            listCamp = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = listCamp });
        }

        [HttpPost]
        public JsonResult GetCustomAdGroupData(PagingOptions options, DateTime startDate, DateTime endDate)
        {
            //paging parameter
            var start = options.Start;
            var length = options.Length;
            //sorting parameter
            var sortColumn = options.ColumnName;
            var sortColumnDir = options.Direction ? "descending" : "ascending";
            //filter parameter
            var searchValue = options.SearchName;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var listAdGroup = dashboardData.GetCustAdGroupData(SessionData.UserID, startDate, endDate);
            //Database query
            var v = from a in listAdGroup select a;

            if (!string.IsNullOrEmpty(searchValue))
            {
                v = v.Where(a =>
                    a.AdGroupName.ToLower().Contains(searchValue.ToLower())
                    );
            }

            //sort
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = SkipZeroAdGroups(sortColumn, v);
                //for make sort simpler we will add Syste.Linq.Dynamic reference
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            listAdGroup = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = listAdGroup });
        }

        [HttpPost]
        public JsonResult GetCharts(DateTime StartDate, DateTime EndDate)
        {
            var res = dashboardData.GetChartData(SessionData.UserID, StartDate, EndDate);
            var gmodel = new GraphModel();
            if (res.Count > 0)
            {
                gmodel.spendmodel = res.Select(s => new SpendModel()
                {
                    ReportDay = s.ReportDay,
                    Spend = s.Spend
                }).ToList();
                gmodel.salemodel = res.Select(r => new SalesModel()
                {
                    ReportDay = r.ReportDay,
                    Sales = r.Sales
                }).ToList();
                gmodel.acosmodel = res.Select(a => new AcosModel()
                {
                    ReportDay = a.ReportDay,
                    Sales = a.Sales,
                    Spend = a.Spend
                }).ToList();
                gmodel.impressions = res.Select(i => new ImpressionModel()
                {
                    ReportDay = i.ReportDay,
                    Impressions = i.Impressions
                }).ToList();
                gmodel.clicks = res.Select(i => new ClickModel()
                {
                    ReportDay = i.ReportDay,
                    Clicks = i.Click
                }).ToList();

                gmodel.ctr = res.Select(i => new CTRModel()
                {
                    ReportDay = i.ReportDay,
                    Impressions = i.Impressions,
                    Clicks=i.Click
                }).ToList();
                gmodel.cpc = res.Select(i => new CPCModel()
                {
                    ReportDay = i.ReportDay,
                    Spend = i.Spend,
                    Clicks = i.Click
                }).ToList();


            }
            return Json(gmodel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetModalChart(DateTime StartDate, DateTime EndDate, String Type)
        {
            object obj = null;
            switch (Type)
            {
                case "Spend":
                    obj = dashboardData.GetSpendChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "Sales":
                    obj = dashboardData.GetSaleChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "Acos":
                    obj = dashboardData.GetAcosChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "Impressions":
                    obj = dashboardData.GetImpressionsChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "Clicks":
                    obj = dashboardData.GetClicksChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "CTR":
                    obj = dashboardData.GetCTRChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                case "CPC":
                    obj = dashboardData.GetCPCChartData(SessionData.UserID, StartDate, EndDate);
                    break;
                default:
                    break;
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAlerts()
        {
            var res = dashboardData.GetAlertData(SessionData.UserID);
            SessionData.AlertCount = res.Count;
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult ClearAlerts(List<Int32> alerts)
        {
            String msg = "";
            foreach (var alertId in alerts)
            {
                msg = dashboardData.DeleteAlertData(alertId);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        //protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}
        private IEnumerable<Log> SkipZeroOptimize(String sortColumn, IEnumerable<Log> v)
        {
            switch (sortColumn)
            {
                case "ModifiedOn":
                    v = v.Where(a => a.ModifiedOn != "");
                    break;
                
                default:
                    break;
            }
            return v;
        }

        private IEnumerable<Campaign> SkipZeroCampaigns(String sortColumn, IEnumerable<Campaign> v)
        {
            switch (sortColumn)
            {
                case "ACoS":
                    v = v.Where(a => a.ACoS != 0);
                    break;
                case "Spend":
                    v = v.Where(a => a.Spend != 0);
                    break;
                case "Sales":
                    v = v.Where(a => a.Sales != 0);
                    break;
                case "Impressions":
                    v = v.Where(a => a.Impressions != 0);
                    break;
                case "Clicks":
                    v = v.Where(a => a.Clicks != 0);
                    break;
                case "CTR":
                    v = v.Where(a => a.CTR != 0);
                    break;
                case "CostPerClick":
                    v = v.Where(a => a.CostPerClick != 0);
                    break;
                default:
                    break;
            }
            return v;
        }

        private IEnumerable<Keyword> SkipZeroKeywords(String sortColumn, IEnumerable<Keyword> v)
        {
            switch (sortColumn)
            {
                case "ACoS":
                    v = v.Where(a => a.ACoS != 0);
                    break;
                case "Spend":
                    v = v.Where(a => a.Spend != 0);
                    break;
                case "Sales":
                    v = v.Where(a => a.Sales != 0);
                    break;
                case "Impressions":
                    v = v.Where(a => a.Impressions != 0);
                    break;
                case "Clicks":
                    v = v.Where(a => a.Clicks != 0);
                    break;
                case "CTR":
                    v = v.Where(a => a.CTR != 0);
                    break;
                case "CostPerClick":
                    v = v.Where(a => a.CostPerClick != 0);
                    break;
                default:
                    break;
            }
            return v;
        }

        private IEnumerable<AdGroup> SkipZeroAdGroups(String sortColumn, IEnumerable<AdGroup> v)
        {
            switch (sortColumn)
            {
                case "ACoS":
                    v = v.Where(a => a.ACoS != 0);
                    break;
                case "Spend":
                    v = v.Where(a => a.Spend != 0);
                    break;
                case "Sales":
                    v = v.Where(a => a.Sales != 0);
                    break;
                case "Impressions":
                    v = v.Where(a => a.Impressions != 0);
                    break;
                case "Clicks":
                    v = v.Where(a => a.Clicks != 0);
                    break;
                case "CTR":
                    v = v.Where(a => a.CTR != 0);
                    break;
                case "CostPerClick":
                    v = v.Where(a => a.CostPerClick != 0);
                    break;
                default:
                    break;
            }
            return v;
        }

        private void GetStartDate(int range,ref DateTime startdate)
        {
            if (range == 7)
                startdate = startdate.AddDays(-6);
            else if (range == 30)
                startdate = startdate.AddDays(-29);
            else if (range == 60)
                startdate = startdate.AddDays(-59);
            else if (range == 90)
                startdate = startdate.AddDays(-89);
        }
        #endregion

    }
}