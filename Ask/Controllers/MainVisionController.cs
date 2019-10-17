using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;

namespace WorkV3.Controllers
{
    public class MainVisionController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            WorkV3.Common.SitePage curPage = WorkV3.Models.DataAccess.CardsDAO.GetPage(model.No);
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);

            ViewBag.SitePage = curPage;

            var data = MainVisionDAO.Get(model.No, true);
            if (data.Count == 0)
                return null;

            return View("Style" + model.StylesID, data);
        }

        public void Click(long id) {
            MainVisionDAO.Click(id);
        }
    }
}