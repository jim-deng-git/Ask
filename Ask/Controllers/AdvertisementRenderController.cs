using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Controllers
{
    public class AdvertisementRenderController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            if (model == null)
                return Content("");

            AdsCustomizeInfo datas = AdvertisementRenderTools.GetADdata(model);
            if(datas.AdsDisplayAreaSetInfo == null)
                return Content("");

            Member curUser = Member.Current;
            long? MemberID = null;
            if (curUser != null)
            {
                MemberID = MemberShipDAO.GetItem(curUser.ID).ID;
            }

            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(model.SiteID, model.AdvertisementMenuID);
            ViewBag.ImgModelForPC = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(datas.PCPicture);
            ViewBag.ImgModelForMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(datas.MobilePicture);
            ViewBag.GroupPosition = datas.AdsDisplayAreaSetInfo.GroupPosition??string.Empty;
            ViewBag.PageNo = model.PageNo;
            ViewBag.MemberID = MemberID;
            return View("AdsRenderStyle_" + model.StylesID, datas);
        }
        [HttpPost]
        public void SetUserEvent(long AdsCustomizeID, long PageID, string Event, long? MemberID)
        {
            AdvertisementRenderTools.SetUserEventLog(AdsCustomizeID, PageID, Event, MemberID);
        }
    }
}