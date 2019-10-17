using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkLib;
using System.IO;
using Newtonsoft.Json;
using WorkV3.Golbal;
using WorkV3.Models.Interface;

namespace WorkV3.Controllers
{
    public class HomeController : Controller
    {
        private IZoneRepository<ZonesModels> zoneRepository;
        private string DefaultPageSN = "Index";

        public ActionResult Index(string SiteSN, string PageSN, long? id)
        {
            bool PageisExist = false;
            byte ShowStatus = 3;
            SitesModels mSites = SitesDAO.GetSiteInfo(SiteSN);
            PageCache pageCache = new PageCache();
            if (mSites != null)
            {
                if (mSites.IsDelete || !mSites.Issue)
                {
                    var member = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent;
                    if(member == null)
                        return HttpNotFound();
                }
                ViewBag.SiteID = mSites.Id;
                Areas.Backend.Models.SiteSeoSettingModels siteSeoSetting =  Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(mSites.Id);
                if (siteSeoSetting != null)
                {
                    if (!siteSeoSetting.Robot)
                        ViewBag.NoRobot = true;
                    ViewBag.GoogleSearchCode = siteSeoSetting.GoogleSearch;
                    ViewBag.BaidoSearchCode = siteSeoSetting.BaiduMA;
                    ViewBag.BingSearchCode = siteSeoSetting.Bing;
                    if (!string.IsNullOrEmpty(siteSeoSetting.GA))
                    {
                        ViewBag.GACode = "<script async src=\"https://www.googletagmanager.com/gtag/js?id="+ siteSeoSetting.GA + "\"></script><script>window.dataLayer = window.dataLayer || [];function gtag(){dataLayer.push(arguments);}gtag('js', new Date());gtag('config', '"+ siteSeoSetting.GA + "');</script>";
                    }
                    if (!string.IsNullOrEmpty(siteSeoSetting.Baidu))
                    {
                        ViewBag.Baidu = @"<script>
                                                        var _hmt = _hmt || [];
                                                        (function() {
                                                            var hm = document.createElement('script');
                                                            hm.src = 'https://hm.baidu.com/hm.js?"+ siteSeoSetting.Baidu + @"';
                                                            var s = document.getElementsByTagName('script')[0];
                                                            s.parentNode.insertBefore(hm, s);
                                                            })();
                                                        </script> ";
                    }
                    if (!string.IsNullOrEmpty(siteSeoSetting.GTM))
                    {
                        ViewBag.GTM_Head = @"<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
                                                        new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
                                                        j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
                                                        'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
                                                        })(window,document,'script','dataLayer','"+ siteSeoSetting.GTM + @"');</script>
                                                        <!-- End Google Tag Manager -->";
                        ViewBag.GTM_Body = "<noscript><iframe src=\"https://www.googletagmanager.com/ns.html?id="+ siteSeoSetting.GTM + "\" height=\"0\" width=\"0\" style = \"display:none;visibility:hidden\"></iframe></noscript>";
                    }
                    if (siteSeoSetting.IsExtraCode)
                    {
                        ViewBag.ExtraCode = siteSeoSetting.ExtraCode;
                    }
                }
                pageCache.SiteID = mSites.Id;
                pageCache.SiteName = mSites.Title;
                pageCache.SiteSN = mSites.SN;
                PagesModels mPages = PagesDAO.GetPageInfo(mSites.Id, PageSN);
                if (mPages != null)
                {
                    pageCache.SiteID = mPages.SiteID;
                    pageCache.MenuID = mPages.MenuID;
                    pageCache.PageNO = mPages.No;
                    pageCache.PageSN = mPages.SN;
                    pageCache.SiteName = mSites.Title;
                    pageCache.SiteSN = mSites.SN;
                    MenusModels mMenus = MenusDAO.GetInfo(mPages.SiteID, mPages.MenuID);

                    if (mMenus != null)
                    {
                        ShowStatus = mMenus.ShowStatus;
                    }
                    if (ShowStatus != 3)
                    {
                        ShowStatus = mPages.ShowStatus;
                    }

                    PageisExist = true;
                    ViewBag.PageNo = mPages.No;
                    ViewBag.Title = mPages.Title;
                    ViewBag.MenuID = mPages.MenuID;
                    ViewBag.SiteName = mSites.Title;
                    ViewBag.FaviconPath = $"{WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(mSites.Id, "Header")}/{mSites.Favicon}";
                    #region 網頁SEO

                    if (mPages != null)
                    {
                        string imgUrl = "", description = "", author = "",Title="";

                        SEOModels pageSEO = null;
                        if (PageSN == "RecruitDetail") //20190516 Nina 徵才需用ID取得SEO
                        {
                            long Id = 0;
                            bool result = long.TryParse(Request.QueryString["ID"], out Id);
                            if (result)
                                pageSEO = PagesDAO.GetPageSEO(mPages.SiteID, mPages.No, mPages.Title, Id);
                        }
                        else
                            pageSEO = PagesDAO.GetPageSEO(mPages.SiteID, mPages.No, mPages.Title);

                        var siteSEO = Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(mPages.SiteID);
                        var contentSEO = PagesDAO.GetContentSEO(mPages.SiteID, mPages.MenuID, mPages.No);
                        string typeKeywords = contentSEO.Keywords;//PagesDAO.GetContentTypeKeyword(mPages.SiteID, mPages.MenuID, mPages.No);

                        if (!string.IsNullOrEmpty(mPages.Title))
                            ViewBag.SEO_Title = mPages.Title; // 以單頁的先給值
                        //先設定頁面的 SEO
                        //若PAGE SEO 存在, 則取代碼 => PAGE > SITE
                        if (pageSEO != null)
                        {
                            string UploadUrl = UpdFileInfo.GetVPathByMenuID(mPages.SiteID, mPages.MenuID).TrimEnd('/') + "/";
                            if (!string.IsNullOrEmpty(pageSEO.Title))
                                ViewBag.SEO_Title = pageSEO.Title;
                            if (!string.IsNullOrEmpty(pageSEO.Description))
                                ViewBag.SEO_Description = pageSEO.Description;
                            if (!string.IsNullOrEmpty(pageSEO.Author))
                                author = pageSEO.Author;
                            if (!string.IsNullOrEmpty(pageSEO.Copyright))
                                ViewBag.SEO_Copyright = pageSEO.Copyright;
                            if (!string.IsNullOrEmpty(pageSEO.Keywords))
                                ViewBag.SEO_Keywords = pageSEO.Keywords;
                            if (!string.IsNullOrEmpty(pageSEO.Image))
                                ViewBag.SEO_Image = UploadUrl + pageSEO.Image;
                            description = ViewBag.SEO_Description;
                            Title = ViewBag.SEO_Title;
                        }
                        if (contentSEO != null)
                        {
                            if (!string.IsNullOrEmpty(contentSEO.SocialImage))
                            {
                                ViewBag.SEO_Image = contentSEO.SocialImage;
                            }
                            if (string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(contentSEO.Description))
                            {
                                ViewBag.SEO_Description = contentSEO.Description;
                            }
                            if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(contentSEO.Title))
                            {
                                ViewBag.SEO_Title = contentSEO.Title;
                            }
                            if (contentSEO != null && !string.IsNullOrEmpty(contentSEO.Author))
                            {
                                author = author.Trim(';') + ";" + contentSEO.Author;
                            }
                        }
                        imgUrl = ViewBag.SEO_Image;
                        ViewBag.SEO_Keywords = typeKeywords;
                        //以SITE SEO 先給值, 若PAGE SEO 存在, 則取代碼 => PAGE > SITE
                        if (siteSEO != null)
                        {
                            if (!string.IsNullOrEmpty(siteSEO.Title) && string.IsNullOrEmpty(ViewBag.SEO_Title))
                                ViewBag.SEO_Title = siteSEO.Title;
                            if (!string.IsNullOrEmpty(siteSEO.Description) && string.IsNullOrEmpty(ViewBag.SEO_Description))
                                ViewBag.SEO_Description = siteSEO.Description;
                            if (!string.IsNullOrEmpty(siteSEO.Author))
                                author = author.Trim(';') + ";" +siteSEO.Author;
                            if (!string.IsNullOrEmpty(siteSEO.Copyright) && string.IsNullOrEmpty(ViewBag.SEO_Copyright))
                                ViewBag.SEO_Copyright = siteSEO.Copyright;
                            if (!string.IsNullOrEmpty(siteSEO.Keywords))
                                ViewBag.SEO_Keywords += (string.IsNullOrEmpty(ViewBag.SEO_Keywords) ? "" : ";") + siteSEO.Keywords;
                            WorkV3.Areas.Backend.Models.SocialSettingModels siteSocialSetting = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(mPages.SiteID);
                            if (siteSocialSetting != null && !string.IsNullOrEmpty(siteSocialSetting.SocialDefaultImage))
                            {
                                if (string.IsNullOrEmpty(imgUrl) && !string.IsNullOrEmpty(siteSocialSetting.SocialDefaultImage))
                                {
                                    string customImageFolder = "SocialImage";
                                    WorkV3.Models.ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkV3.Models.ImageModel>(siteSocialSetting.SocialDefaultImage);
                                    string UploadUrl = UpdFileInfo.GetVPathBySiteID(mPages.SiteID, customImageFolder).TrimEnd('/') + "/";
                                    ViewBag.SEO_Image = UploadUrl + imgModel.Img;
                                }
                            }
                        }
                        ViewBag.SEO_Author = author.Trim(';');
                    }
                    #endregion
                }

