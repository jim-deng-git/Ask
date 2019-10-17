using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using System.IO;
using WorkV3.Golbal;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
namespace WorkV3.Areas.Backend.Controllers
{
    public class MenusController : Controller
    {
        private string UpdFolder = "Menus";
        // GET: Menus
        public ActionResult Index()
        {
            return View();
        }

        #region 選單SN重覆檢查
        public void CheckMenuSNisExist()
        {
            long SiteID = GetItem.SiteID();
            string SN = Request["VSN"];
            bool isExistSN = MenusDAO.isExistSN(SiteID, SN);
            if (isExistSN)
                Response.Write("NO");
            else
                Response.Write("OK");

        }
        #endregion

        #region 新增ADD
        public ActionResult MenuAdd()
        {
            ViewBag.SiteID = GetItem.SiteID();
            int AreaID = 1;
            if (uCheck.IsNumeric(Request["AreaID"]))
            {
                AreaID = int.Parse(Request["AreaID"].ToString());
            }
            ViewBag.AreaID = AreaID;


            List<CardsTypeModels> CList = CardsTypeDAO.GetData();
            ViewBag.CardsTypeList = CList.Where(dr => dr.isOpenCreate == true).ToList();
            ViewBag.CardsTypeListD1 = CList.Where(dr => (dr.Types == (int)CardType.Card || dr.Types == (int)CardType.CardSet) && dr.isOpenCreate == true).ToList();
            ViewBag.CardsTypeListD2 = CList.Where(dr => (dr.Types == (int)CardType.ClassMenu) && dr.isOpenCreate == true).ToList();
            
            return View();
        }
        // GET: BackEnd
        [HttpPost]
        public void MenuAdd(MenusModels MData)
        {
            long NewID = GetItem.NewSN();
            MData.ID = NewID;
            MenusDAO.Insert_Single(MData);

            List<CardsTypeModels> CardsType = CardsTypeDAO.GetData();
            CardsTypeModels CT = CardsType.FindLast(dr => dr.Code == MData.DataType);

            if (CT.isNeedSN == true)
            {
                //新增
                // PubFunc.AddPage(MData.SiteID, MData.ID, MData.SN, MData.DataType, CT.EditURLAction, false, MData.Title, 1, true);
                PubFunc.AddPage(MData.SiteID, MData.ID, MData.SN, MData.DataType, null, false, MData.Title, 1, true);
            }
             SysLog.SaveLog(SysActions.Add, SysMgrNo.Menu, MData.Title,MData.SiteID , MData.ID, null);
            if (CT != null)
            {
                Response.Redirect(Url.Action(CT.URLAction, "Menus", new { id = NewID, SiteID = MData.SiteID }));
            }
            else {
                TempData["refreshData"] = 1;
                Response.Redirect(Request.RawUrl);
            }

        }
        #endregion



        #region 修改選單

        #region Get
        public void GetShowStatusClass()
        {
            if (uCheck.IsNumeric(Request["ShowStatus"]))
                Response.Write(MenusDAO.GetShowStatusClass(byte.Parse(Request["ShowStatus"])));
            else
                Response.Write("");
        }

        public void GetAddMenuTag(long? id) {
            long SiteID = GetItem.SiteID();
            if (uCheck.IsNumeric(id))
            {
                MenusModels mm = MenusDAO.GetInfo(SiteID, (long)id);
                string Fmt = 
                "<li class=\"dd-item\" data-id=\"" + mm.ID + "\">" +
                    "<div class=\"dd-handle\">" +
                    "    <i class=\"\"></i>" +
                    "    <span class=\"menu-title\">" + mm.Title + "</span>" +
                    "    <span class=\"icons-box\">" +
                    "	 <i class=\""+ mm.GetShowStatusClass() + "\"></i>" +
                    "	 <a class=\"openEdit\" href=\"" + Url.Action(mm.MenuURLAction, "Menus", new { id = mm.ID, SiteID = mm.SiteID }) + "\" data-height=\"" + mm.MenuiFrameH + "\" data-width=\"" + mm.MenuiFrameW + "\"><i class=\"cc cc-settings\"></i></a>" +
                    "    </span>" +
                    "</div>" +
                "</li>";
                Response.Write(Fmt);
            }       

        }
        #endregion

        public void MenuEdit_Sort() {
                        
            string items = Request.Form["items"];
            if (string.IsNullOrWhiteSpace(items))
                return;
            long SiteID = GetItem.SiteID();
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<MenusModels> menuList = json.Deserialize<List<MenusModels>>(items);
            MenusDAO.UpdateSort(menuList);
            SysLog.SaveLog(SysActions.Sort, SysMgrNo.Menu, "", SiteID, null, null);
        }

        public ActionResult MenuEdit_Single(long? id)
        {
            ViewBag.HasChildren = false;
            if (uCheck.IsNumeric(id))
            {
                MenusModels mm = MenusDAO.GetInfo(GetItem.SiteID(), (long)id);

                var childList = MenusDAO.GetChildren(id.Value);
                if (childList != null && childList.Count() > 0)
                    ViewBag.HasChildren = true;
                return View(mm);
            }

            return View();
        }

