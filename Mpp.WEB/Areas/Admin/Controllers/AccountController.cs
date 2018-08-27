using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.WEB.Areas.Admin.Models;
using Mpp.WEB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Mpp.WEB.Filters;
using System.Web.Security;
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Areas.Admin.Controllers
{
    [AdminRoleFilter(UserType = "1")]
    [SessionExpireFilter]
    public class AccountController : Controller
    {
        #region Class Variables and Constants
        private AccountData userregistrationData;
        private AdminData adata;
        #endregion

        #region Constructors
        public AccountController()
        {
            this.adata = new AdminData();
            this.userregistrationData = new AccountData();
        }

        #endregion

        #region Public Methods
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }

        [OverrideActionFilters]
        public ActionResult Login()
        {
           

            return View();
        }

        // POST: Account/Login
        [OverrideActionFilters]
        [HttpPost]
     //   [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            String msg = "";
            Int32 AdminID = 0;
            DataTable dt;
            DataRow dr = null;
            if (ModelState.IsValid)
            {
                String Email = model.Email.Trim();
                dt = userregistrationData.ValidateEmailAndGetAdminUserinfo(Email);

                if (dt!=null && dt.Rows.Count > 0)
                {
                    dr = dt.Rows[0];
                    AdminID = Convert.ToInt32(dr["MppAdminID"]);
                    msg = userregistrationData.CheckAdminUserLogin(AdminID, model.Password);
                    if (msg == "")
                    {
                        Session["AdminUserID"] = AdminID;
                        Session["AdminFName"] = Convert.ToString(dr["FirstName"]);
                        Session["AdminLName"] = Convert.ToString(dr["LastName"]);
                        Session["UserType"] = dr["UserType"] is DBNull ? 1 : Convert.ToInt32(dr["UserType"]);
                        var type = dr["UserType"] is DBNull?1:Convert.ToInt32(dr["UserType"]);
                        if(type==1)
                        return RedirectToAction("Dashboard", "Seller", new { area = "Admin"});
                        else
                            return RedirectToAction("Dashboard", "Affiliation", new { area = "Admin" });
                    }
                }
                else
                {
                    msg = "Invalid login attempt!";
                }

                TempData["IsValid"] = msg;
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Users()
        {
            return View();
        }

        [OverrideActionFilters]
        /// <summary>
        /// User will logout 
        /// </summary>
        /// <returns>login view</returns>
        public ActionResult Logout()
        {
            Session["AdminUserID"] = null;
            Session["AdminFName"] = null;
            Session["AdminLName"] = null;
           
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        //GET: Password Reset
        [HttpGet]
        [OverrideActionFilters]
        public ActionResult ResetPassword(String ActivationCode)
        {
            String result;
            TempData["Code"] = ActivationCode;
            ResetPasswordViewModel res = new ResetPasswordViewModel();
            if (!String.IsNullOrEmpty(ActivationCode))
            {
                result = userregistrationData.ValidatePasswordCode(ActivationCode, 1);
                if (!String.IsNullOrEmpty(result))
                {
                    res.UserId = result;
                }
                else
                {
                    TempData["IsValidCode"] = "Invalid Code";
                    return RedirectToAction("Confirmation", "Account", routeValues:new { area = "Admin" });
                }
            }
            return View(res);
        }

        //POST: Password Reset
        [HttpPost]
        [OverrideActionFilters]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            String msg = "";
            String Code = Convert.ToString(TempData["Code"]);
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.UserId))
                {
                    msg = userregistrationData.UpdatePassword(model.UserId, model.Password, 1);
                }
                else
                {
                    msg = "Invalid Operation!";
                }

                if (msg == "")
                {
                    String Msg = SendEmail("", "", model.UserId, model.Password,"-","update");
                    return RedirectToAction("Login", "Account", routeValues: new { area = "Admin"});
                }
                TempData["IsValid"] = msg;
                return RedirectToAction("ResetPassword", "Account", routeValues: new { area = "Admin", ActivationCode = Code });
            }
            return View(model);
        }

        [OverrideActionFilters]
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

        #endregion

        #region Service Methods

        [HttpGet]
        public JsonResult GetUsers()
        {
          var cookie=  FormsAuthentication.FormsCookieName;
            IEnumerable<object> users = null;
            Int32 AdminID = Convert.ToInt32(Session["AdminUserID"]);
            if (AdminID > 0)
            {
                users = adata.GetAdminUsersData(AdminID);
            }
                return Json(users.ToList(), JsonRequestBehavior.AllowGet);
     


        }

        [HttpPost]
        public JsonResult AddUser(UserData udata)
        {
            String res = "";
            if (ModelState.IsValid)
            {
                String Code = Guid.NewGuid().ToString();
                String Email = udata.Email.Trim();
                res = adata.AddUpdateAdminUser(udata.FirstName, udata.LastName, Email, udata.Password,Code,udata.Type);
                 if (res == "")
                 {
                    res = SendEmail(udata.FirstName, udata.LastName, Email, udata.Password, Code, "add_"+udata.Type); 
                 }
            }
            else
            {
                res = "Failed!";
            }
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult DeleteUser(Int32 UserID)
        {
            String res = adata.DeleteAdminUser(UserID);
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult UpdateStatus(Int32 UserID, bool Status)
        {
            String res = adata.UpdateUserStatus(UserID, Status);    
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
       
        #endregion

        #region Private Methods
        private String SendEmail(String FirstName, String LasttName, String Email, String Password, String Code, String Header)
        {
            String msg = "";
            var typeHeading = "Admin";
            if (Header.Contains("add"))
            {
                var type = Header.Substring(Header.IndexOf('_'));
                Header = Header.Substring(0,Header.IndexOf('_'));
                typeHeading = (type == "1"?"Admin":"Affiliate");
            }
            if (!String.IsNullOrEmpty(Code) && !String.IsNullOrEmpty(Email))
            {
                try
                {
                    string headerMsg = "";
                    string host = HttpContext.Request.Url.Host;
                    Int32 port = HttpContext.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Request.ApplicationPath;
                    string serverurl, fburl, twurl, gurl, insatUrl = "";
                    if (absolutePath == "/")
                    {
                        serverurl = host + "/Content/images/logo.png";
                        fburl = host + "/Content/images/facebook.png";
                        twurl = host + "/Content/images/twitter.png";
                        gurl = host + "/Content/images/googleplus.png";
                        insatUrl= host + "/Content/images/instagram.png";
                    }
                    else
                    {
                        serverurl = host + absolutePath + "/Content/images/logo.png";
                        fburl = host + absolutePath + "/Content/images/facebook.png";
                        twurl = host + absolutePath + "/Content/images/twitter.png";
                        gurl = host + absolutePath + "/Content/images/googleplus.png";
                        insatUrl= host + absolutePath + "/Content/images/instagram.png";
                    }

                    if (Header == "add")
                      headerMsg = "You are invited to access My PPC Pal "+ typeHeading + " Services";
                    else
                        headerMsg = "Your account password updated successfully";

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:90%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Dear " + FirstName + " " + LasttName + ",</span><br/>");
                    mailBody.Append("<h4 style ='color:#0074b7;height:10px;'>" + headerMsg + "</h4>");
                    mailBody.Append("<h4 style='color:#0074b7;height:10px;padding-top:10px;'>Login details:</h4>");
                    mailBody.Append("<p>");
                    mailBody.Append("<b style='font-size:#e76969;'>User ID:</b> <span>" + Email + "<br />");
                    mailBody.Append("<b style='font-size:#e76969;'>Password:</b> " + Password + "<br/>");
                    mailBody.Append("</p>");
                    if (Header == "add")
                    {
                        mailBody.Append("<span> Please click the following link to change your account password</span><br />");
                        mailBody.Append("<a href = '" + HttpContext.Request.Url.AbsoluteUri.Replace("Account/AddUser", "Account/ResetPassword?ActivationCode=" + Code) + "'>Click here to change your account password.<br/></a>");
                        mailBody.Append("<span> Please click the following link to login</span><br />");
                        mailBody.Append("<a href = '" + HttpContext.Request.Url.AbsoluteUri.Replace("Account/AddUser", "Account/Login") + "'>Click here to login.</a>");
                    }
                    else
                    {
                        mailBody.Append("<span> Please click the following link to login</span><br />");
                        mailBody.Append("<a href = '" + HttpContext.Request.Url.AbsoluteUri.Replace("Admin/Account/ResetPassword", "Admin/Account/Login") + "'>Click here to login.</a>");
                    }
                    mailBody.Append("<p>");
                    mailBody.Append("Regards, <br>");
                    mailBody.Append("My PPC Pal");
                    mailBody.Append("</p></div>");
                    mailBody.Append("<div style='background:#0074b7;color:#fff;text-align:center;height:38px;padding-top:15px;'>");
                    mailBody.Append("<a href='https://www.facebook.com/myppcpal/'><img src='http://" + fburl + "' alt='Facebook' style='height:25px;'></a>");
                    mailBody.Append("<a href='https://www.instagram.com/myppcpal/' style='padding:0 10px;'><img src='http://" + insatUrl + "' alt='Instagram+' style='height:25px;background-color:white;border-radius:23%;'></a>");
                    mailBody.Append("<a href='https://twitter.com/MyPPCPal'><img src='http://" + twurl + "' alt='Twitter' style='height:25px;'></a>");
                    mailBody.Append("</div></div></div>");
                    mailBody.Append("</body></html>");

                    string fromEmail = ConfigurationManager.AppSettings["Email"].ToString();
                    string password = ConfigurationManager.AppSettings["Password"].ToString();
                    string smtphost = ConfigurationManager.AppSettings["Smtphost"].ToString();
                    Int32 smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["Smtpport"].ToString());
                    string toEmail = Email;
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(new MailAddress(fromEmail, "My PPC Pal"), new MailAddress(toEmail));
                    message.IsBodyHtml = true;
                    message.Priority = MailPriority.High;

                    message.Subject = "||My PPC Pal "+ typeHeading + " Login Details||";
                    message.Body = mailBody.ToString();
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = smtphost;
                    smtp.EnableSsl = true; 
                    NetworkCredential NetworkCred = new NetworkCredential(fromEmail, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = smtpport;
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }
            }
            return msg;
        }
        #endregion
    }
}