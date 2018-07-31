using Mpp.API.Models;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Mpp.API.Controllers.Sandbox
{
    public class AccountController : Controller
    {

        #region Class Variables and Constants

        private AccountData userregistrationData;
        private string _version;
        private string _api;
        private string _returnUri;
        private string _clientkey;
        private string _clientSecretkey;
        #endregion

        #region Constructors
        public AccountController()
        {
            this.userregistrationData = new AccountData();
            _version = MppUtility.ReadConfig("ApiVersion");
            _api = MppUtility.ReadConfig("Url");
            _returnUri = MppUtility.ReadConfig("ReturnUrl");
            _clientkey = MppUtility.ReadConfig("ClientKey");
            _clientSecretkey = MppUtility.ReadConfig("ClientSecretKey");
        }

        #endregion

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public void RedirectLogin()
        {
            String _uri = "https://www.amazon.com/ap/oa?client_id="+_clientkey+"&scope=cpc_advertising:campaign_management&response_type=code&redirect_uri="+_returnUri;
            Response.Redirect(_uri);
        }

        public ActionResult VerifyLogin()
        {
            var code = Request.QueryString["code"];
            String res = "";
            if (!String.IsNullOrWhiteSpace(code))
                res = GetAuthToken(code);
            else
                res = "Code not found";
            TempData["LoginResponse"] = res;
            return RedirectToAction("Index", "Home");
        }

        public String GetAuthToken(String AUTH_CODE)
        {
            String msg = "";
            try
            {
                var _api = "https://api.amazon.com";
                HttpClient hc = new HttpClient();
                var route = "/auth/o2/token";
                hc.BaseAddress = new Uri(_api);
                String content = "grant_type=authorization_code&code=" + AUTH_CODE + "&redirect_uri="+ _returnUri + "&client_id="+_clientkey+"&client_secret="+_clientSecretkey;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = hc.PostAsync(route, httpContent).Result;
                if (!res.IsSuccessStatusCode)
                    throw new Exception(res.ReasonPhrase);
                var res1 = res.Content.ReadAsStringAsync().Result;
                var authres = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationResponse>(res1);
                var Profiles = GetProfiles(authres.access_token, authres.token_type);
                if (Profiles.Count > 0)
                {
                    var profile = Profiles.Where(p => p.countryCode.ToLower() == "us").FirstOrDefault();
                    Session["AccessToken"] = authres.access_token;
                    Session["TokenType"] = authres.token_type;
                    Session["ProfileId"] = profile.profileId;
                }
                else
                {
                    RegisterProfile(authres.access_token, authres.token_type);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public List<UserProfile> GetProfiles(String AccessToken, String TokenType)
        {
            var profiles = new List<UserProfile>();
            try
            {
                if (!String.IsNullOrWhiteSpace(TokenType) && !String.IsNullOrWhiteSpace(AccessToken))
                {
                    HttpClient hc = new HttpClient();
                    var route = "v1/profiles";
                    hc.BaseAddress = new Uri(_api);
                    String Auth_Token = TokenType.PadRight(7) + AccessToken;
                    hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                    var res = hc.GetAsync(route).Result;
                    if (!res.IsSuccessStatusCode)
                        throw new Exception(res.ReasonPhrase);
                    profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfile>>(res.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
            }
            return profiles;
        }

        public List<UserProfile> RegisterProfile(String AccessToken, String TokenType)
        {
            var profiles = new List<UserProfile>();
            String msg = "";
            try
            {
                if (String.IsNullOrWhiteSpace(TokenType) && String.IsNullOrWhiteSpace(AccessToken))
                {
                    HttpClient hc = new HttpClient();
                    var route = "v1/profiles/register";
                    hc.BaseAddress = new Uri(_api);
                    var obj = new
                    {
                        countryCode = "US"
                    };
                    String content = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    HttpContent httpContent = new StringContent(content);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    String Auth_Token = TokenType.PadRight(7) + AccessToken;
                    hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
                    var res = hc.PutAsync(route, httpContent).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        msg = res.ReasonPhrase;
                    }
                    profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfile>>(res.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return profiles;
        }
    }
}