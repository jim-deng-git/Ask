using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;
using WorkV3.Models;
using WorkLib;
using Newtonsoft.Json;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Areas.Backend.Controllers
{
    public class KeywordController : Controller
    {
        private KeywordDAO keywordDao = new KeywordDAO();

        // GET: Backend/Keyword
        public ActionResult Index(long siteId = 1)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = PageCache.MenuID;
            List<KeywordViewModel> model = keywordDao.GetAllKeywords();

            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public string Add(KeywordSaveViewModel model)
        {
            return keywordDao.AddKeyword(model)? "1": "0";
        }

        public ActionResult Edit(long id)
        {
            KeywordQueriedModels queriedItem = keywordDao.GetKeywordQueriedItemByKeywordId(id);
            KeywordSaveViewModel model = new KeywordSaveViewModel();
            KeywordModels keyword = keywordDao.GetKeywordItem(id);

            model.ID = id;
            model.IsIssue = keyword.IsIssue;
            model.Text = queriedItem.Text;

            return View(model);
        }

        [HttpPost]
        public string Edit(KeywordSaveViewModel model)
        {
            return keywordDao.EditKeyword(model)? "1": "0";
        }

        public void ToggleIssue(long id)
        {
            keywordDao.ToggleIssue(id);
        }

        public string GetAllKeywords()
        {
            return JsonConvert.SerializeObject(keywordDao.GetAllKeywords());
        }

        [HttpPost]
        public void DeleteKeywords(IEnumerable<long> IDs)
        {
            keywordDao.DeleteKeyword(IDs);
        }

        [HttpPost]
        public void Sort(IEnumerable<SortItem> items)
        {
            keywordDao.SortKeywords(items);
        }

    }
}