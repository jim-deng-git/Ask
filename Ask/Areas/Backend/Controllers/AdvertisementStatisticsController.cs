using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkV3.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using WorkV3.Areas.Backend.ViewModels;

namespace WorkV3.Areas.Backend.Controllers
{

    public class AdvertisementStatisticsController : Controller
    {
        private string CustomLabelCookieName = "AdsCustomLabel";

        // 廣告成效頁
        public ActionResult Index(AdsStatisticsSearchModel search, long? siteId, int? index)
        {
            DateTime today = DateTime.Now;
            DateTime firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                    search.StartDate = search.StartDate != DateTime.MinValue ? search.StartDate : firstDayOfCurrentMonth;
                    search.EndDate = search.EndDate != DateTime.MinValue ? search.EndDate : endDayOfCurrentMonth;
                    Session["ExportSearch"] = search;
                }
                else
                {
                    AdsStatisticsSearchModel prevSearch = Utility.GetSearchValue<AdsStatisticsSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
                Session["ExportSearch"] = search;
            }

            search.SiteId = search.SiteId ?? siteId;

            if(index == null)
                index = 1;

            int pageSize = 20;
            int recordCount = 0;
            List<AdsStatisticsViewModel> statisticsData = AdvertisementStatisticsDAO.GetAdsStatisticsData(search, pageSize, (int)index, out recordCount);

            ViewBag.Pagination          = new Pagination { PageSize = pageSize, PageIndex = (int)index, TotalRecord = recordCount };
            ViewBag.AdsPositionMapping  = AdvertisementStatisticsDAO.AdsPosition;
            ViewBag.UploadUrl           = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID((long)search.SiteId, "Advertisement");
            ViewBag.Search              = search;
            ViewBag.StartDate           = firstDayOfCurrentMonth;
            ViewBag.EndDate             = endDayOfCurrentMonth;
            ViewBag.SiteId              = search.SiteId;

            return View(statisticsData);
        }

