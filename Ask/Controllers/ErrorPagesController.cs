using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkV3.Controllers
{
    public class ErrorPagesController : Controller
    {
        // GET: ErrorPages
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TimeOut()
        {
            return View();
        }
      public ActionResult PageNotFound()
        {
            return View();
        }
    }
}