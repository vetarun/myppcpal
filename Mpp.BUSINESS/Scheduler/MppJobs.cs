using Mpp.BUSINESS.DataLibrary;
using Quartz;
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

namespace Mpp.BUSINESS.Scheduler
{
    // Inventory Reports (First Time)
    public class ProcessFirstTimeInventoryReportsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessFirstTimeInventoryReports job is runnning");
            ReportsAPI r = new ReportsAPI();
            r.ProcessInventoryReports(ReportStatus.NOTSET_SIGNUP, ReportStatus.SET_SIGNUP);
        }
    }

    // SnapShot Reports
    public class ProcessSnapShotReportsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessSnapShotReports job is runnning");
            ReportsAPI r = new ReportsAPI();
            r.ProcessProductsSnapShot(ReportStatus.NOTSET, ReportStatus.SET);
        }
    }

    // Inventory Reports
    public class ProcessInventoryReportsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessInventoryReportsJob job is running");
            ReportsAPI r = new ReportsAPI();
            r.ProcessInventoryReports(ReportStatus.NOTSET, ReportStatus.SET);
        }
    }

    // Inventory Reports (Refresh)
    public class ProcessRefreshReportsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessRefreshReports job is running");
            ReportsAPI r = new ReportsAPI();
            r.ProcessInventoryReports(ReportStatus.REFRESH_NOTSET, ReportStatus.REFRESH_SET);
        }
    }

    // Failed Reports
    public class ProcessFailedReportsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessFailedReports job is running");
            ReportsAPI r = new ReportsAPI();
            r.ProcessInventoryReports(ReportStatus.FAILED_NOTSET, ReportStatus.NOTREQUIRED);
            r.ProcessProductsSnapShot(ReportStatus.FAILED_NOTSET, ReportStatus.NOTREQUIRED);
        }
    }

    //Process Optimizations
    public class ProcessOptimizationsJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("ProcessOptimizations job is running" + DateTime.Now);
            ReportsAPI r = new ReportsAPI();
            r.ProcessOptimizations();
        }
    }

    //Process All Daily Reports 
    public class Process_DailyServicesJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("Process_DailyServices job is running");
            MppService service = new MppService();
            service.Process_DailyService();
        }
    }
    public class Process_DeleteDataServiceJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("Process_DeleteDataService job is running");
            MppService service = new MppService();
            service.Process_DeleteDataService();
        }
    }

    //Process Billing, Email alerts, Notifications  
    public class Process_AllservicesJob : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SchedulerLog.WriteLog("Process_Allservices job is running");
            MppService service = new MppService();
            service.Process_AllServices();
        }
    }

    public class Process_AnalyticsUsersCount : Quartz.IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            AccountData ar = new AccountData();
            var data = ar.GetArchiveData();
            if (data != null && data.Any())
            {
                foreach (var r in data)
                {
                    String msg = "";
                    if (!String.IsNullOrEmpty(r.ActivationCode.ToString()) && !String.IsNullOrEmpty(r.Email))
                    {
                        try
                        {

                            string host = ConfigurationManager.AppSettings["Hostaddr"]; ;
                            string serverurl, fburl, twurl, gurl, insatUrl = "";
                            serverurl = host + "/Content/images/logo.png";
                            fburl = host + "/Content/images/facebook.png";
                            twurl = host + "/Content/images/twitter.png";
                            gurl = host + "/Content/images/googleplus.png";
                            insatUrl = host + "/Content/images/instagram.png";
                            var confirUrl = host + "/UserAccount/UserActivation?ActivationCode=" + r.ActivationCode;
                            StringBuilder mailBody = new StringBuilder();
                            mailBody.Append("<html><body>");
                            mailBody.Append("<div id='no'><u></u>");
                            mailBody.Append("<div style='margin:0 auto;line-height:18px;padding:0;font-size:16px;font-family:Palanquin,Arial,Helvetica,sans-serif;width:98%; border:1px solid darkgray;background-color:#fff;'>");
                            mailBody.Append("<div style='background-color:#fff;padding:10px;text-align:left'>");
                            mailBody.Append("<img src='http://" + serverurl + "' alt='My PPC Pal'></div>");
                            mailBody.Append("<div style='background-color:#0074b7;height:20px;'></div>");
                            mailBody.Append("<div style='margin-top:20px;text-align:left; padding-left:12.5px;color:#020215;'>");
                            mailBody.Append("<p>Dear " + r.FirstName + " " + r.LastName + ",</p>");
                            mailBody.Append("<h4 style='color:#0074b7; margin: 5px 0;'>Welcome to \"My PPC Pal\" - The most advanced tool for optimizing your Amazon PPC campaigns.</h4>");
                            mailBody.Append("<ul style='line-height:28px;'>");
                            mailBody.Append("<li>Please give user permissions to admin@myppcpal.com in your Amazon Seller Central account and enter your Amazon Seller Name into the software as specified in the instructions provided.</li>");
                            mailBody.Append("<li>Once we have accepted the invitation, Amazon will confirm the invitation has been accepted. Please return to Seller Central to grant us the required user permissions. (See \"Help\" section for additional details).</li>");
                            mailBody.Append("<li>We will begin to import your PPC data and will notify you once it's completed. If you do not receive a confirmation email within a few hours please contact us at support@myppcpal.com</li>");
                            mailBody.Append("<li>Please see our instructional videos under the 'Help' section, this will teach you how to set up My PPC Pal to run your campaigns and how to get the most from the software.</li>");
                            mailBody.Append("<li>Start improving your PPC!</li>");
                            mailBody.Append("</ul>");
                            mailBody.Append("<h4 style='color:#0074b7;margin: 5px 0;'>Login details:</h4>");
                            mailBody.Append("<p>");
                            mailBody.Append("<b style='font-size:#e76969;'>User ID:</b> <span>" + r.Email + "<br />");
                            mailBody.Append("</p>");
                            mailBody.Append("<p> Please click the following link to activate your account</p>");
                            mailBody.Append("<a style='color:#e76969;' href=" + confirUrl + " >Click here to activate your account.</a>");
                            mailBody.Append("<p style='line-height:25px;'>");
                            mailBody.Append("As always, thank you for your business! <br>");
                            mailBody.Append("My PPC Pal");
                            mailBody.Append("</p></div>");
                            mailBody.Append("<div style='background:#0074b7;color:#fff;text-align:center;height:38px;padding-top:15px;'>");
                            mailBody.Append("<a href='https://www.facebook.com/myppcpal/'><img src='http://" + fburl + "' alt='Facebook' style='height:25px;'></a>");
                            mailBody.Append("<a href='https://www.instagram.com/myppcpal/'><img src='http://" + insatUrl + "' alt='Instagram+' style='height:25px;padding:0 10px;border-radius:55%;'></a>");
                            mailBody.Append("<a href='https://twitter.com/MyPPCPal'><img src='http://" + twurl + "' alt='Twitter' style='height:25px;'></a>");
                            mailBody.Append("</div></div></div>");
                            mailBody.Append("</body></html>");

                            string fromEmail = ConfigurationManager.AppSettings["Email"].ToString();
                            string password = ConfigurationManager.AppSettings["Password"].ToString();
                            string smtphost = ConfigurationManager.AppSettings["Smtphost"].ToString();
                            Int32 smtpport = Convert.ToInt32(ConfigurationManager.AppSettings["Smtpport"].ToString());
                            string toEmail = r.Email;
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
                            AccountData dt = new AccountData();
                            dt.Update48HourAlert(r.ActivationCode);
                        }
                        catch (Exception e)
                        {
                            msg = e.Message.ToString();
                            LogFile.WriteLog("SendActivationEmail_1 - " + "System" + ": " + msg);
                        }

                    }
                }
            }
        }
    }
}
