using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Controllers
{
    public class CustomArticleController : Controller
    {
        public ActionResult List(long siteId, long menuId)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            IEnumerable<CustomArticleModel> items = CustomArticleDAO.GetItems(menuId);

            IEnumerable<long> articleIds = items.Select(item => item.ArticleID);
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(articleIds);    
            ViewBag.ItemMenus = CustomArticleDAO.GetArticleMenus(articleIds);

            return View(items);
        }

        [HttpPost]
        public void Sort(long siteId, long menuId, IEnumerable<SortItem> items) {
            CustomArticleDAO.Sort(menuId, items);
        }

        [HttpPost]
        public void Del(long siteId, long menuId, IEnumerable<long> ids) {
            CustomArticleDAO.Remove(menuId, ids);
        }

        public ActionResult Edit(long siteId, long menuId, int? index, string key) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            //if (Request.HttpMethod == "GET")
            //{
            //    if (index == null)
            //        Utility.ClearSearchValue();
            //    else
            //    {
            //        string prevKey = Utility.GetSearchValue<string>();
            //        if (prevKey != null)
            //            key = prevKey;
            //    }
            //}
            //else if (Request.HttpMethod == "POST")
            //{
            //    Utility.SetSearchValue(key);
            //}
            ViewBag.Key = key;

            Pagination pagination = new Pagination {
                PageIndex = index ?? 1,
                PageSize = 5
            };

            int totalRecord;
            IEnumerable<CustomArticleListItemModel> items = CustomArticleDAO.GetItems(menuId, key, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            IEnumerable<long> articleIds = items.Select(item => item.ArticleID);
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(articleIds);
            ViewBag.ItemMenus = CustomArticleDAO.GetArticleMenus(articleIds);

            ViewBag.Pagination = pagination;
            return View(items);
        }

        [HttpPost]
        public void AddTopArticles(long siteId, long menuId, long[] articleIds) {
            CustomArticleDAO.Add(menuId, articleIds);
        }
    }
}