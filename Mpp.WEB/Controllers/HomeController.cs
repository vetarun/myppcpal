using Mpp.BUSINESS;
using Mpp.WEB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Questions()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        //send feedback email
        public ActionResult SendFeedbackEmail(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string from = ConfigurationManager.AppSettings["Email"].ToString();
                    using (MailMessage mail = new MailMessage(from, ConfigurationManager.AppSettings["FeedMailTo"].ToString()))

                    {
                        mail.Subject = "|| MyPPCPal User Feedback||";
                        mail.Body = feedback.Content + "\n\n\nThanks\n " + feedback.Name + "\n" + feedback.Email;
                        if (feedback.Attachment != null)
                        {
                            string fileName = Path.GetFileName(feedback.Attachment.FileName);
                            mail.Attachments.Add(new Attachment(feedback.Attachment.InputStream, fileName));

                        }
                        mail.IsBodyHtml = false;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = ConfigurationManager.AppSettings["Smtphost"].ToString();
                        smtp.EnableSsl = true;
                        NetworkCredential networkCredential = new NetworkCredential(from, ConfigurationManager.AppSettings["Password"]);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = networkCredential;
                        smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Smtpport"]);
                        smtp.Send(mail);
                        return Json(true, JsonRequestBehavior.AllowGet);


                    }
                }

                catch (Exception ex)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}