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
    public class SiteSeoSettingController : Controller
    {
        [HttpGet]
        public ActionResult Index(long SiteID)
        {
            AppendViewBagParameters(SiteID);
            ViewBag.IsExit = false;
            SiteSeoSettingModels siteSeoSettingModel = WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(SiteID);
            if (siteSeoSettingModel == null)
            {
                siteSeoSettingModel = new SiteSeoSettingModels();
                siteSeoSettingModel.SiteID = SiteID;
                siteSeoSettingModel.Robot = true;
                siteSeoSettingModel.Title = "";
                siteSeoSettingModel.Description = "";
                siteSeoSettingModel.Author = "";
                siteSeoSettingModel.Copyright = "";
                siteSeoSettingModel.Keywords = "";
                siteSeoSettingModel.GA = "";
                siteSeoSettingModel.GTM = "";
                siteSeoSettingModel.Baidu = "";
                siteSeoSettingModel.Alexa = "";
                siteSeoSettingModel.GoogleSearch = "";
                siteSeoSettingModel.BaiduMA = "";
                siteSeoSettingModel.Bing = "";
                siteSeoSettingModel.IsExtraCode = false;
                siteSeoSettingModel.ExtraCode = "";
                siteSeoSettingModel.Creator =WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                siteSeoSettingModel.CreateTime = DateTime.Now;
                siteSeoSettingModel.Modifier = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                siteSeoSettingModel.ModifyTime = DateTime.Now;
                WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.SetItem(siteSeoSettingModel);
            }
            return View(siteSeoSettingModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(SiteSeoSettingModels siteSeoSettingModel)
        {
            AppendViewBagParameters(siteSeoSettingModel.SiteID);
            if (WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.SetItem(siteSeoSettingModel))
            {
                ViewBag.IsExit = true;
            }
            return View(siteSeoSettingModel);
        }
        private void AppendViewBagParameters(long SiteID)
        {
            ViewBag.SiteID = SiteID;
            string sitemapName = WorkV3.Golbal.PubFunc.GetSitemapFileName(SiteID);

            ViewBag.SitemapUrl = WorkV3.Golbal.PubFunc.GetSitemapUrl(SiteID);


            string sitePath = WorkLib.uUrl.GetURL().TrimEnd('/');
            var SitesInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SiteID);
            string fileRoot = Server.MapPath($"~/{SitesInfo.SN}/");
            if (!string.IsNullOrEmpty(sitemapName) && System.IO.File.Exists(fileRoot + "\\" + sitemapName))
            {
                DateTime lastModifyDate = System.IO.File.GetLastWriteTime(fileRoot + "\\" + sitemapName);
                ViewBag.SitemapLastModifyTime = lastModifyDate.ToString("yyyy-MM-dd HH:mm");
            }
            ViewBag.Keywords = WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetKeywords(SiteID);
        }
        public ActionResult SEOKeyword(long siteId)
        {
            IEnumerable<string> keywords = WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetKeywords(siteId);

            return View(keywords);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChangeStatus(string SiteID, bool Robot)
        {
            Models.DataAccess.SiteSeoSettingDAO.SetStatus(long.Parse(SiteID), Robot);
            return Json("success");
        }
        
        public ActionResult ReCreateSitemap(long SiteID)
        {
            string sitePath = WorkLib.uUrl.GetURL().TrimEnd('/');
            var SitesInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SiteID);
            string fileRoot = Server.MapPath($"~/{SitesInfo.SN}/");
            if(!System.IO.File.Exists(fileRoot))
            {
                new FileInfo(fileRoot).Directory.Create();
            }
            string fileName = Golbal.PubFunc.CreateSitemap(SiteID, fileRoot);
            if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(fileRoot + "\\" + fileName))
            {
                DateTime lastModifyDate = System.IO.File.GetLastWriteTime(fileRoot + "\\" + fileName);
                return Json(new { FilePath= sitePath+"/"+fileName, LastModifyDate= lastModifyDate.ToString("yyyy-MM-dd HH:mm") }, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}