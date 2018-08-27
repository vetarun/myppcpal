using Mpp.BUSINESS.DataLibrary;
using Mpp.BUSINESS.DataModel;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Mpp.UTILITIES.Statics;

namespace Mpp.BUSINESS
{
    public class ReportsAPI
    {
        #region Class Variables and Constants

        private string _version;
        private Uri _api;
        private string _clientkey;
        private string _clientSecretkey;
        private int maxCount = 80;
        private int maxLimit = 80;
        private int timeLimit = 1;
        private ThrottlingHelper _throttlingHelper;
        private TimeSpan _tLowStarttime;
        private TimeSpan _tLowEndtime;

        #endregion

        #region Constructors
        public ReportsAPI()
        {
            _version = MppUtility.ReadConfig("ApiVersion");
            _api = new Uri(MppUtility.ReadConfig("Url"));
            _clientkey = MppUtility.ReadConfig("ClientKey");
            _clientSecretkey = MppUtility.ReadConfig("ClientSecretKey");
            _tLowStarttime = MppUtility.SetTime(4, 0, 0);
            _tLowEndtime = MppUtility.SetTime(6, 0, 0);
            this.maxCount = maxCount * 2; //Default
            if (DateTime.Now.TimeOfDay >= _tLowStarttime && DateTime.Now.TimeOfDay <= _tLowEndtime)
            {
                this.maxLimit = 20;
                this.maxCount = 40;
            }
            this._throttlingHelper = new ThrottlingHelper(maxLimit, TimeSpan.FromMinutes(timeLimit));
        }

        public ReportsAPI(int Limit)
        {
            _version = MppUtility.ReadConfig("ApiVersion");
            _api = new Uri(MppUtility.ReadConfig("Url"));
            _clientkey = MppUtility.ReadConfig("ClientKey");
            _clientSecretkey = MppUtility.ReadConfig("ClientSecretKey");
            this.maxCount = Limit * maxCount;
            _tLowStarttime = MppUtility.SetTime(4, 0, 0);
            _tLowEndtime = MppUtility.SetTime(6, 0, 0);
            if (DateTime.Now.TimeOfDay >= _tLowStarttime && DateTime.Now.TimeOfDay <= _tLowEndtime)
            {
                this.maxLimit = 20;
                this.maxCount = Limit * 20;
            }
            this._throttlingHelper = new ThrottlingHelper(maxLimit, TimeSpan.FromMinutes(timeLimit));
        }

        #endregion

        #region Public Methods

        //Snap shot(Product information) reports
        public void ProcessProductsSnapShot(ReportStatus Status1, ReportStatus Status2)
        {
            DataTable dt, dt1;
            dt = APIData.GetSnapShotData(Status1, maxCount);

            if (dt.Rows.Count > 0)
            {
                var reports = from row in dt.AsEnumerable()
                              group row by new { ID = row.Field<Int32>("MppUserID") } into grp
                              orderby grp.Key.ID
                              select new { SellerID = grp.Key.ID };

                if (reports.Count() > 0)
                {
                    foreach (var r in reports)
                    {
                        int UserID = r.SellerID;
                        DataTable dtbl = new DataTable();
                        dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("MppUserID") == UserID).CopyToDataTable();

                        if (dtbl.Rows.Count > 0)
                        {
                            string authres = "";
                            DataRow dtblrow = dtbl.Rows[0];

                            var auth = new AuthorizationModel()
                            {
                                access_token = Convert.ToString(dtblrow["AccessToken"]),
                                refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                token_type = Convert.ToString(dtblrow["TokenType"]),
                                expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                            };

                            string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                            DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                authres = UpdateAccessToken(UserID, ref auth);

                            if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                            {
                                foreach (DataRow row in dtbl.Rows)
                                {
                                    int ReportID = Convert.ToInt32(row["ReportID"]);
                                    int RecordType = Convert.ToInt32(row["RecordType"]);
                                    SetProductSnapReport(UserID, ReportID, RecordType, _profileId, auth);
                                }
                            }
                        }
                    }
                }
            }

