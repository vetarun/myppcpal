using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Controllers
{
    public class SessionTimeoutController : Controller
    {
        // GET: /SessionTimeOut/
        public ActionResult SessionTimeOut()
        {
            return View();
        }

        //GET: Admin Session Out
        public ActionResult TimeOut()
        {
            return View();
        }
    }
}