using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Common;
using WorkV3.Models.Repository;

namespace WorkV3.Areas.Backend.Controllers
{
    public class GlobalParaController : Controller
    {
        private CompanyRepository comRepo = new CompanyRepository();

        public ActionResult Para(long siteId, string type, int? parentId)
        {
            string parentName = "";
            var companyItems = comRepo.GetAll();
            if (parentId.HasValue)
            {
                parentName = companyItems.Where(m => m.ID == parentId).Select(m => m.Name).SingleOrDefault();
                companyItems = companyItems.Where(m => m.ParentID == parentId).ToList();
            }
            else
            {
                companyItems = companyItems.Where(m => m.ParentID == null);
            }

            ViewBag.SiteID = siteId;
            ViewBag.Type = type ?? "Company";
            ViewBag.ParentName = parentName;
            ViewBag.Companys = companyItems.OrderBy(m => m.Sort).ToList();
            ViewBag.ParentID = parentId;
            return View();
        }

        public ActionResult Edit(long siteId, int? Id, int? parentId)
        {
            CompanyModel item = null;
            if (Id.HasValue)
                item = comRepo.GetItem((int)Id);

            if (item == null)
                item = new CompanyModel();

            ViewBag.ParentID = parentId;
            ViewBag.SiteID = siteId;
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(long siteId, CompanyModel item, int? parentId)
        {
            bool isNew = item.ID == 0;

            if (isNew)
            {
                if (parentId.HasValue)
                    item.ParentID = (int)parentId;
                item.Creator = Models.DataAccess.MemberDAO.SysCurrent.Id;
                item.CreateTime = DateTime.Now;
                comRepo.CreateItem(item, isWithIdentity:false);
            }
            else
            {
                item.Modifier = Models.DataAccess.MemberDAO.SysCurrent.Id;
                item.ModifyTime = DateTime.Now;
                comRepo.UpdateItemExcept(item, new string[] { "ID", "ParentID", "Creator", "CreateTime" });
            }

            ViewBag.Exit = true;
            ViewBag.SiteID = siteId;
            return View(item);
        }

        #region 休假日設定

        /// <summary>
        /// 休假日設定
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HolidaySet()
        {
            return View();
        }

        /// <summary>
        /// 取得休假日資料
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="jsonDatas"></param>
        /// <returns></returns>
        public ContentResult GetHolidaySet(string Year)
        {
            List<HolidaySetModels> dateList = comRepo.GetHolidaySetList(Year, (int)HolidayType.Holiday);
            string dates = "";
            foreach (HolidaySetModels model in dateList)
                dates += string.Format("'{0}',", model.Date.ToString("yyyy-M-d"));
            dates = dates.TrimEnd(',');
            return Content(dates, "text/plain");

        }

        /// <summary>
        /// 休假日設定資料更新
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="jsonDatas"></param>
        /// <returns></returns>
        public ContentResult UpdateHolidaySet(string Year, string jsonDates)
        {
            if (string.IsNullOrWhiteSpace(jsonDates))
                return Content("0");

            List<HolidaySetModels> dateList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HolidaySetModels>>(jsonDates);
            comRepo.UpdateHolidaySet(Year, dateList, (int)HolidayType.Holiday);
            return Content("1");
        }

        /// <summary>
        /// 是否可被設定為休假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckHolidaySet(DateTime date)
        {
            bool result = comRepo.IsHoliday(date.ToString(WebInfo.DateTimeFmt), (int)HolidayType.NationalHoliday);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 國定假日設定
        /// <summary>
        /// 國定假日設定
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NationalHolidaySet()
        {
            return View();
        }

        /// <summary>
        /// 取得國定假日資料
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="jsonDatas"></param>
        /// <returns></returns>
        public ContentResult GetNationalHolidaySet(string Year)
        {
            List<HolidaySetModels> dateList = comRepo.GetHolidaySetList(Year, (int)HolidayType.NationalHoliday);
            string dates = "";
            foreach (HolidaySetModels model in dateList)
                dates += string.Format("'{0}',", model.Date.ToString("yyyy-M-d"));
            dates = dates.TrimEnd(',');
            return Content(dates, "text/plain");

        }

        /// <summary>
        /// 國定假日設定資料更新
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="jsonDatas"></param>
        /// <returns></returns>
        public ContentResult UpdateNationalHolidaySet(string Year, string jsonDates)
        {
            if (string.IsNullOrWhiteSpace(jsonDates))
                return Content("0");

            List<HolidaySetModels> dateList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HolidaySetModels>>(jsonDates);
            comRepo.UpdateHolidaySet(Year, dateList, (int)HolidayType.NationalHoliday);
            return Content("1");
        }

        /// <summary>
        /// 是否可被設定為國定假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckNationalHolidaySet(DateTime date)
        {
            bool result = comRepo.IsHoliday(date.ToString(WebInfo.DateTimeFmt), (int)HolidayType.Holiday);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sort、Delete、ToggleIssue
        [HttpPost]
        public void ParaSort(int? parentId, IEnumerable<SortItem> items)
        {
            comRepo.Sort(items, parentId);
        }

        [HttpPost]
        public void ParaDel(IEnumerable<long> ids)
        {
            comRepo.Delete(ids);
        }

        [HttpGet]
        public void ParaToggleIssue(long id)
        {
            comRepo.ToggleIssue(id);
        }
        #endregion

        public ActionResult GetDepartments(int parentId)
        {
            var items = comRepo.GetItems(parentId, "ParentID", "Order By Sort").Where(m => m.IsIssue);

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}