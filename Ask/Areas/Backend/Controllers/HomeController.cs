using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkLib;
using WorkV3.Models.Repository;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.Abstracts;

namespace WorkV3.Areas.Backend.Controllers
{
    public class HomeController : Controller
    {
        string uploadUrl = GetItem.ViewUpdUrl().TrimEnd('/') + "/Manager/";
        // GET: BackEnd
        public ActionResult Index(string SiteSN)
        {
            ViewBag.Member = Session[WebInfo.SysMemSkey] == null ?null:(Request.Cookies["sessionId"] != null? MemberDAO.Current(Request.Cookies["sessionId"].Value): null);
            ViewBag.UploadUrl = uploadUrl;
            if (ViewBag.Member != null)
            {
                WorkV3.Models.SitesModels SiteItem = WorkV3.Models.DataAccess.SitesDAO.GetSiteInfo(SiteSN);
                if (SiteItem != null)
                {
                    MemberModels mem = MemberDAO.SysCurrent;

                    PageCache.SiteID = SiteItem.Id;
                    PageCache.SiteName = SiteItem.Title;
                    ViewBag.SiteSN = SiteSN;
                    ViewBag.SiteID = PageCache.SiteID;
                    ViewBag.SiteName = PageCache.SiteName;
                    List<MenusModels> mm = MenusDAO.GetData(PageCache.SiteID);

                    long siteId = PageCache.SiteID;
                    List<Menu> backendMenu = BackendMenuDAO.GetRoots(siteId).ToList();
                    List<CardsViewModel> cards = CardsDAO.GetBySiteID(PageCache.SiteID);
                    MemberModels curUser = ViewBag.Member;
                    GroupModels group = ViewBag.Member == null ? new GroupModels() : GroupDAO.GetItem(curUser.GroupId, siteId);

                    ViewBag.MenuList = mm;
                    ViewBag.Cards = cards;
                    ViewBag.CardTypes = CardsTypeDAO.GetData();
                    ViewBag.BackendMenu = backendMenu;
                    ViewBag.Permissions = group.GetPermissions(siteId);
                }
                else
                {
                    Response.Redirect(Url.Action("Sites"));
                }
                return View();
            }
            else
            {
                Response.Redirect(Url.Action("Login"));
                return View();
            }


        }
        // GET: BackEnd
        public ActionResult Sites()
        {
            ViewBag.Member = Session[WebInfo.SysMemSkey] == null ? null : (Request.Cookies["sessionId"] != null ? MemberDAO.Current(Request.Cookies["sessionId"].Value) : null);
            ViewBag.UploadUrl = uploadUrl;
            if (ViewBag.Member != null)
            {
                var backendMenu = BackendMenuDAO.GetManagerRoots().ToList();
                ViewBag.BackendMenu = backendMenu;
                return View();
            }
            else
            {
                Response.Redirect(Url.Action("Login"));
                return View();
            }
        }

        public ActionResult SiderBar()
        {
            List<Menu> backendMenu = new List<Menu>();
            if ((Session[WebInfo.SysMemSkey] == null ? null : (Request.Cookies["sessionId"] != null ? MemberDAO.Current(Request.Cookies["sessionId"].Value) : null)) == null)
            {
                ViewBag.BackendMenu = backendMenu;
                return HttpNotFound();

                //return View();
            }
            if (MemberDAO.SysCurrent != null)
            {
                if (PageCache.SiteID == 0)
                {
                    backendMenu = BackendMenuDAO.GetManagerRoots().ToList();
                    ViewBag.BackendMenu = backendMenu;
                    return View("Backend/SiderBar");
                }
                else
                {
                    long SiteID = PageCache.SiteID;
                    MemberModels mem = MemberDAO.SysCurrent;
                    List<MenusModels> mm = MenusDAO.GetData(SiteID);
                    backendMenu = BackendMenuDAO.GetRoots(SiteID).ToList();
                    ViewBag.SiteID = SiteID;
                    ViewBag.MenuList = mm;
                    ViewBag.BackendMenu = backendMenu;

                    return View("Backend/SiderBar", new { SiteId = SiteID });
                }
            }
            ViewBag.BackendMenu = backendMenu;
            return View();
        }

        public ActionResult MainContent()
        {
            ViewBag.CardsCode = Request["CardsCode"];
            return View();
        }

        #region 網站
        public ActionResult mySites()
        {
            ViewBag.Member = Request.Cookies["sessionId"] != null ? MemberDAO.Current(Request.Cookies["sessionId"].Value) : null;
            if (ViewBag.Member != null)
            {
                List<WorkV3.Models.SitesModels> mm = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
               var backendMenu = BackendMenuDAO.GetManagerRoots().ToList();
                ViewBag.BackendMenu = backendMenu;
                return View(mm);
            } else
            {
                Response.Redirect(Url.Action("Login"));
                return View();
            }
        }
        #endregion

        #region 登入

