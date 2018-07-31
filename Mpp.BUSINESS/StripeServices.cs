using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mpp.UTILITIES;
using Mpp.BUSINESS.DataLibrary;
using Mpp.UTILITIES.BO;
using System.Data;
using Stripe;
using Mpp.BUSINESS.DataModel;

namespace Mpp.BUSINESS
{
    public class StripeServices
    {
        public static void RenewPlan()
        {
            int UserId = 0;
            try
            {
                var subs = ServicesData.GetSubscribers();
                int plan;
                if (subs.Rows.Count > 0)
                {
                    foreach (DataRow row in subs.Rows)
                    {
                        String msg="";
                        String CustomerId = row["stp_custId"].ToString();
                        String CardId = row["stp_cardId"].ToString();
                        UserId = Convert.ToInt32(row["MppUserID"]);

                        if (!String.IsNullOrWhiteSpace(CustomerId) && !String.IsNullOrWhiteSpace(CardId))
                        {
                            var profile = APIData.GetUserProfileForSkuData(UserId);

                            if (profile.Rows.Count > 0)
                            {
                                DataRow dr = profile.Rows[0];
                                String ProfileId = Convert.ToString(dr["ProfileId"]);
                                DateTime LastUpdatedOn = Convert.ToDateTime(dr["AccessTokenUpdatedOn"]);

                                var Auth = new AuthorizationModel()
                                {
                                    access_token = Convert.ToString(dr["AccessToken"]),
                                    refresh_token = Convert.ToString(dr["RefreshToken"]),
                                    token_type = Convert.ToString(dr["TokenType"]),
                                    expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                                };
                                var reportApi = new ReportsAPI();
                                var skuCount = AccountData.UserSkuCount(UserId);
                                if (String.IsNullOrWhiteSpace(msg))
                                {

                                    if (skuCount > 250)
                                        plan = GetCustomPlan(skuCount);
                                    else
                                        plan = GetPlanBySku(skuCount);
                                    var subsPlan = new StripeSubscription();
                                    var subService = StripeHelper.GetSubscription(CustomerId);
                                    if (subService != null)
                                        subsPlan = StripeHelper.UpdateSubscription(CustomerId, subService.Id, new Stripe.StripeSubscriptionUpdateOptions { PlanId = plan.ToString(), Prorate = false }, out msg);
                                    else
                                        subsPlan = StripeHelper.Subscription(CustomerId, new Stripe.StripeSubscriptionCreateOptions { PlanId = plan.ToString() }, out msg);
                                    if (String.IsNullOrWhiteSpace(msg))
                                    {
                                        UpdateUserInfo userInfo = new UpdateUserInfo
                                        {
                                            Amount = ((double)(subsPlan.StripePlan.Amount) / 100).ToString(),
                                            CardId = CardId,
                                            CustId = CustomerId,
                                            UserId = UserId,
                                            PlanId = plan,
                                            EndedAt = subsPlan.CurrentPeriodEnd.ToString(),
                                            PlanStartDate = subsPlan.CurrentPeriodStart.ToString(),
                                            PlanEndDate = subsPlan.CurrentPeriodEnd.ToString(),
                                            TrialEndDate = row["TrailEndDate"].ToString()
                                        };
                                        ServicesData.UpdateStripeData(userInfo);
                                    }else
                                        LogAPI.WriteLog("RenewPlan - " + msg, UserId.ToString());

                                }else
                                    LogAPI.WriteLog("RenewPlan - " + msg, UserId.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogAPI.WriteLog("RenewPlan - " + ex.Message, UserId.ToString());
            }
        }

        public static string SubscribePlan(String CustomerId, String CardId)
        {
            int plan;
            String msg = "";
            try
            {
                Int32 UserId = SessionData.UserID;
                DateTime Trialdate = SessionData.TrialEndOn;

                if (!String.IsNullOrWhiteSpace(CardId))
                {
                    var profile = APIData.GetUserProfileForSkuData(UserId);
                    if (profile.Rows.Count > 0)
                    {
                        DataRow dr = profile.Rows[0];
                        String ProfileId = Convert.ToString(dr["ProfileId"]);
                        DateTime LastUpdatedOn = Convert.ToDateTime(dr["AccessTokenUpdatedOn"]);

                        var Auth = new AuthorizationModel()
                        {
                            access_token = Convert.ToString(dr["AccessToken"]),
                            refresh_token = Convert.ToString(dr["RefreshToken"]),
                            token_type = Convert.ToString(dr["TokenType"]),
                            expires_in = Convert.ToInt32(dr["TokenExpiresIn"])
                        };
                        var reportApi = new ReportsAPI();
                        var skuCount = AccountData.UserSkuCount(UserId);

                        if (String.IsNullOrWhiteSpace(msg))
                        {
                           
                            var subsPlan = new StripeSubscription();
                            if (skuCount > 250)
                                plan = GetCustomPlan(skuCount);
                            else
                                plan = GetPlanBySku(skuCount);
                            var subService = StripeHelper.GetSubscription(CustomerId);
                            if (subService != null)
                                subsPlan = StripeHelper.UpdateSubscription(CustomerId, subService.Id, new Stripe.StripeSubscriptionUpdateOptions { PlanId = plan.ToString(), Prorate = false }, out msg);
                            else
                                subsPlan = StripeHelper.Subscription(CustomerId, new Stripe.StripeSubscriptionCreateOptions { PlanId = plan.ToString() }, out msg);

                            if (String.IsNullOrWhiteSpace(msg))
                            {
                                UpdateUserInfo userInfo = new UpdateUserInfo
                                {
                                    Amount = ((double)(subsPlan.StripePlan.Amount) / 100).ToString(),
                                    CardId = CardId,
                                    CustId = CustomerId,
                                    UserId = UserId,
                                    PlanId = plan,
                                    EndedAt = subsPlan.CurrentPeriodEnd.ToString(),
                                    PlanStartDate = subsPlan.CurrentPeriodStart.ToString(),
                                    PlanEndDate = subsPlan.CurrentPeriodEnd.ToString(),
                                    TrialEndDate = Trialdate.ToString()
                                };
                                msg = AccountData.UpdateStripeData(userInfo);
                                if (msg == "")
                                    SessionData.PlanID = plan;
                            }else
                            {
                                LogAPI.WriteLog("SubscribePlan - " + msg, SessionData.SellerName);
                            }
                        }
                    }
                    else
                    {
                        msg = Constant.PROFILE_NOTFOUND;
                    }
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog("SubscribePlan - " + msg, SessionData.SellerName);
            }
            return msg;
        }

        public static void UpdateBillingPeriod(String CustId)
        {
            String couponid = null;
            double discount = 0;
            var invoices = StripeHelper.GetInvoices(CustId);
            if (invoices != null)
            {
                foreach (var invoice in invoices)
                {
                    var startperiod = invoice.StripeInvoiceLineItems.Data.First().StripePeriod.Start.Value.ToString();
                    var endperiod = invoice.StripeInvoiceLineItems.Data.First().StripePeriod.End.Value.ToString();
                    var stripe_discount = invoice.StripeDiscount;
                    if (stripe_discount != null)
                    {
                        couponid = stripe_discount.StripeCoupon.Id;
                    }
                    var amount_off = (double)invoice.AmountDue / 100;
                    var total = (double)invoice.Total / 100; //after discount
                    var amount = ((double)invoice.StripeInvoiceLineItems.Data.First().Amount) / 100;
                    discount = Math.Round(amount - total, 2, MidpointRounding.AwayFromZero);
                    var paid = invoice.Paid;
                    ServicesData.UpdatePaymentStatus(CustId, amount, amount_off, couponid, discount, startperiod, endperiod, paid);
                }
            }
        }

        public static string ApplyPromoCode(String custm_id, String code)
        {
            String msg = "";
            try
            {
                var customerUpdateOptions = new StripeCustomerUpdateOptions
                {
                    Coupon = code
                };

                //updated customer
                var customerService = new StripeCustomerService();
                StripeCustomer customer = customerService.Get(custm_id);
                var Discount = customer.StripeDiscount;
                if (Discount != null)
                {
                    var coupon_id = Discount.StripeCoupon.Id;
                    if (!String.IsNullOrWhiteSpace(coupon_id) && coupon_id.ToLower() == code.ToLower())
                    {
                        msg = "Code already applied.";
                    }
                    else
                    {
                        customerService.Update(custm_id, customerUpdateOptions);
                        msg = AccountData.UpdateUserAffiliateCode(custm_id, code);
                    }
                }
                else
                {
                    customerService.Update(custm_id, customerUpdateOptions);
                    msg = AccountData.UpdateUserAffiliateCode(custm_id, code);
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                LogFile.WriteLog(msg, SessionData.SellerName);
            }
            return msg;
        }

        public static int GetPlanBySku(int skuCount)
        {
            int plan = (int)Statics.StripePlans.NO_PLAN;

            if (skuCount > 0 && skuCount <= 10)
                plan = (int)Statics.StripePlans.SOLOPRENEUR;
            else if (skuCount > 10 && skuCount <= 25)
                plan = (int)Statics.StripePlans.STARTUP;
            else if (skuCount > 25 && skuCount <= 50)
                plan = (int)Statics.StripePlans.BUSINESS_TIME;
            else if (skuCount > 50 && skuCount <= 100)
                plan = (int)Statics.StripePlans.BIG_BUSINESS;
            else if (skuCount > 100 && skuCount <= 250)
                plan = (int)Statics.StripePlans.ENTERPRISE;

            return plan;
        }

        private static int GetCustomPlan(int skuCount)
        {
            int Totalcost = 249;
            int planid = 0;
            double Extra_Count = skuCount / 250;
            int Extra_Cost = (int)Math.Floor(Extra_Count) * 50;
            Totalcost += Extra_Cost;

            int OldPlanId = AccountData.GetPlanId(Totalcost);

            if (OldPlanId > 0)
            {
                var plandetials = StripeHelper.GetPlan(OldPlanId.ToString());
                if (plandetials != null)
                    planid = OldPlanId;
            }

            if (planid < 1)
            {
                var plans = StripeHelper.GetTopPlan();
                var Plan = plans[0];
                planid = Convert.ToInt32(Plan.Id) + 1;
                var planOptions = new StripePlanCreateOptions()
                {
                    Id = planid.ToString(),
                    Name = "Custom",
                    Amount = Totalcost * 100,
                    Currency = "usd",
                    Interval = "month",
                };
                var res = StripeHelper.CreatePlan(planOptions);
                if (res != null)
                    AccountData.CreateCustomPlan(planid, Totalcost);
                else
                    planid = 0;
            }
            return planid;
        }
    }
}
