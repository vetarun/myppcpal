using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.BUSINESS.DataModel;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Controllers
{
    [Authorize]
    public class MyCampaignsController : Controller
    {
        #region Class Variables and Constants
        private OptimizeData optimizeData;
        private ReportsAPI api;

        #endregion

        #region Constructors

        public MyCampaignsController()
        {
            
            this.api = new ReportsAPI();
            this.optimizeData = new OptimizeData();
        }
        #endregion

        #region Public Methods
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //GET: Get MyCampaigns
        [SessionTimeOutFilter]
        [NoCache]
        [AuthorizeSellerAccess]
        [AuthorizePlan]
        public ActionResult ViewMyCampaigns()
        {
            return View();
        }

        #endregion

        #region Angular Services
        [HttpGet]
        public JsonResult GetMyCampaigns()
        {
            OptimzeCampaigns Mycampaign = new OptimzeCampaigns();
            var res = optimizeData.GetMyCampaignsData(SessionData.UserID);
            Mycampaign.optimizecamp = res;
            Mycampaign.IsSetFormula = SessionData.FormulaAccess;
            if (res.Count > 0)
            {
                Int64 campid = res.FirstOrDefault().RecordID;
                Mycampaign.formula = optimizeData.GetFormulaData(campid, SessionData.UserID);
            }
            return Json(Mycampaign, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult RevertChanges(Int64 KeyID, Int64 CID, Int64 AID,Int32 RID ,Int32 RSNID, String ModifyDate,bool Checked)
        {
           
            var result = optimizeData.RevertUpdates( KeyID,  CID, AID,RID,RSNID, ModifyDate, SessionData.UserID);
            if (result == "")
            {
                 api.ManualKeywordUpdate(KeyID, CID, AID, RID, RSNID, ModifyDate, SessionData.UserID);
                
            }
            if (Checked)
            {
               optimizeData.HideWarning(SessionData.UserID);
            }
            return new JsonResult() { Data = new {Result=result,Checked= Checked}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
  
        [HttpGet]
        public JsonResult GetFormula(Int64 CampID)
        {
            var result  = optimizeData.GetFormulaData(CampID, SessionData.UserID);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult SaveFormula(Int64 CampID, Formula fm)
        {
            String res = optimizeData.UpdateFormula(SessionData.UserID, CampID, fm);
            return new JsonResult() { Data = res, JsonRequestBehavior= JsonRequestBehavior.AllowGet };         
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult PasteCampFormula(Int64 copyID, Int64 pasteID)
        {
            String res = optimizeData.PasteCampFormulaData(SessionData.UserID, copyID, pasteID);
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult PasteAllCampFormula(Int64 Cid)
        {
            String res = optimizeData.PasteAllCampaignFormulaData(Cid, SessionData.UserID);
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult RunCampaign(Int64 CampID, Int32 status)
        {
            String res = "";
            try
            {
                var profile = APIData.GetUserProfileForSkuData(SessionData.UserID);
                DataRow dr = profile.Rows[0];
                String ProfileId = Convert.ToString(dr["ProfileId"]);
                DateTime LastUpdatedOn = Convert.ToDateTime(dr["AccessTokenUpdatedOn"]);

                var Auth = new AuthorizationModel()
                {
                    access_token = Convert.ToString(dr["AccessToken"]),
                    refresh_token = Convert.ToString(dr["RefreshToken"]),
                    token_type = Convert.ToString(dr["TokenType"]),
                    expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                };
                var msg = string.Empty;
                Int64[] ids = new Int64[1] { CampID };
                ReportsAPI api = new ReportsAPI();
                var list = new List<CampaignStatus>();
                list.Add(new CampaignStatus { RecordID = CampID, Status = status, Count = 0 });
                res = api.GetUserProductAdsCount(SessionData.UserID, ProfileId, LastUpdatedOn, Auth, out msg, list);
                if(String.IsNullOrEmpty(res))
                    res = optimizeData.RunCampaignData(SessionData.UserID, CampID, status);
                if (res == "")
                    SessionData.FormulaAccess = 1;
            }
            catch
            {
                res = "Something went wrong!. Please try again.";
            }
            
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [SessionTimeOutFilter]
        public JsonResult RunCampaigns(List<CampaignStatus> Camplist)
        {
            String res = "";
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] {
            new DataColumn("RecordID", typeof(String)),
            new DataColumn("Status", typeof(bool))
            });

            foreach(var row in Camplist)
            {
                dt.Rows.Add();
                dt.Rows[dt.Rows.Count - 1][0] = row.RecordID;
                dt.Rows[dt.Rows.Count - 1][1] = row.Status;

            }

            if (dt.Rows.Count > 0)
            {
               
                var profile = APIData.GetUserProfileForSkuData(SessionData.UserID);
                DataRow dr = profile.Rows[0];
                String ProfileId = Convert.ToString(dr["ProfileId"]);
                DateTime LastUpdatedOn = Convert.ToDateTime(dr["AccessTokenUpdatedOn"]);

                var Auth = new AuthorizationModel()
                {
                    access_token = Convert.ToString(dr["AccessToken"]),
                    refresh_token = Convert.ToString(dr["RefreshToken"]),
                    token_type = Convert.ToString(dr["TokenType"]),
                    expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                };
                var msg = string.Empty;
                var campaignId=  dt.AsEnumerable().Select(r =>Convert.ToInt64(r.Field<string>("RecordID"))).ToArray();
              
                ReportsAPI api = new ReportsAPI();
                api.GetUserProductAdsCount(SessionData.UserID, ProfileId, LastUpdatedOn, Auth, out msg, Camplist);
                res = optimizeData.RunCampaignData(dt, SessionData.UserID);
            }
            else
            {
                res = "Something went wrong!";
            }

            if (res == "")
            {
                SessionData.FormulaAccess = 1;
            }
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetCampaignLog(PagingOptions options, Int64 CampID)
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
            var firstCmap = "";
            var camplog = optimizeData.GetCampaignLogData(CampID, SessionData.UserID,out firstCmap);

            //Database query
            var v = from a in camplog select a;

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
            camplog = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = camplog });
        }

        [HttpPost]
        public JsonResult GetKeywordLog(PagingOptions options,long CampID)
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

            var keylog = optimizeData.GetKeywordLogData(CampID, SessionData.UserID);

            //Database query
            var v = from a in keylog orderby DateTime.Parse(a.ModifiedOn) descending select a;

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
            keylog = v.Skip(skip).Take(pageSize).ToList();

            return Json(new { recordsTotal = recordsTotal, data = keylog });
        }
        public JsonResult FetchNewCampaigns()
        {
            var respo = new object();
            BUSINESS.ReportsAPI api = new BUSINESS.ReportsAPI();
            try
            {
                var dt = APIData.GetUserProfileForSkuData(SessionData.UserID);
                DataRow dr = dt.Rows[0];
                var Auth = new AuthorizationModel()
                {
                    access_token = Convert.ToString(dr["AccessToken"]),
                    refresh_token = Convert.ToString(dr["RefreshToken"]),
                    token_type = Convert.ToString(dr["TokenType"]),
                    expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                };
                 respo = api.GetNewCampaignsOnDemand(SessionData.UserID, Convert.ToDateTime(dr["AccessTokenUpdatedOn"]), Convert.ToString(dr["ProfileId"]), Auth);
                return Json(respo, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Records =0, Status = 3},JsonRequestBehavior.AllowGet);
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

  

    public class OptimzeCampaigns
    {
        public Formula formula { get; set; }
        public List<Optimize> optimizecamp { get; set; }
        public int IsSetFormula { get; set; }
    }
}