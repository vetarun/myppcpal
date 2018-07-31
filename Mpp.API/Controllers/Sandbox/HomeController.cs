using Mpp.BUSINESS.DataModel;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Mpp.API.Controllers.Sandbox
{
    public class HomeController : Controller
    {
        private string _version;
        private string _api;
        private string _clientkey;
        private string _clientSecretkey;

        public HomeController()
        {
            _version = MppUtility.ReadConfig("ApiVersion");
            _api = MppUtility.ReadConfig("Url");
            _clientkey = MppUtility.ReadConfig("ClientKey");
            _clientSecretkey = MppUtility.ReadConfig("ClientSecretKey");
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Campaigns()
        {
            ViewBag.Type = 1;
            var result = GetCampaigns();
            return View(result);
        }

        [HttpGet]
        public ActionResult AdGroups()
        {
            var result = GetAdGroups();
            return View(result);
        }

        [HttpGet]
        public ActionResult Keywords(string type)
        {
            var result = GetKeywords(type);
            return View(result);
        }

        [HttpGet]
        public ActionResult Ads()
        {
            
            var result = GetAds().Where(s=>s.state!= "archived").ToList();
            return View(result);
        }

        public void SandboxDataManagement()
        {
            bool result;
            //CreateCampaigns(out result);

            //if (result)
            //   CreateAdGroups(out result);
            //if(result)
            //   CreateKeywords(out result);
            //if (result)
               CreateAds(out result);
        }

        public void CreateCampaigns(out bool result)
        {
            result = false;
            var camps = GetCampaigns();
            var ListofCamps = new List<object>();
            if (camps.Count == 0)
            {
                int i = 1;

                while (i <=3)
                {
                    var count = camps.Where(c => c.name.ToLower() == "campaign"+i).Count();
                    if (count == 0)
                    {
                        ListofCamps.Add(new { name = "campaign"+i, campaignType = "sponsoredProducts", targetingType = "manual", state = "enabled", dailyBudget = i, startDate = "20180327" });
                    }
                    i++;
                }
            }

            if (ListofCamps.Count > 0)
            {
                var _tokenType = Convert.ToString(Session["TokenType"]);
                var _accessToken = Convert.ToString(Session["AccessToken"]);
                var _profileId = Convert.ToString(Session["ProfileId"]);

                HttpClient hc = new HttpClient();
                var route = _version + "/campaigns";
                hc.BaseAddress = new Uri(_api);
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(ListofCamps);
                String content = response;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                String Auth_Token = _tokenType.PadRight(7) + _accessToken;
                hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
                var res = hc.PostAsync(route, httpContent).Result;
                var re = res.Content.ReadAsStringAsync().Result;
                if (!res.IsSuccessStatusCode)
                    throw new Exception(res.ReasonPhrase);
                else
                    result = true;
            }
            else
            {
                result = true;
            }
        }

        public void CreateAdGroups(out bool result)
        {
            result = false;
            var camps = GetCampaigns();
            var adgrps = GetAdGroups();
            var ListofAdgroups = new List<object>();

            if (adgrps.Count == 0)
            {
                foreach (var camp in camps)
                {
                    int i = 1;
                    while (i <= 2)
                    {
                        var count = adgrps.Where(a => a.name.ToLower() == "adgroup" + i && a.campaignId == camp.campaignId).Count();
                        if (count == 0)
                        {
                            ListofAdgroups.Add(new { campaignId =camp.campaignId, name = "adgroup" + i, state = "enabled", defaultBid = i });
                        }
                        i++;
                    }
                }
            }

            if (ListofAdgroups.Count > 0)
            {
                var _tokenType = Convert.ToString(Session["TokenType"]);
                var _accessToken = Convert.ToString(Session["AccessToken"]);
                var _profileId = Convert.ToString(Session["ProfileId"]);

                HttpClient hc = new HttpClient();
                var route = _version + "/adGroups";
                hc.BaseAddress = new Uri(_api);
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(ListofAdgroups);
                String content = response;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                String Auth_Token = _tokenType.PadRight(7) + _accessToken;
                hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
                var res = hc.PostAsync(route, httpContent).Result;
                if (!res.IsSuccessStatusCode)
                    throw new Exception(res.ReasonPhrase);
                else
                    result = true;
            }
            else
            {
                result = true;
            }
        }

        public void CreateKeywords(out bool result)
        {
            result = false;
            var camps = GetCampaigns();
            var adgrps = GetAdGroups();
            var keywords = GetKeywords("1");
            var ListofKeys = new List<object>();

            if (camps.Count > 0 && adgrps.Count > 0)
            {
                foreach (var camp in camps)
                {
                    var adgrp = adgrps.Where(a => a.campaignId == camp.campaignId);
                    foreach (var adg in adgrp)
                    {
                        int i = 1;
                        while (i <= 3)
                        {
                            var count = keywords.Where(k => k.keywordText.ToLower() == "keyword" + i && k.campaignId == camp.campaignId && k.adGroupId ==adg.adGroupId).Count();
                            if (count == 0)
                            {
                                ListofKeys.Add(new { campaignId=camp.campaignId, adGroupId=adg.adGroupId, keywordText = "keyword" + i, matchType = "broad", state = "enabled" });
                            }
                            i++;
                        }
                    }
                }
            }

            if (ListofKeys.Count > 0)
            {
                var _tokenType = Convert.ToString(Session["TokenType"]);
                var _accessToken = Convert.ToString(Session["AccessToken"]);
                var _profileId = Convert.ToString(Session["ProfileId"]);

                HttpClient hc = new HttpClient();
                var route = _version + "/keywords";
                hc.BaseAddress = new Uri(_api);
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(ListofKeys);
                String content = response;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                String Auth_Token = _tokenType.PadRight(7) + _accessToken;
                hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
                var res = hc.PostAsync(route, httpContent).Result;
                if (!res.IsSuccessStatusCode)
                    throw new Exception(res.ReasonPhrase);
                else
                    result = true;
            }
            else
            {
                result = true;
            }
        }

        public void CreateAds(out bool result)
        {
            result = false;
            var camps = GetCampaigns();
            var adgrps = GetAdGroups();
            var ads = GetAds();
            var ListofAds = new List<object>();

            if (camps.Count > 0 && adgrps.Count > 0)
            {
                string[] asins = new string[12] { "B013HQZM6A", "B0785975LP", "B01N6WGJ14", "B01INYV29M", "B00I937QEI", "B00TQJWF1I", "B01HTYH8YA", "B0768TGMXT", "B075P1XHRR", "B078MGFRPP", "B071K998XW", "B071K99LPJ" };
                int i = 1;
                foreach (var camp in camps)
                {
                    var adgrp = adgrps.Where(a => a.campaignId == camp.campaignId);
                    foreach (var adg in adgrp)
                    {
                        var count = ads.Where(ad => ad.sku == "sku" + i && ad.campaignId == camp.campaignId && ad.adGroupId == adg.adGroupId).Count();
                        if (count == 0)
                        {
                            ListofAds.Add(new { campaignId = camp.campaignId, adGroupId = adg.adGroupId,asin=asins[i], state = "enabled" });
                        }
                        i++;
                    }
                }
            }

            if (ListofAds.Count > 0)
            {
                var _tokenType = Convert.ToString(Session["TokenType"]);
                var _accessToken = Convert.ToString(Session["AccessToken"]);
                var _profileId = Convert.ToString(Session["ProfileId"]);

                HttpClient hc = new HttpClient();
                var route = _version + "/productAds";
                hc.BaseAddress = new Uri(_api);
                var response = Newtonsoft.Json.JsonConvert.SerializeObject(ListofAds);
                String content = response;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                String Auth_Token = _tokenType.PadRight(7) + _accessToken;
                hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", _profileId);
                var res = hc.PostAsync(route, httpContent).Result;
                if (!res.IsSuccessStatusCode)
                    throw new Exception(res.ReasonPhrase);
                else
                    result = true;
            }
        }




        public List<CampaignModel> GetCampaigns()
        {
            HttpClient hc = new HttpClient();
            String profileId = Convert.ToString(Session["ProfileId"]);
            var route = "v1/campaigns";
            hc.BaseAddress = new Uri(_api);
            String Auth_Token = Convert.ToString(Session["TokenType"]).PadRight(7) + Convert.ToString(Session["AccessToken"]);
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(route).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception(res.ReasonPhrase);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CampaignModel>>(res.Content.ReadAsStringAsync().Result);
            return result;
        }

        public List<AdGroupModel> GetAdGroups()
        {
            HttpClient hc = new HttpClient();
            String profileId = Convert.ToString(Session["ProfileId"]);
            var route = "v1/adGroups";
            hc.BaseAddress = new Uri(_api);
            String Auth_Token = Convert.ToString(Session["TokenType"]).PadRight(7) + Convert.ToString(Session["AccessToken"]);
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(route).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception(res.ReasonPhrase);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdGroupModel>>(res.Content.ReadAsStringAsync().Result);
            return result;
        }

        public List<KeywordModel> GetKeywords(string type)
        {
            HttpClient hc = new HttpClient();
            String profileId = Convert.ToString(Session["ProfileId"]);
            String route = "";
            if (type == "1")
            {
                ViewBag.Name = "Keywords";
                route = "v1/keywords";
            }
            else
            {
                ViewBag.Name = "Negative Keywords";
                route = "v1/campaignNegativeKeywords";
            }
            hc.BaseAddress = new Uri(_api);
            String Auth_Token = Convert.ToString(Session["TokenType"]).PadRight(7) + Convert.ToString(Session["AccessToken"]);
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(route).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception(res.ReasonPhrase);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeywordModel>>(res.Content.ReadAsStringAsync().Result);
            return result;
        }

        public List<AdModel> GetAds()
        {
            HttpClient hc = new HttpClient();
            String profileId = Convert.ToString(Session["ProfileId"]);
            var route = "v1/productAds/extended                                                  ";
            hc.BaseAddress = new Uri(_api);
            String Auth_Token = Convert.ToString(Session["TokenType"]).PadRight(7) + Convert.ToString(Session["AccessToken"]);
            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
            hc.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileId);
            var res = hc.GetAsync(route).Result;
            if (!res.IsSuccessStatusCode)
                throw new Exception(res.ReasonPhrase);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdModel>>(res.Content.ReadAsStringAsync().Result);
            return result;
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }
    }
}