        [HttpPost]
        public void MenuEdit_Single(MenusModels MData)
        {
            ViewBag.HasChildren = false;
            var childList = MenusDAO.GetChildren(MData.ID);
            if (childList != null && childList.Count() > 0)
                ViewBag.HasChildren = true;
            MenusDAO.Save_Single(MData);
            SysLog.SaveLog(SysActions.Edit, SysMgrNo.Menu, MData.Title , MData.SiteID, MData.ID);
            TempData["refreshData"] = SysActions.Edit;
            Response.Redirect(Request.RawUrl);
        }

        #region 連結型選單
        public ActionResult MenuEdit_Link(long? id)
        {
            ViewBag.HasChildren = false;
            List<long> MenuIDs  = new List<long>();
            Dictionary<int, WorkV3.Models.MenusModels> MenuList = new Dictionary<int, WorkV3.Models.MenusModels>();
            if (uCheck.IsNumeric(id))
            {
                long SiteID = GetItem.SiteID();
                MenusModels mm = MenusDAO.GetInfo(GetItem.SiteID(), (long)id);
                ResourceLinksModels newLink = ResourceLinksDAO.GetInfo(SiteID, (long)id, (byte)SourceType.Menu, 1, 1, 1);
                ViewBag.LinkInfo = newLink;
                var childList = MenusDAO.GetChildren(id.Value);
                if (childList != null && childList.Count() > 0)
                    ViewBag.HasChildren = true;
                int menuLev = 0;
                if (!string.IsNullOrEmpty(newLink.Detail))
                {
                    var menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(long.Parse(newLink.Detail));
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
                        var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(newLink.Detail));
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
                                    if (page_menuInfo != null &&!string.IsNullOrEmpty(page_menuInfo.Title))
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
                        var orderMenus = MenuList.OrderByDescending(p => p.Key).Select(p=>p.Value.Id);
                        MenuIDs = orderMenus.ToList();
                    }
                    ViewBag.DefaultMenus = MenuIDs;
                }
                return View(mm);
            }
            return View();
        }

        [HttpPost]
        public void MenuEdit_Link(MenusModels MData, string Sites, string[] Pages, string PageDetailID)
        {
            ViewBag.HasChildren = false;
            var childList = MenusDAO.GetChildren(MData.ID);
            if (childList != null && childList.Count() > 0)
                ViewBag.HasChildren = true;
            MenusDAO.Save_Single(MData);
            

            ResourceLinksModels newLink = new ResourceLinksModels();
            newLink.Id = 1;
            newLink.SiteID = MData.SiteID;
            newLink.SourceNo = MData.ID;
            newLink.SourceType = (byte)SourceType.Menu;
            newLink.Ver = 1;
            newLink.AreaID = 1;
            bool isOpenNewWin = Request["OpenNewWin"].Contains("true");
            if (isOpenNewWin)
            {
                newLink.ClickType = (byte)ClickType.OpenNewWin;
            }
            else
            {
                newLink.ClickType = (byte)ClickType.PageOpen;
            }
            //WorkLib.WriteLog.Write(true, Request["inLink"]);
            if (Request["inLink"].Contains("true"))
            {
                newLink.LinkInfo = Request["LinkInfo"].ToString();

                newLink.LinkType = ResourceLinkType.inLink;
                if (!string.IsNullOrEmpty(PageDetailID))
                {
                    newLink.Detail = PageDetailID;
                    var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(PageDetailID));
                    if (pageInfo != null)
                    {
                        var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageInfo.SiteID);
                        newLink.LinkInfo = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = pageInfo.SN });
                    }
                }
                else if (Pages != null)
                {
                    for (int i = 0; i < Pages.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Pages[i]))
                            newLink.Detail = Pages[i];
                    }
                    if (!string.IsNullOrEmpty(newLink.Detail))
                    {
                        var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(newLink.Detail));
                        if (pageInfo != null)
                        {
                            var siteInfo = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageInfo.SiteID);
                            newLink.LinkInfo = Url.RouteUrl("FrontSitePage", new { SiteSN = siteInfo.SN, PageSN = pageInfo.SN });
                        }
                    }
                }
            }
            else
            {
                newLink.LinkType = ResourceLinkType.outLink;
                newLink.LinkInfo = Request["LinkInfo"].ToString();
            }
            
            ResourceLinksDAO.Save_Menu(newLink);

            SysLog.SaveLog(SysActions.Edit, SysMgrNo.Menu, MData.Title, MData.SiteID, MData.ID);
            TempData["refreshData"] = SysActions.Edit;
            Response.Redirect(Request.RawUrl);

        }

        #endregion

        #region 階層選單

        public ActionResult MenuEdit_Folder(long? id)
        {
            ViewBag.HasChildren = false;
            if (uCheck.IsNumeric(id))
            {
                MenusModels mm = MenusDAO.GetInfo(GetItem.SiteID(), (long)id);
                var childList = MenusDAO.GetChildren(id.Value);
                if(childList != null && childList.Count() > 0)
                    ViewBag.HasChildren = true;
                return View(mm);
            }
            return View();
        }

        [HttpPost]
        public void MenuEdit_Folder(MenusModels MData)
        {
            ViewBag.HasChildren = false;
            var childList = MenusDAO.GetChildren(MData.ID);
            if (childList != null && childList.Count() > 0)
                ViewBag.HasChildren = true;
            MenusDAO.Save_Folder(MData);
            SysLog.SaveLog(SysActions.Edit, SysMgrNo.Menu, MData.Title, MData.SiteID, MData.ID);
            TempData["refreshData"] = SysActions.Edit;
            Response.Redirect(Request.RawUrl);
        }

        #endregion

        #region 檔案型選單

        public ActionResult MenuEdit_File(long? id)
        {
            ViewBag.HasChildren = false;
            //檔案上限
            ViewBag.FileLimit = 1;
            ViewBag.FileExtensions = UpdFileInfo.FileExtensionsType.Custom;//Joe 20190925 問題單:後台單一檔案允許格式

            if (uCheck.IsNumeric(id))
            {
                long SiteId = GetItem.SiteID();
                string vPath = UpdFileInfo.GetVPathBySiteID(SiteId, "Menus");
                MenusModels mm = MenusDAO.GetInfo(SiteId, (long)id);

                ResourceFilesModels newFile = ResourceFilesDAO.GetInfo(SiteId, (long)id, (byte)SourceType.Menu, 1, 1, 1);
                if (newFile != null)
                {
                    ViewBag.FileExists = new HtmlString(ResourceFilesDAO.fileuploader(newFile,vPath));
                }
                var childList = MenusDAO.GetChildren(id.Value);
                if (childList != null && childList.Count() > 0)
                    ViewBag.HasChildren = true;
                return View(mm);


            }
            return View();
        }

        [HttpPost]
        public ActionResult MenuEdit_File(MenusModels MData, HttpPostedFileBase updFiles)
        {
            ViewBag.HasChildren = false;

            var childList = MenusDAO.GetChildren(MData.ID);
            if (childList != null && childList.Count() > 0)
                ViewBag.HasChildren = true;

            MenusDAO.Save_Single(MData);
            //單檔
            if (updFiles != null) {
                if (updFiles.ContentLength > 0)
                {
                    string uPath = UpdFileInfo.GetUPathBySiteID(MData.SiteID, "Menus");
                    string fileName = UpdFileInfo.SaveFiles(updFiles, uPath);
                    
                    ResourceFilesModels newFile = new ResourceFilesModels();

                    newFile.Id = 1;
                    newFile.SiteID = MData.SiteID;
                    newFile.SourceNo = MData.ID;
                    newFile.SourceType = (byte)SourceType.Menu;
                    newFile.Ver = 1;
                    newFile.AreaID = 1;
                    newFile.FileInfo = fileName;
                    newFile.FileType = ResourceFileType.inFile;
                    newFile.ClickType = (byte)ClickType.OpenNewWin;
                    newFile.FileSize =  newFile.GetSize(uPath);
                    newFile.FileMimeType = newFile.GetMimeType();
                    ResourceFilesDAO.Save_Menu(newFile);

                }
                else
                {
                    ResourceFilesDAO.DelAll(MData.SiteID, MData.ID, (byte)SourceType.Menu, 1, 1);
                }
            }
           

            

            //多檔
            //List<ResourceFilesModels> newFileList = new List<ResourceFilesModels>();
            //int Count = 0;
            //foreach (var ff in updFiles)
            //{
            //    if (ff != null)
            //    {
            //        if (ff.ContentLength > 0)
            //        {
            //            Count += 1;
            //            var fileName = Path.GetFileName(ff.FileName);
            //            var path = Path.Combine(GetItem.UpdPath(GetItem.SiteID() + "\\Menus"), fileName);
            //            ff.SaveAs(path);

            //        }
            //    }
            //}

            SysLog.SaveLog(SysActions.Edit, SysMgrNo.Menu, MData.Title, MData.SiteID, MData.ID);
            TempData["refreshData"] = SysActions.Edit;
            Response.Redirect(Request.RawUrl);

            return View();
        }

        #endregion

        #endregion
        #region 選單刪除
        [HttpPost]
        public ActionResult Delete(long siteID, long id, string DeleteAll)
        {
            WorkV3.Areas.Backend.Models.MenusModels item = WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.GetInfo(siteID, id);
            MenusDAO.DeleteMenu(item, (DeleteAll == "1"));
            SysLog.SaveLog(SysActions.Del, SysMgrNo.Menu, item.Title, item.SiteID, id);
            TempData["refreshData"] = SysActions.Edit;
            return Json("success");
        }

        #endregion
    }
}