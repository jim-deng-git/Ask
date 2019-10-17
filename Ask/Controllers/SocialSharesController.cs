using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;

namespace WorkV3.Controllers
{
    public class SocialSharesController : Controller
    {
        [HttpGet]
        public PartialViewResult Index(int CollectionCount = 0)
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            ViewBag.CollectionCount = CollectionCount;
            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
            var itemMain = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
            if (itemMain != null && itemMain.IsOpen)
            {
                IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Share);

                return PartialView("_SocialShares", itemList);
            }
            return PartialView("_SocialShares");
        }
        [HttpGet]
        public PartialViewResult Pad(int CollectionCount = 0)
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            ViewBag.CollectionCount = CollectionCount;
            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
                var itemMain = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
                if (itemMain != null && itemMain.IsOpen)
                {
                    IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Share);

                    return PartialView("_PadSocialShares", itemList);
                }
           
            return PartialView("_PadSocialShares");
        }
        [HttpGet]
        public PartialViewResult Mobile(int CollectionCount)
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);

            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
            ViewBag.CollectionCount = CollectionCount;
                var itemMain = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
                if (itemMain != null && itemMain.IsOpen)
                {
                    IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Share);

                    return PartialView("_MobileSocialShares", itemList);
                }
            
            return PartialView("_MobileSocialShares");
        }
    }
}