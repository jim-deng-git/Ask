using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
namespace WorkV3.Controllers
{
    public class ArticleIntroController : Controller
    {
        [HttpGet]
        public ActionResult Index(CardsModels model)
        {
            // 注意：CardsModels 中的 MenuID 是無效的
            long menuId = CardsDAO.GetMenuID(model.No);

            ArticleIntroModels intro = ArticleIntroDAO.GetItem(model.No);
            ViewBag.SiteID = model.SiteID;
            ViewBag.MenuID = menuId;
            ViewBag.Style = model.StylesID == 0 ? 1 : model.StylesID;
            
            return View(intro);
        }
    }
}