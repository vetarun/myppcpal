using Mpp.BUSINESS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace Mpp.WEB.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception.InnerException == null)
                LogFile.WriteLog(actionExecutedContext.Exception.Message, "");
            else if (actionExecutedContext.Exception.InnerException.InnerException != null)
                LogFile.WriteLog(actionExecutedContext.Exception.Message + " | " + actionExecutedContext.Exception.InnerException.InnerException.Message, "");
            else
                LogFile.WriteLog(actionExecutedContext.Exception.Message + " | " + actionExecutedContext.Exception.InnerException.Message, "");
        }
    }
}