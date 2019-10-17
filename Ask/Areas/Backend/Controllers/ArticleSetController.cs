using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Models.DataAccess;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ArticleSetController : Controller
    {
        [HttpGet]
        public ActionResult Setting(long cardNo) {
            SitePage page = CardsDAO.GetPage(cardNo);
            SettingInit(cardNo, page);

            ArticleSetModels item = ArticleSetDAO.GetItem(cardNo);
            if(item == null) {
                item = new ArticleSetModels {
                    PagingMode = "點擊看更多",
                    PageSize = 10,
                    IssueSetting = "0", // 刊登期間內的當期資料
                    SortMode = "時間排序"
                };
            }
            return View(item);
        }

        [HttpPost]
        public ActionResult Setting(long cardNo, ArticleSetModels item) {
            SitePage page = CardsDAO.GetPage(cardNo);
            SettingInit(cardNo, page);

            if (item.SortMode == "隨機排序")
                item.SortField = "NewID()";
            
            if (!string.IsNullOrWhiteSpace(item.DefaultImg)) {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.DefaultImg);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fDefaultImg"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.DefaultImg = string.Empty;
                    } else {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, page.SiteID, page.MenuID);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.DefaultImg = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            ArticleSetDAO.SetItem(item);

            ViewBag.Success = true;
            return View(item);
        }

        private void SettingInit(long cardNo, SitePage page) {
            ViewBag.CardNo = cardNo;

            IEnumerable<IDValue> listCardMenus = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article");
            Dictionary<long, IEnumerable<IDValue>> listCardTypes = new Dictionary<long, IEnumerable<IDValue>>();
            foreach (IDValue menu in listCardMenus) {
                listCardTypes.Add(menu.ID, ArticleTypesDAO.GetItems(menu.ID).Select(t => new IDValue { ID = t.ID, Value = t.Name }));
            }
            ViewBag.ListCardMenus = listCardMenus;
            ViewBag.ListCardTypes = listCardTypes;
                        
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(page.SiteID, page.MenuID).TrimEnd('/');
            ViewBag.Page = page;
        }
    }
}