            if (Status2 != ReportStatus.NOTREQUIRED)
            {
                dt1 = APIData.GetSnapShotData(Status2, maxCount);
                if (dt1.Rows.Count > 0)
                {
                    var reports = from row in dt1.AsEnumerable()
                                  group row by new { ID = row.Field<Int32>("MppUserID") } into grp
                                  orderby grp.Key.ID
                                  select new { SellerID = grp.Key.ID };

                    if (reports.Count() > 0)
                    {
                        foreach (var r in reports)
                        {
                            int UserID = r.SellerID;
                            DataTable dtbl = new DataTable();
                            dtbl = dt1.AsEnumerable().Where(ro => ro.Field<Int32>("MppUserID") == UserID).CopyToDataTable();

                            if (dtbl.Rows.Count > 0)
                            {
                                string authres = "";
                                DataRow dtblrow = dtbl.Rows[0];

                                var auth = new AuthorizationModel()
                                {
                                    access_token = Convert.ToString(dtblrow["AccessToken"]),
                                    refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                    token_type = Convert.ToString(dtblrow["TokenType"]),
                                    expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                                };

                                string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                                DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                                DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                                if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                    authres = UpdateAccessToken(UserID, ref auth);

                                if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                                {
                                    foreach (DataRow row in dtbl.Rows)
                                    {
                                        int ReportID = Convert.ToInt32(row["ReportID"]);
                                        int RecordType = Convert.ToInt32(row["RecordType"]);
                                        String Snap_ReportID = Convert.ToString(row["Snap_ReportId"]);
                                        GetProductSnapReport(UserID, _profileId, ReportID, Snap_ReportID, RecordType, auth);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _throttlingHelper.Dispose();
        }

        //Inventory(sales etc.) reports
        public void ProcessInventoryReports(ReportStatus Status1, ReportStatus Status2)
        {
            DataTable dt, dt1;
            dt = APIData.GetReportData(Status1, maxCount);

            if (dt.Rows.Count > 0)
            {
                var reports = from row in dt.AsEnumerable()
                              group row by new { ID = row.Field<Int32>("MppUserID") } into grp
                              orderby grp.Key.ID
                              select new { SellerID = grp.Key.ID };

                if (reports.Count() > 0)
                {
                    foreach (var r in reports)
                    {
                        int UserID = r.SellerID;
                        DataTable dtbl = new DataTable();
                        dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("MppUserID") == UserID).CopyToDataTable();

                        if (dtbl.Rows.Count > 0)
                        {
                            string authres = "";
                            DataRow dtblrow = dtbl.Rows[0];

                            var auth = new AuthorizationModel()
                            {
                                access_token = Convert.ToString(dtblrow["AccessToken"]),
                                refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                token_type = Convert.ToString(dtblrow["TokenType"]),
                                expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                            };

                            string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                            DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                authres = UpdateAccessToken(UserID, ref auth);

                            //if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))//--comment by hari
                            //{
                                foreach (DataRow row in dtbl.Rows)
                                {
                                    int ReportID = Convert.ToInt32(row["ReportID"]);
                                    int RecordType = Convert.ToInt32(row["RecordType"]);
                                    DateTime ReportDate = Convert.ToDateTime(row["ReportDate"]);

                                    if (RecordType != 4)
                                        SetInventoryReport(UserID, _profileId, ReportID, ReportDate, RecordType, Status1, auth);
                                    else
                                        SetSearchTermReport(UserID, _profileId, ReportID, ReportDate, Status1, auth);
                                }
                           // }
                        }
                    }
                }
            }

            if (Status2 != ReportStatus.NOTREQUIRED)
            {
                dt1 = APIData.GetReportData(Status2, maxCount);
                if (dt1.Rows.Count > 0)
                {
                    var reports = from row in dt1.AsEnumerable()
                                  group row by new { ID = row.Field<Int32>("MppUserID") } into grp
                                  orderby grp.Key.ID
                                  select new { SellerID = grp.Key.ID };

                    if (reports.Count() > 0)
                    {
                        foreach (var r in reports)
                        {
                            int UserID = r.SellerID;
                            DataTable dtbl = new DataTable();
                            dtbl = dt1.AsEnumerable().Where(ro => ro.Field<Int32>("MppUserID") == UserID).CopyToDataTable();

                            if (dtbl.Rows.Count > 0)
                            {
                                string authres = "";
                                DataRow dtblrow = dtbl.Rows[0];

                                var auth = new AuthorizationModel()
                                {
                                    access_token = Convert.ToString(dtblrow["AccessToken"]),
                                    refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                    token_type = Convert.ToString(dtblrow["TokenType"]),
                                    expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                                };
                                string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                                DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                                DateTime TokenExpiryTime = _lastUpdateTime.AddDays(5);

                                if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                    authres = UpdateAccessToken(UserID, ref auth);

                                if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                                {
                                    foreach (DataRow row in dtbl.Rows)
                                    {
                                        int ReportID = Convert.ToInt32(row["ReportID"]);
                                        int RecordType = Convert.ToInt32(row["RecordType"]);
                                        String Amz_ReportID = Convert.ToString(row["Amz_ReportId"]);
                                        GetInventoryReport(UserID, _profileId, ReportID, Amz_ReportID, RecordType, Status2, auth);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _throttlingHelper.Dispose();
        }

        //Get ProductAds Count
        public String GetUserProductAdsCount(Int32 UserID, String ProfileId, DateTime LastUpdateTime, AuthorizationModel Auth, out string msg, List<CampaignStatus> Camplist)
        {
            msg = "";
            try
            {
                int StartIndex = 0;
                int PageCount = 5000;
                bool MoreRecords = false;
                var ListOfAds = new List<AdModel>();
                List<AdModel> adsList = new List<AdModel>();
                var campIds = Camplist.Where(s => s.Status == 1).Select(s => s.RecordID).ToArray();
                if (campIds.Length > 0)
                {
                    string _profileId = ProfileId;
                    DateTime _lastUpdateTime = LastUpdateTime;
                    DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(Auth.expires_in - 300);
                    if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                        msg = UpdateAccessToken(UserID, ref Auth);

                    var result = new List<AdModel>();
                    if (String.IsNullOrWhiteSpace(msg) && !String.IsNullOrWhiteSpace(_profileId))
                    {
                        do
                        {
                            result = GetUserSku(UserID, StartIndex, _profileId, Auth, out msg, campIds);
                            ListOfAds.AddRange(result);
                            if (result.Count >= PageCount)
                                MoreRecords = true;
                            else
                                MoreRecords = false;
                            StartIndex += PageCount;
                        } while (MoreRecords);
                    }
                    adsList = ListOfAds.Where(r => r.servingStatus == "AD_STATUS_LIVE").ToList();
                    msg = AddOrUpdateAds(adsList, UserID);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("GetUserProductAdsCount-" + ex.Message, UserID.ToString());
            }
            return msg;
        }

        //Keep keywords or SearchTerm optimizations
        public void ProcessOptimizations()
        {
            KeywordLocalOptimizations(); //Keep keywords or SearchTerm optimizations locally
            KeywordsGlobalOptimizations(); //Update keywords or SearchTerm optimizations globally

            if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
            {
                SearchTermLocalOptimizations();
                SearchTermGlobalOptimizations();
            }
        }

        //load campaigns on demand
        public object GetNewCampaignsOnDemand(int userId, DateTime LastUpdateTime, String ProfileId, AuthorizationModel auth)
        {
            var msg = "";
            string _profileId = ProfileId;
            DateTime _lastUpdateTime = LastUpdateTime;
            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);
            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                msg = UpdateAccessToken(userId, ref auth);
            if (String.IsNullOrWhiteSpace(msg) && !String.IsNullOrWhiteSpace(_profileId))
            {
                try
                {
                    var camps = GetNewCampaign(_profileId, auth);
                    var optimizeData = new OptimizeData();
                    var existing = optimizeData.GetMyCampaignsData(userId);

                    if (existing != null && existing.Count > 0)
                    {
                        var existsIds = existing.Select(se => se.RecordID).ToArray();
                        camps = camps.Where(s => !existsIds.Contains(s.campaignId) && s.state != "archived").ToList();
                    }
                    if (camps.Count > 0)
                        AddOrUpdateCampaigns(camps, userId, 0);
                    return new { Records = camps.Count, Status = 1 };
                }
                catch (Exception ex)
                {
                    LogAPI.WriteLog("GetNewCampaignsOnDemand-" + userId + " unable to fetch report" + ex.Message.ToString());
                    return new { Records = 0, Status = 2 };
                }
            }
            return new { Records = 0, Status = 3 };
        }

        //one time method to load skus for all active users
        public void LoadSkusForExistingUsers()
        {
            var dt = APIData.GetActiveCamapignsByUsers();
            var msg = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var userid = Convert.ToInt32(row["MppUserID"]);
                    var records = row["Records"].ToString().Split(',');
                    List<CampaignStatus> Camplist = new List<CampaignStatus>();

                    for (int i = 0; i < records.Length; i++)
                    {
                        Camplist.Add(new CampaignStatus { RecordID = Convert.ToInt64(records[i]), Status = 1 });
                    }
                    var profile = APIData.GetUserProfileForSkuData(userid);
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
                    GetUserProductAdsCount(userid, ProfileId, LastUpdatedOn, Auth, out msg, Camplist);
                    if (String.IsNullOrWhiteSpace(msg))
                    { 
                        DataTable dt1 = new DataTable();
                        dt1.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("RecordID", typeof(String)),
                        new DataColumn("Status", typeof(bool))
                        });

                        foreach (var row1 in Camplist)
                        {
                            dt1.Rows.Add();
                            dt1.Rows[dt1.Rows.Count - 1][0] = row1.RecordID;
                            dt1.Rows[dt1.Rows.Count - 1][1] = row1.Status;
                        }
                        var optmz = new OptimizeData();
                        optmz.RunCampaignData(dt1, userid);
                    }
                }
            }

        }

        //get all best srch term campains to create on amazon
        public void CreateBSTCampaignsOnAmazon()
        {
            var campaignsToCreate = APIData.GetBestSrchTermCampaignsToCreate();
            if (campaignsToCreate.Tables.Count == 2 && campaignsToCreate.Tables[0].Rows.Count > 0 && campaignsToCreate.Tables[1].Rows.Count > 0)
            {
                var data = campaignsToCreate.Tables[0];
                string authres = "";

                DataRow dtblrow = campaignsToCreate.Tables[1].Rows[0];

                Int32 mppuserId = Convert.ToInt32(dtblrow["MppUserID"]);

                var auth = new AuthorizationModel()
                {
                    access_token = Convert.ToString(dtblrow["AccessToken"]),
                    refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                    token_type = Convert.ToString(dtblrow["TokenType"]),
                    expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                };
                string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                    authres = UpdateAccessToken(mppuserId, ref auth);
                if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                {
                    List<BSTCampaignResponse> bstList = new List<BSTCampaignResponse>();
                    foreach (DataRow dr in data.Rows)
                    {

                        var camp = new CampaignModel()
                        {
                            name = Convert.ToString(dr["CampaignName"]),
                            campaignType = "sponsoredProducts",
                            dailyBudget = Convert.ToDouble(dr["DailyBudget"]),
                            startDate = GetAmazonDateFormat(DateTime.Now),
                            targetingType = "manual",
                            state = "enabled"
                        };

                        var respo = CreateCampaigns(camp, mppuserId, _profileId, auth);
                        if (respo.NewCampId > 0)
                            bstList.Add(respo);

                    }
                }
            }
        }

        #endregion

        #region Private Methods
        private void SetProductSnapReport(int UserId, int ReportId, int RecordType, String _profileId, AuthorizationModel auth)
        {
            String recordType = "";
            var res = new HttpResponseMessage();
            if (RecordType == 1)
                recordType = "campaigns";
            else if (RecordType == 2)
                recordType = "adGroups";
            else if (RecordType == 3)
                recordType = "keywords";

            var route = _version + "/" + recordType + "/snapshot";
            var uri = new Uri(_api, route);
            var obj = new
            {
                campaignType = "sponsoredProducts",
                stateFilter = "enabled,paused"
            };

            String content = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            var request = new HttpRequestMessage()
            {
                Content = httpContent,
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.Add("Authorization", Auth_Token);
            request.Headers.Add("Amazon-Advertising-API-Scope", _profileId);
            res = MakeRequestAsync(request).Result;

            if (res.IsSuccessStatusCode)
            {
                var ri = res.Content.ReadAsStringAsync().Result;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(ri);
                APIData.UpdateSnapStatusData(result.snapshotId, ReportId, RecordType, ResponseStatus.SUCCESS);
            }
            else
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    APIData.UpdateSnapStatusData(null, ReportId, RecordType, ResponseStatus.NOTFOUND);
                else if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("SetProductSnapReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private void SetInventoryReport(int UserId, string _profileId, int ReportId, DateTime ReportDate, int RecordType, ReportStatus status, AuthorizationModel auth)
        {
            String recordType = "";

            if (RecordType == 1)
                recordType = "campaigns";
            else if (RecordType == 2)
                recordType = "adGroups";
            else
                recordType = "keywords";

            string route = _version + "/" + recordType + "/report";
            var uri = new Uri(_api, route);

            var obj = new
            {
                campaignType = "sponsoredProducts",
                reportDate = MppUtility.DateFormat(ReportDate),
                metrics = "impressions,clicks,cost,attributedConversions1dSameSKU,attributedSales30d"
            };

            var content = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;

            var request = new HttpRequestMessage()
            {
                Content = httpContent,
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.Add("Authorization", Auth_Token);
            request.Headers.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = MakeRequestAsync(request).Result;
            if (res.IsSuccessStatusCode)
            {
                var ri = res.Content.ReadAsStringAsync().Result;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(ri);
                if (RecordType == 1)
                    APIData.UpdateInventoryStatusData(result.reportId, ReportId, RecordType, status, ResponseStatus.SUCCESS);
                else if (RecordType == 2)
                    APIData.UpdateInventoryStatusData(result.reportId, ReportId, RecordType, status, ResponseStatus.SUCCESS);
                else
                    APIData.UpdateInventoryStatusData(result.reportId, ReportId, RecordType, status, ResponseStatus.SUCCESS);
            }
            else
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    APIData.UpdateInventoryStatusData(null, ReportId, RecordType, status, ResponseStatus.NOTFOUND);
                else if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("SetInventoryReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private void SetSearchTermReport(int UserId, string _profileId, int ReportId, DateTime ReportDate, ReportStatus status, AuthorizationModel auth)
        {
            HttpClient hc = new HttpClient();
            String profileId = _profileId;
            var route = _version + "/keywords/report";
            var uri = new Uri(_api, route);
            var obj = new
            {
                campaignType = "sponsoredProducts",
                segment = "query",
                reportDate = MppUtility.DateFormat(ReportDate),
                metrics = "impressions,clicks,cost,attributedConversions1dSameSKU,attributedSales30d"
            };
            String content = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            var request = new HttpRequestMessage()
            {
                Content = httpContent,
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.Add("Authorization", Auth_Token);
            request.Headers.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = MakeRequestAsync(request).Result;
            if (res.IsSuccessStatusCode)
            {
                var ri = res.Content.ReadAsStringAsync().Result;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(ri);
                APIData.UpdateInventoryStatusData(result.reportId, ReportId, (int)RecordType.SearchTerm, status, ResponseStatus.SUCCESS);
            }
            else
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    APIData.UpdateInventoryStatusData(null, ReportId, (int)RecordType.SearchTerm, status, ResponseStatus.NOTFOUND);
                else if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, (int)RecordType.SearchTerm);
                LogAPI.WriteLog("SetSearchTermReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private void GetProductSnapReport(int UserId, string _profileId, int ReportId, String Snap_ReportID, int RecordType, AuthorizationModel auth)
        {
            var route = _version + "/snapshots/" + Snap_ReportID;
            var uri = new Uri(_api, route);
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", Auth_Token);
            request.Headers.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = MakeRequestAsync(request).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(res.Content.ReadAsStringAsync().Result);
                if (result.status.ToLower() == "success")
                    ExtractProductSnapReport(UserId, _profileId, ReportId, result.location, RecordType, auth);
            }
            else
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    APIData.UpdateSnapStatusData(null, ReportId, RecordType, ResponseStatus.NOTFOUND);
                else if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("GetProductSnapReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private void GetInventoryReport(int UserId, string _profileId, int ReportId, String Amz_ReportID, int RecordType, ReportStatus status, AuthorizationModel auth)
        {
            var route = _version + "/reports/" + Amz_ReportID;
            var uri = new Uri(_api, route);
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", Auth_Token);
            request.Headers.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = MakeRequestAsync(request).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(res.Content.ReadAsStringAsync().Result);
                if (result.status.ToLower() == "success")
                    ExtractInventoryReport(UserId, _profileId, ReportId, result.location, RecordType, status, auth);
            }
            else
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    APIData.UpdateInventoryStatusData(null, ReportId, RecordType, status, ResponseStatus.NOTFOUND);
                else if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("GetInventoryReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private List<AdModel> GetUserSku(int UserID, int startIndex, string _profileId, AuthorizationModel auth, out string msg, Int64[] campIds)
        {
            msg = "";
            var result = new List<AdModel>();
            var route = "v1/productAds/extended?startIndex=" + startIndex + "&stateFilter=enabled&campaignIdFilter=" + string.Join(",", campIds);
            HttpClient hc = new HttpClient();
            hc.BaseAddress = _api;
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            String profileId = _profileId;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(route).Result;
            if (res.IsSuccessStatusCode)
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdModel>>(res.Content.ReadAsStringAsync().Result);
            else
            {
                msg = "Failed to get user SKU's";
                LogAPI.WriteLog("GetUserSku-" + res.ReasonPhrase, UserID.ToString());
            }
            return result;
        }

        private void UpdateKeyBidsOrStatusOnAmz(List<object> listOfKeys, string _profileId, AuthorizationModel auth, int ReportID, IEnumerable<DataRow> KeywordsLog)
        {
            HttpClient hc = new HttpClient();
            var route = _version + "/keywords";
            hc.BaseAddress = _api;
            var response = Newtonsoft.Json.JsonConvert.SerializeObject(listOfKeys);
            String content = response;
            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = hc.PutAsync(route, httpContent).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeywordResponse>>(res.Content.ReadAsStringAsync().Result);
                UpdateOptimizeLogStatus(KeywordsLog, ReportID, RecordType.Keyword, result);
            }
            else
            {
                LogAPI.WriteLog("UpdateKeyBidsOrStatusOnAmz-" + ReportID + ": " + res.ReasonPhrase);
            }
        }

        private void CreateNegKeywordsOnAmz(List<object> listOfNegKeys, String _profileId, AuthorizationModel auth, int ReportID, IEnumerable<DataRow> LogModel)
        {
            HttpClient hc = new HttpClient();
            var route = _version + "/negativeKeywords";
            hc.BaseAddress = _api;
            var response = Newtonsoft.Json.JsonConvert.SerializeObject(listOfNegKeys);
            String content = response;
            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = hc.PostAsync(route, httpContent).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeywordResponse>>(res.Content.ReadAsStringAsync().Result);
                var res1 = result.Where(r => r.keywordId != null);
                var res2 = result.Where(r => r.code == "INVALID_ARGUMENT");
                UpdateOptimizeLogStatus(LogModel, ReportID, RecordType.SearchTerm, result);
            }
            else
            {
                LogAPI.WriteLog("CreateNegKeywordsOnAmz-" + ReportID + ": " + res.ReasonPhrase);
            }
        }

        private void ExtractProductSnapReport(int UserId, string _profileId, int ReportId, String Location, int RecordType, AuthorizationModel auth)
        {
            HttpClient hc = new HttpClient();
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            String profileId = _profileId;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(Location).Result;
            if (res.IsSuccessStatusCode)
            {
                var r = res.Content.ReadAsByteArrayAsync().Result;
                var reportdata = UnzipReportEntry(r);
                if (!String.IsNullOrWhiteSpace(reportdata))
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProductModel>>(reportdata);
                    if (result.Count > 0)
                    {
                        if (RecordType == 1)
                            AddOrUpdateCampaigns(result, UserId, ReportId);
                        else if (RecordType == 2)
                            AddOrUpdateAdgroups(result, UserId, ReportId);
                        else if (RecordType == 3)
                            AddOrUpdateKeywords(result, UserId, ReportId);
                    }
                }
            }
            else
            {
                if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("ExtractProductSnapReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private void ExtractInventoryReport(int UserId, string _profileId, int ReportId, String Location, int RecordType, ReportStatus status, AuthorizationModel auth)
        {
            HttpClient hc = new HttpClient();
            String Auth_Token = auth.token_type.PadRight(7) + auth.access_token;
            String profileId = _profileId;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(Location).Result;
            if (res.IsSuccessStatusCode)
            {
                var r = res.Content.ReadAsByteArrayAsync().Result;
                var reportdata = UnzipReportEntry(r);
                var model = new List<InventoryModel>();
                if (!String.IsNullOrWhiteSpace(reportdata))
                    model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InventoryModel>>(reportdata);
                if (model.Count > 0)
                {
                    if (RecordType < 4)
                        AddOrUpdateProductInventory(model, UserId, ReportId, RecordType, status);
                    else
                        AddOrUpdateSearchTermInventory(model, UserId, ReportId, status);
                }
                else
                {
                    APIData.UpdateInventoryStatusData(null, ReportId, RecordType, status, ResponseStatus.NORECORDS);
                }
            }
            else
            {
                if (res.StatusCode != (System.Net.HttpStatusCode)429 || res.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                    APIData.UpdateAPIAttemptStatusData(ReportId, RecordType);
                LogAPI.WriteLog("ExtractInventoryReport-" + ReportId + ": " + res.ReasonPhrase, UserId.ToString());
            }
        }

        private static String UnzipReportEntry(byte[] zipped)
        {
            string output = "";
            using (var memoryStream = new MemoryStream(zipped))
            {
                using (var bigStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var bigStreamOut = new MemoryStream())
                    {
                        bigStream.CopyTo(bigStreamOut);
                        output = Encoding.UTF8.GetString(bigStreamOut.ToArray());
                    }
                }
            }
            return output;
        }

        private string UpdateAccessToken(int UserId, ref AuthorizationModel authres)
        {
            string msg = "";
            try
            {
                var _api = "https://api.amazon.com";
                HttpClient hc = new HttpClient();
                var route = "/auth/o2/token";
                hc.BaseAddress = new Uri(_api);
                String content = "grant_type=refresh_token&client_id=" + _clientkey + "&client_secret=" + _clientSecretkey + "&refresh_token=" + authres.refresh_token;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = hc.PostAsync(route, httpContent).Result;
                if (res.IsSuccessStatusCode)
                {
                    var res1 = res.Content.ReadAsStringAsync().Result;
                    authres = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationModel>(res1);
                    msg = APIData.UpdateAccessTokenData(UserId, authres.access_token, authres.refresh_token, authres.token_type, authres.expires_in);
                }
                else
                {
                    msg = "Failed - AccessToken";
                    LogAPI.WriteLog("UpdateAccessToken-UserId:" + UserId + ": " + res.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("UpdateAccessToken-UserId:" + UserId + ": " + msg);
            }
            return msg;
        }

        private void AddOrUpdateCampaigns(List<ProductModel> cmodel, int userId, int reportId)
        {
            try
            {
                String emailnotify = "", msg = "";
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[7] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("Name", typeof(String)),
                    new DataColumn("DailyBudget", typeof(double)),
                    new DataColumn("StartDate", typeof(DateTime)),
                    new DataColumn("EndDate", typeof(DateTime)),
                    new DataColumn("TargetingType", typeof(String)),
                    new DataColumn("Status", typeof(String))
                    });

                foreach (var r in cmodel)
                {
                    DateTime StartDate, EndDate;
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = r.campaignId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.name;
                    dt.Rows[dt.Rows.Count - 1][2] = r.dailyBudget;

                    MppUtility.StringToDateFormat(r.startDate, out StartDate);
                    MppUtility.StringToDateFormat(r.endDate, out EndDate);
                    if (StartDate == DateTime.MinValue)
                        dt.Rows[dt.Rows.Count - 1][3] = DBNull.Value;
                    else
                        dt.Rows[dt.Rows.Count - 1][3] = StartDate;
                    if (EndDate == DateTime.MinValue)
                        dt.Rows[dt.Rows.Count - 1][4] = DBNull.Value;
                    else
                        dt.Rows[dt.Rows.Count - 1][4] = EndDate;

                    dt.Rows[dt.Rows.Count - 1][5] = r.targetingType;
                    dt.Rows[dt.Rows.Count - 1][6] = r.state;

                }

                if (dt.Rows.Count > 0)
                    msg = APIData.UpdateCampaignData(dt, userId, reportId, out emailnotify);
                if (msg != "" && reportId > 0)
                    APIData.UpdateReportStatusData(1, reportId, 1, ReportStatus.SET, ResponseStatus.ERROR_SQL);
                if (!String.IsNullOrWhiteSpace(emailnotify))
                    EmailAlert.SendNewCampaignsAlert(emailnotify);
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("AddOrUpdateCampaigns-" + reportId + ": " + ex.Message, userId.ToString());
            }
        }

        private void AddOrUpdateAdgroups(List<ProductModel> adgmodel, int userId, int reportId)
        {
            try
            {
                String msg = "";
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("CampaignID", typeof(Int64)),
                    new DataColumn("Name", typeof(String)),
                    new DataColumn("DailyBudget", typeof(double)),
                    new DataColumn("Bid", typeof(double)),
                    new DataColumn("Status", typeof(String))
                    });

                foreach (var r in adgmodel)
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = r.adGroupId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.campaignId;
                    dt.Rows[dt.Rows.Count - 1][2] = r.name;
                    dt.Rows[dt.Rows.Count - 1][3] = r.dailyBudget;
                    dt.Rows[dt.Rows.Count - 1][4] = r.defaultBid;
                    dt.Rows[dt.Rows.Count - 1][5] = r.state;
                }

                if (dt.Rows.Count > 0)
                    msg = APIData.UpdateAdGroupData(dt, userId, reportId);
                if (msg != "")
                    APIData.UpdateReportStatusData(1, reportId, 2, ReportStatus.SET, ResponseStatus.ERROR_SQL);
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("AddOrUpdateAdgroups-" + reportId + ": " + ex.Message, userId.ToString());
            }
        }

        private void AddOrUpdateKeywords(List<ProductModel> kmodel, int userId, int reportId)
        {
            try
            {
                String msg = "";
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("CampaignID", typeof(Int64)),
                    new DataColumn("AdGroupID", typeof(Int64)),
                    new DataColumn("Keyword", typeof(String)),
                    new DataColumn("DailyBudget", typeof(double)),
                    new DataColumn("MatchType", typeof(String)),
                    new DataColumn("Bid", typeof(double)),
                    new DataColumn("Status", typeof(String))
                    });

                foreach (var r in kmodel)
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = r.keywordId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.campaignId;
                    dt.Rows[dt.Rows.Count - 1][2] = r.adGroupId;
                    dt.Rows[dt.Rows.Count - 1][3] = r.keywordText;
                    dt.Rows[dt.Rows.Count - 1][4] = r.dailyBudget;
                    dt.Rows[dt.Rows.Count - 1][5] = r.matchType;
                    dt.Rows[dt.Rows.Count - 1][6] = r.bid;
                    dt.Rows[dt.Rows.Count - 1][7] = r.state;
                }

                if (dt.Rows.Count > 0)
                    msg = APIData.UpdateKeywordData(dt, userId, reportId);
                if (msg != "")
                    APIData.UpdateReportStatusData(1, reportId, 3, ReportStatus.SET, ResponseStatus.ERROR_SQL);
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("AddOrUpdateKeywords-" + reportId + ": " + ex.Message, userId.ToString());
            }
        }

        private string AddOrUpdateAds(List<AdModel> amodel, int userId)
        {
            String msg = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[4] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("CampaignID", typeof(Int64)),
                    new DataColumn("AdGroupID", typeof(Int64)),
                    new DataColumn("Sku", typeof(String))
                    });

                foreach (var r in amodel)
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = r.adId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.campaignId;
                    dt.Rows[dt.Rows.Count - 1][2] = r.adGroupId;
                    dt.Rows[dt.Rows.Count - 1][3] = r.sku;
                }

                msg = APIData.UpdateAdsData(dt, userId);
            }

            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("AddOrUpdateAds- " + ex.Message, userId.ToString());
            }
            return msg;
        }

        private void AddOrUpdateProductInventory(List<InventoryModel> rmodel, int userId, int reportId, int type, ReportStatus status)
        {
            try
            {
                long RecordId = 0;
                String emailtoNotify = "", msg = "";
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("Impressions", typeof(int)),
                    new DataColumn("Clicks", typeof(int)),
                    new DataColumn("Spend", typeof(decimal)),
                    new DataColumn("Orders", typeof(int)),
                    new DataColumn("Sales", typeof(decimal))
                    });

                foreach (var r in rmodel)
                {
                    if (type == 1)
                        RecordId = r.campaignId;
                    else if (type == 2)
                        RecordId = r.adGroupId;
                    else
                        RecordId = r.keywordId;

                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = RecordId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.impressions;
                    dt.Rows[dt.Rows.Count - 1][2] = r.clicks;
                    dt.Rows[dt.Rows.Count - 1][3] = r.cost;
                    dt.Rows[dt.Rows.Count - 1][4] = r.attributedConversions1dSameSKU;
                    dt.Rows[dt.Rows.Count - 1][5] = r.attributedSales30d;
                }

                if (dt.Rows.Count > 0)
                    msg = APIData.UpdateProductInventoryData(dt, userId, reportId, (int)type, status, out emailtoNotify);
                if (msg != "")
                    APIData.UpdateReportStatusData(2, reportId, type, status, ResponseStatus.ERROR_SQL);

                if (!String.IsNullOrWhiteSpace(emailtoNotify))
                {
                    String emailmsg = EmailAlert.SendNewUserDataAlert(emailtoNotify);
                    if (emailmsg == "")
                        APIData.UpdateFirstEmailNotify(userId, 1);
                }
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("AddOrUpdateProductInventory-" + reportId + ": " + ex.Message, userId.ToString());
            }
        }

        private void AddOrUpdateSearchTermInventory(List<InventoryModel> rmodel, int userId, int reportId, ReportStatus status)
        {
            try
            {
                String msg = "";
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[7] {
                    new DataColumn("RecordID", typeof(Int64)),
                    new DataColumn("Query", typeof(String)),
                    new DataColumn("Impressions", typeof(int)),
                    new DataColumn("Clicks", typeof(int)),
                    new DataColumn("Spend", typeof(Decimal)),
                    new DataColumn("Orders", typeof(int)),
                    new DataColumn("Sales", typeof(Decimal))
                    });

                foreach (var r in rmodel)
                {
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = r.keywordId;
                    dt.Rows[dt.Rows.Count - 1][1] = r.query;
                    dt.Rows[dt.Rows.Count - 1][2] = r.impressions;
                    dt.Rows[dt.Rows.Count - 1][3] = r.clicks;
                    dt.Rows[dt.Rows.Count - 1][4] = r.cost;
                    dt.Rows[dt.Rows.Count - 1][5] = r.attributedConversions1dSameSKU;
                    dt.Rows[dt.Rows.Count - 1][6] = r.attributedSales30d;
                }

                if (dt.Rows.Count > 0)
                    msg = APIData.UpdateSearchTermInventoryData(dt, userId, reportId, status);
                if (msg != "")
                    APIData.UpdateReportStatusData(2, reportId, 4, status, ResponseStatus.ERROR_SQL);
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("AddOrUpdateSearchTermInventory-" + reportId + ": " + ex.Message, userId.ToString());
            }
        }

        private void UpdateOptimizeLogStatus(IEnumerable<DataRow> LogModel, int ReportID, RecordType type, List<KeywordResponse> ResModel)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[10] {
                        new DataColumn("CampaignID", typeof(Int64)),
                        new DataColumn("AdgroupID", typeof(Int64)),
                        new DataColumn("KeywordID", typeof(Int64)),
                        new DataColumn("Query", typeof(String)),
                        new DataColumn("ModifiedField", typeof(String)),
                        new DataColumn("OldValue", typeof(String)),
                        new DataColumn("NewValue", typeof(String)),
                        new DataColumn("ReasonID", typeof(Int32)),
                        new DataColumn("ReasonValue", typeof(String)),
                        new DataColumn("UpdateStatus", typeof(Int16))
                        });

                int i = 0;
                foreach (var r in LogModel)
                {
                    long KeyowrdID = Convert.ToInt64(r["KeywordID"]);
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1][0] = Convert.ToInt64(r["CampaignID"]);
                    dt.Rows[dt.Rows.Count - 1][1] = Convert.ToInt64(r["AdgroupID"]);
                    dt.Rows[dt.Rows.Count - 1][2] = KeyowrdID;
                    dt.Rows[dt.Rows.Count - 1][3] = ((int)type == 3) ? null : Convert.ToString(r["Query"]);
                    dt.Rows[dt.Rows.Count - 1][4] = null;
                    dt.Rows[dt.Rows.Count - 1][5] = null;
                    dt.Rows[dt.Rows.Count - 1][6] = null;
                    dt.Rows[dt.Rows.Count - 1][7] = 0;
                    dt.Rows[dt.Rows.Count - 1][8] = null;
                    if ((int)type == 3)
                        dt.Rows[dt.Rows.Count - 1][9] = ResModel[i].code == "SUCCESS" ? 1 : -1;
                    else
                        dt.Rows[dt.Rows.Count - 1][9] = ResModel[i].keywordId == null ? -1 : 1;
                    i++;
                }

                if (dt.Rows.Count > 0)
                    APIData.UpdateOptimizeLogData(ReportID, dt, type);
            }

            catch (Exception ex)
            {
                LogAPI.WriteLog("UpdateOptimizeLogStatus-" + ReportID + ": " + ex.Message);
            }
        }

