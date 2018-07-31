using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Mpp.UTILITIES.Statics;

namespace Mpp.BUSINESS
{
   public class EmailAlert
    {
        //Sign up email
        public String SendActivationEmail(String FirstName, String LasttName, String Email, String ActivationCode, String CurrentURL)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(ActivationCode) && !String.IsNullOrEmpty(Email))
            {
                try
                {
                    string host = HttpContext.Current.Request.Url.Host;
                    Int32 port = HttpContext.Current.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Current.Request.ApplicationPath;
                    string serverurl, fburl, twurl, gurl,insatUrl = "";
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
                        gurl = host + port + absolutePath + "/Content/images/googleplus.png";
                        insatUrl= host + port + absolutePath +"/Content/images/instagram.png";
                    }
                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:98%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<p>Dear " + FirstName + " " + LasttName + ",</p>");
                    mailBody.Append("<h4 style='color:#0074b7; margin: 5px 0;'>Welcome to \"My PPC Pal\" - The most advanced tool for optimizing your Amazon PPC campaigns.</h4>");
                   // mailBody.Append("<ul style='line-height:28px;'>");
                    //mailBody.Append("<li>Please give user permissions to admin@myppcpal.com in your Amazon Seller Central account and enter your Amazon Seller Name into the software as specified in the instructions provided.</li>");
                    //mailBody.Append("<li>Once we have accepted the invitation, Amazon will confirm the invitation has been accepted. Please return to Seller Central to grant us the required user permissions. (See \"Help\" section for additional details).</li>");
                    //mailBody.Append("<li>We will begin to import your PPC data and will notify you once it's completed. If you do not receive a confirmation email within a few hours please contact us at support@myppcpal.com</li>");
                   // mailBody.Append("<li>Please see our instructional videos under the 'Help' section, this will teach you how to set up My PPC Pal to run your campaigns and how to get the most from the software.</li>");
                    //mailBody.Append("<li>Start improving your PPC!</li>");
                   // mailBody.Append("</ul>");
                    mailBody.Append("<h4 style='color:#0074b7;margin: 5px 0;'>Login details:</h4>");
                    mailBody.Append("<p>");
                    mailBody.Append("<b style='font-size:#e76969;'>User ID:</b> <span>" + Email + "<br />");
                    mailBody.Append("</p>");
                    mailBody.Append("<p> Please click the following link to activate your account</p>");
                    mailBody.Append("<a style='color:#e76969;' href = '" + HttpContext.Current.Request.Url.AbsoluteUri.Replace(CurrentURL, "UserAccount/UserActivation?ActivationCode=" + ActivationCode) + "'>Click here to activate your account.</a>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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

                    message.Subject = "||My PPC Pal Login Details||";
                    message.Body = mailBody.ToString();
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = smtphost;
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(fromEmail, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = smtpport;
                    smtp.Send(message);
                    message.Dispose();
                }
                catch (Exception e)
                {
                    msg = e.Message.ToString();
                    LogFile.WriteLog("SendActivationEmail_1 - " + SessionData.SellerName + ": " + msg);
                }
            }
            return msg;
        }

