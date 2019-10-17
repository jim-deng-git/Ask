using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Common;
using System.Drawing;
using System.Drawing.Imaging;
using WorkV3.Models.Interface;
using WorkV3.Models.Service;

namespace WorkV3.Controllers
{
    public class FormSetController : Controller
    {
        private const string formDesignFileDir = "FormDesign";
        private IEmailService emailService = new EmailService();

        [HttpGet]
        public ActionResult Index(CardsModels model)
        {
            SitePage curPage = CardsDAO.GetPage(model.No);
            ViewBag.CurPage = curPage;

            FormModel item = FormDAO.GetItemFromCardNo(model.No);

            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, formDesignFileDir).TrimEnd('/') + "/";
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            ViewBag.Int64Convert = new Golbal.Int64Converter();
            return View(item);
        }

        [HttpPost]
        public ActionResult Index(CardsModels model, Captcha captcha)
        {
            SitePage curPage = CardsDAO.GetPage(model.No);
            FormModel item = FormDAO.GetItemFromCardNo(model.No);

            if (item.HasCaptcha)
            {
                if (captcha == null || !captcha.Validate())
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.NonAuthoritativeInformation, "驗證碼錯誤");
            }

            FormItem formItem = FormItemSave(item);
            SendEmail(curPage.SiteID, item, formItem);

