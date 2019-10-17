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
    public class MemberShipSettingController : Controller
    {
        string IdentityType = CategoryType.Identity.ToString();
        string FavorityType = CategoryType.Favority.ToString();
        // GET: Backend/MemberShipSetting
        public ActionResult List(long SiteID, long MenuID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            List<CategoryModels> identity_items = CategoryDAO.GetItems(IdentityType);
            if(identity_items != null && identity_items.Count() > 0)
            {
                ViewBag.ListIdentity = identity_items;
            }
            else
            {
                //20181025 nina 身分沒有資料時，新增一般會員
                CategoryModels category = new CategoryModels()
                {
                    Type = IdentityType,
                    Title = "一般會員",
                    ShowStatus = true,
                    MemberSession = (int)MemberSession.不限制,
                    PresetIdentity = (int)PresetIdentity.一般會員
                };
                CategoryDAO.SetItem(category);
                ViewBag.ListIdentity = CategoryDAO.GetItems(IdentityType);
            }

            List<CategoryModels> favority_items = CategoryDAO.GetItems(FavorityType);
            ViewBag.ListFavoity = favority_items;
            return View();
        }
        public ActionResult Edit(long SiteID, long MenuID, long? ID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
            CategoryModels m = new CategoryModels();
            m.Type = IdentityType;
            if (ID.HasValue)
            {
                m = CategoryDAO.GetItem((long)ID);
                ViewBag.IsNew = false;
            }
            else
            {
                ViewBag.IsNew = true;
            }
            ViewBag.ID = ID ?? 0;

            return View(m);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(long SiteID, long MenuID, CategoryModels model)
        {
            if (!string.IsNullOrWhiteSpace(model.Image))
            {
                WorkV3.Models.ResourceImagesModels imgModel = JsonConvert.DeserializeObject<WorkV3.Models.ResourceImagesModels>(model.Image);
                if (imgModel.ID == 0)
                { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fImage"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        model.Image = string.Empty;
                    }
                    else
                    {
                        model.Image = Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, SiteID, MenuID);
                    }
                }
                else
                {
                    model.Image = imgModel.Img;
                }
            }

            if(model.PresetIdentity == (int)PresetIdentity.一般會員) 
            {
                model.MemberSession = (int)MemberSession.不限制;
            }

            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
            CategoryDAO.SetItem(model);
            ViewBag.Exit = true;
            return View(model);
        }
        public ActionResult FavorityEdit(long SiteID, long MenuID, long? ID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
            CategoryModels m = new CategoryModels();
            m.Type = FavorityType;
            if (ID.HasValue)
            {
                m = CategoryDAO.GetItem((long)ID);
                ViewBag.IsNew = false;
            }
            else
            {
                ViewBag.IsNew = true;
            }

            ViewBag.ID = ID ?? 0;

            return View(m);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FavorityEdit(long SiteID, long MenuID, CategoryModels model, HttpPostedFileBase fIcon, string fIconBase64)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
            if (string.IsNullOrEmpty(model.Icon))
            {
                model.Icon = string.Empty;
            }
            else
            {
                ImageModel imgModel = JsonConvert.DeserializeObject<ImageModel>(model.Icon);
                if (imgModel.ID == 0)
                {
                    if (fIcon == null || fIcon.ContentLength == 0)
                        model.Icon = string.Empty;
                    else
                    {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fIcon, SiteID, MenuID, fIconBase64);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        model.Icon = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }
            CategoryDAO.SetItem(model);
            ViewBag.Exit = true;
            return View(model);
        }
        [HttpPost]
        public void MemberShipSettingDel(IEnumerable<long> ids)
        {
            CategoryDAO.DeleteCategories(ids);
        }
        [HttpPost]
        public void MemberShipSettingChangeStatus(long id)
        {
            CategoryDAO.ChangeStatus(id);
        }
        public ActionResult GetAgreeStatement()
        {
            AgreeStatementSetModels model = MemberShipSettingDAO.GetStatementItems();
            // model.StatementContent = HttpUtility.UrlEncode(model.StatementContent);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SetAgreeStatement(AgreeStatementSetModels model)
        {
            model.ModuleName = "Member";
            model.StatementContent = HttpUtility.UrlDecode(model.StatementContent);
            bool Result = MemberShipSettingDAO.SetStatementItems(model);
            bool colSetResult = MemberShipRegSetDAO.UpdateItemSet(PageCache.SiteID, "IsNeedAgreeMemberDesc", model.IsNeedAgree.ToString().ToLower());
            if(!colSetResult)
                return Json("fail");
            return Json(Result ? "success" : "fail");
        }
        public ActionResult GetMailTemplate(string TemplateName)
        {
            MailTemplateSetModels model = MemberShipSettingDAO.GetMailTemplateItems(TemplateName);
            // model.StatementContent = HttpUtility.UrlEncode(model.StatementContent);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SetMailTemplate(MailTemplateSetModels model)
        {
            model.MailContent = HttpUtility.UrlDecode(model.MailContent);
            bool Result = MemberShipSettingDAO.SetMailTemplateItems(model);
            return Json(Result ? "success" : "fail");
        }
        [HttpPost]
        public string FileUpload(long siteId, long menuId, string TemplateName, string Base64 = "")
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;
            ViewModels.MemberShipSetFile fileModel = new ViewModels.MemberShipSetFile(){
                FileInfo = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId, Base64),
                FileSize = file.ContentLength,
                FileSizeDesc = uFiles.SizeToText((long)file.ContentLength),
                ShowName = Path.GetFileNameWithoutExtension(file.FileName)
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(fileModel);
        }
        [HttpPost]
        public ActionResult DeleteFile(string TemplateName,  int fileIndex)
        {
            MailTemplateSetModels model = MemberShipSettingDAO.GetMailTemplateItems(TemplateName);
            List<WorkV3.Models.ResourceFilesModels> fileList =  Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Models.ResourceFilesModels>>(model.AttFiles);
            if (fileIndex < fileList.Count)
                fileList.RemoveAt(fileIndex);
            model.AttFiles = Newtonsoft.Json.JsonConvert.SerializeObject(fileList);
            bool Result = MemberShipSettingDAO.SetMailTemplateItems(model);
            return Json(Result ? "success" : "fail");
        }
        [HttpGet]
        public ActionResult FileUploadEditText()
        {
            return View();
        }
        public ActionResult TryToMail(long SiteID, long MenuID, string TemplateName)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.TemplateName = TemplateName;
            MailTemplateSetModels model = MemberShipSettingDAO.GetMailTemplateItems(TemplateName);
            return View(model);
        }
        public ActionResult UpdateMemberShipSet(long SiteID, string columnName, string columnValue)
        {
            MemberShipRegSetModels model = MemberShipRegSetDAO.GetItem(SiteID);
            bool Result = MemberShipRegSetDAO.UpdateItemSet(SiteID, columnName, columnValue);
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult GetMemberShipSet(long SiteID)
        {
            MemberShipRegSetModels model = MemberShipRegSetDAO.GetItem(SiteID);
            return Json(model);
        }
        public ActionResult UpdateMemberShipColumnSet(long SiteID, string ColumnName, bool IsOpen, bool IsNeedValue)
        {
            MemberShipRegSetModels model = MemberShipRegSetDAO.GetItem(SiteID);
            bool Result = MemberShipRegSetDAO.UpdateColumnItemSet(SiteID, ColumnName, IsOpen, IsNeedValue);
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult UpdateMemberShipColumnOptionSet(long SiteID, string ColumnName, string OptionValue)
        {
            MemberShipRegColumnSetModels model = MemberShipRegSetDAO.GetColumnItem(SiteID, ColumnName);
            List<MemberShipRegOptionModels> optionList = model.OtherOptionList;
            if (optionList == null)
                return Json("fail");
            foreach (MemberShipRegOptionModels optionModel in optionList)
            {
                string optionValue = ColumnName + "_" + optionModel.Value;
                if (optionValue == OptionValue)
                    optionModel.Selected = true;
                else
                    optionModel.Selected = false;
            }
            bool Result = MemberShipRegSetDAO.UpdateColumnItemOptionSet(SiteID, ColumnName, Newtonsoft.Json.JsonConvert.SerializeObject(optionList));
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult GetMemberShipColumnSetItems(long SiteID)
        {
            List<MemberShipRegColumnSetModels> model = MemberShipRegSetDAO.GetColumnItems(SiteID);
            return Json(model);
        }
        public ActionResult UpdateMemberShipColumnSort(long SiteID, string ColumnName, int newSortIndex)
        {
            List<MemberShipRegColumnSetModels> modelList = MemberShipRegSetDAO.GetColumnItems(SiteID);
            var targetModel = modelList.Single(p => p.ColumnName == ColumnName);
            modelList.Remove(targetModel);
            modelList.Insert(newSortIndex - 1, targetModel);
            for (int i = 0; i < modelList.Count; i++)
            {
                MemberShipRegSetDAO.UpdateColumnItemSort(SiteID, modelList[i].ColumnName, (i + 1));
            }
            return Json("success");
        }
        public ActionResult SentTryMail(long SiteID, long MenuID, string TemplateName, string MailToList)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.TemplateName = TemplateName;
            string message = "";
            string SitesTitle = WorkV3.Models.DataAccess.SitesDAO.GetInfo(SiteID).Title;
            MailTemplateSetModels model = MemberShipSettingDAO.GetMailTemplateItems(TemplateName);
            if (model == null)
            {
                return Json(new { result = "fail", message = "範本取得失敗" });
            }
            if (!string.IsNullOrEmpty(MailToList))
            {
                int mailCount = 0;
                string mailSubject = model.MailTitle
                    .Replace("[WebsiteName]", SitesTitle);//Joe 20190924 改抓網站名稱
                string mailContent = model.MailContent
                    .Replace("[WebsiteName]", SitesTitle)
                    .Replace("[SendDate]", DateTime.Now.ToString("yyyy/MM/dd"));
                ArrayList attFileList = new ArrayList();
                if (!string.IsNullOrEmpty(model.AttFiles))
                {
                    string UploadVPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(SiteID, MenuID);
                    List<WorkV3.Areas.Backend.Models.ResourceFilesModels> fileList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkV3.Areas.Backend.Models.ResourceFilesModels>>(model.AttFiles);
                    foreach (WorkV3.Areas.Backend.Models.ResourceFilesModels f in fileList)
                    {
                        //System.Net.Mail.Attachment attFile = new System.Net.Mail.Attachment(UploadVPath + "/" + f.FileInfo);
                        attFileList.Add(UploadVPath + f.FileInfo);
                    }
                }
                string[] mailTos = MailToList.Split(';');
                foreach (string mailTo in mailTos)
                {
                    try
                    {
                        new System.Net.Mail.MailAddress(mailTo);
                    }
                    catch
                    {
                        message += string.Format("[{0}]需為 E-Mail 格式\n", mailTo);
                        continue;
                    }
                    if (attFileList.Count > 0)
                        WorkLib.uEMail.SendMailWithFiles(mailTo, "", mailSubject, mailContent, attFileList, model.MailFromAddress, SitesTitle);
                    else
                        WorkLib.uEMail.SendEMail(mailTo, "", mailSubject, mailContent, model.MailFromAddress, SitesTitle);
                    mailCount++;
                }
                message += string.Format("共寄出{0}封測試信件", mailCount.ToString());
                //string VerifyUrl = string.Format("{0}://{1}{2}{3}/w/{4}/Verify?M={5}&K={6}", HttpContext.Current.Request.Url.Scheme,
                //    HttpContext.Current.Request.Url.DnsSafeHost,
                //    HttpContext.Current.Request.Url.Port == 80 ? "" : ":" + HttpContext.Current.Request.Url.Port.ToString(),
                //    HttpContext.Current.Request.ApplicationPath == "/" ? "" : "/" + HttpContext.Current.Request.ApplicationPath.Trim('/'),
                //    WorkLib.GetItem.appSet("DefaultSiteSN").ToString(),
                //    tableObj["ID"].ToString(), item.VerifyCode);

            }
            else
            {

            }
            return Json(new { result = "success", message = message });
        }
        [HttpGet]
        public ActionResult FileUploadSuccessEditText(int index)
        {
            ViewBag.FileIndex = index;
            return View();
        }
        public ActionResult SetEmailManagers(long SiteID)
        {
            int memberCount = 0;
            ViewBag.SiteID = SiteID;
            List<WorkV3.Areas.Backend.Models.MemberModels> managerList = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount);
            List<MemberShipRegEmailManagersModels> assignedManagerList = Models.DataAccess.MemberShipRegSetDAO.GetMemberShipRegManagers(SiteID);

            var nonSelectManagers = managerList.Where(p => !assignedManagerList.Any(m => m.ManagerID == p.Id.ToString()));
            if(nonSelectManagers!= null)
                ViewBag.Managers = nonSelectManagers.ToList();
            return View();
        }
        public ActionResult UpgradeSelectManagers(long SiteID, string selectManagers)
        {
            if (string.IsNullOrEmpty(selectManagers))
                return Json("fail");
            string[] selectManagerList = selectManagers.Split(',');

           // MemberShipRegSetDAO.DelMemberShipRegManagers(SiteID);
            foreach (string selectManagerID in selectManagerList)
            {
                MemberShipRegEmailManagersModels model = MemberShipRegSetDAO.GetMemberShipRegManagers(SiteID, long.Parse(selectManagerID));
                if (model == null)
                {
                    model = new MemberShipRegEmailManagersModels();
                    model.ID = WorkLib.GetItem.NewSN().ToString();
                    model.SiteID = SiteID;
                    model.IsManager = true;
                    model.ManagerID = selectManagerID;
                    model.Email = "";
                    model.Sort = 0;
                    model.IsSelected = true;
                }
                MemberShipRegSetDAO.SetMemberShipRegManagers(model);
            }
            return Json("success");
        }
        public ActionResult UpgradeSelectEmail(long SiteID, string Email)
        {
                MemberShipRegEmailManagersModels model = MemberShipRegSetDAO.GetMemberShipRegManagerEmail(SiteID, Email);
                if (model == null)
                {
                    model = new MemberShipRegEmailManagersModels();
                    model.ID = WorkLib.GetItem.NewSN().ToString();
                    model.SiteID = SiteID;
                    model.IsManager = false;
                    model.ManagerID = "";
                    model.Email = Email;
                    model.Sort = 0;
                    model.IsSelected = true;
                }
                MemberShipRegSetDAO.SetMemberShipRegManagers(model);
            
            return Json("success");
        }
        public ActionResult GetSelectManagers(long SiteID)
        {
            List<MemberShipRegEmailManagersModels> modelList = MemberShipRegSetDAO.GetMemberShipRegManagers(SiteID);
            return Json(modelList);
        }
        public ActionResult DeleteManagers(long ID)
        {
            bool Result = MemberShipRegSetDAO.DeleteManager(ID);
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult GetMemberShipSocialSetItems(long SiteID)
        {
            List<MemberShipRegSocialSetModels> model = MemberShipRegSetDAO.GetSocialItems(SiteID);
            return Json(model);
        }
        [HttpGet]
        public ActionResult SetSocial(long SiteID, MemberType SocialType)
        {
            //int memberCount = 0;
            ViewBag.SiteID = SiteID;
            MemberShipRegSocialSetModels model = MemberShipRegSetDAO.GetSocialItem(SiteID, SocialType);
            ViewBag.SocialTitle = model.SocialTitle;
            //List<WorkV3.Areas.Backend.Models.MemberModels> managerList = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount);
            //List<MemberShipRegEmailManagersModels> assignedManagerList = Models.DataAccess.MemberShipRegSetDAO.GetMemberShipRegManagers(SiteID);

            //var nonSelectManagers = managerList.Where(p => !assignedManagerList.Any(m => m.ManagerID == p.Id.ToString()));
            //if (nonSelectManagers != null)
            //    ViewBag.Managers = nonSelectManagers.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateSocial(long SiteID, MemberType SocialType, bool IsOpen, string SecretKey, string AppID, string Scope)
        {
            bool Result = MemberShipRegSetDAO.UpdateSocialItemSet(SiteID, SocialType, SecretKey, AppID, Scope, IsOpen);

            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult UpdateMemberShipSocialSet(long SiteID, MemberType SocialType, bool IsOpen)
        {
            MemberShipRegSocialSetModels model = MemberShipRegSetDAO.GetSocialItem(SiteID, SocialType);
            bool Result = MemberShipRegSetDAO.UpdateSocialItemSet(SiteID, SocialType, IsOpen);
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult UpdateMemberShipSocialSort(long SiteID, MemberType SocialType, int newSortIndex)
        {
            List<MemberShipRegSocialSetModels> modelList = MemberShipRegSetDAO.GetSocialItems(SiteID);
            var targetModel = modelList.Single(p => p.SocialType == SocialType);
            modelList.Remove(targetModel);
            modelList.Insert(newSortIndex - 1, targetModel);
            for (int i = 0; i < modelList.Count; i++)
            {
                MemberShipRegSetDAO.UpdateSocialItemSort(SiteID, modelList[i].SocialType, (i + 1));
            }
            return Json("success");
        }

        public ActionResult GetMemberShipPageSetItems(long SiteID)
        {
            IEnumerable<MemberShipPageSetModel> model = MemberShipPageSetDAO.GetItems(SiteID);
            return Json(model);
        }
        public ActionResult UpdateMemberShipPageSet(long SiteID, string PageSN, bool IsOpen)
        {
            bool Result = MemberShipPageSetDAO.UpdatePageSet(SiteID, PageSN, IsOpen);
            if (Result)
                return Json("success");
            else
                return Json("fail");
        }
        public ActionResult UpdateMemberShipPageSort(long SiteID, string PageSN, string Type, int newSortIndex)
        {
            IEnumerable<MemberShipPageSetModel> model = MemberShipPageSetDAO.GetItems(SiteID, Type);
            var targetModel = model.Single(p => p.PageSN == PageSN);
            var modelList = model.ToList();

            modelList.Remove(targetModel);
            modelList.Insert(newSortIndex - 1, targetModel);
            for (int i = 0; i < modelList.Count; i++)
            {
                MemberShipPageSetDAO.UpdatePageSetSort(SiteID, modelList[i].PageSN, (i + 1));
            }
            return Json("success");
        }
    }
}