        //User email update re-activation
        public String SendActivationEmail(Int32 UserID, String FirstName, String LasttName, String Email,String CurrentURL, Enum UserType)
        {
            String msg = "";
            String ActivationCode = Guid.NewGuid().ToString();
            AccountData userregistrationData = new AccountData();
            msg = userregistrationData.InsertActivationCode(UserID, ActivationCode, AccounType.MppUser);
            if (!String.IsNullOrEmpty(ActivationCode) && !String.IsNullOrEmpty(Email))
            {
                try
                {
                    string headerMsg = "";
                    string host = HttpContext.Current.Request.Url.Host;
                    Int32 port = HttpContext.Current.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Current.Request.ApplicationPath;
                    string serverurl = "";
                    string subject = "";
                    int Type = Convert.ToInt32(UserType);
                    if (Type == 1)
                    {
                        headerMsg = "Account Activation";
                        subject = "My PPC Pal Account Activation";
                    }
                    else
                    {
                        headerMsg = "Your email has been updated successfully";
                        subject = "||My PPC Pal Email Update||";
                    }
                    string  fburl, twurl, gurl, insatUrl = "";
                    if (absolutePath == "/")
                    {
                        serverurl = host + "/Content/images/logo.png";
                        fburl = host + "/Content/images/facebook.png";
                        twurl = host + "/Content/images/twitter.png";
                        gurl = host + "/Content/images/googleplus.png";
                        insatUrl = host + "/Content/images/instagram.png";
                    }
                    else
                    {
                        serverurl = host + absolutePath + "/Content/images/logo.png";
                        serverurl = host + absolutePath + "/Content/images/logo.png";
                        fburl = host + absolutePath + "/Content/images/facebook.png";
                        twurl = host + absolutePath + "/Content/images/twitter.png";
                        gurl = host + port + absolutePath + "/Content/images/googleplus.png";
                        insatUrl = host + port + absolutePath + "/Content/images/instagram.png";
                    }
                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:98%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;color:white;height:37px;line-height:34px;'>Account Activation</div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<p>Dear " + FirstName + " " + LasttName + ",</p>");
                   // mailBody.Append("<h4 style='color:#0074b7;height:10px;'>Welcome to \"My PPC Pal\" - The most advanced tool for optimizing your Amazon PPC campaigns.</h4>");
                    // mailBody.Append("<ul style='line-height:28px;'>");
                    //mailBody.Append("<li>Please give user permissions to admin@myppcpal.com in your Amazon Seller Central account and enter your Amazon Seller Name into the software as specified in the instructions provided.</li>");
                    //mailBody.Append("<li>Once we have accepted the invitation, Amazon will confirm the invitation has been accepted. Please return to Seller Central to grant us the required user permissions. (See \"Help\" section for additional details).</li>");
                    //mailBody.Append("<li>We will begin to import your PPC data and will notify you once it's completed. If you do not receive a confirmation email within a few hours please contact us at support@myppcpal.com</li>");
                    // mailBody.Append("<li>Please see our instructional videos under the 'Help' section, this will teach you how to set up My PPC Pal to run your campaigns and how to get the most from the software.</li>");
                    //mailBody.Append("<li>Start improving your PPC!</li>");
                    // mailBody.Append("</ul>");
                   // mailBody.Append("<h4 style='color:#0074b7;height:10px;'>Login details:</h4>");
                   // mailBody.Append("<p>");
                   // mailBody.Append("<b style='font-size:#e76969;'>User ID:</b> <span>" + Email + "<br />");
                 //   mailBody.Append("</p>");
                    mailBody.Append("<p> Please click the following link to activate your account</p>");
                    mailBody.Append("<a style='color:#e76969;' href = '" + HttpContext.Current.Request.Url.AbsoluteUri.Replace(CurrentURL, "UserAccount/UserActivation?ActivationCode=" + ActivationCode) + "'>Click here to activate your account.</a>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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

                    message.Subject = subject;
                    message.Body = mailBody.ToString();
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = smtphost;
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(fromEmail, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = smtpport;
                    smtp.Send(message);
                    message.Dispose();
                }
                catch (Exception e)
                {
                    msg = e.Message.ToString();
                    LogFile.WriteLog("SendActivationEmail_2 - " + SessionData.SellerName + ": " + msg);

                }
            }
            return msg;
        }

        public String SendPasscodeChangeEmail(String Email, String Code)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Code) && !String.IsNullOrEmpty(Email))
            {
                try
                {
                    string host = HttpContext.Current.Request.Url.Host;
                    Int32 port = HttpContext.Current.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Current.Request.ApplicationPath;
                    string serverurl, fburl, twurl, gurl, insatUrl = "";
                    if (absolutePath == "/")
                    {
                        serverurl = host + "/Content/images/logo.png";
                        fburl = host + "/Content/images/facebook.png";
                        twurl = host + "/Content/images/twitter.png";
                        gurl = host + "/Content/images/googleplus.png";
                        insatUrl = host + "/Content/images/instagram.png";
                    }
                    else
                    {
                        serverurl = host + absolutePath + "/Content/images/logo.png";
                        fburl = host + absolutePath + "/Content/images/facebook.png";
                        twurl = host + absolutePath + "/Content/images/twitter.png";
                        gurl = host + absolutePath + "/Content/images/googleplus.png";
                        insatUrl = host + absolutePath + "/Content/images/instagram.png";
                    }

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:70%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello,</span><br/>");
                    mailBody.Append("<h4 style='color:#0074b7;height:10px;'>Password Reset Request.</h4>");
                    mailBody.Append("<span> Please click the following link to reset your account password</span><br />");
                    mailBody.Append("<a href = '" + HttpContext.Current.Request.Url.AbsoluteUri.Replace("UserAccount/Password", "UserAccount/ResetPassword?ActivationCode=" + Code) + "'>Click here to reset your account password.</a>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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

                    message.Subject = "||My PPC Pal Password Reset Details||";
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
                    LogFile.WriteLog("SendPasscodeChangeEmail - " + SessionData.SellerName + ": " + msg);
                }
            }
            return msg;
        }