            ViewBag.CurPage = curPage;
            ViewBag.FormItem = formItem;
            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, formDesignFileDir).TrimEnd('/') + "/";
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            ViewBag.Int64Convert = new Golbal.Int64Converter();

            ViewBag.Exit = true;

            return View(item);
        }
        [HttpGet]
        public ActionResult Edit(CardsModels model)
        {
            SitePage curPage = CardsDAO.GetPage(model.No);
            ViewBag.CurPage = curPage;

            FormModel item = FormDAO.GetItemFromCardNo(model.No);

            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, formDesignFileDir).TrimEnd('/') + "/";
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            ViewBag.Int64Convert = new Golbal.Int64Converter();
            return View(item);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(CardsModels model, Captcha captcha)
        {
            SitePage curPage = CardsDAO.GetPage(model.No);
            FormModel item = FormDAO.GetItemFromCardNo(model.No);
            if (item.HasCaptcha)
            {
                if (captcha == null || !captcha.Validate())
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.NonAuthoritativeInformation, "驗證碼錯誤");
            }

            FormItem formItem = FormItemSave(item);
            SendEmail(curPage.SiteID, item, formItem);

            ViewBag.CurPage = curPage;
            ViewBag.FormItem = formItem;
            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID(curPage.SiteID, formDesignFileDir).TrimEnd('/') + "/";
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(curPage.SiteID, curPage.MenuID);
            ViewBag.Int64Convert = new Golbal.Int64Converter();

            ViewBag.Exit = true;

            return View(item);
        }

        private FormItem FormItemSave(FormModel form)
        {
            FormItem formItem = new FormItem { FormID = form.ID, ID = WorkLib.GetItem.NewSN(), IsTemp = false };
            IEnumerable<FieldModel> fields = form.GetFields();
            foreach (FieldModel field in fields)
            {
                string val = Request.Form[field.ID.ToString()] ?? string.Empty;
                if (field.TypeID == "address")
                {
                    string regions = Request.Form["Regions_" + field.ID] ?? string.Empty;
                    int[] regionList = null;
                    if (regions != string.Empty)
                        regionList = JsonConvert.DeserializeObject<int[]>(regions);

                    string address = Request.Form["Address_" + field.ID] ?? string.Empty;
                    val = JsonConvert.SerializeObject(new FieldAddress { Regions = regionList, Address = address });
                }

                FieldValueDAO.SetItem(new FieldValue
                {
                    FormItemID = formItem.ID,
                    FieldID = field.ID,
                    Value = val
                });

                if (field.DefaultType != null)
                {
                    switch ((FieldDefaultType)(byte)field.DefaultType)
                    {
                        case FieldDefaultType.Email:
                            formItem.Email = val;
                            break;
                        case FieldDefaultType.姓名:
                            formItem.Name = val;
                            break;
                        case FieldDefaultType.性別:
                            formItem.Sex = val;
                            break;
                        case FieldDefaultType.身份證字號:
                            formItem.IDCard = val;
                            break;
                        case FieldDefaultType.手機:
                            formItem.Mobile = val;
                            break;
                        case FieldDefaultType.電話:
                            formItem.Phone = val;
                            break;
                    }
                }
            }

            FormItemDAO.SetItem(formItem);
            return formItem;
        }

        private void SendEmail(long siteId, FormModel form, FormItem formItem)
        {
            if (!form.NotifyAdmin)
                return;

            IEnumerable<FormAdmin> formAdmins = form.GetAdmins();
            if (formAdmins == null || formAdmins.Count() == 0)
                return;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("表單名稱", form.Title);
            dic.Add("姓名", formItem.Name);
            dic.Add("行動電話", formItem.Mobile);
            dic.Add("email", formItem.Email);

            SitesModels site = SitesDAO.GetInfo(siteId);
            dic.Add("網站", site.Title);
            dic.Add("日期", DateTime.Now.ToString(WebInfo.DateFmt));

            string mailContents = System.IO.File.ReadAllText(Server.MapPath("~/Data/MailContent/表單填寫通知管理員.html"));
            mailContents = System.Text.RegularExpressions.Regex.Replace(mailContents, @"\[([^\]]*)\]", match => {
                string key = match.Groups[1].Value;
                string val;
                if (dic.TryGetValue(key, out val))
                    return val;
                return null;
            });

            foreach (FormAdmin admin in formAdmins)
            {
                emailService.SendEMail(siteId,admin.Email, admin.Name, form.Title + "填寫 通知信", mailContents);
            }
                
        }

        [HttpGet]
        public int CheckValueExist(long formId, long fieldId, string value, long? formItemId)
        {
            return FieldValueDAO.IsExist(formId, fieldId, value, formItemId) ? 1 : 0;
        }

        [HttpGet]
        public int GetItemCount(long formId, long fieldId, string value, long? formItemId)
        {
            return FieldValueDAO.GetItemCount(formId, fieldId, value, formItemId);
        }

        [HttpPost]
        public string FileUpload(long siteId, long menuId, string Base64 = "")
        {//wei 20180816 加入Base64
            if (Request.Files == null || Request.Files.Count == 0)
                return null;

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

            string ext = Path.GetExtension(file.FileName).ToLower();
            string[] allowExts = { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".zip", ".rar", ".7z", ".mp3", ".mp4", ".mov" };
            if (!allowExts.Contains(ext))
                return null;

            return JsonConvert.SerializeObject(new { Name = Golbal.UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId, Base64), FileSize = file.ContentLength.ToString(), ShowName = Path.GetFileNameWithoutExtension(file.FileName) });


        }



        [HttpGet]
        public string MailRead(long id, long itemId)
        {
            FormMailDAO.WriteReadLog(id, itemId);
            return "<script language=\"javascript\">window.opener=null;window.close();</script>";
        }

        #region HtmlField、ShowField、GetAddressOption
        /// <summary>
        /// Html 表單欄位元件
        /// </summary>
        /// <param name="field"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public ActionResult HtmlField(FieldModel field, FieldAnswerModel answer)
        {
            ViewBag.Answer = answer;

            switch (field.TypeID)
            {
                #region 地址

                case "address":

                    ViewBag.Levels1 = new List<SelectListItem>();
                    ViewBag.Levels2 = new List<SelectListItem>();
                    ViewBag.Levels3 = new List<SelectListItem>();
                    ViewBag.Levels4 = new List<SelectListItem>();

                    var regionLevel1 = WorldRegionDAO.GetRegionsByLevel(1);
                    var regionLevel2 = WorldRegionDAO.GetRegionsByLevel(2);
                    var regionLevel3 = WorldRegionDAO.GetRegionsByLevel(3);
                    var regionLevel4 = WorldRegionDAO.GetRegionsByLevel(4);

                    AddressAnswerModel ans = null;

                    if (answer != null && !string.IsNullOrWhiteSpace(answer.Answer))
                    {
                        ans = JsonConvert.DeserializeObject<AddressAnswerModel>(answer.Answer);
                    }

                    var levels1 = new List<SelectListItem>();
                    var levels2 = new List<SelectListItem>();
                    var levels3 = new List<SelectListItem>();
                    var levels4 = new List<SelectListItem>();

                    var parentID = field.RangeLimit;

                    if (!field.Range)
                    {
                        foreach (var item in regionLevel1)
                        {
                            var isAns = false;
                            if (ans != null)
                            {
                                if (ans.Level1 == item.ID)
                                {
                                    isAns = true;
                                    parentID = item.ID;
                                }
                            }
                            levels1.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(), Selected = isAns });
                        }
                        ViewBag.Levels1 = levels1;
                    }

                    if ((field.Range && field.RangeLevel == 1) || (ans != null && ans.Level2 != null))
                    {
                        foreach (var item in regionLevel2.Where(x => x.ParentID == parentID))
                        {
                            var isAns = false;
                            if (ans != null)
                            {
                                if (ans.Level2 == item.ID)
                                {
                                    isAns = true;
                                    parentID = item.ID;
                                }
                            }
                            levels2.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(), Selected = isAns });
                        }
                        ViewBag.Levels2 = levels2;
                    }

                    if ((field.Range && field.RangeLevel == 2) || levels2.Count == 0 || (ans != null && ans.Level3 != null))
                    {
                        foreach (var item in regionLevel3.Where(x => x.ParentID == parentID))
                        {
                            var isAns = false;
                            if (ans != null)
                            {
                                if (ans.Level3 == item.ID)
                                {
                                    isAns = true;
                                    parentID = item.ID;
                                }
                            }
                            levels3.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(), Selected = isAns });
                        }
                        ViewBag.Levels3 = levels3;
                    }

                    if ((field.Range && field.RangeLevel == 3) || levels3.Count == 0 || (ans != null && ans.Level4 != null))
                    {

                        foreach (var item in regionLevel4.Where(x => x.ParentID == parentID))
                        {
                            var isAns = false;
                            if (ans != null)
                            {
                                if (ans.Level4 == item.ID)
                                {
                                    isAns = true;
                                    parentID = item.ID;
                                }
                            }
                            levels4.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(), Selected = isAns });
                        }
                        ViewBag.Levels4 = levels4;
                    }

                    break;

                    #endregion
            }

            return View(field);
        }

        /// <summary>
        /// 表單欄位
        /// </summary>
        /// <param name="field"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public ActionResult ShowField(FieldModel field, FieldAnswerModel answer)
        {
            ViewBag.Answer = answer;

            return View(field);
        }

        /// <summary>
        /// 地址
        /// </summary>
        /// <param name="level"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public JsonResult GetAddressOption(int level, int? parentID)
        {
            var regions = WorldRegionDAO.GetRegions(level, parentID);
            return Json(regions, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}