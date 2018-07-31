using Mpp.UTILITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mpp.WEB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "UserAccount", action = "Login", id = UrlParameter.Optional }
            );
        }
    }

    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionTimeOutFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSessionStateBase session = context.HttpContext.Session;

            // If the browser session or authentication session has expired...
            if (session.IsNewSession || HttpContext.Current.Session["UserID"] == null)
            {

                // For round-trip requests,
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "Controller", "SessionTimeOut" },
                { "Action", "SessionTimeOut" }
                });
            }
            base.OnActionExecuting(context);
        }
    }

    /*Admin Session Out */
    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSessionStateBase session = context.HttpContext.Session;

            // If the browser session or authentication session has expired...
            if (session.IsNewSession || HttpContext.Current.Session["AdminUserID"] == null)
            {

                // For round-trip requests,
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "Controller", "SessionTimeOut" },
                { "Action", "TimeOut" },
                {"Area",""}
                });
            }
            base.OnActionExecuting(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizePlanAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSessionStateBase session = context.HttpContext.Session;
            // If the browser session or authentication session has expired...
            if (HttpContext.Current.Session["UserID"] != null)
            {
                if (Convert.ToInt32(HttpContext.Current.Session["PlanID"]) == 1 && Convert.ToDateTime(HttpContext.Current.Session["TrialEndOn"]) < DateTime.Now) //Handles no card and whoever plan is still trail even after ended
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                        { "Controller", "Main" },
                        { "Action", "Dashboard" }
                        });
                }
                else if (Convert.ToInt32(HttpContext.Current.Session["PlanStatus"]) == 0 )
                {
                    // For round-trip requests,
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "Controller", "Settings" },
                    { "Action", "Plan" }
                    });
                }
            }
            base.OnActionExecuting(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeSellerAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSessionStateBase session = context.HttpContext.Session;
            // If the browser session or authentication session has expired...
            if (HttpContext.Current.Session["UserID"] != null)
            {
                if (Convert.ToInt32(HttpContext.Current.Session["IsAgreementAccept"]) == 0)
                {
                    // For round-trip requests,
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "Controller", "UserAccount" },
                    { "Action", "Agreement" }
                    });
                }else if (Convert.ToInt32(HttpContext.Current.Session["ProfileAccess"]) == 0)
                {
                    // For round-trip requests,
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "Controller", "Settings" },
                    { "Action", "AmazonAccount" }
                    });
                }
            }
            base.OnActionExecuting(context);
        }
    }

    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TrailExpiryWithoutCardAndFormulaAttribute : ActionFilterAttribute //Not needed for now
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpSessionStateBase session = context.HttpContext.Session;
            // If the browser session or authentication session has expired...
            if (HttpContext.Current.Session["UserID"] != null)
            {
                if (Convert.ToInt32(HttpContext.Current.Session["PlanID"]) == 1 && 
                    Convert.ToDateTime(HttpContext.Current.Session["TrialEndOn"]) < DateTime.Now &&
                    String.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["StripeCardId"]))){
                }else if (Convert.ToInt32(HttpContext.Current.Session["FormulaAccess"]) == 0)
                {
                    // For round-trip requests,
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "Controller", "MyCampaigns" },
                    { "Action", "ViewMyCampaigns" }
                    });
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class NoCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }

}
