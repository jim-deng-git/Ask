using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Golbal;
using WorkV3.Common;
using WorkLib;
using WorkV3.Areas.Backend.ViewModels;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Models.DataAccess;
namespace WorkV3.Areas.Backend.Controllers
{
    public class MemberAnalysisController : Controller
    {

        string uploadUrl = GetItem.ViewUpdUrl().TrimEnd('/') + "/Member/";
        public ActionResult Logs(MemberAnalysisSearchViewModel searchViewModel)
        {
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn =  SortColumn.TotalViewCount,
                SortDesc =  SortDesc.Desc
            };
            bool IsShowLabelLine = GetCustomLableCookie();
            DateTime SearchStartDate = DateTime.Now.AddMonths(-2);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;
            ViewBag.UploadUrl = uploadUrl;
            List<ViewModels.MemberAnalysisDailyLogViewModel> dailyLogList = Models.DataAccess.MemberShipLoginLogsDAO.GetMemberLoginStatistics(SearchStartDate, SearchEndDate);
            List<ViewModels.MemberAnalysisMonthlyLogViewModel> monthLogList = Models.DataAccess.MemberShipLoginLogsDAO.GetMonthMemberLoginStatistics(SearchStartDate, SearchEndDate);
            MemberAnalysisLogViewModel logModel = new MemberAnalysisLogViewModel();
            logModel.SearchStartDate = SearchStartDate;
            logModel.SearchEndDate = SearchEndDate;
            logModel.LogDailyList = dailyLogList;
            logModel.LogMonthList = monthLogList;
            List<MemberAnalysisLogCustomLabelLineViewModel> LabelLineList = new List<MemberAnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.MemberShipLogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (MemberShipLogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.MemberAnalysisLogCustomLabelLineViewModel() {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = model.LabelColor
                    });
                }
            }
            logModel.LabelLineList = LabelLineList;
            SetCustomLableCookie(IsShowLabelLine);
            logModel.IsShowCustomLableLine = IsShowLabelLine;
            return View(logModel);
        }
        public ActionResult FieldAnalysis(MemberAnalysisSearchViewModel searchViewModel)
        {
            long SiteID = PageCache.SiteID;
            MemberFieldAnalysisViewModel viewModel = new MemberFieldAnalysisViewModel();
            DateTime SearchStartDate = DateTime.Now.AddMonths(-2);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            viewModel.SearchStartDate = SearchStartDate;
            viewModel.SearchEndDate = SearchEndDate;
            IEnumerable<ViewModels.MemberFieldViewModel> fieldList = Models.DataAccess.MemberShipLoginLogsDAO.GetMemberFieldStatistics(SiteID, SearchStartDate, SearchEndDate);
            viewModel.FieldStatisticList = fieldList;
            return View(viewModel);
        }
        private void SetCustomLableCookie(bool IsShowLabel)
        {
            HttpCookie cookie = new HttpCookie("MemberShowCustomLabel");
            cookie.Value = IsShowLabel.ToString();
            Response.Cookies.Add(cookie);
        }
        private bool GetCustomLableCookie()
        {
            if (Request.Cookies["MemberShowCustomLabel"] != null)
            {
                return Convert.ToBoolean(Request.Cookies["MemberShowCustomLabel"].Value);
            }
            else
            {
                SetCustomLableCookie(true);
                return true;
            }
        }

        public ActionResult CustomLabelLine(int? index, MemberShipLogStatisticLabelSearchModels search)
        {
            bool IsShowLabelLine = GetCustomLableCookie();
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    MemberShipLogStatisticLabelSearchModels prevSearch = WorkV3.Common.Utility.GetSearchValue<MemberShipLogStatisticLabelSearchModels>();
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
            IEnumerable<MemberShipLogStatisticLabelModels> items = MemberShipLogStatisticLabelDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            ViewBag.SelectCustomLabel = IsShowLabelLine;
            return View(items);
        }
        [HttpGet]
        public ActionResult CustomLabelLineEdit(long? id)
        {
            MemberShipLogStatisticLabelModels item = null;
            if (id != null)
            {
                item = MemberShipLogStatisticLabelDAO.GetItem((long)id);
                item.IsShowCustomLableLine = GetCustomLableCookie();
            }
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineEdit(MemberShipLogStatisticLabelModels item)
        {
            if (item != null)
            {
                item.CreateTime = DateTime.Now;
                item.Creator = MemberDAO.SysCurrent.Id;
                item.ModifyTime = DateTime.Now;
                item.Modifier = MemberDAO.SysCurrent.Id;
            }
            MemberShipLogStatisticLabelDAO.SetItem(item);
            //ViewBag.Exit = true;
             return RedirectToAction("CustomLabelLine");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineChangeStatus(int ID, bool ShowStatus)
        {
            MemberShipLogStatisticLabelDAO.SetItemStatus(ID, ShowStatus);
            return Json("success");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult LogLabelLineChangeStatus(bool ShowStatus)
        {
            SetCustomLableCookie(ShowStatus);
            return Json("success");
        }

        [HttpPost]
        public void CustomLabelLineDel(IEnumerable<long> ids)
        {
            MemberShipLogStatisticLabelDAO.DeleteItems(ids);
        }
        public void ExportToXLS_Daily(MemberAnalysisSearchViewModel searchViewModel)
        {
            DateTime SearchStartDate = DateTime.Now.AddMonths(-2);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            DataTable dailyLogTable = Models.DataAccess.MemberShipLoginLogsDAO.GetMemberLoginStatisticsTable(SearchStartDate, SearchEndDate);
            DataTable exportTable = new DataTable();

            if (dailyLogTable != null)
            {
                exportTable.Columns.Add(new DataColumn("日期", typeof(string)));
                exportTable.Columns.Add(new DataColumn("註冊數量", typeof(string)));
                exportTable.Columns.Add(new DataColumn("登入數量", typeof(string)));
                foreach (DataRow row in dailyLogTable.Rows)
                {
                    decimal MemberCount = 0, LoginCount = 0;
                    if (!string.IsNullOrEmpty(row["MemberCount"].ToString()))
                        MemberCount = decimal.Parse(row["MemberCount"].ToString());
                    if (!string.IsNullOrEmpty(row["LoginCount"].ToString()))
                        LoginCount = decimal.Parse(row["LoginCount"].ToString());
                    DataRow newRow = exportTable.NewRow();
                    newRow[0] = row["LogDate"].ToString();
                    newRow[1] = MemberCount.ToString("N0");
                    newRow[2] = LoginCount.ToString("N0");
                    exportTable.Rows.Add(newRow);
                }
                string sheetName = string.Format("{0}~{1}日統計量",
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                WorkLib.uXls.DTExpExcel(exportTable, string.Format("{0}", sheetName), sheetName);
            }
        }
        public void ExportToXLS_Month(MemberAnalysisSearchViewModel searchViewModel)
        {
            DateTime SearchStartDate = DateTime.Now.AddMonths(-2);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            DataTable dailyLogTable = Models.DataAccess.MemberShipLoginLogsDAO.GetMonthMemberLoginStatisticsTable(SearchStartDate, SearchEndDate);
            DataTable exportTable = new DataTable();
            if (dailyLogTable != null)
            {
                decimal TotalMemberCount = 0, TotalLoginCount = 0;
                foreach (DataRow row in dailyLogTable.Rows)
                {
                    decimal MemberCount = 0, LoginCount = 0;
                    if (!string.IsNullOrEmpty(row["MemberCount"].ToString()))
                        MemberCount = decimal.Parse(row["MemberCount"].ToString());
                    if (!string.IsNullOrEmpty(row["LoginCount"].ToString()))
                        LoginCount = decimal.Parse(row["LoginCount"].ToString());
                    TotalMemberCount += MemberCount;
                    TotalLoginCount += LoginCount;
                }
                exportTable.Columns.Add(new DataColumn("月份", typeof(string)));
                exportTable.Columns.Add(new DataColumn("註冊數量", typeof(decimal)));
                exportTable.Columns.Add(new DataColumn("註冊百分比", typeof(decimal)));
                exportTable.Columns.Add(new DataColumn("登入數量", typeof(decimal)));
                exportTable.Columns.Add(new DataColumn("登入百分比", typeof(decimal)));
              
                foreach (DataRow row in dailyLogTable.Rows)
                {
                    decimal MemberCount = 0, LoginCount = 0;
                    decimal MemberCountPer = 0, LoginCountPer = 0;
                    if (!string.IsNullOrEmpty(row["MemberCount"].ToString()))
                        MemberCount = decimal.Parse(row["MemberCount"].ToString());
                    if (!string.IsNullOrEmpty(row["LoginCount"].ToString()))
                        LoginCount = decimal.Parse(row["LoginCount"].ToString());
                    if (TotalMemberCount > 0 && MemberCount != 0)
                    {
                        MemberCountPer = Math.Round(100 * MemberCount / TotalMemberCount, 2);
                    }
                    if (TotalLoginCount > 0 && LoginCount != 0)
                    {
                        LoginCountPer = Math.Round(100 * LoginCount / TotalLoginCount, 2);
                    }
                    DataRow newRow = exportTable.NewRow();
                    newRow[0] = row["LogMonth"].ToString();
                    newRow[1] = MemberCount.ToString("N0");
                    newRow[2] = MemberCountPer.ToString("N2");
                    newRow[3] = LoginCount.ToString("N0");
                    newRow[4] = LoginCountPer.ToString("N2");
                    exportTable.Rows.Add(newRow);
                }
                string sheetName = string.Format("{0}~{1}月統計量",
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                WorkLib.uXls.DTExpExcel(exportTable, string.Format("{0}", sheetName), sheetName);
            }
        }
        public void ExportToXLS_Fields(MemberAnalysisSearchViewModel searchViewModel)
        {
            long SiteID = PageCache.SiteID;
            DateTime SearchStartDate = DateTime.Now.AddMonths(-2);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            IEnumerable<ViewModels.MemberFieldViewModel> fieldList = Models.DataAccess.MemberShipLoginLogsDAO.GetMemberFieldStatistics(SiteID, SearchStartDate, SearchEndDate);
           DataTable exportTable = new DataTable();
            exportTable.Columns.Add(new DataColumn("標題", typeof(string)));
            exportTable.Columns.Add(new DataColumn("說明文字", typeof(string)));
            exportTable.Columns.Add(new DataColumn("提示", typeof(string)));
            exportTable.Columns.Add(new DataColumn("選項", typeof(string)));
            exportTable.Columns.Add(new DataColumn("數量", typeof(long)));
            exportTable.Columns.Add(new DataColumn("百分比", typeof(decimal)));
            if (fieldList != null && fieldList.Count() > 0)
            {
                foreach (ViewModels.MemberFieldViewModel fieldModel in fieldList)
                {
                    for (int i = 0; i < fieldModel.StatisticList.Count; i++)
                    {
                        MemberFieldStatisticViewModel sModel = fieldModel.StatisticList.ElementAt(i);
                        DataRow newRow = exportTable.NewRow();
                        if (i == 0)
                        {
                            newRow[0] = fieldModel.FieldName;
                        }
                        else
                        {
                            newRow[0] = "";
                        }
                        newRow[1] = "";
                        newRow[2] = "";
                        newRow[3] = sModel.FieldValue;
                        newRow[4] = sModel.Count;
                        newRow[5] = sModel.Percentage;
                        exportTable.Rows.Add(newRow);
                    }
                }
                string sheetName = string.Format("{0}~{1}會員資料統計分析",
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                WorkLib.uXls.DTExpExcel(exportTable, string.Format("{0}", sheetName), sheetName);
            }
        }
    }
}