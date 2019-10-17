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
    public class CustomArticleController : Controller
    {
        [HttpGet]
        public ActionResult Index(CardsModels model, int? index)
        {
            ViewBag.SiteID = model.SiteID;
            SitePage page = CardsDAO.GetPage(model.No);
            ViewBag.SitePage = page;
            ViewBag.CardNo = model.No;

            int pageIndex = index ?? 1;
            Pagination pagination = new Pagination {
                PageSize = 20, PageIndex = index ?? 1
            };

            int totalRecord;
            IEnumerable<ArticleModels> items = CustomArticleDAO.GetItems(page.MenuID, pagination.PageSize, pagination.PageIndex, out totalRecord);
            
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));
            
            int style = model.StylesID == 0 ? 1 : model.StylesID;            
            return View("ListStyle" + style, items);
        }

        [HttpGet]
        public ActionResult Next(long siteId, long menuId, int style, int pageIndex) {
            int totalRecord;
            IEnumerable<ArticleModels> items = CustomArticleDAO.GetItems(menuId, 20, pageIndex, out totalRecord);
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));
            return View("NextStyle" + style, items);
        }
    }
}