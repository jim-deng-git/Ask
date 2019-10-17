using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using System.IO;

namespace WorkV3.Controllers
{
    public class CommonController : Controller
    {
        string IdentityType = Areas.Backend.Models.DataAccess.CategoryType.Identity.ToString();

        public ActionResult Paragraphs(long siteId, long menuId, long sourceNo, string siteSN, int readMode = 0, IEnumerable<ArticleCategoryModels> readModeCategories = null) {
            IEnumerable<ParagraphModels> paragraphs = ParagraphDAO.GetItems(sourceNo);

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            ViewBag.SiteSN = siteSN;
            ViewBag.ReadMode = readMode; //是否限制會員才能觀看
            ViewBag.IdentityType = IdentityType;
            ViewBag.ReadModeCategories = readModeCategories; //文章限制可觀看的身分

            return View(paragraphs);
        }
        public ActionResult Keywords(long siteId, long menuId, long sourceNo)
        {
            IEnumerable<string> keywordList=null;
            var seoModel = SEODAO.GetItem(sourceNo);
            if (seoModel != null)
            {
                keywordList = seoModel.Keywords.Split(';').ToList();
            }
            var siteInfo = SitesDAO.GetInfo(siteId);
            ViewBag.SiteSN = siteInfo.SN;
            return View(keywordList);
        }
        public ActionResult LightBox(long siteId, long menuId, long sourceNo)
        {
            string Code = "LightBox";
            ViewModels.LightBoxViewModel items = new ViewModels.LightBoxViewModel();
            items.Imgs = ResourceImagesDAO.GetItems(sourceNo, Code);
            items.lightBox = ResourceLightBoxDAO.GetItem(sourceNo, Code);
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            return View(items);
        }
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

        public ActionResult QRCode(string title, string url) {
            ViewBag.PageTitle = title;
            ViewBag.Url = url;
            return View();
        }

        public FileContentResult DownQRCode(string url) {
            string qrUrl = $"http://chart.apis.google.com/chart?cht=qr&chl={ Server.UrlEncode(url) }&chs=170x170&choe=UTF-8";            
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(qrUrl);
            req.ServicePoint.Expect100Continue = false;
            req.KeepAlive = true;
            req.Method = "GET";            
            req.ContentType = "image/png";

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)req.GetResponse();
            System.IO.Stream stream = null;
            System.IO.MemoryStream memoryStream = null;
            try {
                stream = response.GetResponseStream();
                System.Drawing.Image bitmap = System.Drawing.Image.FromStream(stream);

                memoryStream = new System.IO.MemoryStream();
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);                
                return File(memoryStream.ToArray(), "application/octet-stream", "QRCode.png");                
            } finally {
                if (memoryStream != null) memoryStream.Close();
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
        }

        public ActionResult GMap(decimal lat, decimal lng) {
            ViewBag.Lat = lat;
            ViewBag.Lng = lng;
            return View();
        }   
        
        public ActionResult GoogleMap(GoogleMapViewModel model)
        {
            return View(model);
        }
        public string CaptchaCode() {
            Captcha captcha = Captcha.Create();
            return captcha.Code;
        }

        public ActionResult CaptchaImage() {            
            Captcha captcha = Captcha.Current;
            if (captcha == null)
                return Content(string.Empty);

            System.Drawing.Bitmap bitmap = captcha.CreateImage();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return new FileContentResult(memoryStream.ToArray(), "image/png");
        }

        public int CaptchaValidate(Captcha captcha) {
            return captcha.Validate() ? 1 : 0;
        }

        public JsonResult Regions(int? parentId) {
            IEnumerable<WorldRegionModels> items = WorldRegionDAO.GetRegionsByParentId(parentId);
            return Json(items, JsonRequestBehavior.AllowGet);
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
    }
}