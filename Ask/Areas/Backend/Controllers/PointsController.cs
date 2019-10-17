using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Models.DataAccess;
using WorkV3.ViewModels;

namespace WorkV3.Areas.Backend.Controllers
{
    public class PointsController : Controller
    {
        public ActionResult List(long siteId, long menuId, int? index, long memberID,string memberName)
        {
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            PointsRecordViewModel items = new PointsRecordViewModel();            

            int totalRecord;
            IEnumerable<PointsModel> points = PointsDAO.GetItems(siteId, memberID, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;

            items.Points = points;
            items.Name = memberName;
            items.Total = PointsDAO.GetPointsTotal(siteId, memberID);

            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.MemberID = memberID;
            ViewBag.Pagination = pagination;
            return View(items);
        }

        public ActionResult Edit(long siteId, long menuId, long memberID)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.MemberID = memberID;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(long siteId, long menuId, long memberID, PointsModel item)
        {
            ViewBag.SiteID = siteId;
            ViewBag.MenuID = menuId;
            ViewBag.MemberID = memberID;

            item.MemberShipID = memberID;
            item.SiteID = siteId;
            if(item.PointType == 1)
            {
                item.Point = -item.Point;
                item.Remark = "平台管理者主動扣除點數";
            }
            else if(item.PointType == 0)
            {
                item.Remark = "平台管理者協助增加點數";
            }
            PointsDAO.SetItem(item, true);
            ViewBag.Exit = true;
            return View(item);
        }

        [HttpPost]
        public void PointsDel(IEnumerable<long> ids)
        {
            PointsDAO.DeletePoints(ids);
        }
    }
}