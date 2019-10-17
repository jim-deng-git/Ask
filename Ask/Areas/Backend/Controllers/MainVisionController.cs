using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Golbal;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.Controllers
{
    public class MainVisionController : Controller
    {
        public ActionResult List(long SiteID, long MenuID, long CardNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            ViewBag.CardNo = CardNo;
            var model = MainVisionDAO.Get(CardNo);
            return View(model);
        }

        public ActionResult Edit(long SiteID, long MenuID, long CardNo, long? ID = null)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            MainVisionModel model = new MainVisionModel();        
            model.CardNo = CardNo;

            if (ID.HasValue)
            {
                model.ID = ID.Value;

                var query = new Query();
                query.Where.Add(new QWhere("ID", COperator.Equal, model.ID));

                model = MainVisionDAO.Instance.Get(query).FirstOrDefault();
            }
                
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(long SiteID, long MenuID, MainVisionModel model, 
            HttpPostedFileBase ImgFile, HttpPostedFileBase ImgMFile, HttpPostedFileBase fVideoImg, HttpPostedFileBase fVideoImgM, HttpPostedFileBase fIcon, HttpPostedFileBase fHoverIcon,
            string ImgFileBase64, string ImgMFileBase64, string fVideoImgBase64, string fVideoImgMBase64 ,string fIconBase64, string fHoverIconBase64,
            string ImgFileBase64_Resize, string ImgMFileBase64_Resize, string fVideoImgBase64_Resize, string fVideoImgMBase64_Resize, string fIconBase64_Resize, string fHoverIconBase64_Resize)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            if (!ModelState.IsValid)
            {
                ViewBag.IsValid = false;
                return View(model);
            }

            #region 電腦版圖片

            if (!string.IsNullOrWhiteSpace(model.Img)) {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.Img);
                if (imgModel.ID == 0) {
                    if (ImgFile == null || ImgFile.ContentLength == 0)
                        model.Img = string.Empty;
                    else {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(ImgFile, SiteID, MenuID, ImgFileBase64, ImgFileBase64_Resize);
                        imgModel.ID = GetItem.NewSN();
                        imgModel.Img = fileName;
                        model.Img = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            #endregion

            #region 手機版圖片

            if (!string.IsNullOrWhiteSpace(model.ImgM)) {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.ImgM);
                if (imgModel.ID == 0) {
                    if (ImgMFile == null || ImgMFile.ContentLength == 0)
                        model.ImgM = string.Empty;
                    else {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(ImgMFile, SiteID, MenuID, ImgMFileBase64, ImgMFileBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        model.ImgM = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            #endregion

            #region 影片截圖

            if (model.VideoImgIsCustom) {
                if (!string.IsNullOrWhiteSpace(model.VideoImg)) {
                    if (model.VideoImg[0] == '{') {
                        ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.VideoImg);
                        if (imgModel.ID == 0) {
                            if (fVideoImg == null || fVideoImg.ContentLength == 0)
                                model.VideoImg = string.Empty;
                            else {
                                string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fVideoImg, SiteID, MenuID, fVideoImgBase64, fVideoImgBase64_Resize);
                                imgModel.ID = 1;
                                imgModel.Img = fileName;
                                model.VideoImg = JsonConvert.SerializeObject(imgModel);
                            }
                        }
                    } else {
                        model.VideoImg = string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.VideoImgM)) {
                    if (model.VideoImgM[0] == '{') {
                        ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.VideoImgM);
                        if (imgModel.ID == 0) {
                            if (fVideoImgM == null || fVideoImgM.ContentLength == 0)
                                model.VideoImgM = string.Empty;
                            else {
                                string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fVideoImgM, SiteID, MenuID, fVideoImgMBase64, fVideoImgMBase64_Resize);
                                imgModel.ID = 1;
                                imgModel.Img = fileName;
                                model.VideoImgM = JsonConvert.SerializeObject(imgModel);
                            }
                        } 
                    } else {
                        model.VideoImgM = string.Empty;
                    }
                }
            }

            #endregion

            #region Icon

            if (!string.IsNullOrWhiteSpace(model.Icon)) {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.Icon);
                if (imgModel.ID == 0) {
                    if (fIcon == null || fIcon.ContentLength == 0)
                        model.Icon = string.Empty;
                    else {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fIcon, SiteID, MenuID, fIconBase64, fIconBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        model.Icon = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            #endregion

            #region HoverIcon

            if (!string.IsNullOrWhiteSpace(model.HoverIcon))
            {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.HoverIcon);
                if (imgModel.ID == 0)
                {
                    if (fHoverIcon == null || fHoverIcon.ContentLength == 0)
                        model.HoverIcon = string.Empty;
                    else
                    {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fHoverIcon, SiteID, MenuID, fHoverIconBase64, fHoverIconBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        model.HoverIcon = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            #endregion
            if (!ModelState.IsValid) {
                ViewBag.IsValid = false;
                return View(model);
            }

            #region 儲存資料

            DateTime now = DateTime.Now;
            if (model.ID == 0) {
                model.ID = GetItem.NewSN();
                model.Creator = MemberDAO.SysCurrent.Id;
                model.CreateTime = now;
            }

            model.Modifier = MemberDAO.SysCurrent.Id;
            model.ModifyTime = now;

            int result = MainVisionDAO.Instance.Update(model);
            if (result == 0)
                ViewBag.EditResult = "儲存發生錯誤";
            else
                ViewBag.EditResult = "儲存成功";

            return View(model);

            #endregion
        }

        [HttpPost]
        public void Sort(long CardNo, IEnumerable<SortItem> items)
        {
            MainVisionDAO.Sort(CardNo, items);
        }

        [HttpPost]
        public void Delete(IEnumerable<long> IDs)
        {
            MainVisionDAO.Delete(IDs);
        }

        [HttpGet]
        public void ToggleIssue(long ID)
        {
            MainVisionDAO.ToggleIssue(ID);
        }
    }
}