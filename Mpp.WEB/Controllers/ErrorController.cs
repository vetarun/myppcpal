using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mpp.WEB.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
        public ActionResult Error_503()
        {  
            return View("error_503");
        }
    
    }
}