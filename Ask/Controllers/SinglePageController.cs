using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Controllers
{
    public class SinglePageController : Controller
    {
        public ActionResult Index(string SiteSN, string PageSN, string type)
        {
            var mSites = SitesDAO.GetSiteInfo(SiteSN);

            if (mSites != null)
            {
                var mPages = PagesDAO.GetPageInfo(mSites.Id, PageSN);

                if (mPages != null)
                {
                    //取得該Page底下的Zone集合
                    var zones = ZonesDAO.GetPageData(mPages.SiteID, mPages.No);

                    foreach (var zone in zones)
                    {
                        var cards = CardsDAO.GetZoneData(zone.SiteID, zone.No);

                        var card = cards.Where(x => x.CardsType != "Header" && x.CardsType != "BreadCrumbs" && x.CardsType != "Footer").FirstOrDefault();

                        if (card != null)
                        {
                            ViewBag.Type = type;
                            ViewBag.Title = mPages.Title;
                            ViewBag.SiteName = mSites.Title;
                            return View(card);
                        }
                    }
                }
            }

            string DefaultSiteSN = GetItem.appSet("DefaultSiteSN").ToString();

            if (DefaultSiteSN != "")
            {
                Response.Redirect("~/w/" + DefaultSiteSN + "/index");
            }

            return RedirectToAction("EmptyPage", "Home");
        }
    }
}