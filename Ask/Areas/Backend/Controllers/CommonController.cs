using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;
using WorkV3.Golbal;

namespace WorkV3.Areas.Backend.Controllers
{
    public class CommonController : Controller
    {
        [HttpGet]
        public ActionResult ImgTextEdit() {
            return View();
        }

        [HttpGet]
        public ActionResult FileTextEdit() {
            return View();
        }

        [HttpGet]
        public ActionResult LinkTextEdit() {
            return View();
        }

        [HttpPost]
        public string FileUpload(long siteId, long menuId, string Base64 = "")
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

           // System.IO.File.AppendAllText("D:\\Temp\\TaiSound.txt", DateTime.Now.ToString() +" "+Base64);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Name = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId, Base64), FileSize = file.ContentLength.ToString(), ShowName = Path.GetFileNameWithoutExtension(file.FileName) });
        }

        public string FileUploadCustomize(long siteId, long sourceNo, string CustomFolder = "", string Base64 = "")
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

            // System.IO.File.AppendAllText("D:\\Temp\\TaiSound.txt", DateTime.Now.ToString() +" "+Base64);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Name = WorkV3.Golbal.UpdFileInfo.SaveFilesBySiteID(file, siteId, CustomFolder, Base64), FileSize = file.ContentLength.ToString(), ShowName = Path.GetFileNameWithoutExtension(file.FileName) });
        }
        //[HttpPost]
        //public string FileUpload(long siteId, long menuId, long sourceNo) {
        //    if (Request.Files == null || Request.Files.Count == 0)
        //        return null;

        //    //HttpPostedFileBase file = Request.Files[0];
        //    //if (file.ContentLength == 0)
        //    //    return null;

        //    //return Newtonsoft.Json.JsonConvert.SerializeObject(new { Name = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId), FileSize = file.ContentLength.ToString(), ShowName = Path.GetFileNameWithoutExtension(file.FileName) });

        //    if (Request.Files == null || Request.Files.Count == 0)
        //        return null;
        //    //string base64 = "";
        //    //debuglog3(Request.Form["Base64"].ToString());
        //    //if (!string.IsNullOrEmpty(Request.Form["Base64"]))
        //    //{
        //    //    base64 = Request.Form["Base64"];
        //    //}


        //    HttpPostedFileBase file = Request.Files[0];
        //    if (file.ContentLength == 0)
        //        return null;

        //    return JsonConvert.SerializeObject(new { Name = UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId, base64), FileSize = file.ContentLength.ToString() });
        //}

        #region ImgEdit
        [HttpGet]
        public ActionResult ImgEdit(long siteId, long menuId, long sourceNo) {
            IEnumerable<ResourceImagesModels> imgs = ResourceImagesDAO.GetItems(sourceNo, "Match");            
            if (imgs != null && imgs.Count() > 0) {
                ResourceImagesModels first = imgs.First();
                ViewBag.SizeType = first.SizeType;
                ViewBag.IsOriginalSize = first.IsOriginalSize;
                ViewBag.MultiImgStyle = first.MultiImgStyle;
                if (ViewBag.MultiImgStyle == "陳列")
                    ViewBag.MultiImgStyle = "輪播";

                ViewBag.Imgs = Newtonsoft.Json.JsonConvert.SerializeObject(imgs, new WorkV3.Golbal.Int64Converter());                
            } else {
                ViewBag.SizeType = 1;
                ViewBag.IsOriginalSize = false;
                ViewBag.MultiImgStyle = "輪播";

                ViewBag.Imgs = "null";                
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);            
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImgEdit(long siteId, long menuId, long sourceNo, string[] imgs, byte sizeType, string isOriginalSize, string multiImgStyle, string Base64 = "") {
            string code = "Match";

            if(imgs == null || imgs.Length == 0) {
                ResourceImagesDAO.DeleteItemsExcept(sourceNo, code);
                ViewBag.Imgs = "null";
            } else {
                List<ResourceImagesModels> imgList = SaveImgs(imgs, siteId, menuId, sourceNo, code, sizeType, isOriginalSize, multiImgStyle);
                ViewBag.Imgs = Newtonsoft.Json.JsonConvert.SerializeObject(imgList, new WorkV3.Golbal.Int64Converter());
            }

            ViewBag.SizeType = sizeType;
            ViewBag.IsOriginalSize = isOriginalSize == "1";
            ViewBag.MultiImgStyle = multiImgStyle;

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            return View();
        }

        private List<ResourceImagesModels> SaveImgs(string[] imgs, long siteId, long menuId, long sourceNo, string code, byte sizeType, string isOriginalSize, string multiImgStyle) {

            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            List<long> ids = new List<long>();
            List<ResourceImagesModels> imgList = new List<ResourceImagesModels>();
            for (int i = 0, len = imgs.Length; i < len; ++i) {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(imgs[i]);
                if (img == null)
                    continue;
                if (img.ID == 0) { // 新增的圖片                    
                    img.ID = WorkLib.GetItem.NewSN();
                    img.SiteID = siteId;
                    img.SourceNo = sourceNo;
                    img.Code = code;
                    img.Ver = 1;
                    img.Creator = MemberDAO.SysCurrent.Id;
                    img.CreateTime = DateTime.Now;
                } else {
                    img.Modifier = MemberDAO.SysCurrent.Id;
                    img.ModifyTime = DateTime.Now;
                }

                img.SizeType = sizeType;
                img.IsOriginalSize = isOriginalSize == "1";
                img.MultiImgStyle = multiImgStyle;
                img.Sort = i;

                ResourceImagesDAO.SetItem(img);

                ids.Add(img.ID);
                imgList.Add(img);
            }

            ResourceImagesDAO.DeleteItemsExcept(sourceNo, code, ids);

            return imgList;
        }
        #endregion

        #region LightBoxEdit
        [HttpGet]
        public ActionResult LightBoxEdit(long siteId, long menuId, long sourceNo)
        {
            string Code = "LightBox";
            ResourceLightBoxModels item = ResourceLightBoxDAO.GetItem(sourceNo, Code);
            if (item == null)
                item = new ResourceLightBoxModels { BtnName = "", BtnColor = "", Title = "" };

            IEnumerable<ResourceImagesModels> imgs = ResourceImagesDAO.GetItems(sourceNo, Code);
            if (imgs != null && imgs.Count() > 0)
            {
                ViewBag.Imgs = Newtonsoft.Json.JsonConvert.SerializeObject(imgs, new WorkV3.Golbal.Int64Converter());
            }
            else
            {
                ViewBag.Imgs = "null";
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LightBoxEdit(long siteId, long menuId, long sourceNo, ResourceLightBoxModels lightbox, string[] imgs, string Base64 = "")
        {
            string code = "LightBox";

            if (imgs == null || imgs.Length == 0)
            {
                ResourceImagesDAO.DeleteItemsExcept(sourceNo, code);
                ViewBag.Imgs = "null";
            }
            else
            {
                List<ResourceImagesModels> imgList = LightBoxSaveImgs(imgs, siteId, menuId, sourceNo, code);
                ViewBag.Imgs = Newtonsoft.Json.JsonConvert.SerializeObject(imgList, new WorkV3.Golbal.Int64Converter());
            }
            if (lightbox.ID == 0)
            {
                lightbox.ID = WorkLib.GetItem.NewSN();
                lightbox.SiteID = siteId;
                lightbox.SourceNo = sourceNo;
                lightbox.Code = code;
            }

            ResourceLightBoxDAO.SetItem(lightbox);

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            return View(lightbox);
        }

        private List<ResourceImagesModels> LightBoxSaveImgs(string[] imgs, long siteId, long menuId, long sourceNo, string code)
        {

            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            List<long> ids = new List<long>();
            List<ResourceImagesModels> imgList = new List<ResourceImagesModels>();
            for (int i = 0, len = imgs.Length; i < len; ++i)
            {
                ResourceImagesModels img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(imgs[i]);
                if (img == null)
                    continue;
                if (img.ID == 0)
                { // 新增的圖片                    
                    img.ID = WorkLib.GetItem.NewSN();
                    img.SiteID = siteId;
                    img.SourceNo = sourceNo;
                    img.Code = code;
                    img.Ver = 1;
                    img.Creator = MemberDAO.SysCurrent.Id;
                    img.CreateTime = DateTime.Now;
                }
                else
                {
                    img.Modifier = MemberDAO.SysCurrent.Id;
                    img.ModifyTime = DateTime.Now;
                }

                img.Sort = i;

                ResourceImagesDAO.SetItem(img);

                ids.Add(img.ID);
                imgList.Add(img);
            }

            ResourceImagesDAO.DeleteItemsExcept(sourceNo, code, ids);

            return imgList;
        }
        #endregion

        #region LinkEdit
        public ActionResult LinkEdit(long siteId, long menuId, long sourceNo) {
            IEnumerable<ResourceLinksModels> links = ResourceLinksDAO.GetItems(sourceNo, "Match");
            
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;            
            return View(links);
        }

        [HttpPost]
        public ActionResult LinkEdit(long siteId, long menuId, long sourceNo, string[] links) {
            string code = "Match";
            List<ResourceLinksModels> linkList = new List<ResourceLinksModels>();

            if (links == null || links.Length == 0) {
                ResourceLinksDAO.DeleteItemsExcept(sourceNo, code);                
            } else {
                List<long> ids = new List<long>();
                //WorkLib.WriteLog.Write(true, links.Length.ToString());
                for(int i = 0, len = links.Length; i < len; ++i) {
                    string item = links[i];
                    //WorkLib.WriteLog.Write(true, links[i]);
                    ResourceLinksModels link = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceLinksModels>(item);
                    if(link.Id == 0) {
                        link.Id = WorkLib.GetItem.NewSN();
                        link.SiteID = siteId;
                        link.SourceNo = sourceNo;
                        link.SourceType = 1;
                        link.Ver = 1;
                        link.AreaID = 1;
                        link.Code = code;
                        link.ClickType = 0;
                    }
                    if (!string.IsNullOrEmpty(link.Detail))
                    {
                        var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(link.Detail));
                        if (pageInfo != null && !string.IsNullOrEmpty(pageInfo.Title))
                        {
                            var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageInfo.SiteID);
                            link.LinkInfo = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = pageInfo.SN });
                        }
                        else
                        {
                            var menuInfo = WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.GetInfo(long.Parse(link.Detail));
                            if (menuInfo != null && !string.IsNullOrEmpty(menuInfo.Title))
                            {
                                var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(menuInfo.SiteID);
                                link.LinkInfo = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = menuInfo.SN });
                            }
                        }
                    }
                    link.Sort = i + 1;
                    ResourceLinksDAO.SetItem(link);
                    linkList.Add(link);
                    ids.Add(link.Id);
                }

                ResourceLinksDAO.DeleteItemsExcept(sourceNo, code, ids);
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;            
            ViewBag.Exit = true;
            return View(linkList);
        }
        #endregion

        #region FileEdit
        [HttpGet]
        public ActionResult FileEdit(long siteId, long menuId, long sourceNo) {
            IEnumerable<ResourceFilesModels> files = ResourceFilesDAO.GetItems(sourceNo, "Match");
            if (files != null && files.Count() > 0) {                
                ViewBag.Files = Newtonsoft.Json.JsonConvert.SerializeObject(files, new WorkV3.Golbal.Int64Converter());
            } else {                
                ViewBag.Files = "null";
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View();
        }

        [HttpPost]
        public ActionResult FileEdit(long siteId, long menuId, long sourceNo, string[] files) {
            string code = "Match";

            if (files == null || files.Length == 0) {
                ResourceFilesDAO.DeleteItemsExcept(sourceNo, code);
                ViewBag.Files = "null";
            } else {
                List<ResourceFilesModels> fileList = SaveFiles(files, siteId, menuId, sourceNo, code);
                ViewBag.Files = Newtonsoft.Json.JsonConvert.SerializeObject(fileList, new WorkV3.Golbal.Int64Converter());
            }
                        
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            return View();
        }

        private List<ResourceFilesModels> SaveFiles(string[] files, long siteId, long menuId, long sourceNo, string code)
        {
            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            List<long> ids = new List<long>();
            List<ResourceFilesModels> fileList = new List<ResourceFilesModels>();
            for (int i = 0, len = files.Length; i < len; ++i) {
                ResourceFilesModels file = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceFilesModels>(files[i]);
                if (file.Id == 0) { // 新增的檔案                    
                    file.Id = WorkLib.GetItem.NewSN();
                    file.SiteID = siteId;
                    file.SourceNo = sourceNo;
                    file.Code = code;                    
                    file.Ver = 1;                    
                }
                                
                file.Sort = i;

                ResourceFilesDAO.SetItem(file);

                ids.Add(file.Id);
                fileList.Add(file);
            }

            ResourceFilesDAO.DeleteItemsExcept(sourceNo, code, ids);

            return fileList;
        }
        #endregion

        #region VoiceEdit
        [HttpGet]
        public ActionResult VoiceEdit(long siteId, long menuId, long sourceNo) {
            IEnumerable<ResourceVoicesModels> voices = ResourceVoicesDAO.GetItems(sourceNo, "Match");
            if (voices != null && voices.Count() > 0) {
                ViewBag.Voices = Newtonsoft.Json.JsonConvert.SerializeObject(voices, new WorkV3.Golbal.Int64Converter());
            } else {
                ViewBag.Voices = "null";
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View();
        }

        [HttpPost]
        public ActionResult VoiceEdit(long siteId, long menuId, long sourceNo, string[] voices) {
            string code = "Match";

            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (voices == null || voices.Length == 0) {
                ResourceVoicesDAO.DeleteItemsExcept(sourceNo, code);
                ViewBag.Voices = "null";
            } else {
                List<ResourceVoicesModels> voiceList = SaveVoices(voices, siteId, menuId, sourceNo, code);
                ViewBag.Voices = Newtonsoft.Json.JsonConvert.SerializeObject(voiceList, new WorkV3.Golbal.Int64Converter());
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            return View();
        }

        private List<ResourceVoicesModels> SaveVoices(string[] voices, long siteId, long menuId, long sourceNo, string code)
        {
            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            List<long> ids = new List<long>();
            List<ResourceVoicesModels> voiceList = new List<ResourceVoicesModels>();
            for (int i = 0, len = voices.Length; i < len; ++i) {
                ResourceVoicesModels voice = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceVoicesModels>(voices[i]);
                if (voice.ID == 0) { // 新增的聲音
                    voice.ID = WorkLib.GetItem.NewSN();
                    voice.SiteID = siteId;
                    voice.SourceNo = sourceNo;
                    voice.Code = code;                    
                }

                voice.Sort = i;

                ResourceVoicesDAO.SetItem(voice);

                ids.Add(voice.ID);
                voiceList.Add(voice);
            }

            ResourceVoicesDAO.DeleteItemsExcept(sourceNo, code, ids);

            return voiceList;
        }
        #endregion

        #region VideoEdit
        [HttpGet]
        public ActionResult VideoEdit(long siteId, long menuId, long sourceNo) {
            ResourceVideosModels item = ResourceVideosDAO.GetItem(sourceNo, "Match");
            if (item == null)
                item = new ResourceVideosModels { Type = "youtube", SizeType = 1, PlayMode = "燈箱播放", IsAuto = true, IsRepeat = true, ScreenshotIsCustom = false };

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VideoEdit(long siteId, long menuId, long sourceNo, ResourceVideosModels video, string customVideo) {
            string code = "Match";

            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (video.Type == "custom")
                video.Link = customVideo;

            if(video.ScreenshotIsCustom && !string.IsNullOrWhiteSpace(video.Screenshot)) {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(video.Screenshot);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["shotUpload"];
                    string shotUploadBase64 = Request.Form["shotUploadBase64"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        video.Screenshot = string.Empty;
                    } else {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, shotUploadBase64);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        video.Screenshot = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            if(video.ID == 0) {
                video.ID = WorkLib.GetItem.NewSN();
                video.SiteID = siteId;
                video.SourceNo = sourceNo;
                video.Code = code;
            }

            ResourceVideosDAO.SetItem(video);            

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.SourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            return View(video);
        }
        #endregion

        #region ParagraphEdit
        /// <summary>
        /// Paragraph - Edit
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="sourceNo"></param>
        /// <param name="initalMatchType">預設搭配媒體類型(空值代表全部,Img圖片,Video圖片,File檔案)</param>
        /// <param name="hdParaMatchPos">隱藏圖片垂直排版按鈕參數</param>
        /// <param name="hdParaMatchAlign">隱藏圖片水平排版按鈕參數</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ParagraphEdit(long siteId, long menuId, long? sourceNo, string initalMatchType = "",string hdParaMatchPos = null,string hdParaMatchAlign = null,bool IsSingleImgUpload = false) {
            IEnumerable<ParagraphModels> paragraphs = null;
            if (sourceNo != null)
                paragraphs = ParagraphDAO.GetItems((long)sourceNo);

            List<KeyValuePair<string, string>> matchTypeBtnItems;
            SetMatchType(initalMatchType, out matchTypeBtnItems);
            List<KeyValuePair<string, string>> posBtnItems;
            List<KeyValuePair<string, string>> alignBtnItems;
            hdParaMatch(hdParaMatchPos, hdParaMatchAlign,out posBtnItems,out alignBtnItems);

            if (paragraphs == null || paragraphs.Count() == 0)
            {
                List<ParagraphModels> newParagraphs = new List<ParagraphModels>();
                newParagraphs.Add(new ParagraphModels
                {
                    ID = WorkLib.GetItem.NewSN(),
                    ImgPos = posBtnItems.Any() ? posBtnItems.FirstOrDefault().Value : "圖片在上",
                    ImgAlign = alignBtnItems.Any() ? alignBtnItems.FirstOrDefault().Value : "圖片置左",
                    //ImgPos = "圖片在上",
                    //ImgAlign = "圖片置左",
                    MatchType = initalMatchType,
                    IsOriginalSize = false,
                    SizeType = "0"
                });

                paragraphs = newParagraphs;
            }
            else
            {
                foreach (ParagraphModels p in paragraphs)
                {
                    if (p.MatchType == "img")
                    {
                        var imageModelList = ResourceImagesDAO.GetItems(p.ID);
                        if (imageModelList != null && imageModelList.Count() > 0)
                        {
                            p.IsOriginalSize = imageModelList.First().IsOriginalSize;
                            p.SizeType = imageModelList.First().SizeType.Value.ToString();
                        }
                    }
                }
            }
            ViewBag.matchTypeBtnItems = matchTypeBtnItems;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.posBtnItems = posBtnItems;

            if(alignBtnItems.Count==1 && alignBtnItems.Any(m=>m.Key=="center"))
                alignBtnItems = alignBtnItems.Where(m => m.Key != "center").ToList(); //20180425當只有一個置中時、註解不顯示

            ViewBag.alignBtnItems = alignBtnItems;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            ViewBag.IsSingleImgUpload = IsSingleImgUpload;
            return View(paragraphs);
        }
        /// <summary>
        /// 單檔圖片上傳
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="MenuID"></param>
        /// <param name="sourceNo"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SingleImageUpload(long SiteID, long MenuID, long sourceNo)
        {
            IEnumerable<ResourceImagesModels> ImgsModel = ResourceImagesDAO.GetItems(sourceNo, "Match");
            ResourceImagesModels ImgModel = null ;
            if (ImgsModel != null && ImgsModel.Any())
            {
                ImgModel = ImgsModel.FirstOrDefault();
            }

            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            ViewBag.sourceNo = sourceNo;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
            ViewBag.ImgModelJson = ImgModel != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ImgModel) : string.Empty;
            ViewBag.ImgModel = ImgModel;
            return View();
        }
        /// <summary>
        /// 單檔圖片上傳 POST
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="menuId"></param>
        /// <param name="sourceNo"></param>
        /// <param name="Picture"></param>
        /// <param name="Link"></param>
        /// <param name="IsOpenNew"></param>
        /// <param name="sizeType"></param>
        /// <param name="isOriginalSize"></param>
        /// <param name="multiImgStyle"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult SingleImageUpload(long siteId, long menuId, long sourceNo,string Picture,string Link, string IsOpenNew, byte sizeType = 1, string isOriginalSize = null, string multiImgStyle = "輪播")
        {
            string code = "Match";
            bool BoolIsOpenNew = !string.IsNullOrWhiteSpace(IsOpenNew) ? IsOpenNew == "on" ? true : false : false ;
            ResourceImagesModels img = null;
            HttpPostedFileBase postedFile = Request.Files["fPicture"];
            if (!string.IsNullOrWhiteSpace(Picture))
            {
                img = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(Picture);
                if (img.ID == 0)
                {
                    // 新上傳的圖片
                    img.ID = WorkLib.GetItem.NewSN();
                    img.SiteID = siteId;
                    img.SourceNo = sourceNo;
                    img.Code = code;
                    img.Ver = 1;
                    img.Creator = MemberDAO.SysCurrent.Id;
                    img.CreateTime = DateTime.Now;
                    if (postedFile == null || postedFile.ContentLength == 0)
                        img.Img = string.Empty;
                    else
                        img.Img = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId);
                }
                else
                {
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        //img.Img =string.Empty; //傳空值控件會故障, 故註解
                    }
                    else
                        img.Img = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId);

                    img.Modifier = MemberDAO.SysCurrent.Id;
                    img.ModifyTime = DateTime.Now;
                }
                img.SizeType = sizeType;
                img.IsOriginalSize = isOriginalSize == "1";
                img.IsOpenNew = BoolIsOpenNew;
                img.Link = Link;
                img.MultiImgStyle = multiImgStyle;
                img.Sort = 0;
                ResourceImagesDAO.SetItem(img);
                ResourceImagesDAO.DeleteItemsExcept(sourceNo, code, new long[] { img.ID });
            }
            else
            {
                img = new ResourceImagesModels();
                ResourceImagesDAO.DeleteItemsExcept(sourceNo, code);
            }
            ViewBag.ImgModel = img;
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.sourceNo = sourceNo;
            ViewBag.ImgModelJson = img != null ? Newtonsoft.Json.JsonConvert.SerializeObject(img) : string.Empty;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View();
        }
        /// <summary>
        /// 產生搭配媒體類型按鈕
        /// </summary>
        /// <param name="initalMatchType">搭配之媒體類型(空值代表全部,Img圖片,Video圖片,File檔案)</param>
        /// <param name="matchTypeBtnItems"></param>
        private void SetMatchType(string initalMatchType, out List<KeyValuePair<string, string>> matchTypeBtnItems)
        {
            matchTypeBtnItems = new List<System.Collections.Generic.KeyValuePair<string, string>>();
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("Img", "image"));
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("Video", "video"));
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("Voice", "music"));
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("File", "download"));
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("Link", "link-variant")); //charlie_shan 20181120回復
            matchTypeBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("LightBox", "newspaper"));

            if (!string.IsNullOrWhiteSpace(initalMatchType))
            {
                matchTypeBtnItems = matchTypeBtnItems.Where(m => m.Key.ToLower() == initalMatchType).ToList();
            }
        }
        /// <summary>
        /// 隱藏圖片垂直或水平排版按鈕參數
        /// </summary>
        /// <param name="hdParaMatchPos">隱藏圖片水平排版按鈕參數</param>
        /// <param name="hdParaMatchAlign">隱藏圖片垂直排版按鈕參數</param>
        /// <param name="posBtnItems"></param>
        /// <param name="alignBtnItems"></param>
        private void hdParaMatch(string hdParaMatchPos, string hdParaMatchAlign,out List<KeyValuePair<string, string>> posBtnItems, out List<KeyValuePair<string, string>> alignBtnItems)
        {
            posBtnItems = new List<System.Collections.Generic.KeyValuePair<string, string>>();
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("top", "圖片在上"));
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("bottom", "圖片在下"));
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("center", "圖片在標題下方"));
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("float_left", "文繞圖"));

            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("cc-reading", "名言"));
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("cc-book-open-page-variant", "經歷"));

            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("format-left", "左圖右文"));
            posBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("format-right", "左文右圖"));

            alignBtnItems = new List<System.Collections.Generic.KeyValuePair<string, string>>();
            alignBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("left", "圖片置左"));
            alignBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("right", "圖片置右"));
            alignBtnItems.Add(new System.Collections.Generic.KeyValuePair<string, string>("center", "圖片置中"));

            if (hdParaMatchPos != null)
            {
                var hdParaMatchPosArray = hdParaMatchPos.Split(',');
                posBtnItems = posBtnItems.Where(m => !hdParaMatchPosArray.Contains(m.Key)).ToList();
            }

            if (hdParaMatchAlign != null)
            {
                var hdParaMatchAlignArray = hdParaMatchAlign.Split(',');
                alignBtnItems = alignBtnItems.Where(m => !hdParaMatchAlignArray.Contains(m.Key)).ToList();
            }
        }

        [HttpGet]
        public ActionResult ParagraphMatch(long siteId, long menuId, long paragraphId, string matchType, bool IsSingleImgUpload = false) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.ParagraphID = paragraphId;
            ViewBag.MatchType = matchType;
            ViewBag.IsSingleImgUpload = IsSingleImgUpload;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            string code = "Match";
            if (matchType == "img") {
                ViewBag.Items = ResourceImagesDAO.GetItems(paragraphId, code);
            } else if (matchType == "video") {
                ViewBag.Item = ResourceVideosDAO.GetItem(paragraphId, code);                
            } else if (matchType == "file") {
                ViewBag.Items = ResourceFilesDAO.GetItems(paragraphId, code);
            } else if (matchType == "link") {
                ViewBag.Items = ResourceLinksDAO.GetItems(paragraphId, code);
            } else if (matchType == "voice") {
                ViewBag.Items = ResourceVoicesDAO.GetItems(paragraphId, code);
            } else if (matchType == "lightbox") {
                ViewBag.Item = ResourceLightBoxDAO.GetItem(paragraphId, "LightBox");
            }

            return View();
        }

        [HttpGet]
        public int ParagraphMatchDel(long siteId, long menuId, long paragraphId, string matchType) {
            string code = "Match";
            if (matchType == "img")
                return ResourceImagesDAO.DeleteItems(paragraphId, code);

            if (matchType == "video")
                return ResourceVideosDAO.DeleteItems(paragraphId, code);

            if (matchType == "file") 
                return ResourceFilesDAO.DeleteItems(paragraphId, code);

            if (matchType == "link")
                return ResourceLinksDAO.DeleteItems(paragraphId, code);

            if (matchType == "voice")
                return ResourceVoicesDAO.DeleteItems(paragraphId, code);

            return 0;
        }
        #endregion

        #region Pager
        public ActionResult Pager(int totalRecord, int pageIndex, int? pageSize, string blankMessage, string queryParaName) {
            Pager pager = new Pager {
                TotalRecord = totalRecord,
                BlankMessage = blankMessage
            };

            if (totalRecord == 0) {
                pager.BlankMessage = "抱歉，目前沒有結果可以顯示。";
                return PartialView("pager", pager);
            }

            if (pageSize == null || pageSize == 0)
                pageSize = WebInfo.PageSize;
            if (pageIndex <= 0)
                pageIndex = 1;

            pager.TotalPage = (int)Math.Ceiling((double)totalRecord / (int)pageSize);
            pager.PageIndex = (int)pageIndex;
            pager.UrlFmt = GetUrlFmt(queryParaName);

            int startPageIndex = pageIndex - 5;
            if (startPageIndex < 1)
                startPageIndex = 1;
            int endPageIndex = startPageIndex + 9;
            if (endPageIndex > pager.TotalPage) {
                endPageIndex = pager.TotalPage;
                startPageIndex = (endPageIndex - 9 < 1) ? 1 : endPageIndex - 9;
            }

            List<int> pageNumbers = new List<int>();
            if (startPageIndex > 1)
                pageNumbers.Add(1);
            if (startPageIndex > 2)
                pageNumbers.Add(0); // 0 表示分隔符

            for (; startPageIndex <= endPageIndex; ++startPageIndex)
                pageNumbers.Add(startPageIndex);

            if (endPageIndex < pager.TotalPage - 1)
                pageNumbers.Add(0);
            if (endPageIndex < pager.TotalPage)
                pageNumbers.Add(pager.TotalPage);

            pager.PageNumbers = pageNumbers;
            return PartialView("pager", pager);
        }

        private string GetUrlFmt(string queryParaName) {
            if (String.IsNullOrEmpty(queryParaName))
                queryParaName = "index";

            string path = Request.Path;
            string query = Request.Url.Query;
            if (!String.IsNullOrEmpty(query)) {
                query = System.Text.RegularExpressions.Regex.Replace(query, @"[?&]" + queryParaName + @"=\d+", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                query = query.TrimStart('?', '&');
            }
            if (!String.IsNullOrEmpty(query))
                query += "&";

            return path + "?" + query + queryParaName + "=";
        }
        #endregion

        public ActionResult GMap() {
            return View();
        }
        
        public JsonResult Regions(int? parentId) {
            IEnumerable<WorkV3.Models.WorldRegionModels> items = WorkV3.Models.DataAccess.WorldRegionDAO.GetRegionsByParentId(parentId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectRangeDate(DateTime? start, DateTime? end, DateRange? select)
        {
            ViewBag.dtStart = start;
            ViewBag.dtEnd = end;
            ViewBag.EnumSelect = select;
            return View();
        }
        public ActionResult SelectManagers(string SelectMembers, bool IsAutoReload = true)
        {

            string uploadUrl = WorkLib.GetItem.ViewUpdUrl().TrimEnd('/') + "/Manager/";
            List<ViewModels.AnalysisMemberGroupViewModel> GroupMembers = new List<ViewModels.AnalysisMemberGroupViewModel>();
            var groups = Models.GroupDAO.GetItems();
            foreach (Models.GroupModels group in groups)
            {
                ViewModels.AnalysisMemberGroupViewModel groupViewModel = new ViewModels.AnalysisMemberGroupViewModel()
                {
                    GroupModel = group
                };
                int memberCount = 0;
                groupViewModel.GroupMembers = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount, GroupID: group.Id.ToString());
                GroupMembers.Add(groupViewModel);
            }
            ViewBag.GroupMembers = GroupMembers;
            ViewBag.SelectMembers = SelectMembers;
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.IsAutoReload = IsAutoReload;
            return View();
        }

        public ActionResult ManagerInfo(long memberId)
        {
            MemberModels member = MemberDAO.GetItem(memberId);
            GroupModels group = GroupDAO.GetItem(member.GroupId);

            ViewBag.Group = group;

            return View(member);
        }

        #region SEOEdit
        [HttpGet]
        public ActionResult SEOEdit(long siteId, long menuId, long sourceNo)
        {
            WorkV3.Models.SEOModels item = WorkV3.Models.DataAccess.SEODAO.GetItem(sourceNo);
            if (item == null)
            {
                item = new WorkV3.Models.SEOModels { SourceNo = sourceNo, MenuID = menuId, Robot = true };
                long pageNo = 0, cardNo = 0, zoneNo = 0;
                var article = ArticleDAO.GetItem(sourceNo);
                if (article != null)
                {
                    item.Title = article.Title;
                    cardNo = article.CardNo;
                }
                //else
                //{
                //    var events = WorkV3.Models.DataAccess.EventDAO.GetItem(sourceNo);
                //    if (events != null)
                //    {
                //        item.Title = events.Title;
                //        cardNo = events.CardNo;
                //    }
                //    else
                //    {
                //        var questionnaire = WorkV3.Models.DataAccess.QuestionnaireDAO.GetItem(sourceNo);
                //        if (questionnaire != null)
                //        {
                //            item.Title = questionnaire.Title;
                //            cardNo = questionnaire.CardNo;
                //        }
                //        else
                //        {
                //            var seek = WorkV3.Models.SeekDAO.GetItem(sourceNo);
                //            if (seek != null)
                //            {
                //                item.Title = seek.Title;
                //                cardNo = seek.CardNo;
                //            }
                //            else
                //            {
                //                var reward = RewardDAO.GetItem(sourceNo);
                //                if (reward != null)
                //                {

                //                    item.Title = reward.Title;
                //                    cardNo = reward.CardNo;
                //                }
                //            }
                //        }

                //    }
                //}
                if (cardNo != 0)
                {
                    var cardData = WorkV3.Areas.Backend.Models.DataAccess.CardsDAO.GetByNo(cardNo);
                    if (cardData != null && cardData.ZoneNo.HasValue)
                    {
                        var zoneData = WorkV3.Areas.Backend.Models.DataAccess.ZonesDAO.GetByNo(cardData.ZoneNo.Value);
                        if (zoneData != null)
                        {
                            pageNo = zoneData.PageNo;
                        }
                    }
                }
                if (pageNo != 0)
                {
                    WorkV3.ViewModels.SEORelationModel defaultSEO = WorkV3.Models.DataAccess.PagesDAO.GetContentSEO(siteId, menuId, pageNo);
                    if (defaultSEO != null)
                    {
                        item.Keywords = defaultSEO.Keywords;
                        item.Author = defaultSEO.Author;
                        item.Description = defaultSEO.Description;
                        string img = defaultSEO.SocialImage;
                        if (!string.IsNullOrEmpty(img))
                        {
                            string newUploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
                            img = System.IO.Path.GetFileName(img.Replace(newUploadUrl, ""));
                            item.Image = img;
                        }
                    }
                }
                WorkV3.Areas.Backend.Models.SiteSeoSettingModels siteDefaultSEO = WorkV3.Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(siteId);
                if (siteDefaultSEO != null)
                {
                    if (string.IsNullOrEmpty(item.Copyright))
                        item.Copyright = siteDefaultSEO.Copyright;
                    item.Author += (string.IsNullOrEmpty(item.Author) ? "" : ";") + siteDefaultSEO.Author;
                    if (string.IsNullOrEmpty(item.Description))
                        item.Description = siteDefaultSEO.Description;
                }
            }

            ViewBag.SiteID = siteId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            return View(item);
        }

        [HttpPost]
        public ActionResult SEOEdit(long siteId, WorkV3.Models.SEOModels item)
        {
            if (!Common.Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            if (!string.IsNullOrWhiteSpace(item.Image))
            {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.Image);
                if (imgModel.ID == 0)
                { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fImage"];
                    string fImageBase64 = Request.Form["fImageBase64"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.Image = string.Empty;
                    }
                    else
                    {
                        item.Image = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, item.MenuID, fImageBase64);
                    }
                }
                else
                {
                    item.Image = imgModel.Img;
                }
            }

            WorkV3.Models.DataAccess.SEODAO.SetItem(item);

            ViewBag.SiteID = siteId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, item.MenuID);
            ViewBag.Exit = true;
            return View(item);
        }

        public ActionResult SEOKeyword(long siteId, long menuId)
        {
            IEnumerable<string> keywords = WorkV3.Models.DataAccess.SEODAO.GetKeywords(siteId, menuId);

            return View(keywords);
        }
        #endregion
        #region client 跨頁操作功能
        string sessionFixID = "action_{0}_{1}";
        public List<string> GetSelectSessionList(string menuID, string actionType)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = new List<string>();
            if (Session[sessionID] != null)
            {
                selectList = (List<string>)Session[sessionID];
            }
            return selectList;
        }
        public List<string> SetInitSelectSessionList(string menuID, string actionType)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = new List<string>();
            Session[sessionID] = selectList;
            return selectList;
        }
        public List<string> SetSelectSessionList(string menuID, string actionType, List<string> selectIds)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = GetSelectSessionList(menuID, actionType);
            foreach (string selectId in selectIds)
            {
                if (string.IsNullOrEmpty(selectId))
                    continue;
                if (selectList.Contains(selectId))
                    continue;
                selectList.Add(selectId);
            }
            Session[sessionID] = selectList;
            return selectList;
        }
        public List<string> RemoveSelectSessionList(string menuID, string actionType, List<string> selectIds)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            List<string> selectList = GetSelectSessionList(menuID, actionType);
            foreach (string selectId in selectIds)
            {
                if (selectList.Contains(selectId))
                    selectList.Remove(selectId);
            }
            Session[sessionID] = selectList;
            return selectList;
        }
        [HttpGet]
        public ActionResult GetClientSelectList(string menuID, string actionType)
        {
            List<string> selectList = GetSelectSessionList(menuID, actionType);
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SetClientSelectList(string menuID, string actionType, string[] ids)
        {
            List<string> selectList = new List<string>();
            if (ids == null)
            {
                selectList = SetInitSelectSessionList(menuID, actionType);
            }
            else
            {
                selectList = SetSelectSessionList(menuID, actionType, ids.ToList());
            }

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult RemoveClientSelectList(string menuID, string actionType, string[] ids)
        {
            List<string> selectList = new List<string>();
            if (ids != null)
            {
                selectList = RemoveSelectSessionList(menuID, actionType, ids.ToList());
            }
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CancelClientSelectList(string menuID, string actionType)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            Session[sessionID] = null;
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult IsClientSelectList(string menuID, string actionType)
        {
            string sessionID = string.Format(sessionFixID, actionType, menuID);
            if (Session[sessionID] == null)
                return Json("false", JsonRequestBehavior.AllowGet);
            return Json("true", JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region ImgCropper
        [HttpGet]
        public ActionResult ImgCropper(long SiteID, long MenuID)
        {
            ViewBag.SiteID = SiteID;
            ViewBag.MenuID = MenuID;
            return View();
        }
        [HttpPost]
        public ActionResult ImgCropperUpload(long SiteID, long MenuID, string FileName, string Base64)
        {
            string tempPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(SiteID, MenuID);
            if (!System.IO.Directory.Exists(tempPath))
                System.IO.Directory.CreateDirectory(tempPath);
            string saveFileFullPath = tempPath + "\\" + FileName;
            string baseString64 = Base64.Split(',')[1];
            var bytes = Convert.FromBase64String(baseString64);
            using (var imageFile = new FileStream(saveFileFullPath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
                imageFile.Dispose();
            }
            string r_File = tempPath + "\\" + "R_"+FileName;
            if (System.IO.File.Exists(r_File))
            {
                System.IO.File.Copy(saveFileFullPath, r_File, true);
            }
            return Json(FileName);
        }
        [HttpPost]
        public ActionResult ImgCropperTempUpload(long SiteID, string FileName, string Base64)
        {
            string tempPath = WorkV3.Golbal.UpdFileInfo.GetUPathBySiteID(SiteID, "Temp");
            if (!System.IO.Directory.Exists(tempPath))
                System.IO.Directory.CreateDirectory(tempPath);
            if (System.IO.File.Exists(tempPath + "\\" + FileName))
                System.IO.File.Delete(tempPath + "\\" + FileName);
            string tempViewPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, "Temp");
            string saveFileFullPath = tempPath + "\\" + FileName;
            string baseString64 = Base64.Split(',')[1];
            var bytes = Convert.FromBase64String(baseString64);
            using (var imageFile = new FileStream(saveFileFullPath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
                imageFile.Dispose();
            }
            return Json(tempViewPath+"/"+FileName);
        }
        #endregion

        #region 共用的連結編輯/選單功能

        public ActionResult GetSites()
        {
            IEnumerable<WorkV3.Models.SitesModels> modelList = WorkV3.Models.DataAccess.SitesDAO.GetDatas();

            return Json(modelList);
        }
        //public ActionResult CheckIsMenu(long menuID)
        //{
        //    var menuModel = WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.GetInfo(menuID);

        //    if (menuModel != null)
        //    {
        //        var pageModel = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(menuID);
        //        if (pageModel != null )
        //        {
        //            if (pageModel.MenuID == pageModel.No)
        //            {

        //                var pageChildModels = WorkV3.Areas.Backend.Models.DataAccess.StatisticConditionDAO.GetMenuORPages(menuID);
        //                pageChildModels.Where(p=>p.Type == ViewModels.StructureType.Menu)
        //                return Json("true");
        //            }
        //            else
        //                return Json("false");
        //        }

        //        return Json("true");
        //    }
            
        //    return Json("false");
        //}
        #endregion
    }
}