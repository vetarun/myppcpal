using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Controllers
{
    [Authorize]
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Instructions()
        {
            return View();
        }

        [NoCache]
        public ActionResult Videos()
        {
            return View();
        }

        [NoCache]
        public ActionResult Questions()
        {
            return View();
        }
    }
}