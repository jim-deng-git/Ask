using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;

namespace WorkV3.Areas.Backend.Controllers
{
    /// <summary>
    /// 廣告區設定
    /// </summary>
    public class AdvertisementSetController : Controller
    {
        public ActionResult Index(int? page, long siteId = 1, long menuId = 2000)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            List<AdvertisersModel> list = new List<AdvertisersModel>();

            Pagination pagination = new Pagination
            {
                PageIndex = page ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            list = AdvertisementDAO.GetAdvertisers(pagination.PageSize, pagination.PageIndex,siteId, out totalRecord);
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;

            return View(list);
        }
        public ActionResult AdvertisersEdit(long siteId, long menuId, long? id)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;

            AdvertisersModel item = null;
            if (id != null)
                item = AdvertisementDAO.GetAdvertisersItem((long)id);
            if (item == null)
                item = new AdvertisersModel { ID = WorkLib.GetItem.NewSN(), IsIssue = true, Sort = int.MaxValue };

            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AdvertisersEdit(AdvertisersModel model)
        {
            //return RedirectToAction("Index");

            AdvertisementDAO.SetAdvertisersItem(model);
            ViewBag.SiteID = model.SiteID;
            ViewBag.MenuID = model.MenuID;
            ViewBag.Exit = true;
            return View(model);
        }
        [HttpPost]
        public void AdvertisersSort(IEnumerable<SortItem> items)
        {
            AdvertisementDAO.SortEdit(items, "Advertisers");
        }
        [HttpPost]
        public void AdvertisersDel(IEnumerable<long> ids)
        {
            AdvertisementDAO.AdvertisersDelete(ids);
        }
        /// <summary>
        /// 變更刊登狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void AdvertisementSetToggleIssue(long id)
        {
            AdvertisementDAO.ToggleIssue(id, "Advertisers");
        }
    }
}