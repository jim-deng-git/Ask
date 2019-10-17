using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Common;
using Newtonsoft.Json;

namespace WorkV3.Areas.Backend.Controllers
{
    public class FormSetController : Controller
    {
        public ActionResult Setting(long SiteID, long MenuID, long CardNo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.CardNo = CardNo;

            var query = new Query();
            query.Where.Add(new QWhere("SiteID", COperator.Equal, SiteID));
            query.Where.Add(new QWhere("Title", COperator.NoEqual, " "));
            var Model = FormDAO.Instance.Get(query);

            var formSet = FormDAO.getFormSet(CardNo);
            ViewBag.FormSet = formSet;
            return View(Model);
        }
        [HttpPost]
        public ActionResult Setting(long SiteID, long MenuID, long CardNo, long FormID)
        {
            FormDAO.SvaeFormSetting(FormID, CardNo);
            return RedirectToAction("List", "Intro", new { SiteID = SiteID, MenuID = MenuID });
        }

        private const string formDesignFileDir = "FormDesign";
        [HttpGet]
        public ActionResult Edit(long siteId, long menuId)
        {
            FormModel item = FormDAO.GetItemFromSourceID(menuId);
            if (item == null)
            {
                item = new FormModel { ID = WorkLib.GetItem.NewSN(), SiteID = siteId, ForceStatement = true, SourceID = menuId };
                FormDAO.SetItem(item);

                FieldDAO.SetItem(new FieldModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    ParentID = item.ID,
                    SN = 1,
                    DefaultType = (int)FieldDefaultType.姓名,
                    TypeID = "input",
                    Title = "姓名",
                    Width = (int)FieldWidth.Quarter,
                    Requied = true,
                    ShowInList = true
                });
                FieldDAO.SetItem(new FieldModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    ParentID = item.ID,
                    SN = 2,
                    DefaultType = (int)FieldDefaultType.性別,
                    TypeID = "radio",
                    Width = (int)FieldWidth.Quarter,
                    OptionArray = true,
                    Options = "先生;女士",
                    Requied = true
                });
                FieldDAO.SetItem(new FieldModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    ParentID = item.ID,
                    SN = 3,
                    DefaultType = (int)FieldDefaultType.Email,
                    TypeID = "input",
                    Title = "Email",
                    Width = (int)FieldWidth.Half,
                    Requied = true,
                    Fomat = true,
                    FomatType = (int)FieldInputFormat.Email,
                    ShowInList = true
                });
                FieldDAO.SetItem(new FieldModel
                {
                    ID = WorkLib.GetItem.NewSN(),
                    ParentID = item.ID,
                    SN = 4,
                    DefaultType = (int)FieldDefaultType.電話,
                    TypeID = "input",
                    Title = "手機",
                    Width = (int)FieldWidth.Half,
                    Requied = true,
                    Fomat = true,
                    FomatType = (int)FieldInputFormat.台灣手機號,
                    ShowInList = true
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
        public ActionResult Edit(long siteId, long menuId, FormModel item, string fields, string admins)
        {
            if (!string.IsNullOrWhiteSpace(item.Image))
            {
                ResourceImagesModels imgModel = JsonConvert.DeserializeObject<ResourceImagesModels>(item.Image);
                if (imgModel.ID == 0)
                { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fImage"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.Image = string.Empty;
                    }
                    else
                    {
                        item.Image = Golbal.UpdFileInfo.SaveFilesBySiteID(postedFile, siteId, formDesignFileDir);
                    }
                }
                else
                {
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
    }
}