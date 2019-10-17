using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Controllers
{
    public class BreadCrumbsController : Controller
    {
        // GET: BreadCrumbs
        public ActionResult Index()
        {
            //ViewBag.SiteID = PageCache.SiteID;
            //ViewBag.MenuID = PageCache.MenuID;
            //ViewBag.PageNo = PageCache.PageNO;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            ViewBag.SiteID = pageCache.SiteID;
            List<BreadCrumbsModels> BreadCrumbs = MenusDAO.GetBreadCrumbs(pageCache.SiteID, pageCache.MenuID, pageCache.PageNO);
            return View(BreadCrumbs);
        }
    }
}