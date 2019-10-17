using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Models;

namespace WorkV3.Controllers
{
    public class ImageTextController : Controller
    {
        public ActionResult Index(CardsModels model)
        {
            PageCache pageCache = PageCache.GetTempDataPageCache(TempData);
            var datas = ImageTextDAO.Get(model.No, true);
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(pageCache.SiteID, pageCache.MenuID);

            int styleId = model.StylesID;            
            return View("Style" + styleId, datas);            
        }

        public void Click(long id) {
            string key = "prevClickTime";
            DateTime? prevClickTime = Session[key] as DateTime?;
            if(prevClickTime != null) {                
                if ((DateTime.Now - (DateTime)prevClickTime).TotalMilliseconds < 1500)
                    return;
            }

            Session[key] = DateTime.Now;
            ImageTextDAO.Click(id);
        }
    }
}