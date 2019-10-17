using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Golbal;
using WorkV3.Common;
using WorkV3.Models;
using Newtonsoft.Json;
using System.IO;
namespace WorkV3.Areas.Backend.Controllers
{
    public class SocialSettingController : Controller
    {
        string customImageFolder = "SocialImage";
        public ActionResult Index(long SiteID)
        {
            AppendViewBagParameters(SiteID);
            SocialSettingModels socialSettingModel = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(SiteID);
            if (socialSettingModel == null)
            {
                socialSettingModel = new SocialSettingModels();
                socialSettingModel.SiteID = WorkLib.GetItem.NewSN();
                socialSettingModel.SiteID = SiteID;
                socialSettingModel.IsOpen = true;
                socialSettingModel.SocialDefaultImage = "";
                socialSettingModel.IsEDMOpenChannel = false;
                socialSettingModel.IsHeaderOpenChannel = false;
                socialSettingModel.IsFooterOpenChannel = false;
                socialSettingModel.Creator =WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                socialSettingModel.CreateTime = DateTime.Now;
                socialSettingModel.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                socialSettingModel.ModifyTime = DateTime.Now;
                WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.SetItem(socialSettingModel);
            }
            return View(socialSettingModel);
        }
        private void AppendViewBagParameters(long SiteID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.ShareModels = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(SiteID, RelationType.Share);
            ViewBag.ChannelModels = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(SiteID, RelationType.Channel);
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, customImageFolder);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SetSocialSetting(string SiteID, string Column, bool IsChecked)
        {
            SocialSettingModels socialSettingModel = SocialSettingDAO.GetItem(long.Parse(SiteID));
            if (socialSettingModel == null)
            {
                socialSettingModel = new SocialSettingModels();
                socialSettingModel.SiteID = WorkLib.GetItem.NewSN();
                socialSettingModel.SiteID = long.Parse(SiteID);
                socialSettingModel.IsOpen = true;
                socialSettingModel.SocialDefaultImage = "";
                socialSettingModel.IsEDMOpenChannel = false;
                socialSettingModel.IsHeaderOpenChannel = false;
                socialSettingModel.IsFooterOpenChannel = false;
                socialSettingModel.Creator = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                socialSettingModel.CreateTime = DateTime.Now;
                socialSettingModel.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                socialSettingModel.ModifyTime = DateTime.Now;
                WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.SetItem(socialSettingModel);
            }
            if(Column == "IsOpen")
                socialSettingModel.IsOpen = IsChecked;
            if (Column == "IsHeaderOpenChannel")
                socialSettingModel.IsHeaderOpenChannel = IsChecked;
            if (Column == "IsFooterOpenChannel")
                socialSettingModel.IsFooterOpenChannel = IsChecked;
            if (Column == "IsEDMOpenChannel")
                socialSettingModel.IsEDMOpenChannel = IsChecked;
            bool Result = SocialSettingDAO.SetItem(socialSettingModel);
            return Json(Result ? "success" : "fail");
        }
        public ActionResult SetSocialSettingImage(string SiteID, HttpPostedFileBase postedFile, string FileBase64)
        {
            SocialSettingModels socialSettingModel = SocialSettingDAO.GetItem(long.Parse(SiteID));
            WorkV3.Models.ImageModel imgModel = new ImageModel();
            imgModel.ID = 0;
            // 新上傳的圖片
            string postedFileBase64 = FileBase64;
            if (postedFile == null || postedFile.ContentLength == 0)
            {
                socialSettingModel.SocialDefaultImage = "";
            }
            else
            {
                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, long.Parse(SiteID), customImageFolder,  postedFileBase64);
                imgModel.Img = saveName;
                socialSettingModel.SocialDefaultImage = JsonConvert.SerializeObject(imgModel);
            }
            bool Result = SocialSettingDAO.SetItem(socialSettingModel);
            return Json(Result ? "success" : "fail");

        }
        public ActionResult ShareEdit(long SiteID, long? DetailID)
        {
            AppendViewBagParameters(SiteID);

            if (DetailID.HasValue)
            {
                SocialRelationModels item = Models.DataAccess.SocialSettingDAO.GetRelationItem(DetailID.Value);
                return View(item);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ShareEdit(long SiteID, SocialRelationModels item)
        {
            item.SiteID = SiteID;
            item.LinkUrl = "";
            if ( item.ID == "0")
                item.ID = WorkLib.GetItem.NewSN().ToString();
            bool result = Models.DataAccess.SocialSettingDAO.SetRelationItem(item);
            if (result)
            {
                ViewBag.Exit = true;
            }
            AppendViewBagParameters(SiteID);
            return View(item);
        }
        public ActionResult GetShareList(long SiteID)
        {
            IEnumerable<SocialRelationModels> modelList = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(SiteID, RelationType.Share);

            return Json(modelList);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ShareChangeStatus(string ID, bool IsOpen)
        {
            Models.DataAccess.SocialSettingDAO.SetRelationStatus(long.Parse(ID), IsOpen);
            return Json("success");
        }
        [HttpPost]
        public void ShareDel(long siteId, IEnumerable<long> ids)
        {
            Models.DataAccess.SocialSettingDAO.DeleteRelations(ids);
        }
        [HttpPost]
        public void ShareSort(long siteId, IEnumerable<SortItem> items)
        {
            Models.DataAccess.SocialSettingDAO.SortRelations( RelationType.Share, items);
        }

        #region 官方頻道
        public ActionResult ChannelEdit(long SiteID, long? DetailID)
        {
            AppendViewBagParameters(SiteID);

            if (DetailID.HasValue)
            {
                SocialRelationModels item = Models.DataAccess.SocialSettingDAO.GetRelationItem(DetailID.Value);
                return View(item);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChannelEdit(long SiteID, SocialRelationModels item)
        {
            item.SiteID = SiteID;
            if (item.ID == "0")
                item.ID = WorkLib.GetItem.NewSN().ToString();
            bool result = Models.DataAccess.SocialSettingDAO.SetRelationItem(item);
            if (result)
            {
                ViewBag.Exit = true;
            }
            AppendViewBagParameters(SiteID);
            return View(item);
        }
        public ActionResult GetChannelList(long SiteID)
        {
            IEnumerable<SocialRelationModels> modelList = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetRelationItems(SiteID, RelationType.Channel);

            return Json(modelList);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChannelChangeStatus(string ID, bool IsOpen)
        {
            Models.DataAccess.SocialSettingDAO.SetRelationStatus(long.Parse(ID), IsOpen);
            return Json("success");
        }
        [HttpPost]
        public void ChannelDel(long siteId, IEnumerable<long> ids)
        {
            Models.DataAccess.SocialSettingDAO.DeleteRelations(ids);
        }
        [HttpPost]
        public void ChannelSort(long siteId, IEnumerable<SortItem> items)
        {
            Models.DataAccess.SocialSettingDAO.SortRelations(RelationType.Channel, items);
        }
        #endregion
    }
}