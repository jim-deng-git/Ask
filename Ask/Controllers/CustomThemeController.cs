using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Common;

namespace WorkV3.Controllers
{
    public class CustomThemeController : Controller
    {
        // GET: CustomTheme
        public ActionResult Index(CardsModels model)
        {
            return View("Style"+ model.StylesID.ToString());
        }
    }
}