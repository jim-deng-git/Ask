using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;

namespace WorkV3.Controllers
{
    public class ChannelsController : Controller
    {
        // 20190713 neil 避免 post 之後 renderAction 找不到的問題
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public PartialViewResult Head()
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
           
                var model = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
            if (model != null && model.IsHeaderOpenChannel)
            {
                IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Channel);
                return PartialView("_HeadChannels", itemList);
            }
            return PartialView("_HeadChannels");
        }
        public PartialViewResult Footer()
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);

            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
                var model = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
                if (model != null && model.IsFooterOpenChannel)
                {
                    IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Channel);
                    return PartialView("_HeadChannels", itemList);
                }
            
            return PartialView("_HeadChannels");
        }
        public PartialViewResult EDM()
        {
            long siteID = 1;
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            if (pageCache != null)
            {
                siteID = pageCache.SiteID;
            }
            var model = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(siteID);
                if (model != null && model.IsEDMOpenChannel)
                {
                    IEnumerable<Areas.Backend.Models.SocialRelationModels> itemList = Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(siteID, Areas.Backend.Models.RelationType.Channel);
                    return PartialView("_EDMChannels", itemList);
                }
            
            return PartialView("_EDMChannels");
        }
    }
}