        public ActionResult Login()
        {

            if (MemberDAO.SysCurrent != null)
            {
                MemberModels mem = MemberDAO.SysCurrent;
                //Response.Redirect(Url.Action("index", new { SiteSN = "huashan1914" }));
                Response.Redirect(Url.Action("Sites"));
                return View("Sites");
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        public void LoginCheck()
        {
            string IP = GetItem.IPAddr();
            bool isAllow = IntraIPlimitDAO.isAllowIP(IP);
            if (!isAllow)
            {
                //                WriteLog.Write(true, "LoginCheck is Not Allow IP:" + IP);
                Response.Redirect(Url.Action("Login"));
                TempData["message"] = "您沒有權限登入，請洽詢相關人員";
                return;
            }
            if (Request["txtLoginID"] != null && Request["txtPwd"] != null)
            {
                string sessionId = MemberDAO.LoginCheck(Request["txtLoginID"], Request["txtPwd"]);
                if (sessionId == "isSuspension")
                {
                    TempData["message"] = "該帳號已停權，有任何問題請聯繫系統管理員";
                    Response.Redirect(Url.Action("Login"));
                    return;
                }
                if (sessionId != null)
                {
                    SysLog.SaveLog(SysActions.Login, SysMgrNo.Site, "", null, null, null);
                    HttpCookie cookie = new HttpCookie("sessionId");
                    cookie.Value = sessionId;
                    Response.Cookies.Add(cookie);
                    //Response.Write("Cookie创建完毕");
                    Response.Redirect(Url.Action("Sites"));
                }
                else
                {
                   TempData["message"] = "帳號或密碼錯誤";
                    Response.Redirect(Url.Action("Login"));
                }

            }
            else
            {
                TempData["message"] = "帳號及密碼圴不可空白";
            }


        }
        [HttpPost]
        public JsonResult CheckSessionValid()
        {

            var Member = Request.Cookies["sessionId"] != null ? MemberDAO.Current(Request.Cookies["sessionId"].Value) : null;
            if (Member == null)
            //string sessionID = Request.Cookies["sessionId"].Value;
            //if (Session[sessionID] == null)
            {
                return Json("no");
            }
            else
            {
                return Json("yes");
            }
        }

        [HttpPost]
        public JsonResult RefreshLoginInfo()
        {
            ViewBag.UploadUrl = uploadUrl;
            MemberModels mem = MemberDAO.SysCurrent;

            string sql = "select * from Member where LoginID=@ID";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.SelectObject selMem = db.GetSelectObject(sql, new SQLData.ParameterCollection("@ID", mem.LoginID));
            if (selMem != null)
            {
                MemberModels member = new MemberModels
                {
                    Id = long.Parse(selMem["Id"].ToString()),
                    isSysOnly = bool.Parse(selMem["isSysOnly"].ToString()),
                    LoginID = selMem["LoginID"].ToString(),
                    Name = selMem["Name"].ToString(),
                    Img = selMem["Img"].ToString(),
                    GroupId = long.Parse(selMem["GroupId"].ToString()),
                    Email = selMem["Email"].ToString()
                };
                string sessionID = Request.Cookies["sessionId"].Value;
                Session[sessionID] = member;
                return Json("success");
            }
            else
                return Json("fail");
        }

        public ActionResult EditMemberInfo(long? ID)
        {
            ViewBag.UploadUrl = uploadUrl;
            MemberModels m = new MemberModels();
            if (ID.HasValue)
            {
                m = ManagerDAO.GetItem((long)ID);
                ViewBag.IsNew = false;
            }
            else
            {
                ViewBag.IsNew = true;
            }

            var group = GroupDAO.GetItems();
            ViewBag.group = group;

            ViewBag.ID = ID ?? 0;

            return View(m);
        }
        [HttpPost]
        public ActionResult EditMemberInfo(MemberModels model)
        {
            model.IsChangedPassword = true;
            ManagerDAO.SetPersonalItem(model);
            ViewBag.Exit = true;
            ViewBag.RefreshLoginInfo = true;
            ViewBag.UploadUrl = uploadUrl;
            var group = GroupDAO.GetItems();
            ViewBag.group = group;

            return View(model);
        }

        /// <summary>
        /// 上傳檔案(單檔)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPersonPhotoFile(HttpPostedFileBase File)
        {
            if (File != null && File.ContentLength > 0)
            {
                string Path = string.Format("{0}/{1}", GetItem.UpdPath(), "Manager");
                if (!System.IO.Directory.Exists(Path))
                    System.IO.Directory.CreateDirectory(Path);
                string saveName = WorkV3.Golbal.UpdFileInfo.SaveFiles(File, Path);
                MemberModels mem = MemberDAO.SysCurrent;
                string sql = "UPDATE Member  SET Img=@Img WHERE LoginID=@ID";

                SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
                paraList.Add("@ID", mem.LoginID);
                paraList.Add("@Img", saveName);
                int exeCount = db.ExecuteNonQuery(sql, paraList);
                if (exeCount > 0)
                    return Json("success");
            }
            return Json("fail");
        }
        #endregion

        #region 登出  
        public void Logout()
        {
            MemberDAO.SysLogout();
            Response.Redirect(Url.Action("Login"));
        }
        #endregion
    }
}