using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
namespace WorkV3.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages

        public ActionResult Page(long SiteID,long PageNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.PageNO = PageNo;

            //取得該Page底下的Zone集合
            List<ZonesModels> Zones = ZonesDAO.GetPageData(SiteID, PageNo);

            //廣告
            AdvertisementRenderDAO ad = new AdvertisementRenderDAO(SiteID, PageNo);
            Zones = ad.InsertAdsZones(Zones);

            return PartialView("_Pages", Zones);
        }

        public ActionResult Zone(ZonesModels Zone)
        {
            List<CardsModels> Cards;
            ViewBag.ZoneNo = Zone.No;

            //取得該Zone底下的Card集合
            if (Zone.AreaSetID == null)
            {
                Cards = CardsDAO.GetZoneData(Zone.SiteID, Zone.No);
                if (Cards != null && Zone.CardsModels != null) 
                {
                    //Zone.CardsModels 有資料時代表有右側廣告
                    List<CardsModels> RightSideAd = Zone.CardsModels.OrderBy(m => m.TempSort).ToList();
                    foreach (CardsModels card in Cards)
                    {
                        RightSideAd.Insert(0, card);
                    }
                    Cards = RightSideAd;
                }
            }
            else
                Cards = AdvertisementRenderTools.GenCard(Zone); //AreaSetID不為Null代表為廣告

            
            var articleCars = Cards.Where(card => card.CardsType == "Article");
            if (articleCars != null && articleCars.Count() > 0)
            {
                WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(articleCars.First().No);
                var article = ArticleDAO.GetItemByCard(articleCars.First().No);
                if (article != null)
                {
                   var imageTextList = ImageTextDAO.Get(article.ID, true, null);
                    if (imageTextList != null && imageTextList.Count() > 0)
                        Zone.StyleID = 9;
                }
            }
            return PartialView("Zones/_Style" + Zone.StyleID, Cards);
        }
    }
}