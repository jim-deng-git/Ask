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
    public class ArticleController : Controller
    {
        string IdentityType = Areas.Backend.Models.DataAccess.CategoryType.Identity.ToString();

        [HttpGet]
        public ActionResult Index(CardsModels model, string key, string type, long? typeId, int? index)
        {
            ViewBag.SiteID = model.SiteID;

            long menuId = CardsDAO.GetMenuID(model.No);
            ViewBag.Menu = MenusDAO.GetInfo(model.SiteID, menuId);

            ViewBag.SitePage = CardsDAO.GetPage((long)MenusDAO.GetFirstCardNo(menuId, "Article"));
            SitePage page = CardsDAO.GetPage(model.No);

            ArticleSettingModels setting = ArticleSettingDAO.GetItem(menuId);

            #region 文章列表是否限制會員觀看
            if (setting.ReadMode == 1 && !string.IsNullOrWhiteSpace(setting.ReadModeSet))
            {
                Member curUser = Member.Current;
                string IdentityName = ""; //限制身分的名稱
                IEnumerable<Areas.Backend.Models.CategoryModels> listReadModeCategorie = Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType, setting.ReadModeSet);
                foreach (var readModeList in listReadModeCategorie)
                {
                    IdentityName += readModeList.Title + "、";
                }
                IdentityName = IdentityName.TrimEnd('、');
                ViewBag.IdentityName = IdentityName;

                if (curUser == null)
                {
                    ViewBag.CheckReadMode = false;
                    ViewBag.IsLogin = false;
                }
                else
                {
                    bool checkReadMode = ArticleDAO.ListCheckReadMode(curUser.ID, IdentityType, listReadModeCategorie);
                    if (!checkReadMode)
                        ViewBag.CheckReadMode = false;
                }
            }
            #endregion 

            if (setting.PagingMode == "不分頁")
                setting.PageSize = int.MaxValue;

            IEnumerable<ArticleTypesModels> types = ArticleTypesDAO.GetItems(menuId);
            if (setting.Types != "all")
            {
                IEnumerable<long> allowTypeIds = setting.GetTypes();
                types = types.Where(t => allowTypeIds.Contains(t.ID));
                setting.Types = string.Join(",", types.Select(t => t.ID));
            }
            ViewBag.Types = types;

            int pageIndex = index ?? 1;

            int totalRecord;
            IEnumerable<ArticleModels> items = ArticleDAO.GetItems(setting, key, type, typeId, pageIndex, out totalRecord);

            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));

            ViewBag.UploadUrl = UpdFileInfo.GetVPathByMenuID(model.SiteID, menuId).TrimEnd('/') + "/";
            ViewBag.Pagination = new Pagination { PageSize = setting.PageSize, PageIndex = pageIndex, TotalRecord = totalRecord };
            ViewBag.Setting = setting;
            ViewBag.Type = type;

            int style = model.StylesID == 0 ? 1 : model.StylesID;
            // style = 6;
            return View("ListStyle" + style, items);
        }

        [HttpGet]
        public ActionResult Next(long siteId, long menuId, int style, int pageIndex, string key, string type, long? typeId)
        {
            ArticleSettingModels setting = ArticleSettingDAO.GetItem(menuId);

            int totalRecord;
            IEnumerable<ArticleModels> items = ArticleDAO.GetItems(setting, key, type, typeId, pageIndex, out totalRecord);
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));

            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            ViewBag.Setting = setting;
            return View("NextStyle" + style, items);
        }

        public ActionResult Content(CardsModels model)
        {
            long menuId = CardsDAO.GetMenuID(model.No);
            ArticleSettingModels setting = ArticleSettingDAO.GetItem(menuId);
            ViewBag.Setting = setting;

            ArticleModels item = ArticleDAO.GetItemByCard(model.No);
            item.ReplyCommentSetting = setting;
            IEnumerable<ArticleTypesModels> types = item.GetTypes();
            ViewBag.Types = types;
            IEnumerable<ArticleSeriesModels> series = item.GetSeries();
            ViewBag.Series = series;
            IEnumerable<ArticleCategoryModels> readModeCategories = item.GetCategories(IdentityType);
            ViewBag.ReadModeCategories = readModeCategories;

            string uploadUrl = UpdFileInfo.GetVPathByMenuID(model.SiteID, menuId).TrimEnd('/') + "/";
            RecommendHandler(item.ID, setting, uploadUrl, types);
            RecommendHandler2(item.ID, setting, uploadUrl, types);

            //string key = "AllArticleID_" + menuId;
            //long[] allArticleIds = HttpRuntime.Cache[key] as long[];
            //if(allArticleIds == null) {
            //    allArticleIds = ArticleDAO.GetAllIDs(menuId).ToArray();
            //    HttpRuntime.Cache.Add(key, allArticleIds, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 30, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            //}
            long[] allArticleIds = ArticleDAO.GetAllIDs(setting).ToArray();
            int index = Array.IndexOf(allArticleIds, item.ID);
            ViewBag.ListSitePageIndex = (index / setting.PageSize) + 1;            

            if (index <= 0) {
                ViewBag.Prev = null;
            } else {                
                ArticleModels prev = ArticleDAO.GetItem(allArticleIds[index - 1]);
                ViewBag.Prev = prev;
                ViewBag.PrevSitePage = CardsDAO.GetPage(prev.CardNo);
            }

            ViewBag.ListSitePage = MenusDAO.GetListPage(menuId);

            if(index == allArticleIds.Length - 1) {
                ViewBag.Next = null;
            } else {                
                ArticleModels next = ArticleDAO.GetItem(allArticleIds[index + 1]);
                ViewBag.Next = next;
                ViewBag.NextSitePage = CardsDAO.GetPage(next.CardNo);
            }
            SitePage page = CardsDAO.GetPage(model.No);
            long pageID = page.PageNo;
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.SiteID = model.SiteID;
            ViewBag.SiteSN = page.SiteSN;
            ViewBag.MenuID = menuId;
            ViewBag.PageID = pageID;
            ViewBag.CollectionResult = MemberShipDAO.CheckMemberCollectionExist(pageID);
            ViewBag.CollectionCount = MemberShipDAO.GetPageCollectionCount(pageID);
            ViewBag.Style = model.StylesID == 0 ? 1 : model.StylesID;

            ArticleDAO.AddItemClicks(item.ID);

            int style = model.StylesID == 0 ? 1 : model.StylesID;
            string ViewFileName = string.Format("~/Views/Article/ContentStyle{0}.cshtml", style);
            if (!System.IO.File.Exists(Server.MapPath(ViewFileName)))
            {
                style = 1;
            }
            ViewFileName = string.Format("~/Views/Article/ContentStyle{0}.cshtml", style);
            return View(ViewFileName, item);
        }

        private void RecommendHandler(long id, ArticleSettingModels setting, string uploadUrl, IEnumerable<ArticleTypesModels> types)
        {
            if (setting.ExtendReadOpen)
            {
                IEnumerable<ArticleModels> recommends = ArticleDAO.GetRecommendItems(setting, id, types.Select(t => t.ID));
                Dictionary<long, SitePage> recommendSitePages = CardsDAO.GetPages(recommends.Select(r => r.CardNo));

                Dictionary<long, dynamic> imgPathInfo = new Dictionary<long, dynamic>();
                imgPathInfo.Add(setting.MenuID, new { Setting = setting, UploadUrl = uploadUrl });

                foreach (ArticleModels item in recommends)
                {
                    SitePage page = recommendSitePages[item.CardNo];

                    dynamic pathInfo;
                    if (!imgPathInfo.TryGetValue(page.MenuID, out pathInfo))
                    {
                        ArticleSettingModels newSetting = ArticleSettingDAO.GetItem(page.MenuID);
                        string newUploadUrl = UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID).TrimEnd('/') + "/";
                        pathInfo = new { Setting = newSetting, UploadUrl = newUploadUrl };
                        imgPathInfo.Add(page.MenuID, pathInfo);
                    }

                    string img = item.GetFirstImg(pathInfo.Setting);
                    item.Icon = string.IsNullOrWhiteSpace(img) ? string.Empty : pathInfo.UploadUrl + img;
                }

                ViewBag.Recommends = recommends;
                ViewBag.RecommendSitePages = recommendSitePages;
            }
        }
        private void RecommendHandler2(long id, ArticleSettingModels setting, string uploadUrl, IEnumerable<ArticleTypesModels> types)
        {
            if (setting.ExtendReadOpen2)
            {
                IEnumerable<ArticleModels> recommends = ArticleDAO.GetRecommendItems2(setting, id, types.Select(t => t.ID));
                Dictionary<long, SitePage> recommendSitePages = CardsDAO.GetPages(recommends.Select(r => r.CardNo));

                Dictionary<long, dynamic> imgPathInfo = new Dictionary<long, dynamic>();
                imgPathInfo.Add(setting.MenuID, new { Setting = setting, UploadUrl = uploadUrl });

                foreach (ArticleModels item in recommends)
                {
                    SitePage page = recommendSitePages[item.CardNo];

                    dynamic pathInfo;
                    if (!imgPathInfo.TryGetValue(page.MenuID, out pathInfo))
                    {
                        ArticleSettingModels newSetting = ArticleSettingDAO.GetItem(page.MenuID);
                        string newUploadUrl = UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID).TrimEnd('/') + "/";
                        pathInfo = new { Setting = newSetting, UploadUrl = newUploadUrl };
                        imgPathInfo.Add(page.MenuID, pathInfo);
                    }

                    string img = item.GetFirstImg(pathInfo.Setting);
                    item.Icon = string.IsNullOrWhiteSpace(img) ? string.Empty : pathInfo.UploadUrl + img;
                }

                ViewBag.Recommends2 = recommends;
                ViewBag.RecommendSitePages2 = recommendSitePages;
            }
        }


        [HttpGet]
        public CollectionResult CheckMemberCollection(int PageID)
        {
            return MemberShipDAO.CheckMemberCollectionExist(PageID);
        }

    }
}