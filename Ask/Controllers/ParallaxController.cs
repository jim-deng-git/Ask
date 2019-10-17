using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;

namespace WorkV3.Controllers
{
    public class ParallaxController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            var datas = ParallaxDAO.Get(model.No, true);
            if (datas.Count == 0)
                return null;

            int index = new Random().Next(datas.Count);
            var data = datas[index];

            WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(model.No);
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            ViewBag.ImgSrc = JsonConvert.DeserializeObject<ImageModel>(data.Img).Img;

            return View(data);
        }
    }
}