using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Golbal;

namespace WorkV3.Controllers
{
    public class ArticleSetController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            ArticleSetModels setting = ArticleSetDAO.GetItem(model.No);
            ViewBag.Setting = setting;

            IEnumerable<ArticleModels> items = ArticleDAO.GetItems(setting);
            
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));

            ViewBag.SiteID = model.SiteID;
            int style = model.StylesID == 0 ? 1 : model.StylesID;
            // style = 3;
            return View("Style" + style, items);
        }

        public ActionResult Next(long siteId, long cardNo, int style, int pageIndex) {
            ArticleSetModels setting = ArticleSetDAO.GetItem(cardNo);
            ViewBag.Setting = setting;

            IEnumerable<ArticleModels> items = ArticleDAO.GetItems(setting, pageIndex);
            ViewBag.ItemPages = CardsDAO.GetPages(items.Select(item => item.CardNo));
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));
                        
            // style = 2;
            return View("NextStyle" + style, items);
        }
    }
}