        private void KeywordLocalOptimizations()
        {
            var dt = APIData.GetUpldUsersForKeywordOptimize();
            DataTable tbl, tbl1;
            int UserId = 0, ReportID = 0;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow ro in dt.Rows)
                    {
                        DataTable _tbl = new DataTable();
                        _tbl.Columns.AddRange(new DataColumn[10] {
                        new DataColumn("CampaignID", typeof(Int64)),
                        new DataColumn("AdgroupID", typeof(Int64)),
                        new DataColumn("KeywordID", typeof(Int64)),
                        new DataColumn("Query", typeof(String)),
                        new DataColumn("ModifiedField", typeof(String)),
                        new DataColumn("OldValue", typeof(String)),
                        new DataColumn("NewValue", typeof(String)),
                        new DataColumn("ReasonID", typeof(Int32)),
                        new DataColumn("ReasonValue", typeof(String)),
                        new DataColumn("UpdateStatus", typeof(Int16))
                        });

                        UserId = Convert.ToInt32(ro["MppUserID"]);
                        ReportID = Convert.ToInt32(ro["ReportID"]);
                        DataSet ds = APIData.GetOptimizeKeywordsAndFormula(UserId);

                        if (ds.Tables.Count > 0)
                        {
                            tbl = ds.Tables[0];
                            tbl1 = ds.Tables[1];

                            if (tbl.Rows.Count > 0)
                            {
                                foreach (DataRow row in tbl.Rows)
                                {
                                    long CampaignId = Convert.ToInt64(row["CampaignID"]);
                                    Decimal AcosPause = Convert.ToDecimal(row["AcosPause"]);
                                    Decimal AcosLower = Convert.ToDecimal(row["AcosLower"]);
                                    Decimal AcosRaise = Convert.ToInt32(row["AcosRaise"]);
                                    Decimal SpendPause = Convert.ToInt32(row["SpendPause"]);
                                    Decimal SpendLower = Convert.ToInt32(row["SpendLower"]);
                                    Decimal ClicksPause = Convert.ToDecimal(row["ClicksPause"]);
                                    Decimal ClicksLower = Convert.ToDecimal(row["ClicksLower"]);
                                    double BidRaise = Convert.ToDouble(row["BidRaise"]) / 100;
                                    double BidLower = Convert.ToDouble(row["BidLower"]) / 100;
                                    Decimal MinBid = Convert.ToDecimal(row["MinBid"]);
                                    Decimal MaxBid = Convert.ToDecimal(row["MaxBid"]);

                                    var Keywords = tbl1.AsEnumerable().Where(r => r.Field<Int64>("CampaignID").Equals(CampaignId)).ToList();

                                    if (Keywords.Count() > 0)
                                    {
                                        foreach (var k in Keywords)
                                        {
                                            try
                                            {
                                                String ModifiedField = "", OldValue = "", NewValue = "";
                                                Int32 ReasonID = 0;
                                                Decimal Sales = Convert.ToDecimal(k["Sales"]);
                                                Int32 Orders = Convert.ToInt32(k["Orders"]);
                                                Decimal Spend = Convert.ToDecimal(k["Spend"]);
                                                Int32 Impressions = Convert.ToInt32(k["Impressions"]);
                                                Int32 Clicks = Convert.ToInt32(k["Clicks"]);
                                                Decimal Bid = Convert.ToDecimal(k["Bid"]);
                                                String Status = Convert.ToString(k["Status"]);
                                                Decimal Acos = (Sales != 0) ? System.Math.Round((Spend / Sales) * 100, 2, MidpointRounding.AwayFromZero) : 0;
                                                long CTRPause = Convert.ToInt64(row["CTRPause"]);
                                                long CTRLower = Convert.ToInt64(row["CTRLower"]);
                                                Decimal CTR = (Impressions != 0) ? System.Math.Round(((Decimal)Clicks / Impressions) * 100, 2, MidpointRounding.AwayFromZero) : 0;
                                                String OldStatus = Status, NewStatus = "";
                                                Decimal OldBid = Bid, NewBid = 0, ReasonValue = 0;
                                                bool IsUpdated = false;
                                                GetNewCTR(Clicks, ref CTRPause);
                                                GetNewCTR(Clicks, ref CTRLower);

                                                if (Acos > AcosPause)
                                                {
                                                    ModifiedField = "Status";
                                                    ReasonID = 1;
                                                    NewStatus = "paused";
                                                    ReasonValue = Acos;
                                                }
                                                else if (Spend > SpendPause && Sales == 0)
                                                {
                                                    ModifiedField = "Status";
                                                    ReasonID = 2;
                                                    NewStatus = "paused";
                                                    ReasonValue = Spend;
                                                }
                                                else if (Clicks > ClicksPause && Sales == 0)
                                                {
                                                    ModifiedField = "Status";
                                                    ReasonID = 3;
                                                    NewStatus = "paused";
                                                    ReasonValue = Clicks;
                                                }
                                                else if (Impressions > CTRPause)
                                                {
                                                    ModifiedField = "Status";
                                                    ReasonID = 4;
                                                    NewStatus = "paused";
                                                    ReasonValue = Impressions;
                                                }
                                                else if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
                                                {
                                                    if (Acos < AcosPause && Acos >= AcosLower)
                                                    {
                                                        Bid = Bid - Convert.ToDecimal(BidLower * (Double)Bid);
                                                        Bid = System.Math.Round(Bid, 2, MidpointRounding.AwayFromZero);
                                                        if (Bid >= MinBid)
                                                        {
                                                            ModifiedField = "MaxBid";
                                                            ReasonID = 5;
                                                            NewBid = Bid;
                                                            ReasonValue = Acos;
                                                        }
                                                    }
                                                    else if ((Spend < SpendPause && Spend >= SpendLower) && Sales == 0)
                                                    {
                                                        Bid = Bid - Convert.ToDecimal(BidLower * (Double)Bid);
                                                        Bid = System.Math.Round(Bid, 2, MidpointRounding.AwayFromZero);
                                                        if (Bid >= MinBid)
                                                        {
                                                            ModifiedField = "MaxBid";
                                                            ReasonID = 6;
                                                            NewBid = Bid;
                                                            ReasonValue = Spend;
                                                        }
                                                    }
                                                    else if ((Clicks < ClicksPause && Clicks >= ClicksLower) && Sales == 0)
                                                    {
                                                        Bid = Bid - Convert.ToDecimal(BidLower * (Double)Bid);
                                                        Bid = System.Math.Round(Bid, 2, MidpointRounding.AwayFromZero);
                                                        if (Bid >= MinBid)
                                                        {
                                                            ModifiedField = "MaxBid";
                                                            ReasonID = 7;
                                                            NewBid = Bid;
                                                            ReasonValue = Clicks;
                                                        }
                                                    }
                                                    else if (Impressions < CTRPause && Impressions >= CTRLower)
                                                    {
                                                        Bid = Bid - Convert.ToDecimal(BidLower * (Double)Bid);
                                                        Bid = System.Math.Round(Bid, 2, MidpointRounding.AwayFromZero);
                                                        if (Bid >= MinBid)
                                                        {
                                                            ModifiedField = "MaxBid";
                                                            ReasonID = 8;
                                                            NewBid = Bid;
                                                            ReasonValue = Impressions;
                                                        }
                                                    }
                                                    else if (Acos < AcosRaise && Acos != 0)
                                                    {
                                                        Bid = Bid + Convert.ToDecimal(BidRaise * (Double)Bid);
                                                        Bid = System.Math.Round(Bid, 2, MidpointRounding.AwayFromZero);
                                                        if (Bid <= MaxBid)
                                                        {
                                                            ModifiedField = "MaxBid";
                                                            ReasonID = 9;
                                                            NewBid = Bid;
                                                            ReasonValue = Acos;
                                                        }
                                                    }
                                                }

                                                if (!String.IsNullOrWhiteSpace(NewStatus) && OldStatus.ToLower() != NewStatus)
                                                {
                                                    OldValue = OldStatus;
                                                    NewValue = NewStatus;
                                                    IsUpdated = true;
                                                }
                                                else if (NewBid != 0 && OldBid != NewBid)
                                                {
                                                    OldValue = OldBid.ToString();
                                                    NewValue = NewBid.ToString();
                                                    IsUpdated = true;
                                                }

                                                if (IsUpdated)
                                                {
                                                    _tbl.Rows.Add();
                                                    _tbl.Rows[_tbl.Rows.Count - 1][0] = Convert.ToInt64(k["CampaignID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][1] = Convert.ToInt64(k["AdgroupID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][2] = Convert.ToInt64(k["KeywordID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][3] = null;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][4] = ModifiedField;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][5] = OldValue;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][6] = NewValue;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][7] = ReasonID;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][8] = ReasonValue;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][9] = 0;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                LogAPI.WriteLog("Failed to apply approach for bid optimize on CampaignId: " + CampaignId + ", ReportId:" + ReportID + "- " + ex.Message, UserId.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (_tbl.Rows.Count > 0)
                            APIData.UpdateOptimizeLogData(ReportID, _tbl, RecordType.Keyword);
                        else
                            APIData.UpdateOptimizeLocalReportProcess(ReportID, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                APIData.UpdateOptimizeLocalReportProcess(ReportID, -1);
                LogFile.WriteLog("KeywordLocalOptimizations - " + ReportID + ":" + ex.Message, UserId.ToString());
            }
        }

        private void SearchTermLocalOptimizations()
        {
            var dt = APIData.GetUpldUsersForSearchTermOptimize();
            DataTable tbl, tbl1;
            int UserId = 0, ReportID = 0;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow ro in dt.Rows)
                    {
                        DataTable _tbl = new DataTable();
                        _tbl.Columns.AddRange(new DataColumn[10] {
                        new DataColumn("CampaignID", typeof(Int64)),
                        new DataColumn("AdgroupID", typeof(Int64)),
                        new DataColumn("KeywordID", typeof(Int64)),
                        new DataColumn("Query", typeof(String)),
                        new DataColumn("ModifiedField", typeof(String)),
                        new DataColumn("OldValue", typeof(String)),
                        new DataColumn("NewValue", typeof(String)),
                        new DataColumn("ReasonID", typeof(Int32)),
                        new DataColumn("ReasonValue", typeof(String)),
                        new DataColumn("UpdateStatus", typeof(Int16))
                        });

                        UserId = Convert.ToInt32(ro["MppUserID"]);
                        ReportID = Convert.ToInt32(ro["ReportID"]);

                        DataSet ds = APIData.GetOptimizeSearchTermsAndFormula(UserId);

                        if (ds.Tables.Count > 0)
                        {
                            tbl = ds.Tables[0];
                            tbl1 = ds.Tables[1];

                            if (tbl.Rows.Count > 0)
                            {
                                foreach (DataRow row in tbl.Rows)
                                {
                                    long CampaignId = Convert.ToInt64(row["CampaignID"]);
                                    Decimal AcosNegative = Convert.ToDecimal(row["AcosNegative"]);
                                    Decimal SpendNegative = Convert.ToDecimal(row["SpendNegative"]);
                                    Decimal ClicksNegative = Convert.ToDecimal(row["ClicksNegative"]);

                                    var SearchTerm = tbl1.AsEnumerable().Where(r => r.Field<Int64>("CampaignID").Equals(CampaignId)).ToList();

                                    if (SearchTerm.Count() > 0)
                                    {
                                        foreach (var k in SearchTerm)
                                        {
                                            try
                                            {
                                                Int32 ReasonID = 0;
                                                String KeyName = Convert.ToString(k["Query"]);
                                                Decimal Sales = Convert.ToDecimal(k["Sales"]);
                                                Int32 Impressions = Convert.ToInt32(k["Impressions"]);
                                                Decimal Spend = Convert.ToDecimal(k["Spend"]);
                                                Int32 Clicks = Convert.ToInt32(k["Clicks"]);
                                                Decimal Acos = Sales != 0 ? System.Math.Round((Spend / Sales) * 100, 2, MidpointRounding.AwayFromZero) : 0;
                                                Int32 Orders = Convert.ToInt32(k["Orders"]);
                                                String ModifiedField = "MatchType";
                                                String NewValue = "Negative Exact";
                                                bool IsUpdated = false;
                                                Decimal ReasonValue = 0;
                                                long CTRNegative = Convert.ToInt64(row["CTRNegative"]);

                                                CTRNegative = GetNewCTR(Clicks, ref CTRNegative);
                                                if (Acos > AcosNegative)
                                                {
                                                    ReasonID = 10;
                                                    ReasonValue = Acos;
                                                    IsUpdated = true;
                                                }
                                                else if (Spend > SpendNegative && Sales == 0)
                                                {
                                                    ReasonID = 11;
                                                    ReasonValue = Spend;
                                                    IsUpdated = true;
                                                }
                                                else if (Impressions > CTRNegative)
                                                {
                                                    ReasonID = 12;
                                                    ReasonValue = Impressions;
                                                    IsUpdated = true;
                                                }

                                                if (IsUpdated)
                                                {
                                                    _tbl.Rows.Add();
                                                    _tbl.Rows[_tbl.Rows.Count - 1][0] = Convert.ToInt64(k["CampaignID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][1] = Convert.ToInt64(k["AdgroupID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][2] = Convert.ToInt64(k["KeywordID"]);
                                                    _tbl.Rows[_tbl.Rows.Count - 1][3] = KeyName;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][4] = ModifiedField;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][5] = null;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][6] = NewValue;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][7] = ReasonID;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][8] = ReasonValue;
                                                    _tbl.Rows[_tbl.Rows.Count - 1][9] = 0;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                LogAPI.WriteLog("Failed to apply approach on SearchTerm Campaign: " + CampaignId + ", ReportId:" + ReportID + "- " + ex.Message, UserId.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (_tbl.Rows.Count > 0)
                            APIData.UpdateOptimizeLogData(ReportID, _tbl, RecordType.SearchTerm);
                        else
                            APIData.UpdateOptimizeLocalSearchReportProcess(ReportID, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                APIData.UpdateOptimizeLocalSearchReportProcess(ReportID, -1);
                LogFile.WriteLog("SearchTermLocalOptimizations - " + ReportID + ":" + ex.Message, UserId.ToString());
            }
        }

        public string ManualKeywordUpdate(Int64 KeyID, Int64 CID, Int64 AID, Int32 RID, Int32 RSNID, String ModifiedOn, Int32 UserID)
        {
            DateTime d = Convert.ToDateTime(ModifiedOn);
            string res = "";
            var ds = APIData.GetUpdatedData(KeyID, CID, AID, RID, RSNID, d, UserID);
            if (RSNID > 0 && RSNID < 10)
            {

                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    var reports = from row in dt.AsEnumerable()
                                  group row by new { ID = row.Field<Int32>("ReportID") } into grp
                                  orderby grp.Key.ID
                                  select new { ReportID = grp.Key.ID };

                    if (reports.Count() > 0)
                    {
                        foreach (var r in reports)
                        {
                            int ReportID = r.ReportID;
                            DataTable dtbl = new DataTable();
                            dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("ReportID") == ReportID).CopyToDataTable();

                            if (dtbl.Rows.Count > 0)
                            {
                                string authres = "";
                                DataRow dtblrow = dtbl.Rows[0];

                                Int32 mppuserId = Convert.ToInt32(dtblrow["MppUserID"]);

                                var auth = new AuthorizationModel()
                                {
                                    access_token = Convert.ToString(dtblrow["AccessToken"]),
                                    refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                    token_type = Convert.ToString(dtblrow["TokenType"]),
                                    expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                                };
                                string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                                DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                                DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                                if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                    authres = UpdateAccessToken(mppuserId, ref auth);

                                if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                                {
                                    decimal Total = dtbl.Rows.Count;
                                    int Start = 0;
                                    int Skip = 0;
                                    int PageSize = 1000;
                                    decimal MaxCalls = Math.Ceiling(Total / PageSize);

                                    while (Start < MaxCalls)
                                    {
                                        Skip = Start * PageSize;
                                        var KeywordsLog = dtbl.AsEnumerable().Skip(Skip).Take(PageSize);
                                        var ListofKeywords = new List<object>();

                                        foreach (DataRow row in KeywordsLog)
                                        {
                                            long KeywordID = Convert.ToInt64(row["KeywordID"]);
                                            String ModifiedField = Convert.ToString(row["ModifiedField"]);
                                            String ModifiedValue = Convert.ToString(row["NewValue"]);

                                            if (ModifiedField.ToLower() == "status")
                                            {
                                                ListofKeywords.Add(new
                                                {
                                                    keywordId = KeywordID,
                                                    state = ModifiedValue
                                                });
                                            }
                                            else if (ModifiedField.ToLower() == "maxbid")
                                            {
                                                ListofKeywords.Add(new
                                                {
                                                    keywordId = KeywordID,
                                                    bid = ModifiedValue
                                                });
                                            }
                                        }

                                        if (ListofKeywords.Count > 0)
                                        {
                                            UpdateKeyBidsOrStatusOnAmz(ListofKeywords, _profileId, auth, ReportID, KeywordsLog);
                                            Start += 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var dt = ds.Tables[1];

                var reports = from row in dt.AsEnumerable()
                              group row by new { ID = row.Field<Int32>("ReportID") } into grp
                              orderby grp.Key.ID
                              select new { ReportID = grp.Key.ID };

                if (reports.Count() > 0)
                {
                    foreach (var r in reports)
                    {
                        int ReportID = r.ReportID;
                        DataTable dtbl = new DataTable();
                        dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("ReportID") == ReportID).CopyToDataTable();

                        if (dtbl.Rows.Count > 0)
                        {
                            string authres = "";
                            DataRow dtblrow = dtbl.Rows[0];

                            Int32 userId = Convert.ToInt32(dtblrow["MppUserID"]);

                            var auth = new AuthorizationModel()
                            {
                                access_token = Convert.ToString(dtblrow["AccessToken"]),
                                refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                token_type = Convert.ToString(dtblrow["TokenType"]),
                                expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                            };
                            string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                            DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                authres = UpdateAccessToken(userId, ref auth);

                            if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                            {
                                decimal Total = dtbl.Rows.Count;
                                int Start = 0;
                                int Skip = 0;
                                int PageSize = 1000;
                                decimal MaxCalls = Math.Ceiling(Total / PageSize);

                                while (Start < MaxCalls)
                                {
                                    Skip = Start * PageSize;
                                    var SearchTermLog = dtbl.AsEnumerable().Skip(Skip).Take(PageSize);
                                    var ListofNegKeywords = new List<object>();

                                    foreach (DataRow row in SearchTermLog)
                                    {
                                        long CampaignID = Convert.ToInt64(row["CampaignID"]);
                                        long AdgroupID = Convert.ToInt64(row["AdgroupID"]);
                                        String KeyName = Convert.ToString(row["Query"]);

                                        ListofNegKeywords.Add(new
                                        {
                                            campaignId = CampaignID,
                                            adGroupId = AdgroupID,
                                            keywordText = KeyName,
                                            matchType = "negativeExact",
                                            state = "enabled"
                                        });
                                    }

                                    if (ListofNegKeywords.Count > 0)
                                    {
                                        CreateNegKeywordsOnAmz(ListofNegKeywords, _profileId, auth, ReportID, SearchTermLog);
                                        Start += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        private void KeywordsGlobalOptimizations()
        {
            var dt = APIData.GetOptimizeData(RecordType.Keyword);

            if (dt.Rows.Count > 0)
            {
                var reports = from row in dt.AsEnumerable()
                              group row by new { ID = row.Field<Int32>("ReportID") } into grp
                              orderby grp.Key.ID
                              select new { ReportID = grp.Key.ID };

                if (reports.Count() > 0)
                {
                    foreach (var r in reports)
                    {
                        int ReportID = r.ReportID;
                        DataTable dtbl = new DataTable();
                        dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("ReportID") == ReportID).CopyToDataTable();

                        if (dtbl.Rows.Count > 0)
                        {
                            string authres = "";
                            DataRow dtblrow = dtbl.Rows[0];

                            Int32 mppuserId = Convert.ToInt32(dtblrow["MppUserID"]);

                            var auth = new AuthorizationModel()
                            {
                                access_token = Convert.ToString(dtblrow["AccessToken"]),
                                refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                token_type = Convert.ToString(dtblrow["TokenType"]),
                                expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                            };
                            string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                            DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                authres = UpdateAccessToken(mppuserId, ref auth);

                            if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                            {
                                decimal Total = dtbl.Rows.Count;
                                int Start = 0;
                                int Skip = 0;
                                int PageSize = 1000;
                                decimal MaxCalls = Math.Ceiling(Total / PageSize);

                                while (Start < MaxCalls)
                                {
                                    Skip = Start * PageSize;
                                    var KeywordsLog = dtbl.AsEnumerable().Skip(Skip).Take(PageSize);
                                    var ListofKeywords = new List<object>();

                                    foreach (DataRow row in KeywordsLog)
                                    {
                                        long KeywordID = Convert.ToInt64(row["KeywordID"]);
                                        String ModifiedField = Convert.ToString(row["ModifiedField"]);
                                        String ModifiedValue = Convert.ToString(row["NewValue"]);

                                        if (ModifiedField.ToLower() == "status")
                                        {
                                            ListofKeywords.Add(new
                                            {
                                                keywordId = KeywordID,
                                                state = ModifiedValue
                                            });
                                        }
                                        else if (ModifiedField.ToLower() == "maxbid")
                                        {
                                            ListofKeywords.Add(new
                                            {
                                                keywordId = KeywordID,
                                                bid = ModifiedValue
                                            });
                                        }
                                    }

                                    if (ListofKeywords.Count > 0)
                                    {
                                        UpdateKeyBidsOrStatusOnAmz(ListofKeywords, _profileId, auth, ReportID, KeywordsLog);
                                        Start += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SearchTermGlobalOptimizations()
        {
            var dt = APIData.GetOptimizeData(RecordType.SearchTerm);

            if (dt.Rows.Count > 0)
            {
                var reports = from row in dt.AsEnumerable()
                              group row by new { ID = row.Field<Int32>("ReportID") } into grp
                              orderby grp.Key.ID
                              select new { ReportID = grp.Key.ID };

                if (reports.Count() > 0)
                {
                    foreach (var r in reports)
                    {
                        int ReportID = r.ReportID;
                        DataTable dtbl = new DataTable();
                        dtbl = dt.AsEnumerable().Where(ro => ro.Field<Int32>("ReportID") == ReportID).CopyToDataTable();

                        if (dtbl.Rows.Count > 0)
                        {
                            string authres = "";
                            DataRow dtblrow = dtbl.Rows[0];

                            Int32 userId = Convert.ToInt32(dtblrow["MppUserID"]);

                            var auth = new AuthorizationModel()
                            {
                                access_token = Convert.ToString(dtblrow["AccessToken"]),
                                refresh_token = Convert.ToString(dtblrow["RefreshToken"]),
                                token_type = Convert.ToString(dtblrow["TokenType"]),
                                expires_in = Convert.ToInt32(dtblrow["TokenExpiresIn"])
                            };
                            string _profileId = Convert.ToString(dtblrow["ProfileId"]);
                            DateTime _lastUpdateTime = Convert.ToDateTime(dtblrow["AccessTokenUpdatedOn"]);
                            DateTime TokenExpiryTime = _lastUpdateTime.AddSeconds(auth.expires_in - 300);

                            if (!(DateTime.Now < TokenExpiryTime) && !String.IsNullOrWhiteSpace(_profileId))
                                authres = UpdateAccessToken(userId, ref auth);

                            if (String.IsNullOrWhiteSpace(authres) && !String.IsNullOrWhiteSpace(_profileId))
                            {
                                decimal Total = dtbl.Rows.Count;
                                int Start = 0;
                                int Skip = 0;
                                int PageSize = 1000;
                                decimal MaxCalls = Math.Ceiling(Total / PageSize);

                                while (Start < MaxCalls)
                                {
                                    Skip = Start * PageSize;
                                    var SearchTermLog = dtbl.AsEnumerable().Skip(Skip).Take(PageSize);
                                    var ListofNegKeywords = new List<object>();

                                    foreach (DataRow row in SearchTermLog)
                                    {
                                        long CampaignID = Convert.ToInt64(row["CampaignID"]);
                                        long AdgroupID = Convert.ToInt64(row["AdgroupID"]);
                                        String KeyName = Convert.ToString(row["Query"]);

                                        ListofNegKeywords.Add(new
                                        {
                                            campaignId = CampaignID,
                                            adGroupId = AdgroupID,
                                            keywordText = KeyName,
                                            matchType = "negativeExact",
                                            state = "enabled"
                                        });
                                    }

                                    if (ListofNegKeywords.Count > 0)
                                    {
                                        CreateNegKeywordsOnAmz(ListofNegKeywords, _profileId, auth, ReportID, SearchTermLog);
                                        Start += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private long GetNewCTR(Int32 clicks, ref long ctr)
        {
            if (clicks == 1)
            {
                ctr = Convert.ToInt64(1.5 * ctr);
            }
            else if (clicks > 1)
            {
                ctr = clicks * ctr;
            }
            return ctr;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request)
        {
            //Wait while the limit has been reached. 
            while (!_throttlingHelper.RequestAllowed)
            {
                await Task.Delay(1000);
            }

            var client = new HttpClient();

            _throttlingHelper.StartRequest();
            var result = await client.SendAsync(request).ConfigureAwait(false);
            _throttlingHelper.EndRequest();
            return result;
        }

        #endregion

        #region CampaignApis
        private List<ProductModel> GetNewCampaign(String _profileId, AuthorizationModel auth)
        {
            HttpClient hc = new HttpClient();
            //String profileId = Convert.ToString(Session["ProfileId"]);
            var route = "v1/campaigns?stateFilter=enabled,paused";
            hc.BaseAddress = _api;
            String Auth_Token = Convert.ToString(auth.token_type).PadRight(7) + auth.access_token;
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
            var res = hc.GetAsync(route).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception(res.ReasonPhrase);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProductModel>>(res.Content.ReadAsStringAsync().Result);
            return result;
        }
        #endregion

        #region Best Customer serach #108
        private BSTCampaignResponse CreateCampaigns(CampaignModel camp, int userId, String _profileId, AuthorizationModel auth)
        {
            var msg = "";
            var respo = new BSTCampaignResponse() { campaignId = camp.campaignId, userid = userId };
            var _tokenType = auth.token_type;
            var _accessToken = auth.access_token;
            var campToAdd = new
            {
                name = camp.name,
                campaignType = camp.campaignType,
                targetingType = camp.targetingType,
                state = camp.state,
                dailyBudget = Convert.ToInt32(camp.dailyBudget),
                startDate = camp.startDate
            };
            try
            {
                HttpClient hc = new HttpClient();
                var route = _version + "/campaigns";
                hc.BaseAddress = _api;
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(campToAdd);
                String content = response;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                String Auth_Token = _tokenType.PadRight(7) + _accessToken;
                hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
                var res = hc.PostAsync(route, httpContent).Result;
                var re = res.Content.ReadAsStringAsync().Result;
                if (res.IsSuccessStatusCode)
                {
                    msg = "Success";
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateCampaignResponse>(res.Content.ReadAsStringAsync().Result);
                    respo.NewCampId = result.campaignId;

                }
                else
                {
                    msg = "Api error.";
                    LogAPI.WriteLog("CreateCampaigns" + msg);
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                LogAPI.WriteLog("CreateCampaigns" + msg);
            }
            return respo;
        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public string GetAmazonDateFormat(DateTime dateAndTime)
        {
        
            int year = dateAndTime.Year;
            int month = dateAndTime.Month;
            int day = dateAndTime.Day;
            var m = (month <= 9 ? "0" + month.ToString() : month.ToString());
            var d = (day <= 9 ? "0" + day.ToString() : day.ToString());
            var str =string.Format("{0}{1}{2}",year,m,d);
            return str;
        }
        #endregion
    }
}
