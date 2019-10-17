using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;
using System.Collections.Generic;
using WorkV3.Common;
using WorkV3.Models.DataAccess;

namespace WorkV3.Controllers
{
    public class MemberRecommandController : Controller
    {

        public ActionResult Index(CardsModels model)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            List<ViewModels.MemberCollectionViewModel> pageRecommandList = new List<ViewModels.MemberCollectionViewModel>();
            ViewBag.SiteID = model.SiteID;
            int style = model.StylesID == 0 ? 1 : model.StylesID;
            Member curUser = Member.Current;
            if (curUser != null)
            {
                MemberShipModels item = MemberShipDAO.GetItem(curUser.ID);
                pageRecommandList = WorkV3.Models.DataAccess.MemberShipDAO.GetMemberFavitoryPages(curUser.ID);
                foreach (ViewModels.MemberCollectionViewModel vmodel in pageRecommandList)
                {
                    vmodel.LinkUrl = Url.Action("Index", "Home", new { siteSn = pageCache.SiteSN, pageSn = vmodel.SN });
                    vmodel.ImgSrc = PagesDAO.GetContentImage(pageCache.SiteID, vmodel.MenuID, vmodel.No);
                    vmodel.ImgAlt = vmodel.Title;
                    vmodel.Summary = PagesDAO.GetContentDesc(pageCache.SiteID, vmodel.MenuID, vmodel.No);
                }
            }
            // style = 3;
            return View("Style" + style, pageRecommandList);
        }
    }
}