        // 廣告成效詳細內容頁
        public ActionResult Detail(AdsDetailStatisticsSearchModel search, int? index)
        {
            DateTime today = DateTime.Now;
            DateTime firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);
            AdsCustomizeModel adCustomize = AdvertisementDAO.GetAdsCustomizeItem(search.AdsCustomId);
            adCustomize = adCustomize ?? new AdsCustomizeModel();

            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                    search.StartDate = search.StartDate != DateTime.MinValue ? search.StartDate : firstDayOfCurrentMonth;
                    search.EndDate = search.EndDate != DateTime.MinValue ? search.EndDate : endDayOfCurrentMonth;
                    Session["ExportDetailSearch"] = search;
                }
                else
                {
                    AdsDetailStatisticsSearchModel prevSearch = Utility.GetSearchValue<AdsDetailStatisticsSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
                Session["ExportDetailSearch"] = search;
            }

            if (index == null)
                index = 1;

            int pageSize = 20;
            int recordCount = 0;
            List<AdsStatisticsDetailViewModel> statisticsData = AdvertisementStatisticsDAO.GetStatisticsDetail(search, pageSize, (int)index, out recordCount);

            ViewBag.Pagination = new Pagination { PageSize = pageSize, PageIndex = (int)index, TotalRecord = recordCount };
            ViewBag.Search = search;
            ViewBag.StartDate = firstDayOfCurrentMonth;
            ViewBag.EndDate = endDayOfCurrentMonth;
            ViewBag.AdsCustomizeDescription = adCustomize.Description;
            return View(statisticsData);
        }

        // 廣告主統計頁
        public ActionResult AdvertiserStatistics(AdsDetailStatisticsSearchModel search, long siteId, int? index)
        {
            DateTime today = DateTime.Now;
            DateTime firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                {
                    Utility.ClearSearchValue();
                    search.StartDate = firstDayOfCurrentMonth;
                    search.EndDate = endDayOfCurrentMonth;
                    Session["ExportAdvertiserSearch"] = search;
                }
                else
                {
                    AdsDetailStatisticsSearchModel prevSearch = Utility.GetSearchValue<AdsDetailStatisticsSearchModel>();
                    if (prevSearch != null)
                        search = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                Utility.SetSearchValue(search);
                Session["ExportAdvertiserSearch"] = search;
            }

            if (index == null)
                index = 1;

            int pageSize = 20;
            int recordCount = 0;

            List<AdvertiserStatisticsViewModel> data = AdvertisementStatisticsDAO.GetAdvertiserStatistics(search, pageSize, (int)index, out recordCount);

            ViewBag.Pagination = new Pagination { PageSize = pageSize, PageIndex = (int)index, TotalRecord = recordCount };
            ViewBag.AdsPositionMapping = AdvertisementStatisticsDAO.AdsPosition;
            ViewBag.UploadUrl = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(siteId, "Advertisement");
            ViewBag.Search = search;
            ViewBag.StartDate = firstDayOfCurrentMonth;
            ViewBag.EndDate = endDayOfCurrentMonth;
            ViewBag.SiteID = siteId;

            return View(data);
        }

        public ActionResult DateSelector(DateTime startDate, DateTime endDate)
        {
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View();
        }

        public ActionResult AdvertiserSelector(List<long> advertiserIds)
        {
            IEnumerable<AdvertisersModel> advertisers = AdvertisementDAO.GetAllAdvertisers();

            ViewBag.AdvertiserIds = advertiserIds;

            return View(advertisers);
        }

        public ActionResult GetChart(DateTime startTime, DateTime endTime, List<long> advertisers = null, long adsCustomizeId = 0)
        {
            string jsonData = AdvertisementStatisticsDAO.GetChart(startTime, endTime, advertisers, adsCustomizeId);

            return Content(jsonData);
        }

        public ActionResult CustomLabelLine(int? index, AdsStatisticsLabelsSearchModel search)
        {
            bool IsShowLabelLine = GetCustomLableCookie();
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AdsStatisticsLabelsSearchModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AdsStatisticsLabelsSearchModel>();
                    if (prevSearch != null)
                    {
                        search = prevSearch;
                    }
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(search);
            }


            ViewBag.Search = search;

            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            int totalRecord;
            IEnumerable<AdsStatisticsLabelsModel> items = AdsStatisticsLabelDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            ViewBag.SelectCustomLabel = IsShowLabelLine;
            return View(items);
        }

        [HttpGet]
        public ActionResult CustomLabelLineEdit(long? id)
        {
            AdsStatisticsLabelsModel item = null;
            if (id != null)
            {
                item = AdsStatisticsLabelDAO.GetItem((long)id);
                item.IsShowCustomLableLine = GetCustomLableCookie();
            }
            return View(item);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineEdit(AdsStatisticsLabelsModel item)
        {
            if (item != null)
            {
                item.CreateTime = DateTime.Now;
                item.Creator = MemberDAO.SysCurrent.Id;
                item.ModifyTime = DateTime.Now;
                item.Modifier = MemberDAO.SysCurrent.Id;
            }
            AdsStatisticsLabelDAO.SetItem(item);
            //ViewBag.Exit = true;
            return RedirectToAction("CustomLabelLine");
        }

        public ActionResult FeeDetail(long adsCustomizedId)
        {
            AdsCustomizeModel customized = Backend.Models.DataAccess.AdvertisementDAO.GetAdsCustomizeItem(adsCustomizedId);
            IEnumerable<AdsCustomizeAccountSet> model = customized.AdsCustomizeAccountSet;

            return View(model);
        }

        private bool GetCustomLableCookie()
        {
            if (Request.Cookies[CustomLabelCookieName] != null)
            {
                return Convert.ToBoolean(Request.Cookies[CustomLabelCookieName].Value);
            }
            else
            {
                SetCustomLableCookie(true);
                return true;
            }
        }

        private void SetCustomLableCookie(bool IsShowLabel)
        {
            HttpCookie cookie = new HttpCookie(CustomLabelCookieName);
            cookie.Value = IsShowLabel.ToString();
            Response.Cookies.Add(cookie);
        }

        public string GetPlotLines(DateTime startDate, DateTime endDate)
        {
            IEnumerable<AdsStatisticsLabelsModel> lines = AdsStatisticsLabelDAO.GetItems(startDate, endDate);
            JavaScriptSerializer jss = new JavaScriptSerializer();

            return jss.Serialize(lines);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineChangeStatus(int ID, bool ShowStatus)
        {
            AdsStatisticsLabelDAO.SetItemStatus(ID, ShowStatus);
            return Json("success");
        }

        public FileResult Export(bool? privacy)
        {
            AdsStatisticsSearchModel search = Session["ExportSearch"] as AdsStatisticsSearchModel;
            if (search == null)
                search = new AdsStatisticsSearchModel();

            ViewData["Info"]                = AdvertisementStatisticsDAO.GetAll(search);
            ViewData["Privacy"]             = privacy ?? false;
            ViewData["AdsPositionMapping"]  = AdvertisementStatisticsDAO.AdsPosition;

            string html = Utility.GetViewHtml(this, "Export", null);
            
            string title = $"廣告分析報表-{DateTime.Now.ToString("yyyyMMddHHmmsss")}.xls";
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }

        public FileResult ExportDetail(bool? privacy, bool IsClick = true)
        {
            AdsDetailStatisticsSearchModel search = Session["ExportDetailSearch"] as AdsDetailStatisticsSearchModel;
            if (search == null)
                search = new AdsDetailStatisticsSearchModel();

            ViewData["DetailInfo"]                = AdvertisementStatisticsDAO.GetAllDetail(search);
            ViewData["DetailPrivacy"]             = privacy ?? false;

            string html = Utility.GetViewHtml(this, "ExportDetail", null);

            string titleCategory = IsClick ? "點擊量" : "瀏覽量";
            string title = $"廣告{titleCategory}分析報表-{DateTime.Now.ToString("yyyyMMddHHmmsss")}.xls";
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }

        public FileResult ExportAdvertisers(bool? privacy)
        {
            AdsDetailStatisticsSearchModel search = Session["ExportAdvertiserSearch"] as AdsDetailStatisticsSearchModel;
            if (search == null)
                search = new AdsDetailStatisticsSearchModel();

            ViewData["AdvertiserInfo"]            = AdvertisementStatisticsDAO.GetAllAdvertisers(search);
            ViewData["AdvertiserPrivacy"]             = privacy ?? false;

            string html = Utility.GetViewHtml(this, "ExportAdvertisers", null);

            string title = $"廣告主分析報表-{DateTime.Now.ToString("yyyyMMddHHmmsss")}.xls";
            return File(System.Text.Encoding.UTF8.GetBytes(html), "application/vnd.ms-excel", title);
        }
    }
}