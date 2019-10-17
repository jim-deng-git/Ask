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
    public class ImageTextListController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(model.No);
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            return View(model);
        }
        public ActionResult Render(long SiteID, long ZoneNo)
        {
            List<ImageTextModel> imageTextList = new List<ImageTextModel>();
            var Zone = CardsDAO.GetZoneInfo(ZoneNo);
            var Cards = CardsDAO.GetZoneData(SiteID, ZoneNo);

            string RenderViewFileName = "~/Views/Article/ImageTextListStyleDefault.cshtml";
            if (Cards != null && Cards.Count > 0)
            {
                foreach (CardsModels Card in Cards)
                {
                    if (Card.CardsType == "Article")
                    {
                        WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(Card.No);
                        ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
                        var article = ArticleDAO.GetItemByCard(Card.No);
                        if (article != null)
                        {
                            imageTextList = ImageTextDAO.Get(article.ID, true, null);
                            string ViewFileName = string.Format("~/Views/Article/ImageTextListStyle{0}.cshtml", Card.StylesID.ToString());
                            if (System.IO.File.Exists(Server.MapPath(ViewFileName)))
                            {
                                RenderViewFileName = ViewFileName;
                            }
                        }
                    }
                }
            }
            return View(RenderViewFileName, imageTextList);
        }
    }
}