                #region GA
                List<SitesModels.GAInfoCont> GA = new List<SitesModels.GAInfoCont>();
                if (!string.IsNullOrEmpty(mSites.GAInfo))
                {
                    GA = JsonConvert.DeserializeObject<List<SitesModels.GAInfoCont>>(mSites.GAInfo);
                }
                ViewBag.GA = GA;
                #endregion
            }
            ViewBag.SiteSN = SiteSN;
            ViewBag.PageSN = PageSN;
            ViewBag.Id = id;
            PageCache.SetTempDataPageCache(TempData, pageCache);
            ViewBag.CustomCont = LoadCustomCont(pageCache);
            if (PageisExist == true)
                if (ShowStatus == 3)
                {
                    Response.Redirect(Url.Action("PageNotFound", "ErrorPages"));
                }
                else
                {
                    #region 網頁點擊LOG
                    long? memberID = null;

                    WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
                    if (curUser != null)
                    {
                        memberID = curUser.ID;
                    }
                    string referUrl = "", referUrlTitle = "", referrerUrlPageNo = "";
                    if (Request.UrlReferrer != null)
                    {
                        referUrl = Request.UrlReferrer.AbsoluteUri;
                        if (referUrl.StartsWith(WorkLib.uUrl.GetURL()))
                        {
                            if (referUrl.Split('/').Length > 0)
                            {
                                string pageSN = referUrl.Split('/')[referUrl.Split('/').Length -1];
                                Models.PagesModels pageInfo =  Models.DataAccess.PagesDAO.GetPageInfo(pageCache.SiteID, pageSN);
                                if (pageInfo != null)
                                {
                                    referUrlTitle = pageInfo.Title;
                                    referrerUrlPageNo = pageInfo.No.ToString();
                                }
                                else
                                {
                                    Models.MenusModels menuInfo = Models.DataAccess.MenusDAO.GetInfo(pageCache.SiteID, pageSN);
                                    if (menuInfo != null)
                                    {
                                        referUrlTitle = menuInfo.Title;
                                        referrerUrlPageNo = menuInfo.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    AddPageLog((long)pageCache.PageNO, mSites.Lang, 3, mSites.Id, getSessionID(), memberID, referUrl, referUrlTitle, referrerUrlPageNo);
                    #endregion
                    return View();
                }

            else
            {
                //客製start
                string DefaultWebSite = Request.Url.AbsoluteUri;

                if(DefaultWebSite.Contains("webdemo") || DefaultWebSite.Contains("localhost"))
                {
                    string DefaultSiteSN = GetItem.appSet("DefaultSiteSN").ToString();
                    if (DefaultSiteSN != null)
                    {
                        Response.Redirect("~/w/" + DefaultSiteSN + "/index");
                    }
                }
                else
                {
                    DefaultWebSite = DefaultWebSite.Replace("www.", "").Replace("http://", "").Replace(".com.tw/", "");
                    if (DefaultWebSite == "nhmc")
                    {
                        DefaultWebSite = "NewHopeMarketing";
                    }
                    else if (DefaultWebSite == "edec")
                    {
                        DefaultWebSite = "ELITEDREAM";
                    }
                    Response.Redirect("~/w/" + DefaultWebSite + "/index");
                }
                //客製end
            }

            return View("EmptyPage");

        }
        public ActionResult IndexWithoutLayout(string SiteSN, string PageSN, long? id,string lay="")
        {
            PageCache pageCache = new PageCache();
            bool PageisExist = false;
            byte ShowStatus = 3;
            SitesModels mSites = SitesDAO.GetSiteInfo(SiteSN);
            if (mSites != null)
            {
                ViewBag.SiteID = mSites.Id;
                PagesModels mPages = PagesDAO.GetPageInfo(mSites.Id, PageSN);
                if (mPages != null)
                {
                    pageCache.SiteID = mPages.SiteID;
                    pageCache.MenuID = mPages.MenuID;
                    pageCache.PageNO = mPages.No;
                    pageCache.PageSN = mPages.SN;
                    pageCache.SiteName = mSites.Title;
                    pageCache.SiteSN = mSites.SN;
                    MenusModels mMenus = MenusDAO.GetInfo(mPages.SiteID, mPages.MenuID);

                    if (mMenus != null)
                    {
                        ShowStatus = mMenus.ShowStatus;
                    }
                    if (ShowStatus != 3)
                    {
                        ShowStatus = mPages.ShowStatus;
                    }

                    PageisExist = true;
                    ViewBag.PageNo = mPages.No;
                    ViewBag.Title = mPages.Title;
                    ViewBag.SiteName = mSites.Title;
                    #region 網頁SEO wei 20180914 對集點

                    if (mPages != null)
                    {
                        string imgUrl = "", description = "", author = "",Title ="";
                        var pageSEO = PagesDAO.GetPageSEO(mPages.SiteID, mPages.No, mPages.Title);
                        var siteSEO = Areas.Backend.Models.DataAccess.SiteSeoSettingDAO.GetItem(mPages.SiteID);
                        var contentSEO = PagesDAO.GetContentSEO(mPages.SiteID, mPages.MenuID, mPages.No);
                        string typeKeywords = contentSEO.Keywords;//PagesDAO.GetContentTypeKeyword(mPages.SiteID, mPages.MenuID, mPages.No);

                        if (!string.IsNullOrEmpty(mPages.Title))
                            ViewBag.SEO_Title = mPages.Title; // 以單頁的先給值
                        //先設定頁面的 SEO
                        //若PAGE SEO 存在, 則取代碼 => PAGE > SITE
                        if (pageSEO != null)
                        {
                            string UploadUrl = UpdFileInfo.GetVPathByMenuID(mPages.SiteID, mPages.MenuID).TrimEnd('/') + "/";
                            if (!string.IsNullOrEmpty(pageSEO.Title))
                                ViewBag.SEO_Title = pageSEO.Title;
                            if (!string.IsNullOrEmpty(pageSEO.Description))
                                ViewBag.SEO_Description = pageSEO.Description;
                            if (!string.IsNullOrEmpty(pageSEO.Author))
                                author = pageSEO.Author;
                            if (!string.IsNullOrEmpty(pageSEO.Copyright))
                                ViewBag.SEO_Copyright = pageSEO.Copyright;
                            if (!string.IsNullOrEmpty(pageSEO.Keywords))
                                ViewBag.SEO_Keywords = pageSEO.Keywords;
                            if (!string.IsNullOrEmpty(pageSEO.Image))
                                ViewBag.SEO_Image = UploadUrl + pageSEO.Image;
                            description = ViewBag.SEO_Description;
                            Title = ViewBag.SEO_Title;
                        }
                        if (contentSEO != null)
                        {
                            if (!string.IsNullOrEmpty(contentSEO.SocialImage))
                            {
                                ViewBag.SEO_Image = contentSEO.SocialImage;
                            }
                            if (string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(contentSEO.Description))
                            {
                                ViewBag.SEO_Description = contentSEO.Description;
                            }
                            if(string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(contentSEO.Title))
                            {
                                ViewBag.SEO_Title = contentSEO.Title;
                            }
                            if (contentSEO != null && !string.IsNullOrEmpty(contentSEO.Author))
                            {
                                author = author.Trim(';') + ";" + contentSEO.Author;
                            }
                        }
                        imgUrl = ViewBag.SEO_Image;
                        ViewBag.SEO_Keywords = typeKeywords;
                        //以SITE SEO 先給值, 若PAGE SEO 存在, 則取代碼 => PAGE > SITE
                        if (siteSEO != null)
                        {
                            if (!string.IsNullOrEmpty(siteSEO.Title) && string.IsNullOrEmpty(ViewBag.SEO_Title))
                                ViewBag.SEO_Title = siteSEO.Title;
                            if (!string.IsNullOrEmpty(siteSEO.Description) && string.IsNullOrEmpty(ViewBag.SEO_Description))
                                ViewBag.SEO_Description = siteSEO.Description;
                            if (!string.IsNullOrEmpty(siteSEO.Author))
                                author = author.Trim(';') + ";" + siteSEO.Author;
                            if (!string.IsNullOrEmpty(siteSEO.Copyright) && string.IsNullOrEmpty(ViewBag.SEO_Copyright))
                                ViewBag.SEO_Copyright = siteSEO.Copyright;
                            if (!string.IsNullOrEmpty(siteSEO.Keywords))
                                ViewBag.SEO_Keywords += (string.IsNullOrEmpty(ViewBag.SEO_Keywords) ? "" : ";") + siteSEO.Keywords;
                            WorkV3.Areas.Backend.Models.SocialSettingModels siteSocialSetting = WorkV3.Areas.Backend.Models.DataAccess.SocialSettingDAO.GetItem(mPages.SiteID);
                            if (siteSocialSetting != null && !string.IsNullOrEmpty(siteSocialSetting.SocialDefaultImage))
                            {
                                if (string.IsNullOrEmpty(imgUrl) && !string.IsNullOrEmpty(siteSocialSetting.SocialDefaultImage))
                                {
                                    string customImageFolder = "SocialImage";
                                    WorkV3.Models.ImageModel imgModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkV3.Models.ImageModel>(siteSocialSetting.SocialDefaultImage);
                                    string UploadUrl = UpdFileInfo.GetVPathBySiteID(mPages.SiteID, customImageFolder).TrimEnd('/') + "/";
                                    ViewBag.SEO_Image = UploadUrl + imgModel.Img;
                                }
                            }
                        }
                        ViewBag.SEO_Author = author.Trim(';');
                    }
                    #endregion
                }

                #region GA
                List<SitesModels.GAInfoCont> GA = new List<SitesModels.GAInfoCont>();
                if (!string.IsNullOrEmpty(mSites.GAInfo))
                {
                    GA = JsonConvert.DeserializeObject<List<SitesModels.GAInfoCont>>(mSites.GAInfo);
                }
                ViewBag.GA = GA;
                #endregion
            }
            PageCache.SetTempDataPageCache(TempData, pageCache);
            ViewBag.SiteSN = SiteSN;
            ViewBag.PageSN = PageSN;
            ViewBag.Id = id;
            ViewBag.CustomCont = LoadCustomCont(pageCache);
            if (PageisExist == true)
                if (ShowStatus == 3)
                {
                    Response.Redirect(Url.Action("PageNotFound", "ErrorPages"));
                }
                else
                {
                    #region 網頁點擊LOG
                    long? memberID = null;
                    WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
                    if (curUser != null)
                    {
                        memberID = curUser.ID;
                    }

                    string referUrl = "", referUrlTitle = "", referrerUrlPageNo = "";
                    if (Request.UrlReferrer != null)
                    {
                        referUrl = Request.UrlReferrer.AbsoluteUri;
                        if (referUrl.StartsWith(WorkLib.uUrl.GetURL()))
                        {
                            if (referUrl.Split('/').Length > 0)
                            {
                                string pageSN = referUrl.Split('/')[referUrl.Split('/').Length - 1];
                                Models.PagesModels pageInfo = Models.DataAccess.PagesDAO.GetPageInfo(pageCache.SiteID, pageSN);
                                if (pageInfo != null)
                                {
                                    referUrlTitle = pageInfo.Title;
                                    referrerUrlPageNo = pageInfo.No.ToString();
                                }
                                else
                                {
                                    Models.MenusModels menuInfo = Models.DataAccess.MenusDAO.GetInfo(pageCache.SiteID, pageSN);
                                    if (menuInfo != null)
                                    {
                                        referUrlTitle = menuInfo.Title;
                                        referrerUrlPageNo = menuInfo.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    AddPageLog((long)pageCache.PageNO, mSites.Lang, 3, mSites.Id, getSessionID(),memberID, referUrl, referUrlTitle, referrerUrlPageNo);
                    #endregion
                    ViewBag.lay = null;
                    if (lay != "")
                    {
                        ViewBag.lay = lay;
                    }
                    return View();
                }

            else
            {
                string DefaultSiteSN = GetItem.appSet("DefaultSiteSN").ToString();

                if (DefaultSiteSN != "")
                {
                    Response.Redirect("~/w/" + DefaultSiteSN + "/index");
                }
            }

            return View("EmptyPage");

        }

        public ActionResult CleanIndex(string SiteSN, string PageSN, long? id)
        {

            bool PageisExist = false;
            byte ShowStatus = 3;
            SitesModels mSites = SitesDAO.GetSiteInfo(SiteSN);
            PageCache pageCache = new PageCache();
            if (mSites != null)
            {
                ViewBag.SiteID = mSites.Id;
                PagesModels mPages = PagesDAO.GetPageInfo(mSites.Id, PageSN);
                if (mPages != null)
                {
                    pageCache.SiteID = mPages.SiteID;
                    pageCache.MenuID = mPages.MenuID;
                    pageCache.PageNO = mPages.No;
                    pageCache.PageSN = mPages.SN;
                    pageCache.SiteName = mSites.Title;
                    pageCache.SiteSN = mSites.SN;
                    MenusModels mMenus = MenusDAO.GetInfo(mPages.SiteID, mPages.MenuID);

                    if (mMenus != null)
                    {
                        ShowStatus = mMenus.ShowStatus;
                    }
                    if (ShowStatus != 3)
                    {
                        ShowStatus = mPages.ShowStatus;
                    }

                    PageisExist = true;
                    ViewBag.PageNo = mPages.No;
                    ViewBag.Title = mPages.Title;
                    ViewBag.SiteName = mSites.Title;
                }

                #region GA
                List<SitesModels.GAInfoCont> GA = new List<SitesModels.GAInfoCont>();
                if (mSites.GAInfo != string.Empty)
                {
                    GA = JsonConvert.DeserializeObject<List<SitesModels.GAInfoCont>>(mSites.GAInfo);
                }
                ViewBag.GA = GA;
                #endregion
            }
            ViewBag.SiteSN = SiteSN;
            ViewBag.PageSN = PageSN;
            ViewBag.Id = id;
            PageCache.SetTempDataPageCache(TempData, pageCache);
            ViewBag.CustomCont = LoadCustomCont(pageCache);
            if (PageisExist == true)
                if (ShowStatus == 3)
                {
                    Response.Redirect(Url.Action("PageNotFound", "ErrorPages"));
                }
                else
                {
                    #region 網頁點擊LOG

                    long? memberID = null;
                    WorkV3.Common.Member curUser = WorkV3.Common.Member.Current;
                    if (curUser != null)
                    {
                        memberID = curUser.ID;
                    }

                    string referUrl = "", referUrlTitle = "", referrerUrlPageNo = "";
                    if (Request.UrlReferrer != null)
                    {
                        referUrl = Request.UrlReferrer.AbsoluteUri;
                        if (referUrl.StartsWith(WorkLib.uUrl.GetURL()))
                        {
                            if (referUrl.Split('/').Length > 0)
                            {
                                string pageSN = referUrl.Split('/')[referUrl.Split('/').Length - 1];
                                Models.PagesModels pageInfo = Models.DataAccess.PagesDAO.GetPageInfo(pageCache.SiteID, pageSN);
                                if (pageInfo != null)
                                {
                                    referUrlTitle = pageInfo.Title;
                                    referrerUrlPageNo = pageInfo.No.ToString();
                                }
                                else
                                {
                                    Models.MenusModels menuInfo = Models.DataAccess.MenusDAO.GetInfo(pageCache.SiteID, pageSN);
                                    if (menuInfo != null)
                                    {
                                        referUrlTitle = menuInfo.Title;
                                        referrerUrlPageNo = menuInfo.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    AddPageLog((long)pageCache.PageNO, mSites.Lang, 3, mSites.Id, getSessionID(), memberID, referUrl, referUrlTitle, referrerUrlPageNo);
                    #endregion
                    return View();
                }

            else
            {
                string DefaultSiteSN = GetItem.appSet("DefaultSiteSN").ToString();

                if (DefaultSiteSN != "")
                {
                    Response.Redirect("~/w/" + DefaultSiteSN + "/index");
                }
            }

            return View("EmptyPage");

        }
        public ActionResult EmptyPage()
        {

            return View();
        }

        public ActionResult EmptyGroove()
        {

            return View();
        }
        protected string LoadCustomCont(PageCache pageCache)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string[,] Linkfmt ={
                            {"<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" />","css"}
                            ,
                            {"<script type=\"text/javascript\" src=\"{0}\"></script>","js"}
                            };


            #region 引用 客製內容

            for (int i = 0; i < (Linkfmt.Length / 2); i++)
            {

                string linkfmt = Linkfmt[i, 0] + "\r\n";
                string folder = Server.MapPath("~/websites/") + pageCache.SiteSN + "\\";
                string checkExt = "." + Linkfmt[i, 1];
                string ViewFolder = "~/WebSites/" + pageCache.SiteSN + "/";

                if (Directory.Exists((folder)))
                {
                    foreach (string FileNames in Directory.GetFiles(folder))
                    {
                        DirectoryInfo FileNamesInfo = new DirectoryInfo(FileNames);
                        FileInfo TmpFileInfo = new FileInfo(FileNames);

                        if (TmpFileInfo.Extension.ToLower() == checkExt)
                        {
                            System.Web.UI.Control newControl = new System.Web.UI.Control();

                            var resolvedUrl = newControl.ResolveUrl(ViewFolder + TmpFileInfo.Name);
                            sb.AppendFormat(linkfmt, resolvedUrl);
                        }
                    }
                }
            }

            #endregion

            return sb.ToString();

        }
        protected void AddPageLog(long PagesNo, string Lang, int Ver, long SiteID, string SessionID, long? MemberID, string ReferrerUrl, string ReferrerUrlTitle, string ReferrerUrlPageNo)
        {
            try
            {
            PagesView_LogModel logModel = new PagesView_LogModel()
            {
                PagesNo = PagesNo,
                ReferrerUrl = ReferrerUrl,
                ReferrerUrlTitle= ReferrerUrlTitle,
                ReferrerUrlPageNo = ReferrerUrlPageNo,
                Lang = Lang,
                Ver = Ver,
                SiteID = SiteID,
                SessionID = SessionID,
                MemberID = MemberID,
                Browser = Request.Browser.Browser,
                UserAgent =  Request.UserAgent,
                IP = WorkLib.GetItem.IPAddr(),
                AddTime = DateTime.Now,
                IPNum = (long)WorkLib.GetItem.GetIPNum()
            };
            PagesView_LogDAO.AddPageLogs(logModel);
        }
            catch(Exception exp)
            {
                try
                {
                    //WorkLib.WriteLog.Write(true, exp.Message);
                }
                catch { }
            }
        }
        private string PageViewLogKey = "PageViewLog";
        private string getSessionID()
        {
            if (Session[PageViewLogKey] == null)
            {
                Session[PageViewLogKey] = this.Session.SessionID;
            }
            return Session[PageViewLogKey].ToString();
        }
    }
}