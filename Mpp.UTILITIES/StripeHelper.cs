using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;
using System.Collections;

namespace Mpp.UTILITIES
{
    public class StripeHelper
    {
        public static StripeCustomer AddCustomer(StripeCustomerCreateOptions strpCust, string StripeCustId)
        {
            //create customer on stripe   
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = null;
            try
            {
                if (string.IsNullOrWhiteSpace(StripeCustId))
                {
                    stripeCustomer = customerService.Create(strpCust); //customer added
                }
                return stripeCustomer;
            }
            catch (Exception ex)
            {
                var exp = ex.Message.ToString();
                return null;
            }
        }

        public static StripeCustomer UpdateCustomer(StripeCustomerUpdateOptions strpCust, string StripeCustId)
        {
            //create customer on stripe   
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(StripeCustId))
                {
                    stripeCustomer = customerService.Update(StripeCustId, strpCust); //customer added
                }
                return stripeCustomer;
            }
            catch (Exception ex)
            {
                var exp = ex.Message.ToString();
                return null;
            }
        }

        public static List<StripePlan> GetTopPlan()
        {
            var invoiceSevice = new StripeInvoiceService();
            try
            {
                var planService = new StripePlanService();
                StripeList<StripePlan> planItems = planService.List(new StripeListOptions()
                {
                    Limit = 1,
                    
                }
                );
                return planItems.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static StripePlan CreatePlan(StripePlanCreateOptions planOptions)
        {
            var invoiceSevice = new StripeInvoiceService();
            try
            {
                var planService = new StripePlanService();
                StripePlan plan = planService.Create(planOptions);
                return plan;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static StripeCharge MakePayment(StripeChargeCreateOptions chargeOptions)
        {
            try
            {
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                return charge;
            }
            catch (Exception ex)
            {
                var exp = ex.Message.ToString();
                return null;
            }
        }

        public static StripeRefund RefundCharge(string chargeId)
        {
            try
            {
                var refundService = new StripeRefundService();
                StripeRefund refund = refundService.Create(chargeId);
                return refund;
            }
            catch (Exception ex)
            {
                var exp = ex.Message.ToString();
                return null;
            }
        }

        public static StripeSubscription Subscription(string custId, StripeSubscriptionCreateOptions options, out string msg)
        {
            msg = "";
            try
            {
                var subscriptionServrive = new StripeSubscriptionService();
                StripeSubscription subscription = subscriptionServrive.Create(custId, options);
                return subscription;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return null;
            }
        }

        public static StripeSubscription UpdateSubscription(string custId, string subId,StripeSubscriptionUpdateOptions options, out string msg)
        {
            msg = "";
            try
            {
                var subscriptionServrive = new StripeSubscriptionService();
                var subscription = subscriptionServrive.Update(subId, options);
                return subscription;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
                return null;
            }
        }

        public static StripeCustomer GetStripeCustomer(string custId)
        {
            var customerService = new StripeCustomerService();
            try
            {
                StripeCustomer stripeCustomer = customerService.Get(custId); //customer added
                return stripeCustomer;
            }
            catch
            {
                return null;
            }
        }

        public static StripeSubscription GetSubscription(String custId)
        {
            var customerService = new StripeCustomerService();
            try
            {
                StripeCustomer stripeCustomer = customerService.Get(custId); //customer added
                return stripeCustomer.Subscriptions.Data[0];
            }
            catch
            {
                return null;
            }
        }

        public static bool DeleteCard(string cardId, string customerID)
        {
            var cardService = new StripeCardService();
            try
            {
                cardService.Delete(customerID, cardId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<StripeCard> GetCard(string customerId)
        {
            var cardService = new StripeCardService();
            try
            {
                if (customerId != null)
                {
                    var response = cardService.List(customerId).ToList();
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public static StripeCustomer UpdateCustomer(string customerId, StripeCustomerUpdateOptions obj)
        {
            var customerService = new StripeCustomerService();
            try
            {
                StripeCustomer stripeCustomer = customerService.Update(customerId, obj);
                return stripeCustomer;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static StripeSubscription UpdatePlan(string subsId, StripeSubscriptionUpdateOptions obj)
        {
            var subscriptionService = new StripeSubscriptionService();
            try
            {
                StripeSubscription stripeSubscription = subscriptionService.Update(subsId, obj);
                return stripeSubscription;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<StripeSubscription> ExpiredTrialSubs()
        {
            var subscriptionService = new StripeSubscriptionService();
            var subs = new List<StripeSubscription>();
            try
            {
                var allSubs = subscriptionService.List().ToList();
                subs = subscriptionService.List().Where(s => s.CurrentPeriodEnd == DateTime.Now).ToList();  
                //subs = subs.Where(s => s.CurrentPeriodEnd <= DateTime.Now.AddDays(-1)).ToList();
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
            }
            return subs;
        }

        public static StripeCard GetCardDetails(String custmId, String cardId)
        {
            var cardService = new StripeCardService();
            try
            {
                return cardService.Get(custmId, cardId);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<StripeCharge> GetCharges(string StripeCustId, string StripeCardId)
        {
            var chargeService = new StripeChargeService();
            var response = chargeService.List().Data.Where(c => c.CustomerId == StripeCustId && c.Source.Card.Id == StripeCardId).ToList();
            return response;
        }

        public static bool UnSubscribe(string CustId)
        {

            var subsService = new StripeSubscriptionService();

            StripeList<StripeSubscription> activeSub = subsService.List(new StripeSubscriptionListOptions()
            {
                CustomerId = CustId,
                Limit = 10
            });
            try
            {
                if (activeSub.Count() > 0)
                {
                    foreach (var sub in activeSub)
                    {
                        StripeSubscription subs = subsService.Cancel(sub.Id);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static List<StripeInvoice> GetInvoices(string custId)
        {
            var invoiceSevice = new StripeInvoiceService();
            try
            {
                StripeList<StripeInvoice> invoiceItems = invoiceSevice.List(new StripeInvoiceListOptions()
                {
                    CustomerId = custId,
                    Limit = 50
                });
                return invoiceItems.ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static StripeInvoice GetInvoice(string Id)
        {
            var invoiceSevice = new StripeInvoiceService();
            try
            {
                return invoiceSevice.Get(Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static StripeEvent GetStripeEvent(string eventId)
        {       
            var eventService = new StripeEventService();
            try
            {
                return eventService.Get(eventId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void CreateCoupon(StripeCouponCreateOptions couponOptions)
        {
            var couponService = new StripeCouponService();
            couponService.Create(couponOptions);
        }

        public static StripeCoupon GetCoupon(string Id)
        {
            var couponService = new StripeCouponService();
            try
            {
                return couponService.Get(Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool DeleteCoupon(String couponId)
        {
            var couponService = new StripeCouponService();
            try
            {
                StripeDeleted coupon = couponService.Delete(couponId);
                return coupon.Deleted;
            }
            catch
            {
                return false;
            }
        }

        public static StripePlan GetPlan(string PlanId)
        {
            var plan = new StripePlan();
            try
            {
                var planService = new StripePlanService();
                plan = planService.Get(PlanId);
                return plan;
            }
            catch
            {
                return null;
            }
        }
    }
}
