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
    public class FooterController : Controller
    {
        // GET: Footer
        public ActionResult Index(CardsModels card)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            ViewBag.SiteID = pageCache.SiteID;
           
            SitesModels Msites = SitesDAO.GetInfo(pageCache.SiteID);
            ViewBag.SitesInfo = Msites;
            if (Msites.FooterCustomized)
                ViewBag.FooterCont = Msites.FooterCont;
            else
                ViewBag.FooterCont = "";
            ViewBag.Copyright = Msites.Copyright;

            List<MenusModels> Menus = MenusDAO.GetFrontMenus(pageCache.SiteID);


            var channel_models = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(pageCache.SiteID);
            if (channel_models != null && channel_models.IsFooterOpenChannel)
            {
                IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(pageCache.SiteID, Areas.Backend.Models.RelationType.Channel);
                ViewBag.Channels = itemList;
            }

            return View("Style" + card.StylesID, Menus);
        }

        
    }
}