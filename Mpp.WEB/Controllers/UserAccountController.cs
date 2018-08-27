using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using Mpp.UTILITIES.BO;
using Mpp.WEB.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Security;
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Controllers
{
   
    public class UserAccountController : Controller
    {
       
        #region Class Variables and Constants

        private AccountData userregistrationData;
        private EmailAlert emailalert;
        // private bool ShowMsg = true;
        #endregion

        #region Constructors
        public UserAccountController()
        {
            this.userregistrationData = new AccountData();
            this.emailalert = new EmailAlert();
        }

        #endregion

        #region Action Methods

        // GET: Index
        public ActionResult Index()
        {
            return View();          
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, String returnUrl)
        {
            String msg = "";
            Int32 UserID = 0;
            DataSet ds;
            DataTable dt, dtbl;
            DataRow dr = null, dr1 = null;
            if (ModelState.IsValid)
            {
                String email = model.Email.Trim();
                ds = userregistrationData.ValidateEmailAndGetUserinfo(email);
             
                if (ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];
                    dr = dt.Rows[0];
                    UserID = Convert.ToInt32(dr["MppUserID"]);
                    msg = userregistrationData.CheckUserLogin(UserID, model.Password);
                   
                    if (msg == "")
                    {
                        dtbl = ds.Tables[1];
                        dr1 = dtbl.Rows[0];
                        String CustmId, CardId;

                        SessionData.UserID = UserID;

                        String FirstName = Convert.ToString(dr["FirstName"]);
                        SessionData.LastName = Convert.ToString(dr["LastName"]);
                        String PlanDate = Convert.ToString(dr1["PlanDate"]);
                        DateTime? Today, NextPlanDate = DateTime.MaxValue; 
                        Today = DateTime.Now;

                        DateTime StartDate = Convert.ToDateTime(dr["StartDate"]);

                        if (!String.IsNullOrWhiteSpace(PlanDate))
                            NextPlanDate = Convert.ToDateTime(PlanDate);

                        SessionData.StartDate = StartDate.ToString("yyyy/MM/dd hh:mm:ss tt");
                        SessionData.PlanStatus = Convert.ToInt32(dr["PlanStatus"]);
                        SessionData.PlanID = Convert.ToInt32(dr["PlanID"]);
                        SessionData.FormulaAccess = Convert.ToInt16(dr["IsSetFormula"]);
                        SessionData.TrialEndOn = Convert.ToDateTime(dr["TrailEndDate"]);
                        SessionData.Email = email;
                        SessionData.IsAgreementAccept = Convert.ToInt16(dr["IsAgreementConfirm"]);
                        SessionData.ProfileAccess = Convert.ToInt16(dr["ProfileAccess"]);
                        SessionData.AlertCount = Convert.ToInt32(dr1["total_alerts"]);
                        CustmId = Convert.ToString(dr["stp_custId"]);
                        CardId = Convert.ToString(dr["stp_cardId"]);

                        if (!String.IsNullOrWhiteSpace(CustmId)){
                            FormsAuthentication.SetAuthCookie(FirstName, false);
                            var custm = StripeHelper.GetStripeCustomer(CustmId);
                            if (!String.IsNullOrWhiteSpace(custm.DefaultSourceId)&& String.IsNullOrWhiteSpace(CardId))
                                StripeHelper.DeleteCard(custm.DefaultSourceId, CustmId);
                            SessionData.StripeCardId = CardId;
                            SessionData.StripeCustId = CustmId;

                            if (((SessionData.PlanID == 1 && SessionData.TrialEndOn < Today && !String.IsNullOrWhiteSpace(CardId)) || NextPlanDate < Today) && SessionData.PlanStatus == 1) // When plan was expired
                            {
                                if (String.IsNullOrWhiteSpace(CardId))
                                    return RedirectToAction("Plan", "Settings");
                                else
                                    msg = StripeServices.SubscribePlan(CustmId, CardId);
                                if (!String.IsNullOrWhiteSpace(msg))
                                {
                                    TempData["IsValid"] = msg;
                                    return RedirectToAction("Login", "UserAccount");
                                }
                            }

                            if (SessionData.PlanID != 1){
                                if (SessionData.PlanStatus == 0) // handles unsubscribed status after trial period
                                    return RedirectToAction("Plan", "Settings");
                                else if(String.IsNullOrWhiteSpace(CardId) && SessionData.PlanStatus == 1) // When CardId was not found and Plan was not trail
                                    return RedirectToAction("Cards", "Settings");
                            }
                        }
                        else{
                            return View("Error");
                        }

                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("Dashboard", "Main");
                    }
                }
                else
                {
                    msg = "Invalid login attempt!";
                }

                TempData["IsValid"] = msg;
                return RedirectToAction("Login", "UserAccount");
            }
            return View(model);
        }

        [HttpGet]
        [NoCache]
        [Authorize]
        public ActionResult Agreement()
        {
            return View();
        }

        [HttpPost]
        [SessionTimeOutFilterAttribute]
        public JsonResult Agreement(Int16 isAgreed)
        {
            String msg = "";
            SellerData sellerData = new SellerData();
            msg = userregistrationData.UpdateUserAgreement(SessionData.UserID, isAgreed);
            if (msg == "")
                SessionData.IsAgreementAccept = 1;
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            String msg = "";
            if (ModelState.IsValid)
            {
                String email = model.Email.Trim();
                String ActivationCode = Guid.NewGuid().ToString();
                msg = userregistrationData.InsertUpdateUser(model.FirstName, model.LasttName, email, model.Password, ActivationCode);
                if (msg == "")
                {
                    String _url = "UserAccount/Register";
                    msg =  emailalert.SendActivationEmail(model.FirstName, model.LasttName, email, ActivationCode, _url);
                    TempData["IsSuccess"] = true;
                    if (msg == "")
                    {
                        userregistrationData.UpdateActivationEmailAlert(email);
                        TempData["IsValid"] = "Registration successful. Activation email has been sent.";
                    }
                    else
                        TempData["IsValid"] = "Registration successful but there was an issue in sending an email, error:" + msg;
                    return RedirectToAction("Register", "UserAccount");
                }
                else
                {
                    TempData["IsValid"] = msg;
                    TempData["IsSuccess"] = false;
                    return RedirectToAction("Register", "UserAccount");
                }
            }
            return View(model);
        }

        //GET: Account/Password
        [HttpGet]
        public ActionResult Password()
        {
            return View(new EmailViewModel());
        }

        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Password(EmailViewModel model)
        {
            String msg = "";
            if (ModelState.IsValid)
            {
                String Code = Guid.NewGuid().ToString();
                String email = model.Email.Trim();
                msg = userregistrationData.ChangePasswordCode(email, Code);
                if (msg == "")
                {
                    msg = emailalert.SendPasscodeChangeEmail(email, Code);
                    if (msg == "")
                        TempData["IsValid"] = "Email has been sent. Please reset your account password, Thank you!";
                    else
                        TempData["IsValid"] = "There was an issue in sending an email, error: " + msg;
                }
                else
                {
                    TempData["IsValid"] = msg;
                }
                return RedirectToAction("Password", "UserAccount");
            }
            return View(model);
        }

        //GET: Password Reset
        [HttpGet]
        public ActionResult ResetPassword(String ActivationCode)
        { 
            String result;
            TempData["Code"] = ActivationCode;
            ResetPasswordViewModel res = new ResetPasswordViewModel();
            if (!String.IsNullOrEmpty(ActivationCode))
            {
                result = userregistrationData.ValidatePasswordCode(ActivationCode, 2);
                if (!String.IsNullOrEmpty(result))
                {
                    res.UserId = result;
                }
                else
                {
                    ViewBag.Confirmation = "Invalid operation, please contact My PPC Pal team";
                }
            }
            return View(res);
        }

        //POST: Password Reset
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            String msg = "";
            String Code = Convert.ToString(TempData["Code"]);
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.UserId))
                {
                    msg = userregistrationData.UpdatePassword(model.UserId, model.Password,2);
                }
                else
                {
                    msg = "Invalid Operation!";
                }

                if (msg == "")
                {
                    String Msg = emailalert.SendPasswordUpdateEmail("","", model.UserId);
                    Code = null;
                    msg = "Password updated successfully"+ Msg;
                }
                TempData["IsValid"] = msg;
                return RedirectToAction("ResetPassword", "UserAccount",routeValues: new { ActivationCode = Code });
            }
            return View(model);
        }

        //GET: Activate Account
        [HttpGet]
        public ActionResult UserActivation(String ActivationCode)
        {
            string  result = "";
            var userInfo = new UpdateUserInfo();
            if (!String.IsNullOrEmpty(ActivationCode))
            {
                var dr = userregistrationData.AccountConfirmation(ActivationCode);
                if(dr != null)
                {
                    result = "true";
                    String StripeCustmId = Convert.ToString(dr["stp_custId"]);
                    var customerOptions = new StripeCustomerCreateOptions
                    {
                        Email = Convert.ToString(dr["Email"]),
                        Description = Convert.ToString(dr["name"]),
                    };
                    var customerDetails = StripeHelper.AddCustomer(customerOptions, StripeCustmId);
                    if (customerDetails != null)
                    {
                        var planId = (int)Statics.StripePlans.Trial;
                        var subscription = StripeHelper.Subscription(customerDetails.Id, new StripeSubscriptionCreateOptions { PlanId = planId.ToString() }, out result);
                        if (String.IsNullOrEmpty(result))
                        {
                            userInfo.CustId = customerDetails.Id;
                            userInfo.UserId = Convert.ToInt32(dr["id"]);
                            userInfo.PlanId = planId;
                            userInfo.PlanStartDate = Convert.ToDateTime(subscription.CurrentPeriodStart).ToString();
                            userInfo.TrialEndDate = subscription.TrialEnd.ToString();
                            userInfo.Amount = subscription.StripePlan.Amount.ToString();
                            userInfo.PlanEndDate = Convert.ToDateTime(subscription.CurrentPeriodEnd).ToString();
                            AccountData.UpdateStripeData(userInfo);
                        }
                        else
                        {
                            result = "Subscription error, please contact Administrator";
                        }
                    }
                }else
                {
                    result = "false";
                }
            }

            ViewBag.Confirmation = result;
            return View("Login");
        }

        [HttpGet]
        public ActionResult Confirmation()
        {
            if (TempData["IsValidCode"] == null)
            {
                TempData["IsValidCode"] = "Invalid Code";
            }
            else
            {
                TempData["IsValidCode"] = TempData["IsValidCode"];
            }

            ViewBag.ConfirmationResult = TempData["IsValidCode"];
            return View();
        }

        /// <summary>
        /// User will logout 
        /// </summary>
        /// <returns>login view</returns>

        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();//clear session
            Session["UserID"] = null;
            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["NextPlanDate"] = null;
            Session["PlanStatus"] = null;
            Session["SellerAccess"] = null;
            Session["IsAgreementConfirm"] = null;
            Session["AlertCount"] = null;
            Session["TrialEndOn"] = null;
            Session["StripeCardId"] = null;
            Session["StripeCustId"] = null;
            Session["Email"] = null;
            Session["FormulaAccess"] = null;
            Session["NegAccess"] = null;
            Session["BulkAccess"] = null;
            Session["PlanID"] = null;
            Session["StartDate"] = null;
            FormsAuthentication.SignOut();
            return View();
        }

        #endregion

        #region Service Methods

        //Email Validation
        public JsonResult CheckEmailId(String Email)
        {
            bool EmailExists = false;
            Int32 res = 0;
            res = userregistrationData.CheckEmailExists(Email);
            if (res > 0)
            {
                EmailExists = true;
            }
            else
            {
                EmailExists = false;
            }
            return Json(!EmailExists, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}