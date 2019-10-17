using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Controllers
{
    public class KeywordQueriedController : Controller
    {
        private KeywordDAO keywordDao = new KeywordDAO();
        private int countPerPage = 20;

        // GET: Backend/KeywordQueried
        public ActionResult Index(int? index, long siteId = 1, FormCollection search = null)
        {

            JObject searchData = getSearchData(search);

            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                }
                else
                {
                    JObject prevSearch = Utility.GetSearchValue<JObject>();
                    if (prevSearch != null)
                        searchData = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(searchData);
            }

            ViewBag.Search = searchData;

            int pageSize = 20;
            int iIndex = index ?? 1;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = PageCache.MenuID;
            ViewBag.TotalCount = keywordDao.GetKeywordQueriedPagedItemCount(iIndex, searchData);
            ViewBag.Pagination = new Pagination { PageSize = pageSize, PageIndex = iIndex, TotalRecord = ViewBag.TotalCount };
            List<KeywordQueriedViewModel> model = keywordDao.GetKeywordQueriedPagedItems(iIndex, searchData, pageSize);

            return View(model);
        }

        public JObject getSearchData(FormCollection data)
        {
            JObject retValue = new JObject();

            retValue["search"] = "";
            retValue["leastSearch"] = "";
            retValue["maxSearch"] = "";
            retValue["searchStart"] = "";
            retValue["searchEnd"] = "";

            foreach (string key in data.Keys)
            {
                if (key.Contains("-"))
                    continue;
                if (key.Contains("SiteID"))
                    continue;
                if (key.Contains("index"))
                    continue;
                retValue[key] = data[key].ToString();
            }

            return retValue;
        }
    }
}