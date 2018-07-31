using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Controllers
{
    public class WebHookController : Controller
    {
        // GET: WebHook
        SellerData sellerData;
        UserPlanData upData;
        public WebHookController()
        {
            this.sellerData = new SellerData();
            upData = new UserPlanData();
        }
        public ActionResult Index()
        {
            return View();
        }

        public HttpStatusCode WebHook()
        {
            try
            {
                var paymentAlertStatus = 0;
                string secret =MppUtility.ReadConfig("MS_WebHookReceiverSecret_Custom");
                var postdata = new StreamReader(HttpContext.Request.InputStream).ReadToEnd();
                var emailboday = "";
                var signature = Request.Headers["Stripe-Signature"];
                var stripeEvent = StripeEventUtility.ConstructEvent(postdata,signature, secret);
                var eventid = stripeEvent.Id;
                var eventdata = StripeHelper.GetStripeEvent(eventid);
                var data = JObject.Parse(postdata);
                var custid = data["data"].First().First["customer"].ToString();
                var msg = "";

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
                            emailboday = "A payment of $" + Amount_off + " was made using your card on file ending with <b> " + last4 + " </b>";
                            paymentAlertStatus = 1;
                            break;
                        case "invoice.payment_failed":
                            msg = AccountData.UpdatePaymentStatus(custid, Amount, Amount_off, coupon_id, discount, startperiod, endperiod, 0);
                            upData.UpdatePlanData(custid,0);
                            StripeHelper.UnSubscribe(custid);
                            emailboday = @"A payment $" + Amount_off + " failed from your card ending with <b> " + last4 + " </b>" +"Please check your card details or add new card.";
                            paymentAlertStatus = 2;
                            break;
                       
                    }
                    if (msg == "" && Amount_off != 0)
                    {
                        var respo = EmailAlert.PaymentAlertMail(email, emailboday);
                        if (respo == "success")
                        {
                            sellerData.UpdatePaymentAlert(paymentAlertStatus, custid);
                        }
                    }
                }
                else if(stripeEvent.Type == "charge.failed")
                {
                    var Amount = ((double)data["data"].First().First["amount"]) / 100;
                    var failure_message = data["data"].First().First["failure_message"].ToString();
                    // msg = AccountData.UpdatePaymentStatus(custid, Amount, Amount, 0, 0.00, startperiod, endperiod, 0);
                    if (Amount > 1)//Card verification charge
                    {
                        upData.UpdatePlanData(custid, 0);
                        StripeHelper.UnSubscribe(custid);
                    }
                    var last4 = "";
                    var carddetails = StripeHelper.GetCard(custid);
                    var customerdata = StripeHelper.GetStripeCustomer(custid);
                    var email = customerdata.Email;
                    foreach (var card in carddetails)
                    {
                        last4 = card.Last4;
                    }
                    emailboday = @"A payment $" + Amount + " failed from your card ending with <b> " + last4 + " </b>" + "Please check your card details or add new card.</b>Possible reason:"+failure_message;
                    var respo = EmailAlert.PaymentAlertMail(email, emailboday);
                    if (respo == "success" && Amount > 1)
                    {
                        sellerData.UpdatePaymentAlert(2, custid);
                    }
                }
              
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                LogFile.WriteLog(msg);
            }
            return HttpStatusCode.OK;
        }
    }
}