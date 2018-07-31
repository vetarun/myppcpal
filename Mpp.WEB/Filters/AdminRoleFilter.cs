using Mpp.WEB.Areas.Admin.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mpp.WEB.Filters
{
    public class AdminRoleFilter:ActionFilterAttribute
    {
        public string UserType { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var type="";
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"];
            if(filterContext.HttpContext.Session!=null && filterContext.HttpContext.Session.Contents["UserType"]!=null)
             type = filterContext.HttpContext.Session.Contents["UserType"].ToString();
            if(string.IsNullOrWhiteSpace(type))
            {
                filterContext.Result= new RedirectToRouteResult(
                                         new RouteValueDictionary(new { controller = "Account", action = "Login" })
                                                                   );

            }
           else if((!UserType.Contains(type)))
            {
                filterContext.Result = new ViewResult{ViewName= "UnAuthorised"};
            }
            base.OnActionExecuting(filterContext);
        } 
        
    }
}