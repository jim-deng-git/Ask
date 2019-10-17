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
    public class ImageTextListController : Controller
    {
        public ActionResult List(long SiteID, long MenuID, long CardNo, SearchModel Search)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            ViewBag.CardNo = CardNo;
            ViewBag.Search = Search;

            var model = ImageTextDAO.Get(CardNo, Search: Search);
            return View(model);
        }

        public ActionResult Edit(long SiteID, long MenuID, long CardNo, long? ID = null)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            ImageTextModel model = new ImageTextModel();
            model.CardNo = CardNo;

            if (ID.HasValue)
            {
                model.ID = ID.Value;

                var query = new Query();
                query.Where.Add(new QWhere("ID", COperator.Equal, model.ID));

                model = ImageTextDAO.Instance.Get(query).FirstOrDefault();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(long SiteID, long MenuID, ImageTextModel model, HttpPostedFileBase ImgFile, string ImgFileBase64, string ImgFileBase64_Resize)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

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
                        string fileName = UpdFileInfo.SaveFilesByMenuID(ImgFile, SiteID, MenuID, ImgFileBase64, ImgFileBase64_Resize);
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

        [HttpPost]
        public void Sort(long CardNo, IEnumerable<SortItem> items)
        {
            ImageTextDAO.Sort(CardNo, items);
        }

        [HttpPost]
        public void Delete(IEnumerable<long> IDs)
        {
            ImageTextDAO.Delete(IDs);
        }

        [HttpGet]
        public void ToggleIssue(long ID)
        {
            ImageTextDAO.ToggleIssue(ID);
        }
    }
}