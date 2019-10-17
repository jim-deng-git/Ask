using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ArticleIntroController : Controller
    {
        [HttpGet]
        public ActionResult Edit(long siteId, long menuId)
        {
            ArticleIntroModels item = ArticleIntroDAO.GetItem(menuId);
            if(item == null) {
                item = new ArticleIntroModels {
                    ID = WorkLib.GetItem.NewSN(),
                    MenuID = menuId, 
                    IsIssue = true
                };
            }
            
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.DateFmt = WebInfo.DateFmt; // View 中，WebInfo 會出現命名衝突            
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(long siteId, long menuId, ArticleIntroModels item, IEnumerable<ParagraphModels> paragraphs, string deletedParagraphs)
        {
            if(!string.IsNullOrWhiteSpace(item.Icon)) {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.Icon);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fIcon"];
                    string fIconBase64 = Request.Form["fIconBase64"];
                    string fIconBase64_Resize = Request.Form["fIconBase64_Resize"];
                    if(postedFile == null || postedFile.ContentLength == 0) {
                        item.Icon = string.Empty;
                    } else {                        
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, fIconBase64, fIconBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.Icon = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            item.MenuID = menuId;
            item.Creator = MemberDAO.SysCurrent.Id;
            item.CreateTime = DateTime.Now;
            item.Modifier = item.Creator;
            item.ModifyTime = item.CreateTime;

            ArticleIntroDAO.SetItem(item);

            if (paragraphs?.Count() > 0) {
                for (int i = 0, len = paragraphs.Count(); i < len; ++i) {
                    ParagraphModels p = paragraphs.ElementAt(i);
                    p.Sort = (byte)i;
                    p.SourceNo = item.ID;
                    ParagraphDAO.SetItem(p);
                }
            }

            if (!string.IsNullOrWhiteSpace(deletedParagraphs)) {
                IEnumerable<long> delParagraphIds = deletedParagraphs.Split(',').Select(p => long.Parse(p));
                ParagraphDAO.Delete(delParagraphIds);
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.Success = true;
            ViewBag.DateFmt = WebInfo.DateFmt; // View 中，WebInfo 會出現命名衝突                                               
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            return View(item);
        }        
    }
}