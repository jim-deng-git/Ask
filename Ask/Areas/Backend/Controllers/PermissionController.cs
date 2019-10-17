using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;
using WorkV3.Areas.Backend.Abstracts;

namespace WorkV3.Areas.Backend.Controllers
{
    public class PermissionController : Controller
    {
        // GET: Backend/Permission

        public ActionResult PermissionGroup()
        {
            IEnumerable<GroupModels> items = GroupDAO.GetItems();

            ViewBag.WebUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"].TrimEnd('/');

            return View(items);
        }

        public ActionResult GroupSetting(long? ID = null)
        {
            GroupModels model = new GroupModels();
            ViewBag.BodyClass = "body-admin-main";
            if (ID.HasValue)
            {
                model = GroupDAO.GetItem((long)ID, PageCache.SiteID);
            }

            ViewBag.WebUrl = System.Configuration.ConfigurationManager.AppSettings["WebUpdUrl"].Replace("WebUPD","").TrimEnd('/');
            return View(model);
        }

        [HttpPost]
        public ActionResult GroupSetting(GroupModels model)
        {
            GroupDAO.SetItem(model);
            ViewBag.Exit = true;
            return View(model);
        }

        public ActionResult PermissionSetting(long ID)
        {
            IEnumerable<WorkV3.Models.SitesModels> sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            List<ViewModels.GroupPermissionViewModel> siteMenuList = new List<ViewModels.GroupPermissionViewModel>();

            foreach (WorkV3.Models.SitesModels site in sites)
            {
                siteMenuList.Add(new ViewModels.GroupPermissionViewModel { SiteID = site.Id, menus = BackendMenuDAO.GetRoots(site.Id) });
            }

            long siteId = PageCache.SiteID;
            //IEnumerable<BackendMenuModel> rootMenu = BackendMenuDAO.GetRoots();
            //string jsonResult = JsonConvert.SerializeObject(siteMenuList);
            GroupModels group = GroupDAO.GetItem(ID, siteId);

            ViewBag.BodyClass = "body-admin-main";
            ViewBag.Group = group;
            ViewBag.RootMenu = siteMenuList;
            ViewBag.SiteID = siteId;
            //ViewBag.RootMenuJson = jsonResult;
            ViewBag.WebUrl = System.Configuration.ConfigurationManager.AppSettings["WebUrl"].TrimEnd('/');

            return View();
        }

        [HttpPost]
        public ActionResult PermissionSetting(List<GroupPermissionModels> permissions, long groupId, long siteId)
        {
            GroupPermissionDAO.SetItems(permissions, groupId, siteId);

            return RedirectToAction("PermissionSetting");
        }

        public ActionResult PermissionGroup_Unit_Setting(long? ID = null)
        {
            ViewBag.BodyClass = "body-admin-main";
            ViewBag.GroupId = ID ?? 0;
            return View();
        }

        public JsonResult GetPermissionGroup(long ID)
        {
            List<PermissionGroupModels> PermissionGroup = GroupDAO.GetPermissionGroup((long)ID);
            return Json(PermissionGroup, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PermissionGroupSetting(long ID,long SiteID)
        {
            string[] arrPermissionGroup = (Request["PermissionGroup"].ToString() ?? "").Split(',');
            GroupDAO.PermissionGroupSetting(ID, SiteID, arrPermissionGroup);

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermissionGroup_User(long ID)
        {
            List<MemberModels> GetMemberByGroup = GroupDAO.GetMemberByGroup(ID);

            string GroupName = "";
            GroupModels GM = GroupDAO.GetItem(ID);
            if(GM != null)
            {
                GroupName = GM.Name;
            }
            ViewBag.GroupName = GroupName;
            ViewBag.BodyClass = "body-admin-main";
            return View(GetMemberByGroup);
            
        }

        public ActionResult DeletePermission(IEnumerable<long> ids)
        {
            GroupDAO.DeletePermission(ids);

            return View();

        }

        [HttpPost]
        public void Sort(IEnumerable<SortItem> items)
        {
            CommonDA.Sort("[Group]", items, "");
        }

    }
}