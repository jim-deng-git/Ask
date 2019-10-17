using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkLib;
using WorkV3.Golbal;
using WorkV3.Models;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ClickController : Controller
    {
        public ActionResult ImgEdit(long SiteID, long MenuID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ImgEdit(string[] imgs)
        {
            if (imgs == null || imgs.Length == 0)
            {
                ViewBag.Exit = true;
                return View();
            }


            List<ClickImgModel> model = new List<ClickImgModel>();
            int index = 0;
            foreach (var img in imgs)
            {
                ClickImgModel data = JsonConvert.DeserializeObject<ClickImgModel>(img);
                if (data.ID == 0)
                {
                    data.ID = GetItem.NewSN();
                }

                model.Add(data);
                index++;
            }

            ViewBag.Data = JsonConvert.SerializeObject(model);
            ViewBag.Exit = true;

            return View();
        }

        public ActionResult VideoEdit(long SiteID, long MenuID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            return View();
        }

        [HttpPost]
        public ActionResult VideoEdit(ClickVideoModel model, string customVideo)
        {
            if (model.Type == "custom")
                model.Link = customVideo;

            if (model.ID == 0)
            {
                model.ID = GetItem.NewSN();
            }

            ViewBag.Data = JsonConvert.SerializeObject(model);
            ViewBag.Exit = true;

            return View();
        }

        public ActionResult VoiceEdit(long SiteID, long MenuID, ClickVideoModel model, string customVideo)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            return View();
        }

        [HttpPost]
        public ActionResult VoiceEdit(string[] voices)
        {
            if (voices == null || voices.Length == 0)
            {
                ViewBag.Exit = true;
                return View();
            }

            ClickVoiceModel model = JsonConvert.DeserializeObject<ClickVoiceModel>(voices[0]);
            if (model.ID == 0)
            {
                model.ID = GetItem.NewSN();
                model.MimeType = uMimeType.GetMimeType(model.Path);
            }

            ViewBag.Data = JsonConvert.SerializeObject(model);
            ViewBag.Exit = true;

            return View();
        }

        public ActionResult FileEdit(long SiteID, long MenuID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            return View();
        }

        [HttpPost]
        public ActionResult FileEdit(long SiteID, long MenuID, string[] files)
        {
            if (files == null || files.Length == 0)
            {
                ViewBag.Exit = true;
                return View();
            }

            ClickFileModel model = JsonConvert.DeserializeObject<ClickFileModel>(files[0]);
            if (model.ID == 0)
            {
                if (Request.Files.Count == 0)
                {
                    ViewBag.Exit = true;
                    return View();
                }

                HttpPostedFileBase file = Request.Files[0];

                if (file == null || file.ContentLength == 0)
                {
                    ViewBag.Exit = true;
                    return View();
                }

                string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(file, SiteID, MenuID);

                model.ID = GetItem.NewSN();
                model.FileInfo = fileName;
                model.FileSize = file.ContentLength;
                model.FileMimeType = uMimeType.GetMimeType(fileName);
            }

            ViewBag.Data = JsonConvert.SerializeObject(model);
            ViewBag.Exit = true;

            return View();
        }

        public ActionResult LinkEdit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ClickPreview(long SiteID, long MenuID, int clickType, string data)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);

            var model = new object();

            switch (clickType)
            {
                case 1:
                    model = JsonConvert.DeserializeObject<List<ClickImgModel>>(data);
                    return View("ImgPreview", model);
                case 2:
                    model = JsonConvert.DeserializeObject<ClickVideoModel>(data);
                    return View("VideoPreview", model);
                case 3:
                    model = JsonConvert.DeserializeObject<ClickVoiceModel>(data);
                    return View("VoicePreview", model);
                case 4:
                    model = JsonConvert.DeserializeObject<ClickFileModel>(data);
                    return View("FilePreview", model);
                case 5:
                    model = JsonConvert.DeserializeObject<ClickLinkModel>(data);
                    return View("LinkPreview", model);
            }

            return null;
        }

        [HttpPost]
        public string FileUpload(long SiteID, long MenuID)
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;
            string base64 = "", base64_Resize = "";
            if (!string.IsNullOrEmpty(Request.Form["Base64"]))
            {
                base64 = Request.Form["Base64"];
            }
            if (!string.IsNullOrEmpty(Request.Form["Base64_Resize"]))
            {
                base64_Resize = Request.Form["Base64_Resize"];
            }
            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

            return JsonConvert.SerializeObject(new { Name = UpdFileInfo.SaveFilesByMenuID(file, SiteID, MenuID, base64, base64_Resize), FileSize = file.ContentLength.ToString() });
        }
        [HttpPost]
        public string FileUploadCustom(long SiteID, string CustomFolder)
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;
            string base64 = "", base64_Resize = "";
            if (!string.IsNullOrEmpty(Request.Form["Base64"]))
            {
                base64 = Request.Form["Base64"];
            }
            if (!string.IsNullOrEmpty(Request.Form["Base64_Resize"]))
            {
                base64_Resize = Request.Form["Base64_Resize"];
            }
            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

            return JsonConvert.SerializeObject(new { Name = UpdFileInfo.SaveFilesBySiteID(file, SiteID, CustomFolder, base64, base64_Resize), FileSize = file.ContentLength.ToString() });
        }
    }
}