        public string SendPasswordUpdateEmail(String FirstName, String LasttName, String Email)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    string host = HttpContext.Current.Request.Url.Host;
                    Int32 port = HttpContext.Current.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Current.Request.ApplicationPath;
                    string serverurl, fburl, twurl, gurl, insatUrl = "";
                    if (absolutePath == "/")
                    {
                        serverurl = host + "/Content/images/logo.png";
                        fburl = host + "/Content/images/facebook.png";
                        twurl = host + "/Content/images/twitter.png";
                        gurl = host + "/Content/images/googleplus.png";
                        insatUrl = host + "/Content/images/instagram.png";
                    }
                    else
                    {
                        serverurl = host + absolutePath + "/Content/images/logo.png";
                        fburl = host + absolutePath + "/Content/images/facebook.png";
                        twurl = host + absolutePath + "/Content/images/twitter.png";
                        gurl = host + absolutePath + "/Content/images/googleplus.png";
                        insatUrl = host + absolutePath + "/Content/images/instagram.png";
                    }

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:70%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello " + FirstName + " " + LasttName + ",</span><br/>");
                    mailBody.Append("<h4 style='color:#0074b7;height:10px;'>Password updated successfully.</h4>");
                    mailBody.Append("<h4 style='color:#0074b7;height:10px;padding-top:10px;'>Login details:</h4>");
                    mailBody.Append("<p>");
                    mailBody.Append("<b style='font-size:#e76969;'>User ID:</b> <span>" + Email + "<br />");
                    //mailBody.Append("<b style='font-size:#e76969;'>Password:</b> " + Password + "<br/>");
                    mailBody.Append("</p>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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
                    message.Priority = MailPriority.Normal;

                    message.Subject = "||My PPC Pal Password Update||";
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
                    LogFile.WriteLog("SendPasswordUpdateEmail - " + SessionData.SellerName + ": " + msg);
                }
            }
            return msg;
        }

        public static string PaymentAlertMail(string Email, string headerMsg)
        {
            var msg = "";
            if (!string.IsNullOrEmpty(headerMsg) && !string.IsNullOrEmpty(Email))
            {
                try
                {
                    string host = HttpContext.Current.Request.Url.Host;
                    Int32 port = HttpContext.Current.Request.Url.Port; //Needed for local host (Ex: localhost:4917)
                    string absolutePath = HttpContext.Current.Request.ApplicationPath;
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
                        insatUrl = host + absolutePath + "/Content/images/instagram.png";
                    }

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:70%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello, </span><br/>");
                    mailBody.Append("<p style='color:#0074b7;font-weight:bold;margin-bottom:15px;'>" + headerMsg+"</p>");
                    mailBody.Append("<p>");
                    mailBody.Append("If you have any questions regarding this payment, please contact My PPC Pal customer support - support@myppcpal.com.");
                    mailBody.Append("</p>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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
                    message.Priority = MailPriority.Normal;

                    message.Subject = "||My PPC Pal Payment|| ";
                    message.Body = mailBody.ToString();
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = smtphost;
                    smtp.EnableSsl = true; 
                    NetworkCredential NetworkCred = new NetworkCredential(fromEmail, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = smtpport;
                    smtp.Send(message);
                    msg = "success";
                }
                catch (Exception e)
                {
                    msg= e.Message.ToString();
                    LogFile.WriteLog("PaymentAlert - " + Email + ": " + msg);
                }
            }
            return msg;
        }

        //Email notification for new sellers
        public static String SendNewUserDataAlert(String Email)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    String hostaddr = ConfigurationManager.AppSettings["Hostaddr"].ToString();
                    String serverurl = hostaddr + "/UserAccount/Login";
                    String logo = hostaddr + "/Content/images/logo.png";
                    String fburl = hostaddr + "/Content/images/facebook.png";
                    String twurl = hostaddr + "/Content/images/twitter.png";
                    String gurl = hostaddr + "/Content/images/googleplus.png";
                    string insatUrl = hostaddr + "/Content/images/instagram.png";

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:80%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='" + logo + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello,</span><br/>");
                    mailBody.Append("<h4 style='color:#0074b7;height:10px;'>Thank you for waiting, your PPC data has imported successfully.</h4>");
                    mailBody.Append("<ul style='line-height:28px;padding-top:15px;'>");
                    mailBody.Append("<li>All of your campaigns are turned off in My PPC Pal and set to \"Recommended\" approach by default.</li>");
                    mailBody.Append("<li>You must activate and set up at least one campaign in My PPC Pal to begin optimizations to your Amazon account.</li>");
                    mailBody.Append("<li>Please see our instructional video's under the 'Help' section, this will teach you how to set up My PPC Pal to run your campaigns and how to get the most from the software.</li>");
                    mailBody.Append("</ul>");
                    mailBody.Append("<div style='text-align:center;'>");
                    mailBody.Append("<a href= '" + serverurl + "' style='font-size:16px;color:#fff;text-decoration:none;'>");
                    mailBody.Append("<div style='text-align:center;background:#0074b7;height:35px;width:100px;border:1px solid #0074b7;border-radius:6px;margin:0 auto;padding-top:16px;'>Login</div>");
                    mailBody.Append("</a></div>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
                    mailBody.Append("My PPC Pal");
                    mailBody.Append("</p></div>");
                    mailBody.Append("<div style='background:#0074b7;color:#fff;text-align:center;height:38px;padding-top:15px;'>");
                    mailBody.Append("<a href='https://www.facebook.com/myppcpal/'><img src='http://" + fburl + "' alt='Facebook' style='height:25px;'></a>");
                    mailBody.Append("<a href='https://www.instagram.com/myppcpal/'><img src='http://" + insatUrl + "' alt='Instagram+' style='height:25px;padding:0 10px'></a>");
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

                    message.Subject = "||Data Imported||";
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

        //Email notification for new campaigns added 
        public static String SendNewCampaignsAlert(String Email)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    String hostaddr = ConfigurationManager.AppSettings["Hostaddr"].ToString();
                    String serverurl = hostaddr + "/UserAccount/Login";
                    String logo = hostaddr + "/Content/images/logo.png";
                    String fburl = hostaddr + "/Content/images/facebook.png";
                    String twurl = hostaddr + "/Content/images/twitter.png";
                    String gurl = hostaddr + "/Content/images/googleplus.png";
                    string insatUrl = hostaddr + "/Content/images/instagram.png";
                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:80%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='" + logo + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello,</span><br/><br/>");
                    mailBody.Append("<span> We have imported one or more new campaigns to your \"My Campaigns\" section in My PPC Pal</span><br /><br/>");
                    mailBody.Append("<span>Please login in to set up your optimization rules for your new campaign/s</span><br /><br/>");
                    mailBody.Append("<div style='text-align:center;'>");
                    mailBody.Append("<a href= '" + serverurl + "' style='font-size:16px;color:#fff;text-decoration:none;'>");
                    mailBody.Append("<div style='text-align:center;background:#0074b7;height:35px;width:100px;border:1px solid #0074b7;border-radius:6px;margin:0 auto;padding-top:16px;'>Login</div>");
                    mailBody.Append("</a></div>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
                    mailBody.Append("My PPC Pal");
                    mailBody.Append("</p></div>");
                    mailBody.Append("<div style='background:#0074b7;color:#fff;text-align:center;height:38px;padding-top:15px;'>");
                    mailBody.Append("<a href='https://www.facebook.com/myppcpal/'><img src='http://" + fburl + "' alt='Facebook' style='height:25px;'></a>");
                    mailBody.Append("<a href='https://www.instagram.com/myppcpal/'><img src='http://" + insatUrl + "' alt='Instagram+' style='height:25px;padding:0 10px'></a>");
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

                    message.Subject = "||My PPC Pal New Campaigns Imported||";
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

        public String SendNewUserAccessAlert(String Email, String FirstName, String LastName)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    String hostaddr = ConfigurationManager.AppSettings["Hostaddr"].ToString();
                    String serverurl = hostaddr + "/UserAccount/Login";
                    String logo = hostaddr + "/Content/images/logo.png";
                    String fburl = hostaddr + "/Content/images/facebook.png";
                    String twurl = hostaddr + "/Content/images/twitter.png";
                    String gurl = hostaddr + "/Content/images/googleplus.png";
                    string insatUrl = hostaddr + "/Content/images/instagram.png";

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:80%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + logo + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello " + FirstName + " " + LastName + ", </span><br/><br/>");
                    mailBody.Append("<span>Please login to your My PPC Pal account and complete your on boarding process. We still need access to your PPC data via the Amazon Advertising API. Access is granted using Amazon's secure platform 'Login with Amazon'. </span><br /><br/>");
                    mailBody.Append("<span>Please click here to complete your account setup!</span><br /><br/>");
                    mailBody.Append("<div style='text-align:center;'>");
                    mailBody.Append("<a href= 'http://" + serverurl + "' style='font-size:16px;color:#fff;text-decoration:none;'>");
                    mailBody.Append("<div style='text-align:center;background:#0074b7;height:35px;width:100px;border:1px solid #0074b7;border-radius:6px;margin:0 auto;padding-top:16px;'>Login</div>");
                    mailBody.Append("</a></div>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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

                    message.Subject = "||My PPC Pal Continue Onboarding||";
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

        public String SendTrialExpiresAlert(String Email, DateTime ExpiryDate, String FirstName, String LastName)
        {
            String msg = "";
            if (!String.IsNullOrEmpty(Email))
            {
                try
                {
                    String hostaddr = ConfigurationManager.AppSettings["Hostaddr"].ToString();
                    String serverurl = hostaddr + "/UserAccount/Login";
                    String logo = hostaddr + "/Content/images/logo.png";
                    String fburl = hostaddr + "/Content/images/facebook.png";
                    String twurl = hostaddr + "/Content/images/twitter.png";
                    String gurl = hostaddr + "/Content/images/googleplus.png";
                    string insatUrl = hostaddr + "/Content/images/instagram.png";
                    String Subject;

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<html><body>");
                    mailBody.Append("<div id='no'><u></u>");
                    mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:80%; border:1px solid darkgray;background-color:#fff;'>");
                    mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                    mailBody.Append("<img src='http://" + logo + "' alt='My PPC Pal'></div>");
                    mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                    mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                    mailBody.Append("<span>Hello " + FirstName + " " + LastName + ", </span><br/><br/>");
                    if (ExpiryDate < DateTime.Now)
                    {
                        Subject = "My PPC Pal - Trial Ended";
                        mailBody.Append("<span>We just wanted to let you know that your 30-day free trial with My PPC Pal has ended</span><br /><br/>");
                        mailBody.Append("<span>To continue using My PPC Pal, please login to your account, subscribe and add a payment method</span><br /><br/>");
                    }
                    else
                    {
                        Subject = "||Your free trial is coming to an end soon||";
                        mailBody.Append("<span>Your trial will expire on " + ExpiryDate.ToShortDateString() + "</span><br /><br/>");
                        mailBody.Append("<span>Please add a payment method to your account and click subscribe to continue using My PPC Pal!</span><br /><br/>");
                    }
                    mailBody.Append("<div style='text-align:center;'>");
                    mailBody.Append("<a href= 'http://" + serverurl + "' style='font-size:16px;color:#fff;text-decoration:none;'>");
                    mailBody.Append("<div style='text-align:center;background:#0074b7;height:35px;width:100px;border:1px solid #0074b7;border-radius:6px;margin:0 auto;padding-top:16px;'>Login</div>");
                    mailBody.Append("</a></div>");
                    mailBody.Append("<p style='line-height:25px;'>");
                    mailBody.Append("As always, thank you for your business! <br>");
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

                    message.Subject = Subject;
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

       
    }
}
