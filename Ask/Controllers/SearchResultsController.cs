using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Controllers
{
    public class SearchResultsController : Controller
    {
        [HttpGet]
        public ActionResult Index(CardsModels model, string key, string SearchType, long? SearchPoster, long? SearchSeries)
        {
            ViewBag.Card = model;
            ViewBag.Key = key;
            
            // neil 20180331 新增關鍵字搜尋 start
            bool isPreview = Request["isPreview"] != null && Request["isPreview"] == "true";

            if (!isPreview)
            {
                WorkV3.Areas.Backend.Models.DataAccess.KeywordDAO keywordDao = new Areas.Backend.Models.DataAccess.KeywordDAO();
                keywordDao.AddKeywordQueried(key);
            }
            // neil 20180331 新增關鍵字搜尋 end
            
            IEnumerable<SearchMenuModel> menus = SearchResultDAO.GetSearchMenus(model.SiteID);
            IEnumerable<SearchResultModel> items = null;
            // charlie_shan 20180802 新增作者搜尋-> 只搜文章模組 start
            if (!string.IsNullOrEmpty(SearchType))
            {
                if (SearchType == "Poster")
                {
                    if (SearchPoster.HasValue)
                    {
                        ViewBag.PosterModel = ArticlePosterDAO.GetItem(SearchPoster.Value);
                    }
                    items = SearchResultDAO.GetItemsByPoster(menus, key, SearchPoster);
                }
                // charlie_shan 20180822 新增系列搜尋-> 只搜文章模組 start
                else if (SearchType == "Series")
                {
                    items = SearchResultDAO.GetItemsBySeries(menus, key, SearchSeries);
                }
                // charlie_shan 20180822 新增系列搜尋-> 只搜文章模組 end
                else
                    items = SearchResultDAO.GetItems(menus, key);
            }
            else
            {
                items = SearchResultDAO.GetItems(menus, key);
            }
            // charlie_shan 20180802 新增作者搜尋-> 只搜文章模組 end

            ViewBag.SitePages = Models.DataAccess.CardsDAO.GetPages(items.Select(item => item.CardNo)) ?? new Dictionary<long, Common.SitePage>();

            IEnumerable<long> menuIds = items.Select(item => item.MenuID).Distinct();

            menus = menus.Where(m => menuIds.Contains(m.ID));
            ViewBag.Menus = menus;

            List<SearchMenuModel> singlePageMenus = menus.Where(m => m.Module.ToLower() == "articleintro").ToList();            
            List<string[]> singleRootMenus = SearchResultDAO.GetRootMenus(singlePageMenus.Where(m => m.ParentID > 0).Select(m => m.ParentID));
            Dictionary<string, List<SearchResultModel>> singlePageResults = new Dictionary<string, List<SearchResultModel>>();
            foreach(SearchResultModel item in items) {
                SearchMenuModel menu = singlePageMenus.FirstOrDefault(s => s.ID == item.MenuID);
                if (menu == null)
                    continue;

                item.Title = menu.Title;

                string[] rootMenuInfo = singleRootMenus == null ? null : singleRootMenus.FirstOrDefault(r => r[0] == menu.ParentID.ToString());

                List<SearchResultModel> results;
                if(rootMenuInfo == null) {
                    if(!singlePageResults.TryGetValue(menu.Title, out results)) {
                        results = new List<SearchResultModel>();
                        singlePageResults.Add(menu.Title, results);
                    }
                    results.Add(item);
                    continue;
                }

                string rootMenuName = rootMenuInfo[2];
                if (string.IsNullOrWhiteSpace(rootMenuName)) {
                    rootMenuName = rootMenuInfo[1];
                } else {
                    item.Title = rootMenuInfo[1] + " / " + item.Title;
                }

                if(!singlePageResults.TryGetValue(rootMenuName, out results)) {
                    results = new List<SearchResultModel>();
                    singlePageResults.Add(rootMenuName, results);
                }
                results.Add(item);                
            }
            ViewBag.SinglePageResults = singlePageResults;
            
            SearchResultDAO.GetMenuItems(model.SiteID, key);
            if (!string.IsNullOrEmpty(SearchType))
            {
                if (SearchType == "Poster")
                    ViewBag.MenuResults = new List<SearchMenuResultModel>();
                else
                    ViewBag.MenuResults = SearchResultDAO.GetMenuItems(model.SiteID, key);
            }
            else
            {
                ViewBag.MenuResults = SearchResultDAO.GetMenuItems(model.SiteID, key);
            }

            return View("Style" + model.StylesID, items);
        }
    }
}