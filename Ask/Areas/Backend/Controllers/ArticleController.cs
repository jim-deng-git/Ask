using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using Newtonsoft.Json;

namespace WorkV3.Areas.Backend.Controllers
{
    public class ArticleController : Controller
    {
        string IdentityType = CategoryType.Identity.ToString();
        string FavorityType = CategoryType.Favority.ToString();
        public ActionResult List(long siteId, long menuId, int? index, ArticleSearchModels search) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.Types = ArticleTypesDAO.GetIssueItems(menuId);
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            if (Request.HttpMethod == "GET") {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else {
                    ArticleSearchModels prevSearch = WorkV3.Common.Utility.GetSearchValue<ArticleSearchModels>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            } else if(Request.HttpMethod == "POST") {
                WorkV3.Common.Utility.SetSearchValue(search);
            }
            ViewBag.Search = search;

            Pagination pagination = new Pagination {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            IEnumerable<ArticleModels> items = ArticleDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            ViewBag.SitePages = WorkV3.Models.DataAccess.CardsDAO.GetPages(items.Select(item => item.CardNo));
            ViewBag.ItemTypes = ArticleDAO.GetItemTypes(items.Select(item => item.ID));

            ViewBag.Pagination = pagination;
            return View(items);
        }

        #region Edit
        [HttpGet]
        public ActionResult EditMode(long siteId, long menuId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            IEnumerable<WorkV3.Models.TemplateModels> templates = ArticleDAO.GetTemplates(menuId);
            return View(templates);
        }

        [HttpGet]
        public ActionResult Edit(long siteId, long menuId, string type, long? id, long? templateId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.Types = ArticleTypesDAO.GetIssueItems(menuId);
            ViewBag.Series = ArticleSeriesDAO.GetIssueItems(menuId);
            ViewBag.Categories = CategoryDAO.GetItems(FavorityType);
            ViewBag.Sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.ListIdentity = CategoryDAO.GetIssueItems(IdentityType);

            int cardStyleID = 1;
            List<CardsModels> cardItem = CardsDAO.GetPageData(siteId, menuId);
            foreach (var cards in cardItem)
            {
                if (cards.CardsType == "Article")
                {
                    cardStyleID = cards.StylesID;
                    break;
                }
            }

            ArticleModels item = null;
            if (id != null)
            {
                item = ArticleDAO.GetItem((long)id);
                SetLinkDetailViewBag(item.LinkDetail);
            }

            if(item == null && templateId != null) {
                MenusModels menu = MenusDAO.GetInfo(siteId, menuId);
                long newArticleId = WorkLib.GetItem.NewSN();                
                long cardNo = WorkV3.Golbal.PubFunc.AddPage(siteId, menuId, menu.SN, "Article", "Content", true, CardStyleId: cardStyleID);
                ArticleDAO.Copy((long)templateId, newArticleId, cardNo, menuId, siteId, true);
                item = ArticleDAO.GetItem(newArticleId);
                item.VideoType = "youtube";
                ViewBag.FromTemplate = true;
            }

            if (item == null) {
                item = new ArticleModels { ID = WorkLib.GetItem.NewSN(), Type = type, IsIssue = true };
                ViewBag.ArticleTypes = new long[] { };
                ViewBag.ArticleSeries = new long[] { };
                ViewBag.ArticleCategories = new long[] { };
                ViewBag.ArticlePosters = new ArticlePosterModels[] { };                
            } else {
                ViewBag.ArticleTypes = ArticleDAO.GetItemTypes(item.ID);
                ViewBag.ArticleSeries = ArticleDAO.GetItemSeries(item.ID);
                ViewBag.ArticleCategories = ArticleDAO.GetItemCategories(item.ID);

                IEnumerable<ArticlePosterModels> posters = ArticleDAO.GetItemPosters(item.ID);
                foreach(ArticlePosterModels p in posters) {
                    if(!string.IsNullOrWhiteSpace(p.Photo)) {
                        ResourceImagesModels photo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(p.Photo);
                        if (photo.Img != string.Empty)
                            p.Photo = photo.Img;
                    }
                }
                ViewBag.ArticlePosters = posters;
            }
            ViewBag.showMainVideo = false;
            if (item.VideoID != "")
            {
                ViewBag.showMainVideo = true;
            }

            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(long siteId, long menuId, ArticleModels item, long[] types, long[] series, long[] categories, long[] sites, string posters, IEnumerable<ParagraphModels> paragraphs, string deletedParagraphs, bool saveTemplate, 
            HttpPostedFileBase fVideoImg, string fVideoImgBase64, string fVideoImgBase64_Resize, string customVideo, string LinkSites, string[] Pages, string PageDetailID, long[] readModeCategories) {
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.Types = ArticleTypesDAO.GetIssueItems(menuId);
            ViewBag.Series = ArticleSeriesDAO.GetIssueItems(menuId);
            ViewBag.Categories = CategoryDAO.GetItems(FavorityType);
            ViewBag.Sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');

            SaveIconAndArchive(siteId, menuId, item);

            item.MenuID = menuId;

            if (sites?.Length > 0)
                item.IsIssue = true;

            if (item.VideoType == "custom")
                item.VideoID = customVideo;
            #region 影片截圖

            if (!string.IsNullOrWhiteSpace(item.VideoImg) && item.VideoImgIsCustom)
            {
                if (item.VideoImg[0] == '{')
                {
                    WorkV3.Models.ImageModel imgModel = JsonConvert.DeserializeObject<WorkV3.Models.ImageModel>(item.VideoImg);
                    if (imgModel.ID == 0)
                    {
                        if (fVideoImg == null || fVideoImg.ContentLength == 0)
                            item.VideoImg = string.Empty;
                        else
                        {
                            string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fVideoImg, siteId, menuId, fVideoImgBase64, fVideoImgBase64_Resize);
                            imgModel.ID = 1;
                            imgModel.Img = fileName;
                            item.VideoImg = JsonConvert.SerializeObject(imgModel);
                        }
                    }
                }
                else
                {
                    item.VideoImg = string.Empty;
                }
            }

            #endregion

            int cardStyleID = 1;
            List<CardsModels> cardItem = CardsDAO.GetPageData(siteId, menuId);
            foreach (var cards in cardItem)
            {
                if (cards.CardsType == "Article")
                {
                    cardStyleID = cards.StylesID;
                    break;
                }
            }
            if (Request["LinkType"].Contains("true"))
            {

                item.LinkType = ResourceLinkType.inLink;
                if (!string.IsNullOrEmpty(PageDetailID))
                {
                    item.LinkDetail = PageDetailID;
                    var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(PageDetailID));
                    if (pageInfo != null)
                    {
                        var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageInfo.SiteID);
                        item.Link = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = pageInfo.SN });
                    }
                }
                else if (Pages != null)
                {
                    for (int i = 0; i < Pages.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Pages[i]))
                            item.LinkDetail = Pages[i];
                    }
                    if (!string.IsNullOrEmpty(item.LinkDetail))
                    {
                        var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(item.LinkDetail));
                        if (pageInfo != null)
                        {
                            var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageInfo.SiteID);
                            item.Link = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = pageInfo.SN });
                        }
                    }
                }
                item.Link = item.Link;

            }
            else
            {
                item.LinkType = ResourceLinkType.outLink;
            }
            ArticleDAO.SetItem(item, cardStyleID);
            SysLog.SaveArticleEditLog(item.ID);
            ArticleDAO.SetItemTypes(item.ID, types);
            ArticleDAO.SetItemSeries(item.ID, series);
            ArticleDAO.SetItemCategories(item.ID, FavorityType, categories);
            ArticleDAO.SetItemCategories(item.ID, IdentityType, readModeCategories);
            ArticleDAO.SetItemPosters(item.ID, Newtonsoft.Json.JsonConvert.DeserializeObject<long[]>(posters));            
            ArticleDAO.SetItemSites(item.ID, sites);

            SaveParagraph(item, paragraphs, deletedParagraphs);

            ViewBag.ArticleTypes = types ?? (new long[] { });
            ViewBag.ArticleSeries = series ?? (new long[] { });
            ViewBag.ArticlePosters = GetPoster(item, posters);
            ViewBag.ArticleCategories = categories ?? (new long[] { });
            SetLinkDetailViewBag(item.LinkDetail);

            if (saveTemplate) {
                long tmplId = WorkLib.GetItem.NewSN();
                ArticleDAO.Copy(item.ID, tmplId, 0, -menuId, null, true);
                ViewBag.TemplateID = tmplId;
            } else {
                ViewBag.Exit = true;
            }
            return View(item);
        }
        
        private void SaveIconAndArchive(long siteId, long menuId, ArticleModels item) {
            if (!string.IsNullOrWhiteSpace(item.Icon)) {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.Icon);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fIcon"];
                    string postedFileBase64 = Request.Form["fIconBase64"];
                    string postedFileBase64_Resize = Request.Form["fIconBase64_Resize"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Icon = string.Empty;
                    } else {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, postedFileBase64, postedFileBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.Icon = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(item.Archive)) {
                ResourceFilesModels archiveModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceFilesModels>(item.Archive);
                if (archiveModel.Id == 0) {
                    HttpPostedFileBase postedFile = Request.Files["fArchive"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Icon = string.Empty;
                    } else {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId);
                        archiveModel.Id = 1;
                        archiveModel.FileInfo = saveName;
                        archiveModel.FileSize = postedFile.ContentLength;

                        item.Archive = Newtonsoft.Json.JsonConvert.SerializeObject(archiveModel);
                    }
                }
            }
        }

        private static void SaveParagraph(ArticleModels item, IEnumerable<ParagraphModels> paragraphs, string deletedParagraphs) {
            //WorkLib.WriteLog.Write(true, paragraphs.Count().ToString());
            if (paragraphs != null && paragraphs.Count() > 0) {
                for (int i = 0, len = paragraphs.Count(); i < len; ++i) {
                    ParagraphModels p = paragraphs.ElementAt(i);
                    p.Sort = (byte)i;
                    p.SourceNo = item.ID;
                    ParagraphDAO.SetItem(p);
                }
            }

            if (!string.IsNullOrWhiteSpace(deletedParagraphs)) {
                IEnumerable<long> delParagraphIds = deletedParagraphs.Split(',').Select(p => long.Parse(p));
                ParagraphDAO.Delete(delParagraphIds);
            }
        }

        private IEnumerable<ArticlePosterModels> GetPoster(ArticleModels item, string posters) {
            if (posters == null || posters.Count() == 0) {
                return new ArticlePosterModels[] { };
            }

            IEnumerable<ArticlePosterModels> articlePosters = ArticleDAO.GetItemPosters(item.ID);
            foreach (ArticlePosterModels p in articlePosters) {
                if (!string.IsNullOrWhiteSpace(p.Photo)) {
                    ResourceImagesModels photo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(p.Photo);
                    if (photo.Img != string.Empty)
                        p.Photo = photo.Img;
                }
            }
            return articlePosters;
        }        
        #endregion

        [HttpPost]
        public void ArticleSort(long siteId, long menuId, IEnumerable<SortItem> items) {
            ArticleDAO.Sort(menuId, items);
        }

        [HttpPost]
        public void ArticleDel(long siteId, long menuId, IEnumerable<long> ids) {

            foreach (long id in ids)
            {
                SysLog.SaveArticleDelLog(id);
            }
            ArticleDAO.Delete(ids);
        }

        [HttpGet]
        public void ArticleToggleIssue(long siteId, long menuId, long id) {
            ArticleDAO.ToggleIssue(id);
        }
        public void SetLinkDetailViewBag(string Detail)
        {
            List<long> MenuIDs = new List<long>();
            Dictionary<int, WorkV3.Models.MenusModels> MenuList = new Dictionary<int, WorkV3.Models.MenusModels>();

            int menuLev = 0;
            if (!string.IsNullOrEmpty(Detail))
            {
                var menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(long.Parse(Detail));
                if (menuInfo != null && !string.IsNullOrEmpty(menuInfo.Title))
                {
                    ViewBag.DefaultSiteID = menuInfo.SiteID;
                    menuLev++;
                    MenuList.Add(menuLev, menuInfo);
                    while (menuInfo.ParentID != 0)
                    {
                        menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(menuInfo.ParentID);
                        if (menuInfo != null && !string.IsNullOrEmpty(menuInfo.Title))
                        {
                            menuLev++;
                            MenuList.Add(menuLev, menuInfo);
                        }
                        else
                            break;
                    }
                }
                else
                {
                    var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(Detail));
                    if (pageInfo != null && !string.IsNullOrEmpty(pageInfo.Title))
                    {
                        ViewBag.DefaultSiteID = pageInfo.SiteID;
                        ViewBag.DefaultPageTitle = pageInfo.Title;
                        var page_menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(pageInfo.MenuID);
                        if (page_menuInfo != null)
                        {
                            menuLev++;
                            MenuList.Add(menuLev, page_menuInfo);
                            while (page_menuInfo.ParentID != 0)
                            {
                                page_menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(page_menuInfo.ParentID);
                                if (page_menuInfo != null && !string.IsNullOrEmpty(page_menuInfo.Title))
                                {
                                    menuLev++;
                                    MenuList.Add(menuLev, page_menuInfo);
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
                if (MenuList != null && MenuList.Count > 0)
                {
                    var orderMenus = MenuList.OrderByDescending(p => p.Key).Select(p => p.Value.Id);
                    MenuIDs = orderMenus.ToList();
                }
                ViewBag.DefaultMenus = MenuIDs;
            }
        }
        public ActionResult Para(long siteId, long menuId, string type) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.Type = type ?? "series";
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            IEnumerable<ArticleSeriesModels> series = ArticleSeriesDAO.GetItems(menuId);
            if (series == null)
                series = new List<ArticleSeriesModels>();
            ViewBag.Series = series;

            IEnumerable<ArticleTypesModels> types = ArticleTypesDAO.GetItems(menuId);
            if (types == null)
                types = new List<ArticleTypesModels>();
            ViewBag.Types = types;

            IEnumerable<ArticlePosterModels> posters = ArticlePosterDAO.GetItems(menuId);
            if (posters == null)
                posters = new List<ArticlePosterModels>();
            ViewBag.Posters = posters;

            return View();
        }
        
        [HttpGet]
        public ActionResult TypeEdit(long siteId, long menuId, long? id) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            ArticleTypesModels type = null;
            if (id != null)
                type = ArticleTypesDAO.GetItem((long)id);
            if(type == null) 
                type = new ArticleTypesModels { ID = WorkLib.GetItem.NewSN(), IsIssue = true, Sort = int.MaxValue };
                        
            return View(type);
        }

        [HttpPost]
        public ActionResult TypeEdit(long siteId, long menuId, ArticleTypesModels item, HttpPostedFileBase fIcon, string fIconBase64, string fIconBase64_Resize)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            if (string.IsNullOrEmpty(item.Icon))
            {
                item.Icon = string.Empty;
            }
            else
            {
                WorkV3.Models.ImageModel imgModel = JsonConvert.DeserializeObject<WorkV3.Models.ImageModel>(item.Icon);
                if (imgModel.ID == 0)
                {
                    if (fIcon == null || fIcon.ContentLength == 0)
                        item.Icon = string.Empty;
                    else
                    {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fIcon, siteId, menuId, fIconBase64, fIconBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        item.Icon = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }
            ArticleTypesDAO.SetItem(item);

            return View(item);
        }

        [HttpPost]
        public void TypeSort(long siteId, long menuId, IEnumerable<SortItem> items) {
            ArticleTypesDAO.Sort(menuId, items);
        }

        [HttpPost]
        public void TypeDel(long siteId, long menuId, IEnumerable<long> ids) {
            ArticleTypesDAO.Delete(ids);
        }

        [HttpGet]
        public void TypeToggleIssue(long siteId, long menuId, long id) {
            ArticleTypesDAO.ToggleIssue(id);
        }

        [HttpGet]
        public ActionResult SeriesEdit(long siteId, long menuId, long? id)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            ArticleSeriesModels Series = null;
            if (id != null)
                Series = ArticleSeriesDAO.GetItem((long)id);
            if (Series == null)
                Series = new ArticleSeriesModels { ID = WorkLib.GetItem.NewSN(), IsIssue = true, Sort = int.MaxValue };

            return View(Series);
        }

        [HttpPost]
        public ActionResult SeriesEdit(long siteId, long menuId, ArticleSeriesModels item, HttpPostedFileBase fIcon, string fIconBase64, string fIconBase64_Resize)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadVPath = UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;
            if (string.IsNullOrEmpty(item.Icon))
            {
                item.Icon = string.Empty;
            }
            else
            {
                WorkV3.Models.ImageModel imgModel = JsonConvert.DeserializeObject<WorkV3.Models.ImageModel>(item.Icon);
                if (imgModel.ID == 0)
                {
                    if (fIcon == null || fIcon.ContentLength == 0)
                        item.Icon = string.Empty;
                    else
                    {
                        string fileName = Golbal.UpdFileInfo.SaveFilesByMenuID(fIcon, siteId, menuId, fIconBase64, fIconBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = fileName;
                        item.Icon = JsonConvert.SerializeObject(imgModel);
                    }
                }
            }
            ArticleSeriesDAO.SetItem(item);

            return View(item);
        }

        [HttpPost]
        public void SeriesSort(long siteId, long menuId, IEnumerable<SortItem> items)
        {
            ArticleSeriesDAO.Sort(menuId, items);
        }

        [HttpPost]
        public void SeriesDel(long siteId, long menuId, IEnumerable<long> ids)
        {
            ArticleSeriesDAO.Delete(ids);
        }

        [HttpGet]
        public void SeriesToggleIssue(long siteId, long menuId, long id)
        {
            ArticleSeriesDAO.ToggleIssue(id);
        }
        [HttpGet]
        public ActionResult PosterEdit(long siteId, long menuId, long? id) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);

            ArticlePosterModels type = null;
            if (id != null)
                type = ArticlePosterDAO.GetItem((long)id);
            if (type == null)
                type = new ArticlePosterModels { ID = WorkLib.GetItem.NewSN(), IsIssue = true, Sort = int.MaxValue };

            return View(type);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PosterEdit(long siteId, long menuId, ArticlePosterModels item)
        {
            if (!Utility.CheckIsLogin())
            {
                Response.Redirect(Url.Action("Login", "Home"));
            }
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId);
            ViewBag.Exit = true;

            if (!string.IsNullOrWhiteSpace(item.Photo)) {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.Photo);
                if (imgModel.ID == 0) { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fPhoto"];
                    string fPhotoBase64 = Request.Form["fPhotoBase64"];
                    string fPhotoBase64_Resize = Request.Form["fPhotoBase64_Resize"];
                    if (postedFile == null || postedFile.ContentLength == 0) {
                        item.Photo = string.Empty;
                    } else {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, fPhotoBase64, fPhotoBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.Photo = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            ArticlePosterDAO.SetItem(item);

            return View(item);
        }

        [HttpPost]
        public void PosterSort(long siteId, long menuId, IEnumerable<SortItem> items) {
            ArticlePosterDAO.Sort(menuId, items);
        }

        [HttpPost]
        public void PosterDel(long siteId, long menuId, IEnumerable<long> ids) {
            ArticlePosterDAO.Delete(ids);
        }

        [HttpGet]
        public void PosterToggleIssue(long siteId, long menuId, long id) {
            ArticlePosterDAO.ToggleIssue(id);
        }

        [HttpGet]
        public ActionResult PosterSelect(long siteId, long menuId, long articleId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            IEnumerable<ArticlePosterModels> items = ArticlePosterDAO.GetIssueItems(menuId);
            string uploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            foreach (ArticlePosterModels p in items) {
                if (!string.IsNullOrWhiteSpace(p.Photo)) {
                    ResourceImagesModels photo = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(p.Photo);
                    if (photo.Img != string.Empty)
                        p.Photo = uploadUrl + photo.Img;
                }
            }

            IEnumerable<long> curPosterIds = ArticleDAO.GetItemPosters(articleId).Select(p => p.ID);
            if (curPosterIds == null)
                curPosterIds = new List<long>();
            ViewBag.Posters = curPosterIds;

            ViewBag.Int64Converter = new WorkV3.Golbal.Int64Converter();
            return View(items);
        }

        public string PosterQuickAdd(long siteId, long menuId, string name) {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            ArticlePosterModels poster = new ArticlePosterModels {
                ID = WorkLib.GetItem.NewSN(),
                MenuID = menuId,
                Name = name,
                Photo = string.Empty,
                Intro = string.Empty,
                Resume = string.Empty,
                IsIssue = true,
                Sort = int.MaxValue
            };

            ArticlePosterDAO.SetItem(poster);
            return poster.ID.ToString();
        }

        [HttpGet]
        public ActionResult Setting(long siteId, long menuId) {
            ViewBag.Menu = MenusDAO.GetInfo(siteId, menuId);

            IEnumerable<ArticleTypesModels> types = ArticleTypesDAO.GetIssueItems(menuId); 
            ViewBag.Types = types;

            //ViewBag.ListCards = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article").Where(c => c.ID != menuId);
            ViewBag.ListCards = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article"); // shan 20180102 依 CC 要求修改為本單元亦列入, 且預設勾選
            ViewBag.ListCards2 = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article"); // shan 20180102 依 CC 要求修改為本單元亦列入, 且預設勾選
            ViewBag.ListIdentity = CategoryDAO.GetIssueItems(IdentityType);

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";

            ViewBag.MemberRegSet = MemberShipRegSetDAO.GetItem(siteId);

            List<WorkV3.ViewModels.CommentType> ReplyItemList = WorkV3.ViewModels.CommentTypeLibs.GetCommitTypeList();
            ViewBag.ReplyItemList = ReplyItemList;

            WorkV3.Models.ArticleSettingModels item = WorkV3.Models.DataAccess.ArticleSettingDAO.GetItem(menuId);
            if (item.Types == "all")
                item.Types = string.Join(",", types.Select(t => t.ID));

            return View(item);
        }
        [HttpPost]
        public ActionResult Setting(long siteId, WorkV3.Models.ArticleSettingModels item, string[] ExtendReadMenus, string[] ExtendReadMenus2, string[] ReadModeSet)
        {
            long menuId = item.MenuID;
            ViewBag.Menu = MenusDAO.GetInfo(siteId, menuId);
            ViewBag.Types = ArticleTypesDAO.GetIssueItems(menuId);
            //ViewBag.ListCards = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article").Where(c => c.ID != menuId);
            ViewBag.ListCards = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article"); // shan 20180102 依 CC 要求修改為本單元亦列入, 且預設勾選
            ViewBag.ListCards2 = WorkV3.Models.DataAccess.MenusDAO.GetListCards("Article"); // shan 20180102 依 CC 要求修改為本單元亦列入, 且預設勾選
            ViewBag.ListIdentity = CategoryDAO.GetIssueItems(IdentityType);

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/');

            ViewBag.MemberRegSet = MemberShipRegSetDAO.GetItem(siteId);

            List<WorkV3.ViewModels.CommentType> ReplyItemList = WorkV3.ViewModels.CommentTypeLibs.GetCommitTypeList();
            ViewBag.ReplyItemList = ReplyItemList;

            if (item.SortMode == "隨機排序")
                item.SortField = "NewID()";
            if (ExtendReadMenus != null)
            {
                string eMenus = "";
                foreach (string ExtendReadMenu in ExtendReadMenus)
                {
                    if(!string.IsNullOrEmpty(ExtendReadMenu))
                        eMenus += ExtendReadMenu + ",";
                }
                eMenus = eMenus.Trim(',');
                item.ExtendReadMenus = eMenus;
            }
            else
                item.ExtendReadMenus = string.Empty;
            if (ExtendReadMenus2 != null)
            {
                string eMenus = "";
                foreach (string ExtendReadMenu in ExtendReadMenus2)
                {
                    if (!string.IsNullOrEmpty(ExtendReadMenu))
                        eMenus += ExtendReadMenu + ",";
                }
                eMenus = eMenus.Trim(',');
                item.ExtendReadMenus2 = eMenus;
            }
            else
                item.ExtendReadMenus2 = string.Empty;

            if (ReadModeSet != null)
            {
                string readModeSetStr = "";
                foreach (string readModeSet in ReadModeSet)
                {
                    if (!string.IsNullOrEmpty(readModeSet))
                        readModeSetStr += readModeSet + ",";
                }
                readModeSetStr = readModeSetStr.Trim(',');
                item.ReadModeSet = readModeSetStr;
            }
            else
                item.ReadModeSet = string.Empty;

            if (!string.IsNullOrWhiteSpace(item.DefaultImg))
            {
                ResourceImagesModels imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceImagesModels>(item.DefaultImg);
                if (imgModel.ID == 0)
                { // 新上傳的圖片
                    HttpPostedFileBase postedFile = Request.Files["fDefaultImg"];
                    string fDefaultImgBase64 = Request.Form["fDefaultImgBase64"];
                    string fDefaultImgBase64_Resize = Request.Form["fDefaultImgBase64_Resize"];
                    if (postedFile == null || postedFile.ContentLength == 0)
                    {
                        item.DefaultImg = string.Empty;
                    }
                    else
                    {
                        string saveName = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(postedFile, siteId, menuId, fDefaultImgBase64, fDefaultImgBase64_Resize);
                        imgModel.ID = 1;
                        imgModel.Img = saveName;

                        item.DefaultImg = Newtonsoft.Json.JsonConvert.SerializeObject(imgModel);
                    }
                }
            }

            WorkV3.Models.DataAccess.ArticleSettingDAO.SetItem(item);

            ViewBag.Success = true;
            return View(item);
        }

        public ActionResult Template(long siteId, long menuId, long templateId, long sourceId) {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.TemplateID = templateId;

            string uploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathByMenuID(siteId, menuId).TrimEnd('/') + "/";
            
            WorkV3.Models.TemplateModels template = ArticleDAO.GetTemplate(templateId);
            if (template == null || string.IsNullOrWhiteSpace(template.Thumb)) {
                WorkV3.Models.ArticleModels article = WorkV3.Models.DataAccess.ArticleDAO.GetItem(sourceId);
                WorkV3.Common.SitePage page = WorkV3.Models.DataAccess.CardsDAO.GetPage(article.CardNo);

                string url = Request.Url.ToString();
                url = System.Text.RegularExpressions.Regex.Match(url, @"^https?://[^/]+/").Value.TrimEnd('/');
                url += Url.Action("Index", "Home", new { SiteSN = page.SiteSN, PageSN = page.PageSN, area = string.Empty });
                //WorkV3.Common.PageToImage imgConverter = new WorkV3.Common.PageToImage(url);

                string uploadPath = WorkV3.Golbal.UpdFileInfo.GetUPathByMenuID(siteId, menuId).TrimEnd('\\') + "\\";
                string thumb = WorkLib.GetItem.NewSN().ToString() + ".jpg";
                //imgConverter.SaveImage(uploadPath + thumb);
                WorkV3.Models.DataAccess.SiteCoverRecreateSchedule snapCoverItem = new WorkV3.Models.DataAccess.SiteCoverRecreateSchedule(Request.ApplicationPath.Trim('/'));
                snapCoverItem.SnapCoverNow(url, uploadPath + thumb);
                string name = "自訂範本" + DateTime.Now.ToString("yyyyMMdd");
                ArticleDAO.SetTemplateName(templateId, name, thumb);
                                
                template = new WorkV3.Models.TemplateModels { ID = templateId, Name = name, Thumb = uploadUrl + thumb };
            } else {
                if (!string.IsNullOrWhiteSpace(template.Thumb))
                    template.Thumb = uploadUrl + template.Thumb;
            }

            return View(template);
        }

        public void TemplateDel(long id) {
            ArticleDAO.Delete(id);
        }

        public void TemplateNameSet(long id, string name) {
            ArticleDAO.SetTemplateName(id, name);
        }

        #region Copy, Move, SelectNodes
        public ActionResult SelectNodes(long siteId, long menuId, string act)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.ActionType = act;
            return View();
        }
        [HttpPost]
        public void Copy(long siteId, long menuId, long[] ArticleIds, long TargetMenuID)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.ActionType = "copy";
            if (ArticleIds.Length == 0)
                return;

            var targetMenu = MenusDAO.GetInfo(TargetMenuID);
            foreach (long ArticleId in ArticleIds)
            {
                ArticleModels articleItem = ArticleDAO.GetItem(ArticleId);
                CardsModels cardItem = CardsDAO.GetByNo(articleItem.CardNo);
                ZonesModels zoneItem = ZonesDAO.GetByNo(cardItem.ZoneNo.Value);
                PagesModels pageItem = PagesDAO.GetPageInfo(zoneItem.PageNo);

                long newCardNo = WorkV3.Golbal.PubFunc.CopyPage(pageItem, targetMenu.SiteID, targetMenu.ID);
                long newArticleId = WorkLib.GetItem.NewSN();
                bool IsForceRelative = false;
                if (pageItem.SiteID == targetMenu.SiteID && pageItem.MenuID == targetMenu.ID)
                    IsForceRelative = true;
                ArticleDAO.CopyArticle(ArticleId, newArticleId, newCardNo, targetMenu.ID, targetMenu.SiteID, IsForceRelative);
                WorkV3.Golbal.PubFunc.CopyIcon(articleItem.Icon, siteId, menuId, targetMenu.SiteID, targetMenu.ID);
                WorkV3.Golbal.PubFunc.CopyParagraphPhotos(articleItem.ID, siteId, menuId, targetMenu.SiteID, targetMenu.ID);
            }
        }
        [HttpPost]
        public void Move(long siteId, long menuId, long[] ArticleIds, long TargetMenuID)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.ActionType = "move";
            if (ArticleIds.Length == 0)
                return;
            foreach (long ArticleId in ArticleIds)
            {
                ArticleModels articleItem = ArticleDAO.GetItem(ArticleId);
                CardsModels cardItem = CardsDAO.GetByNo(articleItem.CardNo);
                ZonesModels zoneItem = ZonesDAO.GetByNo(cardItem.ZoneNo.Value);
                PagesModels pageItem = PagesDAO.GetPageInfo(zoneItem.PageNo);

                var targetMenu = MenusDAO.GetInfo(TargetMenuID);
                WorkV3.Golbal.PubFunc.MovePage(pageItem, targetMenu.SiteID, targetMenu.ID);
                ArticleDAO.Move(ArticleId, targetMenu.ID, targetMenu.SiteID);
                WorkV3.Golbal.PubFunc.CopyIcon(articleItem.Icon, siteId, menuId, targetMenu.SiteID, targetMenu.ID);
                WorkV3.Golbal.PubFunc.CopyParagraphPhotos(articleItem.ID, siteId, menuId, targetMenu.SiteID, targetMenu.ID);
            }
        }
        #endregion


        [HttpPost]
        public string VideoUpload(long siteId, long menuId, string Base64 = "", string Base64_Resize = "")
        {
            if (Request.Files == null || Request.Files.Count == 0)
                return null;

            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength == 0)
                return null;

            // System.IO.File.AppendAllText("D:\\Temp\\TaiSound.txt", DateTime.Now.ToString() +" "+Base64);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Name = WorkV3.Golbal.UpdFileInfo.SaveFilesByMenuID(file, siteId, menuId, Base64, Base64_Resize),
                FileSize = file.ContentLength.ToString(), ShowName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) });
        }
    }
}