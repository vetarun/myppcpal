using iTextSharp.text;
using iTextSharp.text.pdf;
using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.BUSINESS.DataModel;
using Mpp.UTILITIES;
using Mpp.UTILITIES.BO;
using Mpp.WEB.Models;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Controllers
{
    [Authorize]
    [SessionTimeOutFilter]
    public class SettingsController : Controller
    {
        #region Class Variables and Constants
        private string _version;
        private string _api;
        private string _redirecturi;
        private string _clientkey;
        private string _clientSecretkey;
        ProfileData pdata;
        EmailAlert ealert;
        SellerData sellerData;
        DashboardData dashboardData;
        #endregion

        #region Constructors
        public SettingsController()
        {
            this.pdata = new ProfileData();
            this.ealert = new EmailAlert();
            this.sellerData = new SellerData();
            this.dashboardData = new DashboardData();
            _version = ConfigurationManager.AppSettings["ApiVersion"];
            _api = ConfigurationManager.AppSettings["Url"];
            _redirecturi = ConfigurationManager.AppSettings["LwaRedirectUrl"];
            _clientkey = ConfigurationManager.AppSettings["ClientKey"];
            _clientSecretkey = ConfigurationManager.AppSettings["ClientSecretKey"];
        }

        #endregion

        #region Public Methods 

        public ActionResult Index()
        {
            return View();
        }

        [OverrideActionFilters]
        [HttpPost]
        public void WebHook()
        {
            try
            {
                var paymentAlertStatus = 0;
                var postdata = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();
                var data = JObject.Parse(postdata);
                var signature = Request.Headers["Stripe-Signature"];
                var eventid = data["id"].ToString();
                var custid = data["data"].First().First["customer"].ToString();
                var msg = "";
                var emailboday = "";
                var eventdata = StripeHelper.GetStripeEvent(eventid);

                if (eventdata != null && (eventdata.Type == "invoice.payment_succeeded" || eventdata.Type == "invoice.payment_failed"))
                {
                    String coupon_id = null;
                    var list = data["data"].First().First["lines"];
                    var Amount = ((double)list["data"].First["amount"]) / 100;
                    var Amount_off = ((double)data["data"].First().First["amount_due"]) / 100;
                    var Discount = data["data"].First().First["discount"];
                    var start = (double)list["data"].First["period"]["start"];
                    var end = (double)list["data"].First["period"]["end"];
                    var startperiod = MppUtility.UnixTimeStampToDateTime(start);
                    var endperiod = MppUtility.UnixTimeStampToDateTime(end);
                    var discount = Math.Round(Amount - Amount_off, 2, MidpointRounding.AwayFromZero);
                    var customerdata = StripeHelper.GetStripeCustomer(custid);
                    var email = customerdata.Email;
                    var last4 = "";
                    var carddetails = StripeHelper.GetCard(custid);
                    foreach (var card in carddetails)
                    {
                        last4 = card.Last4;
                    }

                    if (Discount.Count() > 0)
                        coupon_id = Discount["coupon"]["id"].ToString();

                    switch (eventdata.Type)
                    {
                        case "invoice.payment_succeeded":
                            msg = AccountData.UpdatePaymentStatus(custid, Amount, Amount_off, coupon_id, discount, startperiod, endperiod, 1);
                            emailboday = "Payment Success. A payment of $" + Amount_off + " was made using your card on file ending with <b> " + last4 + " </b>";
                            paymentAlertStatus = 1;
                            break;
                        case "invoice.payment_failed":
                            msg = AccountData.UpdatePaymentStatus(custid, Amount, Amount_off, coupon_id, discount, startperiod, endperiod, 0);
                            emailboday = "Payment failed.Payment $" + Amount_off + " failed from your card ending with <b> " + last4 + " </b> ";
                            paymentAlertStatus = 2;
                            break;
                    }
                    if (msg == "" && Amount_off != 0)
                    {
                      var respo=  EmailAlert.PaymentAlertMail(email, emailboday);
                        if(respo=="success")
                        {
                            sellerData.UpdatePaymentAlert(paymentAlertStatus, custid);
                        }
                    }
                        
                }
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
            }
        }

        [NoCache]
        [AuthorizePlan]
        public ActionResult Billing()
        {
            return View();
        }

        // GET: ProfileEmail
        //[AuthorizePlan]
        [NoCache]
        [HttpGet]
        public ActionResult ProfileEmail()
        {
            return View();
        }

        // GET: ProfilePassword
        [HttpGet]
        [NoCache]
        [AuthorizePlan]
        public ActionResult ProfilePassword()
        {
            return View();
        }

        // GET: Plan
        [NoCache]
        public ActionResult Plan()
        {
            ViewBag.IsCardAdded = false;
            ViewBag.TrailEnd = SessionData.TrialEndOn;
            var plan = new UserPlan();
            DataTable dt = null, dt1 = null;
            UserPlanData pdata = new UserPlanData();
            var ds = pdata.GetPlanData(SessionData.UserID);
            dt = ds.Tables[0];
            dt1 = ds.Tables[1];

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                String PlanNextDate = dr.Field<DateTime?>("PlanEndDate") == null ? "" : Convert.ToDateTime(dr["PlanEndDate"]).ToString();
                plan.NextDate = PlanNextDate;
            }

            if (dt1 != null && dt1.Rows.Count > 0)
            {
                DataRow dr1 = dt1.Rows[0];
                plan.SkuCount = Convert.ToInt32(dr1["SKU"]);
                plan.KeyCount = Convert.ToInt32(dr1["Keyword"]);
                if (plan.SkuCount > 250)
                {
                    plan.Price = UserPlans.GetCustomPlanCost(plan.SkuCount);
                    plan.PlanName = "Custom";
                }
                else
                {
                    int planid = UserPlans.GetPlanBySku(plan.SkuCount);
                    plan.PlanName = Statics.GetEnumDescription((Statics.StripePlans)(planid));
                    plan.Price = UserPlans.GetPlanCost(planid);
                }
            }

            if(!String.IsNullOrWhiteSpace(SessionData.StripeCardId))
                ViewBag.IsCardAdded = true;
            return View(plan);
        }

        //GET: AmazonAccount
        [NoCache]
        [HttpGet]
        public ActionResult AmazonAccount()
        {
            ViewBag.LwaResponse = TempData["LwaResponse"];
            ViewBag.ProfilesModel = TempData["ProfilesModel"];
            return View();
        }

        public void LoginWithAmazon()
        {
            String _uri = "https://www.amazon.com/ap/oa?client_id="+ _clientkey + "&scope=cpc_advertising:campaign_management&response_type=code&redirect_uri="+_redirecturi;
            Response.Redirect(_uri);
        }

        public ActionResult VerifyLoginWithAmazon()
        {
            var code = Request.QueryString["code"];
            String msg = "";
            var profilesListModel = new List<SelectListItem>();
            if (!String.IsNullOrWhiteSpace(code))
                profilesListModel = GetAuthTokenAndProfile(code, out msg);
            if (String.IsNullOrWhiteSpace(msg))
                TempData["ProfilesModel"] = profilesListModel;
            else
                TempData["LwaResponse"] = msg;
            return RedirectToAction("AmazonAccount", "Settings");
        }

        public List<SelectListItem> GetAuthTokenAndProfile(String AUTH_CODE, out String msg)
        {
            msg = "";
            var profilesList = new List<SelectListItem>();
            try
            {
                var _tokenapi = "https://api.amazon.com";
                HttpClient hc = new HttpClient();
                var route = "/auth/o2/token";
                hc.BaseAddress = new Uri(_tokenapi);
                String content = "grant_type=authorization_code&code=" + AUTH_CODE + "&redirect_uri=" +_redirecturi+ "&client_id=" + _clientkey+ "&client_secret=" +_clientSecretkey;
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var res = hc.PostAsync(route, httpContent).Result;
                if (res.IsSuccessStatusCode)
                {
                    var res1 = res.Content.ReadAsStringAsync().Result;
                    var authres = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationModel>(res1);
                    bool Profileres = false;
                    var Profiles = GetProfiles(authres.access_token, authres.token_type, out Profileres);
                    if (Profileres)
                    {
                        if (Profiles.Count > 0)
                        {
                            var profiles_us = Profiles.Where(p => p.countryCode.ToLower() == "us");
                            if (profiles_us.Count() > 0)
                            {
                                msg = APIData.UpdateAccessTokenData(SessionData.UserID, authres.access_token, authres.refresh_token, authres.token_type, authres.expires_in);

                                if (String.IsNullOrWhiteSpace(msg))
                                {
                                    foreach (var p in profiles_us)
                                    {
                                        profilesList.Add(new SelectListItem()
                                        {
                                            Text = p.accountinfo.sellerStringId,
                                            Value = p.profileId
                                        });
                                    }     
                                }
                                else
                                {
                                    msg = Constant.SOMETHING_WRONG;
                                }
                            }
                            else
                                msg = "US Marketplace (.com) account is required";
                        }
                        else
                            msg = Constant.PROFILE_NOTFOUND;
                    }
                    else
                        msg = Constant.SOMETHING_WRONG;
                }else
                {
                    msg = res.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return profilesList;
        }

        public List<UserProfileModel> GetProfiles(String AccessToken, String TokenType, out bool result)
        {
            var profiles = new List<UserProfileModel>();
            var UserId = SessionData.UserID.ToString();
            result = false;
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
                        LogFile.WriteLog("GetProfiles - " + res.ReasonPhrase, UserId);
                    else
                    {
                        profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfileModel>>(res.Content.ReadAsStringAsync().Result);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLog("GetProfiles - " + ex.Message, UserId);
            }
            return profiles;
        }

        [HttpPost]
        public JsonResult UpdateSellerProfile(String ProfileId, String SellerId)
        {
            String msg = "";
            if (!String.IsNullOrWhiteSpace(ProfileId) && !String.IsNullOrWhiteSpace(SellerId))
            {
                var dt = APIData.GetUserProfileForUpdate(SessionData.UserID);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    var Auth = new AuthorizationModel()
                    {
                        access_token = Convert.ToString(dr["AccessToken"]),
                        refresh_token = Convert.ToString(dr["RefreshToken"]),
                        token_type = Convert.ToString(dr["TokenType"]),
                        expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                    };
                    var reportapi = new ReportsAPI();
                    msg = APIData.UpdateProfileData(SessionData.UserID, ProfileId, SellerId);
                    if (String.IsNullOrWhiteSpace(msg))
                    {
                        SessionData.ProfileAccess = 1;
                        msg = Constant.SUCCESS;
                    }
                    else
                        msg = Constant.SOMETHING_WRONG;
                }
                else
                    msg = Constant.SOMETHING_WRONG;
            }else
            {
               msg = "Seller ID is not valid";
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //GET: Cards
        [NoCache]
        public ActionResult Cards()
        {
            ViewData["Last4"] = "XXXX";
            ViewData["Expiry"] = "00/00";
            ViewData["Brand"] = "XYZ";
            ViewBag.IsCardAdded = false;
            ViewBag._TrailEnd = SessionData.TrialEndOn;
            if (!string.IsNullOrWhiteSpace(SessionData.StripeCardId))
            {
                var card = StripeHelper.GetCardDetails(SessionData.StripeCustId, SessionData.StripeCardId);
                if (card != null)
                {
                    ViewData["Last4"] = card.Last4;
                    ViewData["Expiry"] = card.ExpirationMonth + "/" + card.ExpirationYear;
                    ViewData["Brand"] = card.Brand;
                    ViewBag.IsCardAdded = true;
                }
            }
            return View();
        }

        [NoCache]
        [AuthorizePlan]
        //GET: Statements
        public ActionResult Statements()
        {
            //StripeServices.RenewTrailPlan();
            var invoices = StripeHelper.GetInvoices(SessionData.StripeCustId);
            //var customer = StripeHelper.GetStripeCustomer(SessionData.StripeCustId);
            List<StatementViewModel> statements = new List<StatementViewModel>();
            //var charges = StripeHelper.GetCharges(SessionData.StripeCustId);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            if (invoices != null && invoices.Count > 0)
            {
                foreach (var item in invoices)
                {
                    if (item.StripeInvoiceLineItems.Data.First().Plan != null)
                    {
                        statements.Add(new StatementViewModel
                        {
                            Date = item.Date.Value.ToShortDateString(),
                            Period = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(item.StripeInvoiceLineItems.Data.First().StripePeriod.Start.Value.ToString()), timeZone).ToShortDateString() + "-" + TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(item.StripeInvoiceLineItems.Data.First().StripePeriod.End.Value.ToString()), timeZone).AddDays(-1).ToShortDateString(),
                            Price = ((double)(item.AmountDue / 100)).ToString(),
                            Description = item.StripeInvoiceLineItems.Data.First().Plan.Name,
                            Id = item.Id
                        });
                    }

                }
            }
            return View(statements);
        }

        //Billing
        public ActionResult CreateStripeCustomer(string token)
        {
            var email = Session["Email"].ToString();
            var subscription = new StripeSubscription();
            var customerDetails = new StripeCustomer();
            var userInfo = new UpdateUserInfo();
            string msg = "";
            var cardStatus = 0;
            DataTable userData;
            StripeCharge charge = null;
            try
            {
                if (token != null)
                {
                    var ds = new AccountData().ValidateEmailAndGetUserinfo(email);
                    userData = ds.Tables[0];
                    var custID = Convert.ToString(userData.Rows[0]["stp_custId"]);
                    if (userData.Rows.Count > 0 && !string.IsNullOrWhiteSpace(custID))                                 //update
                    {
                        var customerUpdateOptions = new StripeCustomerUpdateOptions
                        {
                            Email = email,
                            Description = Session["FirstName"] + " " + Session["LastName"],
                            SourceToken = token
                        };

                        //updated customer
                        customerDetails = StripeHelper.UpdateCustomer(custID, customerUpdateOptions);

                        //add a test charge for $1
                        var makeTestCharge = new StripeChargeCreateOptions
                        {
                            CustomerId = customerDetails.Id,
                            Amount = 100,
                            Currency = "usd",
                            Description = "Card verification charge",
                            SourceTokenOrExistingSourceId = customerDetails.DefaultSourceId
                        };
                        charge = StripeHelper.MakePayment(makeTestCharge);
                        if (charge != null)
                        {
                            var refund = StripeHelper.RefundCharge(charge.Id);
                            cardStatus = 1;
                            userInfo.CustId = customerDetails.Id;
                            userInfo.CardId = customerDetails.DefaultSourceId;
                            userInfo.UserId = Convert.ToInt32(Session["UserID"]);
                            AccountData.UpdateStripeCardData(userInfo);
                            if ((SessionData.PlanID == 1 && SessionData.TrialEndOn < DateTime.Now) || SessionData.PlanStatus == 0) //When trail was expired || Unsubscribed and adding a card since payments are not succesfull
                                msg = StripeServices.SubscribePlan(SessionData.StripeCustId, userInfo.CardId);
                            if (String.IsNullOrWhiteSpace(msg))
                            {
                                SessionData.PlanStatus = 1; //Default subscription
                                SessionData.StripeCustId = customerDetails.Id;
                                SessionData.StripeCardId = customerDetails.DefaultSourceId;
                            }
                        }
                        else
                        {
                            StripeHelper.DeleteCard(customerDetails.DefaultSourceId, custID);
                            cardStatus = 0;
                        }
                    }
                    customerDetails = StripeHelper.GetStripeCustomer(customerDetails.Id);
                }

                var respo = new { Status = cardStatus, Message = msg, CustomerDetails = customerDetails };
                return Json(respo, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                LogFile.WriteLog("CreateCustomer - " +ex.Message.ToString());
                  return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        //Invoice Report
        public void Invoice(string invoiceId)
        {
            var invoice = StripeHelper.GetInvoice(invoiceId);
            var unitprice = ((double)invoice.StripeInvoiceLineItems.Data.First().Amount) / 100;
            var quantity = 1;
            var tax = invoice.Tax == null ? 0 : (double)(invoice.Tax) / 100;
            var total = unitprice * quantity + tax;
            var amount = (double)invoice.Total / 100; //after discount
            var discount = Math.Round(total - amount, 2, MidpointRounding.AwayFromZero);
            var username = HttpContext.User.Identity.Name + " " + SessionData.LastName;
            var email = SessionData.Email;
            var desc = "Monthly Plan - " + invoice.StripeInvoiceLineItems.Data.First().Plan.Name;
            var fileName = "Invoice_MPP_" + invoice.StripeInvoiceLineItems.Data.First().Plan.Name + "_" + DateTime.Now.ToShortDateString();


            Document document = new Document(PageSize.A4, 88f, 88f, 50f, 10f);
            Font NormalFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
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



                //Company Name and Address
                phrase = new Phrase();
                phrase.Add(new Chunk("INVOICE", FontFactory.GetFont("Arial", 20, Font.BOLD, BaseColor.BLACK)));
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


                //vendor or user details
                table = new PdfPTable(3);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 0.3f, 0.3f, 0.4f });
                table.SpacingBefore = 80f;

                //row2

                //user address 
                phrase = new Phrase();
                phrase.Add(new Chunk(username + "\n\n" + email, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Date
                phrase = new Phrase();
                phrase.Add(new Chunk("Date\n\n" + invoice.Date.Value.ToShortDateString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_CENTER);
                table.AddCell(cell);
                //vendor address
                phrase = new Phrase();
                phrase.Add(new Chunk("My PPC Pal, LLC \n P.O. Box 2126 \n Yorba Linda, CA 92885", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_RIGHT);
                table.AddCell(cell);
                document.Add(table);


                //row 3
                table = new PdfPTable(1);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 35f;

                phrase = new Phrase();
                phrase.Add(new Chunk("Invoice Number : " + invoice.Id + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                document.Add(table);

                DrawLine(writer, 25f, document.Top - 190f, document.PageSize.Width - 25f, document.Top - 190f, BaseColor.BLACK); //line 2

                //row 4
                var FontColor = new BaseColor(51, 122, 183);
                var HeaderFont = FontFactory.GetFont("Arial", 10, Font.BOLD, FontColor);

                table = new PdfPTable(5);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 15f;
                table.SetWidths(new float[] { 0.4f, 0.2f, 0.2f, 0.1f, 0.1f });

                //description
                phrase = new Phrase();
                phrase.Add(new Chunk("Description", HeaderFont));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Unit price
                phrase = new Phrase();
                phrase.Add(new Chunk("Unit Price", HeaderFont));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Discount
                phrase = new Phrase();
                phrase.Add(new Chunk("Discount", HeaderFont));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Tax
                phrase = new Phrase();
                phrase.Add(new Chunk("Tax", HeaderFont));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Amount
                phrase = new Phrase();
                phrase.Add(new Chunk("Amount", HeaderFont));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);

                document.Add(table);
                DrawLine(writer, 25f, document.Top - 220, document.PageSize.Width - 25f, document.Top - 220f, BaseColor.BLACK); //line 3
                //row 5
                table = new PdfPTable(5);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 25f;
                table.SetWidths(new float[] { 0.4f, 0.2f, 0.2f, 0.1f, 0.1f });

                //description
                phrase = new Phrase();
                phrase.Add(new Chunk(desc, FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Unit price
                phrase = new Phrase();
                phrase.Add(new Chunk("$ " + unitprice.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Discount
                phrase = new Phrase();
                phrase.Add(new Chunk("$ " + discount.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Tax
                phrase = new Phrase();
                phrase.Add(new Chunk("$ " + tax.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                //Amount
                phrase = new Phrase();
                phrase.Add(new Chunk("$ " + amount.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);

                document.Add(table);
                DrawLine(writer, 25f, document.Top - 260, document.PageSize.Width - 25f, document.Top - 260f, BaseColor.BLACK); //line 3
                //row 6
                table = new PdfPTable(2);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 25f;
                table.SetWidths(new float[] { 0.9f, 0.1f });

                //Total
                phrase = new Phrase();
                phrase.Add(new Chunk("Total:", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_RIGHT);
                table.AddCell(cell);
                //Total Amount
                phrase = new Phrase();
                phrase.Add(new Chunk("$ " + amount.ToString(), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                table.AddCell(cell);
                document.Add(table);

                //row 7
                table = new PdfPTable(1);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 80f;
                //declaration
                var card = StripeHelper.GetCard(SessionData.StripeCustId);
                if (SessionData.StripeCustId!=null && card != null && card.FirstOrDefault()!=null)
                {
                    
                    var str = "Your card on file ending in " + StripeHelper.GetCard(SessionData.StripeCustId).FirstOrDefault().Last4 + " will be billed automatically for the total amount shown on this invoice";
                    phrase = new Phrase();
                    phrase.Add(new Chunk(str, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell = PhraseCell(phrase, Element.ALIGN_LEFT);
                    table.AddCell(cell);
                    document.Add(table);
                }
                    


                //row 8
                table = new PdfPTable(1);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.SpacingBefore = 50f;
                //declaration

                var renewals = @"Renewals: This order renews for additional 1 month periods, unless either party provides the other with a notice of non-renewal prior to the renewal date. 
                              Terms: This order is governed by the terms of the Subscription Services Agreement between the parties, which terms are incorporated into this order for all purposes.
                             If there is a conflict between the terms of this order and the agreement, this order governs.
                             This order and the agreement are the entire agreement between the parties, and they supersede and replace all prior and contemporaneous negotiations, agreements, representations and discussions regarding this subject matter.
                             Only a signed writing of the parties may amend this order.";
                renewals = renewals.Replace(Environment.NewLine, String.Empty).Replace("  ", String.Empty);
                Chunk beginning = new Chunk(renewals, FontFactory.GetFont("Arial", 6));
                Phrase p1 = new Phrase(beginning);


                cell = PhraseCell(p1, Element.ALIGN_LEFT);

                table.AddCell(cell);
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

        #endregion

        #region Serivce Methods

        [HttpPost]
        public JsonResult PlanSubscription(Int32 planStatus, String nextPlanDate)
        {
            string msg = "";
            var subscription = false;
            String CustmId = SessionData.StripeCustId;
            String CardId = SessionData.StripeCardId;
            if (!String.IsNullOrWhiteSpace(CustmId))
            {
                DateTime plandate = !String.IsNullOrWhiteSpace(nextPlanDate) ? Convert.ToDateTime(nextPlanDate) : plandate = Convert.ToDateTime("1900-01-01 00:00:00");
                if (planStatus == 1 && plandate <= DateTime.Now)
                    msg = StripeServices.SubscribePlan(CustmId, CardId);
                else
                    subscription = true;

                if (subscription || planStatus == 2)
                {
                    UserPlanData pdata = new UserPlanData();
                    msg = pdata.UpdatePlanData(SessionData.UserID, planStatus);
                }
                if (msg == "")
                    SessionData.PlanStatus = planStatus;
            }else
            {
                msg = Constant.STRIPE_ID_NOTFOUND;
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult UpdateUserEmail(EmailViewModel model)
        {
            String FirstName = HttpContext.User.Identity.Name;
            String Email = model.Email.Trim();
            String msg = "";
            if (!String.IsNullOrWhiteSpace(Email))
            {
                if (!String.IsNullOrWhiteSpace(SessionData.StripeCustId))
                {
                    msg = pdata.UpdateUserProfile(SessionData.UserID, Email);
                    if (msg == "")
                    {
                        var customerOptions = new StripeCustomerUpdateOptions
                        {
                            Email = Email
                        };
                        StripeHelper.UpdateCustomer(SessionData.StripeCustId, customerOptions);
                        SessionData.Email = Email;
                        var res = dashboardData.GetAlertData(SessionData.UserID);
                        SessionData.AlertCount = res.Count;
                        String _url = "Settings/UpdateUserEmail";
                        String emailConfirm = ealert.SendActivationEmail(SessionData.UserID, FirstName, SessionData.LastName, Email, _url, AccounType.MppUser);
                        msg += emailConfirm;
                    }
                }
                else
                {
                    msg = Constant.STRIPE_ID_NOTFOUND;
                }
            }else
            {
                msg = Constant.EMAIL_NOTFOUND;
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult AmazonAccount(String sellerName)
        {
            String msg = "";
            SellerData sellerData = new SellerData();
            msg = sellerData.UpdateSellerAccount(SessionData.UserID, sellerName.Trim());
            if (msg == "-1")
                msg = "Seller Name already exists, please contact administrator!";
            else if (msg == "-2")
                msg = "Seller Name already exists with trail ended.Please contact administrator!";
            else if (msg == "1")
                msg = "";
            else
                msg = "Error! Please try again.";
            
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ResendEmail(String Email)
        {
            String FirstName = HttpContext.User.Identity.Name;
            String _url = "Settings/ResendEmail";
            String msg = ealert.SendActivationEmail(SessionData.UserID, FirstName, SessionData.LastName, Email.Trim(), _url, AccounType.MppUser);
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult UpdateUserPassword(String oldPwd, String newPwd)
        {
            String msg = pdata.UpdatePassword(SessionData.UserID, oldPwd, newPwd);
            if (msg == "")
            {
                var res = dashboardData.GetAlertData(SessionData.UserID);
                SessionData.AlertCount = res.Count;
                String FirstName = HttpContext.User.Identity.Name;
                String emailConfirm = ealert.SendPasswordUpdateEmail(FirstName, SessionData.LastName, SessionData.Email);
                msg += emailConfirm;
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ApplyCoupon(String code)
        {
            String msg = "";
            var custm_id = SessionData.StripeCustId;
            msg = !string.IsNullOrWhiteSpace(custm_id) ? StripeServices.ApplyPromoCode(custm_id, code.Trim()) : Constant.STRIPE_ID_NOTFOUND;
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public List<UserProfile> RegisterProfile(String AccessToken, String TokenType)
        //{
        //    var profiles = new List<UserProfile>();
        //    String msg = "";
        //    try
        //    {
        //        if (!String.IsNullOrWhiteSpace(TokenType) && !String.IsNullOrWhiteSpace(AccessToken))
        //        {
        //            HttpClient hc = new HttpClient();
        //            var route = "v1/profiles/register";
        //            hc.BaseAddress = new Uri(_api);
        //            var obj = new
        //            {
        //                countryCode = "US"
        //            };
        //            String content = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        //            HttpContent httpContent = new StringContent(content);
        //            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //            String Auth_Token = TokenType.PadRight(7) + AccessToken;
        //            hc.DefaultRequestHeaders.Add("Authorization", Auth_Token);
        //            var res = hc.PutAsync(route, httpContent).Result;
        //            if (!res.IsSuccessStatusCode)
        //            {
        //                msg = res.ReasonPhrase;
        //            }
        //            profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfile>>(res.Content.ReadAsStringAsync().Result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = ex.Message;
        //    }
        //    return profiles;
        //}

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
        #endregion
    }
}