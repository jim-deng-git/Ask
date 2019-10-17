using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;

using WorkV3.Golbal;
using WorkV3.Common;
using WorkV3.Models.Interface;
using WorkV3.Models.Repository;
using System.Transactions;
using WorkV3.Areas.Backend.Models;

namespace WorkV3.Areas.Backend.Controllers
{
    public class SitesController : Controller
    {
        private ISiteMailSettingRepository<SiteMailSettingModel> siteMailSettingRepository = new SiteMailSettingRepository();
        private MailTemplateSetRepository MTSRepository = new MailTemplateSetRepository();
        // GET: Backend/Sites
        public ActionResult Index()
        {
            return View();
        }
        #region Edit
        public ActionResult Edit(long? id)
        {
            AppendViewBags();
            SitesModels item = null;
            if (id != null)
            {
                item = WorkV3.Models.DataAccess.SitesDAO.GetInfo((long)id);
            }
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(SitesModels item, string[] domains, string webType)
        {
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            bool IsNeedCreateBaseBackendMenus = false,  IsNeedCreateHomePageInfo = false ;
            AppendViewBags();
            if (item.Id == 0)
            {
                item.Id = WorkLib.GetItem.NewSN();
                IsNeedCreateBaseBackendMenus = true;
                IsNeedCreateHomePageInfo = true;
            }
            var orginalSiteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(item.Id);
            if (orginalSiteInfo != null)
            {
                item.Descriptions = orginalSiteInfo.Descriptions;
                item.Author = orginalSiteInfo.Author;
                item.Copyright = orginalSiteInfo.Copyright;
                item.ICO = orginalSiteInfo.ICO;
                item.Logo = orginalSiteInfo.Logo;
                item.FooterCont = orginalSiteInfo.FooterCont;
                item.GAInfo = orginalSiteInfo.GAInfo;
                item.Logoshrink = orginalSiteInfo.Logoshrink;
                item.socialSetting = orginalSiteInfo.socialSetting;
                item.LogoMobile = orginalSiteInfo.LogoMobile;
                item.HeaderCont = orginalSiteInfo.HeaderCont;
                item.HeaderCustomized = orginalSiteInfo.HeaderCustomized;
                item.FooterCustomized = orginalSiteInfo.FooterCustomized;
                item.IsLang = orginalSiteInfo.IsLang;
                item.IsSearchEnabled = orginalSiteInfo.IsSearchEnabled;
                item.IsPathBread = orginalSiteInfo.IsPathBread;
            }
            if (webType == "Indep")
            {
                item.ParentID = (long?)null;
            }
            List<ViewModels.SiteDomain> DomainList = new List<ViewModels.SiteDomain>();
            long domainIndex = 0;
            if (domains != null)
            {
                foreach (string domain in domains)
                {
                    if (!string.IsNullOrEmpty(domain))
                    {
                        domainIndex++;
                        ViewModels.SiteDomain domainModel = new ViewModels.SiteDomain()
                        {
                            ID = domainIndex,
                            Domain = domain
                        };
                        DomainList.Add(domainModel);
                    }
                }
            }
            item.Domin = Newtonsoft.Json.JsonConvert.SerializeObject(DomainList);

            WorkV3.Models.DataAccess.SitesDAO.SetItem(item);
            if (IsNeedCreateBaseBackendMenus)
            {
                WorkV3.Models.DataAccess.SitesDAO.AddInitBackendStructure(item.Id, !item.ParentID.HasValue);
            }
            if (IsNeedCreateHomePageInfo)
            {
                WorkV3.Models.DataAccess.SitesDAO.AddInitHomePage(item.Id);
                WorkV3.Models.DataAccess.SitesDAO.AddInitSystemPages(item.Id);
            }
            ViewBag.Exit = true;

            return View(item);
        }
        #endregion
        #region Copy
        public ActionResult Copy(long SourceID)
        {
            AppendViewBags();
            ViewBag.IsCopy = true;
            SitesModels item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SourceID);
            item.Id = WorkLib.GetItem.NewSN();
            item.Title += "(複製)";
            item.SN += "Copy";
            item.Issue = false;
            ViewBag.SourceID = SourceID;
            return View("Edit", item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Copy(SitesModels item, string[] domains, string webType, long SourceID)
        {
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            AppendViewBags();
            ViewBag.IsCopy = true;
            if (item.Id == 0)
                item.Id = WorkLib.GetItem.NewSN();
            if (webType == "Indep")
            {
                item.ParentID = (long?)null;
            }
            List<ViewModels.SiteDomain> DomainList = new List<ViewModels.SiteDomain>();
            long domainIndex = 0;
            if (domains != null)
            {
                foreach (string domain in domains)
                {
                    if (!string.IsNullOrEmpty(domain))
                    {
                        domainIndex++;
                        ViewModels.SiteDomain domainModel = new ViewModels.SiteDomain()
                        {
                            ID = domainIndex,
                            Domain = domain
                        };
                        DomainList.Add(domainModel);
                    }
                }
                item.Domin = Newtonsoft.Json.JsonConvert.SerializeObject(DomainList);
            }

            using (var scope = new TransactionScope())
            {
                try
                {
                    if (!WorkV3.Models.DataAccess.SitesDAO.CopyStructure(SourceID, item.Id))
                        throw new Exception("結構複製失敗");

                    if (!WorkV3.Models.DataAccess.SitesDAO.CopyBackendStructure(SourceID, item.Id))
                        throw new Exception("後台選單結構複製失敗");

                    SitesModels source_item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SourceID);
                    string absFileSourcePath = Server.MapPath("~/Websites/" + source_item.SN);
                    string absFileTargetPath = Server.MapPath("~/Websites/" + item.SN);
                    WorkV3.Models.DataAccess.SitesDAO.SetItem(item);
                    WorkV3.Models.DataAccess.SitesDAO.CopyWebsiteFiles(absFileSourcePath, absFileTargetPath);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            ViewBag.Exit = true;
            return View("Edit", item);
        }
        #endregion
        private void AppendViewBags()
        {
            List<WorkV3.Models.SitesModels> sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            ViewBag.Sites = sites;
        }
        #region ChangeStatus, ChangeIssue, Delete, IsExist
        public ActionResult ChangeStatus(long id)
        {
            SitesModels item =WorkV3.Models.DataAccess.SitesDAO.GetInfo(id);
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChangeIssue(string id, bool Issue)
        {
            SitesModels item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(long.Parse(id));
            item.Issue = Issue;
            
            WorkV3.Models.DataAccess.SitesDAO.SetItem(item);

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(string id)
        {
            SitesModels source_item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(long.Parse(id));
            string absFileSourcePath = Server.MapPath("~/Websites/" + source_item.SN);
            bool IsSuccess = WorkV3.Models.DataAccess.SitesDAO.Delete(long.Parse(id), absFileSourcePath);
            //SitesModels item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(long.Parse(id));
            //item.IsDelete = true;            
            //WorkV3.Models.DataAccess.SitesDAO.SetItem(item);

            return Json(IsSuccess ? "success" : "fail", JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsExist(string ID, string SN)
        {
            bool IsSnExist = WorkV3.Models.DataAccess.SitesDAO.IsSNExist(ID, SN);
            return Json(IsSnExist.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region
        public ActionResult Setting(long SiteID)
        {
            AppendViewBags();
            ViewBag.SiteID = SiteID;
            ViewBag.Langs = WorkV3.Models.DataAccess.SiteLangMenuDAO.GetDatas(SiteID);
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, "Header");
            ViewBag.MailSetting = siteMailSettingRepository.GetItem(SiteID, "SiteID");
            SitesModels item = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SiteID);
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Setting(SitesModels item, string[] domains, string webType, SiteMailSettingModel mailSetting)
        {
            ViewBag.SiteID = item.Id;
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            AppendViewBags();
            ViewBag.Langs = WorkV3.Models.DataAccess.SiteLangMenuDAO.GetDatas(item.Id);
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(item.Id, "Header");
            if (webType == "Indep")
            {
                item.ParentID = (long?)null;
            }
            if (!string.IsNullOrEmpty(item.Logo))
            {
                // 新上傳的圖片
                HttpPostedFileBase postedFile = Request.Files["fLogo"];
                string postedFileBase64 = Request.Form["fLogoBase64"];
                string postedFileBase64_Resize = Request.Form["fLogoBase64_Resize"];
                if (!string.IsNullOrEmpty(postedFileBase64))
                {
                    string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                    item.Logo = saveName;
                }
                else
                {
                    ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageModel>(item.Logo);
                    if (!string.IsNullOrEmpty(imgModel.Img))
                    {
                        item.Logo = System.IO.Path.GetFileName(imgModel.Img);
                    }
                    else
                    {
                        if (postedFile == null || postedFile.ContentLength == 0)
                            item.Logo = "";
                        else
                        {
                            string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                            item.Logo = saveName;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(item.Favicon))
            {
                // 新上傳的圖片
                HttpPostedFileBase postedFile = Request.Files["fFavicon"];
                string postedFileBase64 = Request.Form["fFaviconBase64"];
                string postedFileBase64_Resize = Request.Form["fFaviconBase64_Resize"];
                if (!string.IsNullOrEmpty(postedFileBase64))
                {
                    string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                    item.Favicon = saveName;
                }
                else
                {
                    ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageModel>(item.Favicon);
                    if (!string.IsNullOrEmpty(imgModel.Img))
                    {
                        item.Favicon = System.IO.Path.GetFileName(imgModel.Img);
                    }
                    else
                    {
                        if (postedFile == null || postedFile.ContentLength == 0)
                            item.Favicon = "";
                        else
                        {
                            string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                            item.Favicon = saveName;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(item.Logoshrink))
            {
                // 新上傳的圖片
                HttpPostedFileBase postedFile = Request.Files["fLogoshrink"];
                string postedFileBase64 = Request.Form["fLogoshrinkBase64"];
                string postedFileBase64_Resize = Request.Form["fLogoshrinkBase64_Resize"];
                if (!string.IsNullOrEmpty(postedFileBase64))
                {
                    string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                    item.Logoshrink = saveName;
                }
                else
                {
                    ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageModel>(item.Logoshrink);
                    if (!string.IsNullOrEmpty(imgModel.Img))
                    {
                        item.Logoshrink = System.IO.Path.GetFileName(imgModel.Img);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imgModel.Img))
                        {
                            item.Logoshrink = System.IO.Path.GetFileName(imgModel.Img);
                        }
                        else
                        {
                            if (postedFile == null || postedFile.ContentLength == 0)
                                item.Logoshrink = "";
                            else
                            {
                                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                                item.Logoshrink = saveName;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(item.LogoMobile))
            {
                // 新上傳的圖片
                HttpPostedFileBase postedFile = Request.Files["fLogoMobile"];
                string postedFileBase64 = Request.Form["fLogoMobileBase64"];
                string postedFileBase64_Resize = Request.Form["fLogoMobileBase64_Resize"];
                if (!string.IsNullOrEmpty(postedFileBase64))
                {
                    string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                    item.LogoMobile = saveName;
                }
                else
                {
                    ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageModel>(item.LogoMobile);
                    if (!string.IsNullOrEmpty(imgModel.Img))
                    {
                        item.LogoMobile = System.IO.Path.GetFileName(imgModel.Img);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imgModel.Img))
                        {
                            item.LogoMobile = System.IO.Path.GetFileName(imgModel.Img);
                        }
                        else
                        {

                            if (postedFile == null || postedFile.ContentLength == 0)
                                item.LogoMobile = "";
                            else
                            {
                                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, item.Id, "Header", postedFileBase64, postedFileBase64_Resize);
                                item.LogoMobile = saveName;
                            }
                        }
                    }
                }
            }
            List<ViewModels.SiteDomain> DomainList = new List<ViewModels.SiteDomain>();
            long domainIndex = 0;
            if (domains != null)
            {
                foreach (string domain in domains)
                {
                    if (!string.IsNullOrEmpty(domain))
                    {
                        domainIndex++;
                        ViewModels.SiteDomain domainModel = new ViewModels.SiteDomain()
                        {
                            ID = domainIndex,
                            Domain = domain
                        };
                        DomainList.Add(domainModel);
                    }
                }
            }
            item.Domin = Newtonsoft.Json.JsonConvert.SerializeObject(DomainList);

            if (siteMailSettingRepository.GetItem(mailSetting.SiteID, "SiteID") == null)
                siteMailSettingRepository.CreateItem(mailSetting);
            else
                siteMailSettingRepository.UpdateItemExcept(mailSetting, new string[] { }, "SiteID");

            MTSRepository.SetMailFromName(item.Title);//Joe 20190923設定寄件人為後台的網站設定之網站名稱

            WorkV3.Models.DataAccess.SitesDAO.SetItem(item);
            ViewBag.Exit = true;
            return View(item);
        }
        #endregion

        public ActionResult EditLang(long SiteID, long? id)
        {
            AppendViewBags();
            ViewBag.SiteID = SiteID;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, "Header");
            WorkV3.Models.SiteLangMenuModel item = null;
            if (id != null)
            {
                item = WorkV3.Models.DataAccess.SiteLangMenuDAO.GetItem((long)id);
            }
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditLang(long SiteID, WorkV3.Models.SiteLangMenuModel item)
        {
            ViewBag.SiteID = SiteID;
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (item.ID == 0)
                item.ID = WorkLib.GetItem.NewSN();
            if (item.LinkType == LangLinkType.CnTw)
                item.Translate = true;
            else
            {
                item.Translate = false;
                if (item.SelectSiteID.HasValue)
                {
                    SitesModels siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(item.SelectSiteID.Value);
                    item.Url = string.Format("~/w/{0}", siteModel.SN);
                }
            }
            AppendViewBags();
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, "Header");
            WorkV3.Models.DataAccess.SiteLangMenuDAO.SetItem(item);
            ViewBag.Exit = true;
            return View(item);
        }
        public ActionResult GetLangList(long SiteID)
        {
            IEnumerable<SiteLangMenuModel> modelList = WorkV3.Models.DataAccess.SiteLangMenuDAO.GetDatas(SiteID);
            foreach (SiteLangMenuModel item in modelList)
            {
                item.IDStr = item.ID.ToString();
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChangeStatus(string ID, bool Status)
        {
            WorkV3.Models.SiteLangMenuModel item = WorkV3.Models.DataAccess.SiteLangMenuDAO.GetItem(long.Parse(ID));
            item.IsShow = Status;
            WorkV3.Models.DataAccess.SiteLangMenuDAO.SetItem(item);
            return Json("success");
        }
        [HttpPost]
        public void LangDelete(IEnumerable<long> ids)
        {
            WorkV3.Models.DataAccess.SiteLangMenuDAO.Delete(ids);
        }
        [HttpPost]
        public void LangSort(long siteID, IEnumerable<SortItem> items)
        {
            WorkV3.Models.DataAccess.SiteLangMenuDAO.Sort(siteID, items);
        }
    }
}