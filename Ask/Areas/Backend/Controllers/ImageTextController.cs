using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Golbal;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ImageTextController : Controller
    {
        public ActionResult Edit(long SiteID, long MenuID, long CardNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            ImageTextModel model = new ImageTextModel();
            model.CardNo = CardNo;

            var temp = ImageTextDAO.Get(CardNo).FirstOrDefault();
            if (temp != null)
                model = temp;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(long SiteID, long MenuID, ImageTextModel model, HttpPostedFileBase ImgFile, string ImgFileBase64, string ImgFileBase64_Resize)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (!ModelState.IsValid)
            {
                ViewBag.IsValid = false;
                return View(model);
            }

            #region 圖片

            if (!string.IsNullOrWhiteSpace(model.Img))
            {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.Img);
                if (imgModel.ID == 0)
                {
                    if (ImgFile == null || ImgFile.ContentLength == 0)
                        model.Img = string.Empty;
                    else
                    {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(ImgFile, SiteID, MenuID, ImgFileBase64, ImgFileBase64_Resize);
                        imgModel.ID = GetItem.NewSN();
                        imgModel.Img = fileName;
                        model.Img = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            #endregion

            #region 儲存資料

            if (model.Description == null)
                model.Description = "";

            DateTime now = DateTime.Now;
            if (model.ID == 0)
            {
                model.ID = GetItem.NewSN();
                model.Creator = MemberDAO.SysCurrent.Id;
                model.CreateTime = now;
            }

            model.Modifier = MemberDAO.SysCurrent.Id;
            model.ModifyTime = now;

            int result = ImageTextDAO.Instance.Update(model);
            if (result == 0)
                ViewBag.EditResult = "儲存發生錯誤";
            else
                ViewBag.EditResult = "儲存成功";

            return View(model);

            #endregion
        }
    }
}