using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.Common;
using WorkV3.Models.Interface;
using WorkV3.Models.Service;

namespace WorkV3.Areas.Backend.Controllers
{    
    public class FormController : Controller
    {
        private IEmailService emailService = new EmailService();

        private const string formDesignFileDir = "FormDesign";

        #region Form 操作
        [HttpGet]
        public ActionResult Edit(long siteId, long menuId) {
            FormModel item = FormDAO.GetItemFromSourceID(menuId);
            if (item == null) {
                item = new FormModel { ID = WorkLib.GetItem.NewSN(), SiteID = siteId, ForceStatement = true, SourceID = menuId };
                FormDAO.SetItem(item);

                FieldDAO.SetItem(new FieldModel { ID = WorkLib.GetItem.NewSN(), ParentID = item.ID, SN = 1, DefaultType = (int)FieldDefaultType.姓名,
                    TypeID = "input", Title = "姓名", Width = (int)FieldWidth.Quarter, Requied = true, ShowInList = true });
                FieldDAO.SetItem(new FieldModel { ID = WorkLib.GetItem.NewSN(), ParentID = item.ID, SN = 2, DefaultType = (int)FieldDefaultType.性別, TypeID = "radio",
                    Width = (int)FieldWidth.Quarter, OptionArray = true, Options = "先生;女士", Requied = true });
                FieldDAO.SetItem(new FieldModel { ID = WorkLib.GetItem.NewSN(), ParentID = item.ID, SN = 3, DefaultType = (int)FieldDefaultType.Email,
                    TypeID = "input", Title = "Email", Width = (int)FieldWidth.Half, Requied = true, Fomat = true, FomatType = (int)FieldInputFormat.Email, ShowInList = true });
                FieldDAO.SetItem(new FieldModel { ID = WorkLib.GetItem.NewSN(), ParentID = item.ID, SN = 4, DefaultType = (int)FieldDefaultType.電話,
                    TypeID = "input", Title = "手機", Width = (int)FieldWidth.Half, Requied = true, Fomat = true, FomatType = (int)FieldInputFormat.台灣手機號, ShowInList = true
                });
            }
            
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID(siteId, formDesignFileDir).TrimEnd('/');
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SitePage = FormDAO.GetFormPage(item.ID);            
            ViewBag.Int64Convert = new WorkV3.Golbal.Int64Converter();
            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(long siteId, long menuId, FormModel item, string fields, string admins) {
            if (!string.IsNullOrWhiteSpace(item.Image)) {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.Image);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fImage"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Image = string.Empty;
                    } else {
                        item.Image = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, siteId, formDesignFileDir);
                    }
                } else {
                    item.Image = imgModel.Img;
                }
            }

            item.SourceID = menuId;
            item.SiteID = siteId;
            FormDAO.SetItem(item);

            FieldDesignItem[] fieldList = null;
            if (!string.IsNullOrWhiteSpace(fields))
                fieldList = JsonConvert.DeserializeObject<FieldDesignItem[]>(fields);
            item.SetFields(fieldList);            

            FormAdmin[] adminList = null;
            if (!string.IsNullOrWhiteSpace(admins))
                adminList = JsonConvert.DeserializeObject<FormAdmin[]>(admins);
            item.SetAdmins(adminList);

            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID(siteId, formDesignFileDir).TrimEnd('/');
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SitePage = FormDAO.GetFormPage(item.ID);
            ViewBag.Int64Convert = new WorkV3.Golbal.Int64Converter();
            ViewBag.Exit = true;
            return View(item);
        }

        [HttpGet]
        public ActionResult AdminSelect(long siteId, long formId) {
            ViewBag.SiteID = siteId;
            
            IEnumerable<FormAdmin> sysAdmins = FormDAO.GetSystemAdmins(siteId);            
            
            ViewBag.Int64Converter = new WorkV3.Golbal.Int64Converter();
            return View(sysAdmins);
        }

        [HttpGet]
        public ActionResult AddField(long formId) {
            ViewBag.FormID = formId;

            return View();
        }
        #endregion

        #region Field 操作
        [HttpGet]
        public ActionResult FieldSetting(long formId, string type, long? id) {
            FieldModel field = null;
            if(id != null) 
                field = FieldDAO.GetItem((long)id);

            if(field == null) {
                field = new FieldModel { ID = WorkLib.GetItem.NewSN(), TypeID = type, Width = (int)WorkV3.Common.FieldWidth.Half, High = (int)WorkV3.Common.FieldHeight.五行, RepeatLimit = (byte)FieldRepeatLimit.可任意報名 };
            }

            FormModel form = FormDAO.GetItem(formId);
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID((long)form.SiteID, formDesignFileDir).TrimEnd('/');
            ViewBag.SiteID = form.SiteID;
            ViewBag.MenuID = form.SourceID;
            ViewBag.FormID = formId;
            ViewBag.CustomFolder = formDesignFileDir;
            return View($"Field{ type }", field);
        }

        [HttpPost]
        public ActionResult FieldSetting(long formId, FieldModel item, byte? oldRepeatLimit) {
            item.ParentID = formId;

            FormModel form = FormDAO.GetItem(formId);
            if (!string.IsNullOrWhiteSpace(item.Template)) {
                ResourceFilesModels fileModel = JsonConvert.DeserializeObject<ResourceFilesModels>(item.Template);
                if (fileModel.Id == 0) { // 新上傳的檔案
                    HttpPostedFileBase postedFile = Request.Files["fTemplate"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Template = string.Empty;
                    } else {
                        item.Template = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, (long)form.SiteID, formDesignFileDir);
                    }
                } else {
                    item.Template = fileModel.FileInfo;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.Image)) {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.Image);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fImage"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Image = string.Empty;
                    } else {
                        item.Image = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, (long)form.SiteID, formDesignFileDir);
                    }
                } else {
                    item.Image = imgModel.Img;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.VideoCustomPhoto))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.VideoCustomPhoto);
                if (imgModel.ID == 0)
                { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["youtubeCustomImg"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.VideoCustomPhoto = string.Empty;
                    }
                    else
                    {
                        item.VideoCustomPhoto = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, (long)form.SiteID, formDesignFileDir);
                    }
                }
                else
                {
                    item.VideoCustomPhoto = imgModel.Img;
                }
            }

            FieldDAO.SetItem(item);
                        
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathBySiteID((long)form.SiteID, formDesignFileDir).TrimEnd('/');
            ViewBag.SiteID = form.SiteID;
            ViewBag.MenuID = form.SourceID;
            ViewBag.FormID = formId;
            ViewBag.CustomFolder = formDesignFileDir;
            ViewBag.Exit = true;
            if(item.TypeID == "input" && oldRepeatLimit == (byte)FieldRepeatLimit.可任意報名 && item.RepeatLimit != (byte)FieldRepeatLimit.可任意報名) {
                ViewBag.IsFirst = true;
            }

            return View($"Field{ item.TypeID }", item);
        }
        
        [HttpPost]
        public ActionResult FieldDesignViewAsync(string ids) {
            IEnumerable<long> idList = ids.Split(',').Select(item => long.Parse(item));
            IEnumerable<FieldModel> fields = FieldDAO.GetItems(idList);

            FormModel form = FormDAO.GetItem(fields.First().ParentID);
            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID((long)form.SiteID, formDesignFileDir).TrimEnd('/');

            return View("FieldDesignView", fields);
        }

        [HttpGet]
        public ActionResult FieldCopy(long id) {
            FieldModel field = FieldDAO.GetItem(id);
            field.ID = WorkLib.GetItem.NewSN();

            FieldDAO.SetItem(field);

            FormModel form = FormDAO.GetItem(field.ParentID);
            ViewBag.UploadDesignUrl = Golbal.UpdFileInfo.GetVPathBySiteID((long)form.SiteID, formDesignFileDir).TrimEnd('/');

            return View("FieldDesignView", new FieldModel[] { field });
        }
        #endregion
        
        [HttpGet]
        public ActionResult QuickChoose(long formId) {

            ViewBag.FormID = formId;            
            return View();
        }

        [HttpPost]
        public ActionResult QuickChoose(long formId, string[] fields) {
            ViewBag.FormID = formId;

            List<long> fieldIds = new List<long>();

            if(fields?.Count() > 0) {
                foreach(string field in fields) {
                    switch(field) {
                        case "姓名 + 性別":
                            AddNameSex(formId, fieldIds);
                            break;
                        case "名稱":
                            AddName(formId, fieldIds);
                            break; 
                        case "身份證字號":
                            AddIdentity(formId, fieldIds);
                            break;
                        case "出生年月日":
                            AddBirthday(formId, fieldIds);
                            break;
                        case "手機":
                            AddMobile(formId, fieldIds);
                            break;
                        case "電話":
                            AddPhone(formId, fieldIds);
                            break;
                        case "傳真":
                            AddFax(formId, fieldIds);
                            break;
                        case "Email":
                            AddEmail(formId, fieldIds);
                            break;
                        case "地址":
                            AddAddress(formId, fieldIds);
                            break;
                        case "婚姻狀況":
                            AddMarital(formId, fieldIds);
                            break;
                        case "學歷":
                            AddDegree(formId, fieldIds);
                            break;
                        case "職業":
                            AddProfession(formId, fieldIds);
                            break;
                        case "產業":
                            AddIndustrial(formId, fieldIds);
                            break;
                        case "公司名稱":
                            AddCompany(formId, fieldIds);
                            break;
                        case "部門":
                            AddDepartment(formId, fieldIds);
                            break;
                        case "職稱":
                            AddJobTitle(formId, fieldIds);
                            break;
                        case "喜好":
                            AddInterest(formId, fieldIds);
                            break;
                        case "專長":
                            AddSpeciality(formId, fieldIds);
                            break;
                        case "年收入":
                            AddYearlyIncome(formId, fieldIds);
                            break;
                        case "照片":
                            AddPhoto(formId, fieldIds);
                            break;
                        case "緊急聯絡人姓名":
                            AddUrgentName(formId, fieldIds);
                            break;
                        case "緊急聯絡人手機":
                            AddUrgentMobile(formId, fieldIds);
                            break;
                        case "緊急聯絡人電話":
                            AddUrgentPhone(formId, fieldIds);
                            break;
                        case "緊急聯絡人Email":
                            AddUrgentEmail(formId, fieldIds);
                            break;
                        case "留言":
                            AddMessage(formId, fieldIds);
                            break;
                        case "滿意度調查":
                            AddSatisfaction(formId, fieldIds);
                            break;
                        case "如何得知？":
                            AddSource(formId, fieldIds);
                            break;
                        case "願意收到訊息？":
                            AddReceiveMessage(formId, fieldIds);
                            break;
                        case "開立發票":
                            AddInvoice(formId, fieldIds);
                            break;
                        case "攜伴參加":
                            AddPartner(formId, fieldIds);
                            break;
                        case "用餐需求":
                            AddDinner(formId, fieldIds);
                            break;
                        case "住宿需求":
                            AddLodging(formId, fieldIds);
                            break;
                    }
                }
            }
            ViewBag.FieldIds = fieldIds.Select(id => id.ToString());

            ViewBag.Exit = true;
            return View(fields);
        }

        #region 快速增加欄位
        private void AddNameSex(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, DefaultType = (int)FieldDefaultType.姓名, TypeID = "input", Title = "姓名", Width = (int)FieldWidth.Quarter, Requied = true, ShowInList = true
            });
            fieldIds.Add(id);

            id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, DefaultType = (int)FieldDefaultType.性別, TypeID = "radio", Width = (int)FieldWidth.Quarter, OptionArray = true, Options = "先生;女士", Requied = true
            });
            fieldIds.Add(id);
        }

        private void AddName(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "名稱", Width = (int)FieldWidth.Half, Requied = true, ShowInList = true
            });
            fieldIds.Add(id);
        }

        private void AddIdentity(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "身分證字號", Width = (int)FieldWidth.Quarter, Fomat = true, FomatType = (int)FieldInputFormat.台灣身份證
            });
            fieldIds.Add(id);
        }

        private void AddBirthday(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "datetime", Title = "出生年月日", Width = (int)FieldWidth.Quarter, Fomat = true, FomatType = (int)FieldDateFormat.西元日期,
                IsDescription = true, Description = "YYYY/MM/DD"
            });
            fieldIds.Add(id);
        }

        private void AddMobile(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, DefaultType = (int)FieldDefaultType.電話, TypeID = "input", Title = "手機", Width = (int)FieldWidth.Quarter,
                Requied = true, Fomat = true, FomatType = (int)FieldInputFormat.台灣手機號, ShowInList = true
            });
            fieldIds.Add(id);
        }

        private void AddPhone(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "電話", Width = (int)FieldWidth.Quarter
            });
            fieldIds.Add(id);
        }

        private void AddFax(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "傳真", Width = (int)FieldWidth.Quarter
            });
            fieldIds.Add(id);
        }

        private void AddEmail(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, DefaultType = (int)FieldDefaultType.Email, TypeID = "input", Title = "Email", Width = (int)FieldWidth.Half,
                Requied = true, Fomat = true, FomatType = (int)FieldInputFormat.Email, ShowInList = true
            });
            fieldIds.Add(id);
        }

        private void AddAddress(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "address", Title = "地址", Width = (int)FieldWidth.Full
            });
            fieldIds.Add(id);
        }

        private void AddMarital(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "radio", Title = "婚姻狀況", Width = (int)FieldWidth.Half, OptionArray = true, Options = "未婚;已婚;離婚;喪偶"
            });
            fieldIds.Add(id);
        }

        private void AddDegree(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "select", Title = "學歷", Width = (int)FieldWidth.Half, Options = "小學;國中;高中\\高職;大學\\技術學院;碩士;博士"
            });
            fieldIds.Add(id);
        }

        private void AddProfession(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "select", Title = "職業", Width = (int)FieldWidth.Half, Options = "行政\\秘書;執行\\管理;金融服務;建築;法律;醫師;軍人\\民政服務;零售;退休;行銷\\市場;學生;教師;技士工程;旅遊\\飯店;非營利義工"
            });
            fieldIds.Add(id);
        }

        private void AddIndustrial(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "select", Title = "產業", Width = (int)FieldWidth.Half, Options = "產業;批發╱零售╱傳直銷業;文教相關業;大眾傳播相關業;旅遊╱休閒╱運動業;一般服務業;電子資訊 ╱軟體╱半導體相關業;一般製造業;農林漁牧水電資源業;運輸物流及倉儲;政治宗教及社福相關業;金融投顧及保險業;法律╱會計╱顧問╱研發╱設計業;建築營造及不動產相關業;醫療保健及環境衛生業;礦業及土石採取業;住宿╱餐飲服務業"
            });
            fieldIds.Add(id);
        }

        private void AddCompany(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "公司名稱", Width = (int)FieldWidth.Half
            });
            fieldIds.Add(id);
        }

        private void AddDepartment(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "部門", Width = (int)FieldWidth.Half
            });
            fieldIds.Add(id);
        }

        private void AddJobTitle(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "職稱", Width = (int)FieldWidth.Half
            });
            fieldIds.Add(id);
        }

        private void AddInterest(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "checkbox", Title = "喜好", Width = (int)FieldWidth.Full, OptionArray = true,
                Options =""
            });
            fieldIds.Add(id);
        }

        private void AddSpeciality(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "checkbox", Title = "專長", Width = (int)FieldWidth.Full, OptionArray = true,
                Options = ""
            });
            fieldIds.Add(id);
        }

        private void AddYearlyIncome(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "select", Title = "年收入", Width = (int)FieldWidth.Half, Options = "30萬以下;31~60萬;60萬~100萬;101萬~200萬;200萬以上"
            });
            fieldIds.Add(id);
        }

        private void AddPhoto(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "image", Title = "照片", Width = (int)FieldWidth.Half, IsDescription = true, Description = "限制上傳檔案格式 (jpg、png、gif)", Fomat = true, FomatType = (int)FieldFileFormat.圖檔
            });
            fieldIds.Add(id);
        }

        private void AddUrgentName(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "緊急聯絡人姓名", Width = (int)FieldWidth.Quarter
            });
            fieldIds.Add(id);
        }

        private void AddUrgentMobile(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "緊急聯絡人手機", Width = (int)FieldWidth.Quarter, Fomat = true, FomatType = (int)FieldInputFormat.台灣手機號
            });
            fieldIds.Add(id);
        }

        private void AddUrgentPhone(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "緊急聯絡人電話", Width = (int)FieldWidth.Quarter
            });
            fieldIds.Add(id);
        }

        private void AddUrgentEmail(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "緊急聯絡人Email", Width = (int)FieldWidth.Half, Fomat = true, FomatType = (int)FieldInputFormat.Email
            });
            fieldIds.Add(id);
        }

        private void AddMessage(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "textarea", Title = "留言", Width = (int)FieldWidth.Full, High = (int)FieldHeight.五行
            });
            fieldIds.Add(id);
        }

        private void AddSatisfaction(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "radio", Title = "滿意度調查", Width = (int)FieldWidth.Full, OptionArray = true, Options = "非常滿意;滿意;普通;不滿意;非常不滿意"
            });
            fieldIds.Add(id);
        }

        private void AddSource(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "checkbox", Title = "如何得知此訊息？", Width = (int)FieldWidth.Full, OptionArray = true, Options = "本網站;廣告;EDM;本站社群平台;親友介紹;海報;宣傳單"
            });
            fieldIds.Add(id);
        }

        private void AddReceiveMessage(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "radio", Title = "是否願意收到本站資訊？", Width = (int)FieldWidth.Half, OptionArray = true, Options = "是;否"
            });
            fieldIds.Add(id);
        }

        private void AddInvoice(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "radio", Title = "是否需要開立發票？", Width = (int)FieldWidth.Quarter, OptionArray = true, Options = "是;否"
            });
            fieldIds.Add(id);

            id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Width = (int)FieldWidth.Quarter, Fomat = true, FomatType = (int)FieldInputFormat.台灣統一編號
            });
            fieldIds.Add(id);
        }

        private void AddPartner(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "radio", Title = "是否會攜伴參加？", Width = (int)FieldWidth.Quarter, OptionArray = true, Options = "是;否"
            });
            fieldIds.Add(id);

            id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "input", Title = "如果攜伴，會帶幾人？", Width = (int)FieldWidth.Quarter, Fomat = true, FomatType = (int)FieldInputFormat.數字
            });
            fieldIds.Add(id);
        }

        private void AddDinner(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "checkbox", Title = "用餐需求", Width = (int)FieldWidth.Full, OptionArray = true, Options = "不用餐;要用餐;全素;奶蛋素;不吃牛;不吃豬;不吃羊;不吃海鮮"
            });
            fieldIds.Add(id);
        }

        private void AddLodging(long formId, List<long> fieldIds) {
            long id = WorkLib.GetItem.NewSN();
            FieldDAO.SetItem(new FieldModel {
                ID = id, ParentID = formId, TypeID = "checkbox", Title = "住宿需求", Width = (int)FieldWidth.Full, OptionArray = true, Options = "不住宿;要住宿;單人房;雙人房;三人房;四人房;團體房"
            });
            fieldIds.Add(id);
        }
        #endregion

        #region 範本操作
        [HttpGet]
        public ActionResult SaveTemplate(long id) {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public void SaveTemplate(long id, string name) {
            FormDAO.SaveTemplate(id, name);
        }

        [HttpGet]
        public ActionResult LoadTemplate(long siteId, long menuId, long formId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.FormID = formId;
            
            return View(FormDAO.GetTemplates(siteId));
        }

        [HttpPost]
        public void LoadTemplate(long siteId, long menuId, long formId, long templateId) {
            FormDAO.CreateFromTemplate(menuId, formId, templateId);
        }
        #endregion

        #region 填表名單
        public ActionResult List(long siteId, long menuId, int? index, FormItemSearch search) {
            FormModel form = FormDAO.GetItemFromSourceID(menuId);

            if (Request.HttpMethod == "GET") {
                if (index == null)
                    Utility.ClearSearchValue();
                else {
                    FormItemSearch prevSearch = Utility.GetSearchValue<FormItemSearch>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            } else if (Request.HttpMethod == "POST") {
                Utility.SetSearchValue(search);
            }
            search.FormID = form.ID;

            ViewBag.Search = search;

            Pagination pagination = new Pagination {
                PageIndex = index ?? 1,
                PageSize = 20                
            };

            int totalRecord;
            IEnumerable<FormItem> items = FormItemDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);           

            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;

            ViewBag.Form = form;
            ViewBag.Fields = FieldDAO.GetItems(search.FormID).Where(f => f.ShowInList).ToList();

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            return View(items);
        }

        public ActionResult Statistics(long siteId, long menuId, long? id, bool? isEvent) {
            if (id == null)
            {
                id = 0;
                FormModel item = FormDAO.GetItemFromSourceID(menuId);
                if (item != null) id = item.ID;
            }
            IEnumerable<FieldModel> fields = FieldDAO.GetItems((long)id);
            IEnumerable<FieldValue> fieldValues = FieldValueDAO.GetItemsByFormID((long)id);
            bool hasValues = false;
            foreach (var item in fields)
            {
                IEnumerable<FieldValue> values = fieldValues.Where(v => v.FieldID == item.ID);
                int total = values.Count();
                if (total > 0)
                {
                    hasValues = true;
                    break;
                }
            }
            ViewBag.hasValues = hasValues;
            ViewBag.Values = fieldValues;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.IsEvent = isEvent ?? false;

            return View(fields);
        }

        [HttpGet]
        public ActionResult FormItemEdit(long siteId, long menuId, long formId, long? id, bool? isEvent) {
            FormItem item = null;
            if (id != null)
                item = FormItemDAO.GetItem((long)id);

            if (item == null)
                item = new FormItem { ID = WorkLib.GetItem.NewSN(), FormID = formId, IsBack = true };

            ViewBag.IsEvent = isEvent ?? false;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');
            return View(item);
        }

        [HttpPost]
        public ActionResult FormItemEdit(long siteId, long menuId, FormItem formItem, bool? isEvent) {
            FormItemSave(formItem);

            ViewBag.IsEvent = isEvent ?? false;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');

            ViewBag.Exit = true;
            return View(formItem);
        }

        [HttpGet]
        public ActionResult FormItemView(long siteId, long menuId, long id, bool? isEvent) {
            FormItem item = FormItemDAO.GetItem(id);
            
            ViewBag.IsEvent = isEvent ?? false;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');
            return View(item);
        }

        [HttpPost]
        public ActionResult FormItemView(long siteId, long menuId, long id, byte checkStatus, string remark, bool? isEvent) {
            FormItem item = FormItemDAO.GetItem(id);
            item.CheckStatus = checkStatus;
            item.Remark = remark;
            FormItemDAO.SetItem(item);
            
            ViewBag.IsEvent = isEvent ?? false;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');

            ViewBag.Exit = true;
            return View(item);
        }
        
        private FormItem FormItemSave(FormItem formItem) {
            formItem.IsTemp = false;
            IEnumerable<FieldModel> fields = FieldDAO.GetItems(formItem.FormID);
            foreach (FieldModel field in fields) {
                string val = Request.Form[field.ID.ToString()] ?? string.Empty;
                if (field.TypeID == "address") {
                    string regions = Request.Form["Regions_" + field.ID] ?? string.Empty;
                    int[] regionList = null;
                    if (regions != string.Empty)
                        regionList = JsonConvert.DeserializeObject<int[]>(regions);

                    string address = Request.Form["Address_" + field.ID] ?? string.Empty;
                    val = JsonConvert.SerializeObject(new FieldAddress { Regions = regionList, Address = address });
                }

                FieldValueDAO.SetItem(new FieldValue {
                    FormItemID = formItem.ID, FieldID = field.ID, Value = val
                });

                if (field.DefaultType != null) {
                    switch ((FieldDefaultType)(byte)field.DefaultType) {
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

        [HttpGet]
        public ActionResult Import(long siteId, long menuId, long formId, bool? isEvent) {
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.FormID = formId;
            ViewBag.IsEvent = isEvent ?? false;
            return View();
        }

        [HttpPost]
        public ActionResult Import(long siteId, long menuId, long formId, bool? isEvent, HttpPostedFileBase fFile) {
            string uploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            if (fFile?.ContentLength > 0) {
                string filePath = Golbal.UpdFileInfo.SaveFilesByMenuID(fFile, siteId, menuId);
            }             

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.FormID = formId;
            ViewBag.IsEvent = isEvent ?? false;
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.Exit = true;
            return View();
        }

        public FileResult ImportTmpl(long siteId, long menuId, long formId, bool? isEvent) {
            ViewData["Fields"] = FieldDAO.GetItems(formId);        

            string html = Utility.GetViewHtml(this, "ImportTmpl", null);

            FormModel form = FormDAO.GetItem(formId);
            string title = form.Title;

            title = $"{ title }-匯入名單範本.xls";

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.FormID = formId;
            ViewBag.IsEvent = isEvent ?? false;
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }

        public FileResult FormItemExport(long id, bool? privacy, bool? isEvent = false) {
            FormItemSearch search = Utility.GetSearchValue<FormItemSearch>();
            if (search == null)
                search = new FormItemSearch();
            search.FormID = id;
            ViewData["FormItems"] = FormItemDAO.GetItems(search);

            FormModel form = FormDAO.GetItem(id);
            ViewData["Fields"] = form.GetFields();
            ViewData["Privacy"] = privacy ?? false;
            ViewData["IsEvent"] = isEvent ?? false;

            string html = Utility.GetViewHtml(this, "FormItemExport", null);

            string title = $"{ form.Title }-填寫.xls";

            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }

        public void FormItemDel(long siteId, long menuId, IEnumerable<long> ids) {
            FormItemDAO.Delete(ids);
        }

        [HttpGet]
        public ActionResult SendMail(long siteId, long menuId, long formId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.FormID = formId;
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            return View();
        }

        [HttpGet]
        public ActionResult TestEmail() {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public string SendMail(long siteId, long menuId, FormMailModel mail, string[] fileList, string recipientIds, string recipientEmails, bool isTest, bool? isEvent = false) {
            if (mail == null || string.IsNullOrWhiteSpace(mail.MailBody))
                return "Mail 為 NULL";

            if (isTest && string.IsNullOrWhiteSpace(recipientEmails))
                return "測試郵件缺少收信人";

            if (!isTest && string.IsNullOrWhiteSpace(recipientIds))
                return "未選擇郵件發送用戶";
            
            SitesModels site = SitesDAO.GetInfo(siteId);            

            System.Collections.ArrayList emailFiles = new System.Collections.ArrayList();
            List<ResourceFilesModels> files = new List<ResourceFilesModels>();
            if(fileList?.Length > 0) {
                string uploadPath = Golbal.UpdFileInfo.GetUPathByMenuID(siteId, menuId).TrimEnd('\\') + "\\";
                foreach(string item in fileList) {
                    ResourceFilesModels file = JsonConvert.DeserializeObject<ResourceFilesModels>(item);
                    files.Add(file);
                    emailFiles.Add(uploadPath + file.FileInfo);
                }
            }
            mail.Files = JsonConvert.SerializeObject(files);

            FormModel form = FormDAO.GetItem(mail.FormID);
            string mailSubject = mail.MailSubject.Replace("[WebsiteName]", site.Title).Replace("[SendDate]", DateTime.Now.ToString(WebInfo.DateFmt));
            string mailBody = mail.MailBody.Replace("[WebsiteName]", site.Title).Replace("[SendDate]", DateTime.Now.ToString(WebInfo.DateFmt));   
                      
            mailSubject = mailSubject.Replace("[FormName]", form.Title);
            mailBody = mailBody.Replace("[FormName]", form.Title);
            
            if (isTest) {
                SendMailTest(siteId,mail, mailSubject, mailBody, emailFiles, recipientEmails, isEvent);
                return "測試郵件發送成功";
            }

            mail.ID = WorkLib.GetItem.NewSN();
            FormMailDAO.SetItem(mail);
            MailSend(siteId,mail, mailSubject, mailBody, emailFiles, recipientIds);
            return "郵件發送成功";
        }

        private void SendMailTest(long siteId,FormMailModel mail, string mailSubject, string mailBody, System.Collections.ArrayList fileList, string recipientEmails, bool? isEvent) {
            string[] emails = recipientEmails.Split(';');
            string SitesTitle = SitesDAO.GetInfo(siteId).Title;//網站名稱
            foreach (string email in emails) {
                emailService.SendMailWithFiles(siteId,email, email, mailSubject, mailBody, fileList, mail.SenderEmail, SitesTitle);
            }
        }

        private void MailSend(long siteId, FormMailModel mail, string mailSubject, string mailBody, System.Collections.ArrayList fileList, string recipientIds) {
            string SitesTitle = SitesDAO.GetInfo(siteId).Title;//網站名稱
            IEnumerable<long> recipientIdList = recipientIds.Split(',').Select(id => long.Parse(id));
            IEnumerable<FormItem> formItems = FormItemDAO.GetItems(recipientIdList);
            string rootUrl = Utility.GetRootUrl() + "/";
            foreach(FormItem item in formItems) {
                if (string.IsNullOrWhiteSpace(item.Email))
                    continue;

                FormCheckStatus status = (FormCheckStatus)item.CheckStatus;
                string result = status == FormCheckStatus.正取 ? "報名成功" : status.ToString();
                string subject = mailSubject.Replace("[RegTime]", item.CreateDate.ToString(WebInfo.DateFmt))
                    .Replace("[RegName]", item.Name)
                    .Replace("[RegGender]", item.Sex)
                    .Replace("[RegResult]", result);

                string receiveUrl = $"{ rootUrl }Form/MailRead/{ mail.ID }?itemId={ item.ID }";
                string body = mailBody.Replace("[RegTime]", item.CreateDate.ToString(WebInfo.DateFmt))
                    .Replace("[RegName]", item.Name)
                    .Replace("[RegGender]", item.Sex)
                    .Replace("[Receive]", receiveUrl)
                    .Replace("[RegPhone]", item.Mobile)
                    .Replace("[RegTel]", item.Phone)
                    .Replace("[RegEmail]", item.Email)
                    .Replace("[RegResult]", result);

                emailService.SendMailWithFiles(siteId,item.Email, item.Name, subject, body, fileList, mail.SenderEmail, SitesTitle);
                FormMailDAO.WriteLog(mail.ID, item.ID);
            }       
        }

        [HttpGet]
        public ActionResult AddFlag(long siteId) {
            ViewBag.SiteID = siteId;
            ViewBag.Flags = UserFlagDAO.GetFlags(siteId);
            return View();
        }

        [HttpPost]
        public void AddFlag(long siteId, long[] formItemIds, string[] flags) {
            if (formItemIds == null || formItemIds.Length == 0 || flags == null || flags.Length == 0)
                return;

            IEnumerable<FormItem> formItems = FormItemDAO.GetItems(formItemIds);
            foreach(FormItem item in formItems) {
                foreach(string flag in flags) {
                    UserFlagDAO.SetItem(siteId, flag, item.Email, item.Mobile, item.Phone, item.IDCard);
                }
            }
        }

        [HttpGet]
        public ActionResult ModifyFlag(long siteId, long formItemId) {
            FormItem formItem = FormItemDAO.GetItem(formItemId);
            IEnumerable<string> flags = UserFlagDAO.GetFlags(siteId, formItem.Email, formItem.Mobile, formItem.Phone, formItem.IDCard);

            ViewBag.SiteID = siteId;
            ViewBag.FormItemID = formItemId;
            ViewBag.AllFlags = UserFlagDAO.GetFlags(siteId);
            return View(flags);
        }

        [HttpPost]
        public void ModifyFlag(long siteId, long formItemId, string[] flags) {
            FormItem formItem = FormItemDAO.GetItem(formItemId);
            IEnumerable<string> oldFlags = UserFlagDAO.GetFlags(siteId, formItem.Email, formItem.Mobile, formItem.Phone, formItem.IDCard);

            if(flags == null)
                flags = new string[] { };
            if (oldFlags == null)
                oldFlags = new string[] { };

            IEnumerable<string> addFlags = flags.Except(oldFlags);
            foreach(string flag in addFlags) {
                UserFlagDAO.SetItem(siteId, flag, formItem.Email, formItem.Mobile, formItem.Phone, formItem.IDCard);
            }

            IEnumerable<string> delFlags = oldFlags.Except(flags);
            foreach(string flag in delFlags) {
                UserFlagDAO.Delete(siteId, flag, formItem.Email, formItem.Mobile, formItem.Phone, formItem.IDCard);
            }
        }

        [HttpGet]
        public ActionResult MailLog(long siteId, long menuId, long formId, bool? isEvent) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.IsEvent = isEvent ?? false;

            IEnumerable<FormMailModel> items = FormMailDAO.GetItems(formId);
            return View(items);
        }

        [HttpGet]
        public ActionResult MailView(long siteId, long menuId, long id) {
            ViewBag.UploadUrl = Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            FormMailModel item = FormMailDAO.GetItem(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult MailLogList(long siteId, long menuId, long id, bool? isEvent) {
            IEnumerable<FormMailLogModel> logs = FormMailDAO.GetMailLogs(id);
            ViewBag.FormItems = FormItemDAO.GetItems(logs.Select(log => log.FormItemID));

            ViewBag.SiteID = siteId;
            ViewBag.IsEvent = isEvent ?? false;
            return View(logs);
        }
        #endregion
    }
}