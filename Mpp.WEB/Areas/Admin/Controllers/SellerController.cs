using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using Mpp.WEB.Filters;
using Stripe;
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
using static Mpp.UTILITIES.Statics;

namespace Mpp.WEB.Areas.Admin.Controllers
{
    [SessionExpireFilter]
    [AdminRoleFilter(UserType = "1")]
    public class SellerController : Controller
    {
        #region Class Variables and Constants
        private AdminData adata;
        private EmailAlert emailalert;
        private OptimizeData optimizeData;
        #endregion

        #region Constructors
        public SellerController()
        {
            this.adata = new AdminData();
            this.emailalert = new EmailAlert();
            this.optimizeData = new OptimizeData();
           
        }

        #endregion
        
        #region Public Methods
        // GET: Admin/Seller
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Dashboard()
        {
            return View();
        }
        #endregion

        #region Service Methods

        [HttpPost]
        public String SendAccessCode(String Email, String AccessCode)
        {
            return SendAccessCodeEmail(Email.Trim(), AccessCode);
        }

        [HttpGet]
        public JsonResult GetSellerDashboard(int type)
        {
            var SellerDashboard = new Dictionary<string, object>();
            var res = adata.GetSellerDashboardData();
            var res1 = adata.GetSellerData(type);
            SellerDashboard.Add("Dashboard", res);
            SellerDashboard.Add("SellerUsers", res1);
            return Json(SellerDashboard, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetRevenue(int type)
        {
            var payDetail = adata.GetRevenueData(type);
            return Json(payDetail, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetYearlyRevenue(int type)


        {
            var yearlyPayDetail = adata.GetYearlyRevenueData(type);
            return Json(yearlyPayDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSellers(int type)
        {
            var res = adata.GetSellerData(type);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSeller(AdminSeller seller)
        {
            String res = adata.UpdateSeller(seller.SellerID, seller.Active, seller.PlanStatus, seller.SellerAccess);
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult DeleteSeller(Int32 sellerID, String CustID)
        {
            String res = "";
            var customerService = new StripeCustomerService();
            try
            {
                if (!String.IsNullOrWhiteSpace(CustID))
                    customerService.Delete(CustID);
                    res = DeletesellerDetails(sellerID);
            }
            catch (Exception ex)
            {
                res = "Customer not registred on stripe.";
                res = DeletesellerDetails(sellerID);
            }
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult ApplyPromo(String custm_id, String code)
        {
            String msg = "";
            if (!string.IsNullOrWhiteSpace(custm_id))
            {
                msg = StripeServices.ApplyPromoCode(custm_id, code.Trim());
            }
            else
            {
                msg = Constant.STRIPE_ID_NOTFOUND;
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SendActivationEmail(AdminSeller model)
        {
            String msg = "";
            if (!model.Active)
            {
                String _url = "Admin/Seller/SendActivationEmail";
                if (String.IsNullOrWhiteSpace(model.Stp_CustID))
                {
                    String ActivationCode = Guid.NewGuid().ToString();
                    AccountData userregistrationData = new AccountData();
                    msg = userregistrationData.InsertActivationCode(model.SellerID, ActivationCode, AccounType.Admin);
                    if (msg == "")
                    {
                        msg = emailalert.SendActivationEmail(model.FirstName, model.LastName, model.Email, ActivationCode, _url);
                    }
                }else
                {
                    msg = emailalert.SendActivationEmail(model.SellerID, model.FirstName, model.LastName, model.Email, _url, AccounType.Admin);//When user updates emial etc//

                }

            }else
            {
                msg = "Email can not be sent since employee is already active in the system";
            }
            return new JsonResult() { Data = msg, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult SearchCampaigns(string term,int userId)

        {
            var data = adata.GetCampaigns(term,userId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetCampaignLog(PagingOptions options, Int64 CampID,int userId)
        {
          
            var start = options.Start;
            var length = options.Length;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;
            var firstCampaign = "";
            List<Log> camplog = null;
            if (CampID > 0)
            {
                camplog = optimizeData.GetCampaignLogData(CampID, userId, out firstCampaign);

                var v = from a in camplog select a;

                recordsTotal = v.Count();
                camplog = v.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new { recordsTotal = recordsTotal, data = camplog,FirstCampaign=firstCampaign });
        }

        [HttpPost]
        public JsonResult GetUserAccessLog(PagingOptions options, int userId)
        {

            var start = options.Start;
            var length = options.Length;
            int pageSize = length;
            int skip = start;
            int recordsTotal = 0;

            var accessLog = optimizeData.GetSellerAccessLogData(userId);
            if (accessLog != null)
            {
                var v = from a in accessLog select a;
                recordsTotal = v.Count();
                accessLog = v.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new { recordsTotal = recordsTotal, data = accessLog });
        }
        #endregion

        #region Private Methods
        private String DeletesellerDetails(Int32 UserID)
        {
            String res = adata.DeleteSeller(UserID);
            //if (res == "")
            //    MppUtility.DeleteSellerDirectory(UserID);
            return res;
        }
        private String SendAccessCodeEmail(String Email, String AccessCode)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    string headerMsg = "";
                    string host = HttpContext.Request.Url.Host;
                    Int32 port = HttpContext.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Request.ApplicationPath;
                    string serverurl = "";
                    headerMsg = "Seller Access Code";
                    if (absolutePath == "/")
                        serverurl = host + "/Content/images/logo.png";
                    else
                        serverurl = host + absolutePath + "/Content/images/logo.png";

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no' style='overflow: hidden; background-color: aliceblue; '><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;text-align:center; border: 1px; font-size:12px;font-family:Tahoma,Verdana,Arial,sans-serif;width:80%; border:1px solid;background-color: white;'>");
                    mailBody.Append("<div style='background-color:white;color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:black;color:#fff;font-size:16px;padding:8px;text-align:left'>");
                    mailBody.Append(headerMsg + "  </div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:8.5px'>");
                    mailBody.Append("<span>Hello,</span><br/>");
                    mailBody.Append("<span> Please verify the following code in your Amazon Seller Central account under Settings > User Permissions and continue the final steps to grant access to My PPC Pal.</span><br /><br/>");
                    mailBody.Append("<span><b>Access Code: </b>" + AccessCode + "</span><br />");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
                    mailBody.Append("My PPC Pal");
                    mailBody.Append(" </p></div>");
                    mailBody.Append("<div style='border-top:1px dotted;margin-top:40px;text-align:left'>");
                    mailBody.Append("This is a system-generated e-mail; please do not reply to this message.</div></div></div>");
                    mailBody.Append("</body></html>");

                    string fromEmail = ConfigurationManager.AppSettings["Email"].ToString();
                    string password = ConfigurationManager.AppSettings["Password"].ToString();
                    string smtphost = ConfigurationManager.AppSettings["Smtphost"].ToString();
                    Int32 smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["Smtpport"].ToString());
                    string toEmail = Email;
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(new MailAddress(fromEmail, "My PPC Pal"), new MailAddress(toEmail));
                    message.IsBodyHtml = true;
                    message.Priority = MailPriority.High;

                    message.Subject = "||My PPC Pal Seller Access Code||";
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
                    msg = e.Message.ToString();
                }
            }
            return msg;
        }
        #endregion
    }
}