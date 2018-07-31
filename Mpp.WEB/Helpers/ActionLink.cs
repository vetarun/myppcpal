using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Helpers
{
    public static class ActionLinkExtensions
    {
        //Creates actionlink with icon 
        public static HtmlString CustomActionLink(this HtmlHelper helper, string text, string target, String faclass)
        {
            return new HtmlString(String.Format("<a href='{0}'><i class='{2}'></i> {1} </a>", target, text, faclass));
        }
    }
}