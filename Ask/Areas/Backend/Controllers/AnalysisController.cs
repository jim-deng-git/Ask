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
    public class AnalysisController : Controller
    {
        // GET: Backend/Analysis
        public ActionResult Index()
        {

            return View();
        }

        string uploadUrl = GetItem.ViewUpdUrl().TrimEnd('/') + "/Manager/";
        public ActionResult ListWeb(AnalysisSearchViewModel searchViewModel, int? index)
        {
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AnalysisSearchViewModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AnalysisSearchViewModel>();
                    if (prevSearch != null)
                        searchViewModel = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(searchViewModel);
            }
            ViewBag.Search = searchViewModel;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.TotalViewCount,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            bool IsShowLabelLine = GetCustomLableCookie();
            string Keywords = "";
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            WorkV3.Common.DateRange rangeType = DateRange.DoubleWeeks;
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (!string.IsNullOrEmpty(searchViewModel.Keywords))
            {
                Keywords = searchViewModel.Keywords;
            }
            if (searchViewModel!= null)
            {
                rangeType = searchViewModel.RangeType;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;

            int totalRecord = 0;
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };
            ViewBag.UploadUrl = uploadUrl;
            AnalysisSiteLogViewModel siteLogModel = Models.DataAccess.PagesView_LogDAO.GetWebSiteLogStatistics(SearchStartDate, SearchEndDate, SelectMembers, Keywords, orderBy,
                pagination.PageSize, pagination.PageIndex, out totalRecord);
            List<AnalysisLogCustomLabelLineViewModel> LabelLineList = new List<AnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.LogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (LogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.AnalysisLogCustomLabelLineViewModel()
                    {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = WorkV3.Areas.Backend.Models.StatisticConditionModels.ColorBar(model.LabelColor)
                    });
                }
            }
            siteLogModel.SearchStartDate = SearchStartDate;
            siteLogModel.SearchEndDate = SearchEndDate;
            siteLogModel.Keywords = Keywords;
            siteLogModel.RangeType = rangeType;
            siteLogModel.SelectMembers = SelectMembers;
            siteLogModel.IsShowCustomLableLine = IsShowLabelLine;
            siteLogModel.LabelLineList = LabelLineList;
            siteLogModel.OrderBy = orderBy;
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            SetCustomLableCookie(IsShowLabelLine);
            return View(siteLogModel);
        }
        private void SetCustomLableCookie(bool IsShowLabel)
        {
            HttpCookie cookie = new HttpCookie("ShowCustomLabel");
            cookie.Value = IsShowLabel.ToString();
            Response.Cookies.Add(cookie);
        }
        private bool GetCustomLableCookie()
        {
            if (Request.Cookies["ShowCustomLabel"] != null)
            {
                return Convert.ToBoolean(Request.Cookies["ShowCustomLabel"].Value);
            }
            else
            {
                SetCustomLableCookie(true);
                return true;
            }
        }

        public ActionResult CustomLabelLine(int? index, LogStatisticLabelSearchModels search)
        {
            bool IsShowLabelLine = GetCustomLableCookie();
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    LogStatisticLabelSearchModels prevSearch = WorkV3.Common.Utility.GetSearchValue<LogStatisticLabelSearchModels>();
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
            IEnumerable<LogStatisticLabelModels> items = LogStatisticLabelDAO.GetItems(search, pagination.PageSize, pagination.PageIndex, out totalRecord);
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            ViewBag.SelectCustomLabel = IsShowLabelLine;
            return View(items);
        }
        [HttpGet]
        public ActionResult CustomLabelLineEdit(long? id)
        {
            LogStatisticLabelModels item = null;
            if (id != null)
            {
                item = LogStatisticLabelDAO.GetItem((long)id);
                item.IsShowCustomLableLine = GetCustomLableCookie();
            }
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineEdit(LogStatisticLabelModels item)
        {
            if (item != null)
            {
                item.CreateTime = DateTime.Now;
                item.Creator = MemberDAO.SysCurrent.Id;
                item.ModifyTime = DateTime.Now;
                item.Modifier = MemberDAO.SysCurrent.Id;
            }
            LogStatisticLabelDAO.SetItem(item);
            ViewBag.Exit = true;
            return View(item);
            //return RedirectToAction("CustomLabelLine");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CustomLabelLineChangeStatus(int ID, bool ShowStatus)
        {
            LogStatisticLabelDAO.SetItemStatus(ID, ShowStatus);
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
            LogStatisticLabelDAO.DeleteItems(ids);
        }
        public void ExportToXLS(AnalysisSearchViewModel searchViewModel)
        {
            bool IsShowLabelLine = true;
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            string SelectMembers = "", Keywords= "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (!string.IsNullOrEmpty(searchViewModel.Keywords))
            {
                Keywords = searchViewModel.Keywords;
            }
            int totalRecord = 0;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.TotalViewCount,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return;
                }
            }
            DataTable pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebPageLogStatisticsTable(SearchStartDate, SearchEndDate, orderBy, SelectMembers, Keywords, 99999, 1, out totalRecord);
            if (pageLogTable != null)
            {
                pageLogTable.Columns.Add(new DataColumn("瀏覽量總計", typeof(string)));
                pageLogTable.Columns.Add(new DataColumn("人次總計", typeof(string)));
                pageLogTable.Columns.Add(new DataColumn("會員總計", typeof(string)));
                foreach (DataRow row in pageLogTable.Rows)
                {
                    decimal TotalHits = 0, PersonCounts = 0, MemberHits = 0;
                    if (!string.IsNullOrEmpty(row["TotalHits"].ToString()))
                        TotalHits = decimal.Parse(row["TotalHits"].ToString());
                    if (!string.IsNullOrEmpty(row["PersonCounts"].ToString()))
                        PersonCounts = decimal.Parse(row["PersonCounts"].ToString());
                    if (!string.IsNullOrEmpty(row["MemberHits"].ToString()))
                        MemberHits = decimal.Parse(row["MemberHits"].ToString());
                    row["瀏覽量總計"] = TotalHits.ToString("N0");
                    row["人次總計"] = PersonCounts.ToString("N0");
                    row["會員總計"] = MemberHits.ToString("N0");
                }
                string sheetName = string.Format("{0}~{1}網頁流量分析表",
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                pageLogTable.Columns["MenuTitle"].ColumnName = "選單";
                pageLogTable.Columns["Title"].ColumnName = "頁面";
                pageLogTable.Columns["Name"].ColumnName = "創建者";
                if (pageLogTable.Columns.Contains("PagesNo"))
                    pageLogTable.Columns.Remove("PagesNo");
                if (pageLogTable.Columns.Contains("ROWSTAT"))
                    pageLogTable.Columns.Remove("ROWSTAT");
                if (pageLogTable.Columns.Contains("TotalHits"))
                    pageLogTable.Columns.Remove("TotalHits");
                if (pageLogTable.Columns.Contains("PersonCounts"))
                    pageLogTable.Columns.Remove("PersonCounts");
                if (pageLogTable.Columns.Contains("MemberHits"))
                    pageLogTable.Columns.Remove("MemberHits");
                if (pageLogTable.Columns.Contains("No"))
                    pageLogTable.Columns.Remove("No");
                if (pageLogTable.Columns.Contains("SN"))
                    pageLogTable.Columns.Remove("SN");
                if (pageLogTable.Columns.Contains("SiteID"))
                    pageLogTable.Columns.Remove("SiteID");
                if (pageLogTable.Columns.Contains("SiteSN"))
                    pageLogTable.Columns.Remove("SiteSN");
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }

        public ActionResult PageAnalysisDetails(long? PageID, AnalysisSearchViewModel searchViewModel, int? index)
        {
            ViewBag.VMemberPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(1, "Member");
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AnalysisSearchViewModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AnalysisSearchViewModel>();
                    if (prevSearch != null)
                        searchViewModel = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(searchViewModel);
            }
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            string qType = "";
            if (!string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                qType = Request.QueryString["type"];
            }
            bool IsShowLabelLine = GetCustomLableCookie();
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            WorkV3.Common.DateRange rangeType = DateRange.DoubleWeeks;
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
            }
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;
            int totalRecord = 0;
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };

            ViewBag.UploadUrl = uploadUrl;
            AnalysisPageStatLogViewModel pageLogModel = Models.DataAccess.PagesView_LogDAO.GetWebPageStatLogStatistics(PageID, qType, SearchStartDate, SearchEndDate, orderBy,
                    pagination.PageSize, pagination.PageIndex, out totalRecord);
            if (PageID.HasValue)
            {
                Models.PagesModels pageModel = Models.DataAccess.PagesDAO.GetPageInfo(PageCache.SiteID, PageID.Value);
                Models.MenusModels menuModel = Models.DataAccess.MenusDAO.GetInfo(PageCache.SiteID, pageModel.MenuID);
                pageLogModel.PageID = PageID.ToString();
                pageLogModel.Title = pageModel.Title;
                pageLogModel.MenuTitle = menuModel.Title;
            }
            else
            {
                pageLogModel.PageID = "";
                pageLogModel.Title = "全站";
                pageLogModel.MenuTitle = "";

            }
            List<AnalysisLogCustomLabelLineViewModel> LabelLineList = new List<AnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.LogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (LogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.AnalysisLogCustomLabelLineViewModel()
                    {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = model.LabelColor
                    });
                }
            }
            pageLogModel.SearchStartDate = SearchStartDate;
            pageLogModel.SearchEndDate = SearchEndDate;
            pageLogModel.RangeType = rangeType;
            pageLogModel.IsShowCustomLableLine = IsShowLabelLine;
            pageLogModel.LabelLineList = LabelLineList;
            pageLogModel.OrderBy = orderBy;
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            SetCustomLableCookie(IsShowLabelLine);
            return View(pageLogModel);
        }
        public ActionResult PageAnalysisViewList()
        {
            ViewBag.VMemberPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(1, "Member");
            return View();
        }
        public ActionResult PageAnalysisViewUserList()
        {
            ViewBag.VMemberPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(1, "Member");
            return View();
        }
        public void ExportPageSessionListToXLS(string type, long? PageID, string startDate, string endDate )
        {
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(startDate))
            {
                SearchStartDate = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                SearchEndDate = DateTime.Parse(endDate);
            }
            int totalRecord = 0;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return;
                }
            }
            DataTable pageLogTable = null;
            if (string.IsNullOrEmpty(type) || type == "main")
                pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebPageLogSessionTable(SearchStartDate, SearchEndDate, PageID, orderBy, 99999, 1, out totalRecord);
            else if (type == "user")
                pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebPageLogUserTable(SearchStartDate, SearchEndDate, PageID, orderBy, 99999, 1, false, out totalRecord);
            else if (type == "member")
                pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebPageLogUserTable(SearchStartDate, SearchEndDate, PageID, orderBy, 99999, 1, true, out totalRecord);

            string pageTitle = "";
            if (PageID.HasValue)
            {
                Models.PagesModels pageModel = Models.DataAccess.PagesDAO.GetPageInfo(PageCache.SiteID, PageID.Value);
                Models.MenusModels menuModel = Models.DataAccess.MenusDAO.GetInfo(PageCache.SiteID, pageModel.MenuID);
                pageTitle = ((!string.IsNullOrEmpty(menuModel.Title))?(menuModel.Title + "-"):"") + pageModel.Title;
            }
            else
            {
                pageTitle = "全站";
            }
            if (pageLogTable != null)
            {
                string sheetName = string.Format("{0}_{1}~{2}明細表",
                    pageTitle,
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                

                pageLogTable.Columns["SessionID"].ColumnName = "Session ID";
                if (string.IsNullOrEmpty(type) || type == "main")
                {
                    pageLogTable.Columns["AddTime"].ColumnName = "進入時間";
                    pageLogTable.Columns["StaySeconds"].ColumnName = "停留秒數";
                }
                else
                {
                    pageLogTable.Columns["AddTime"].ColumnName = "首次進入時間";
                }
                pageLogTable.Columns["Browser"].ColumnName = "瀏覽器";
                //pageLogTable.Columns["ReferrerUrl"].ColumnName = "來源網址";
                pageLogTable.Columns["MemberInfo"].ColumnName = "會員姓名/帳號";
                pageLogTable.Columns["IP"].ColumnName = "IP";
                if (pageLogTable.Columns.Contains("PagesNo"))
                    pageLogTable.Columns.Remove("PagesNo");
                if (pageLogTable.Columns.Contains("UserAgent"))
                    pageLogTable.Columns.Remove("UserAgent");
                if (pageLogTable.Columns.Contains("MemberID"))
                    pageLogTable.Columns.Remove("MemberID");
                if (pageLogTable.Columns.Contains("ReferrerUrlTitle"))
                    pageLogTable.Columns.Remove("ReferrerUrlTitle");
                if (pageLogTable.Columns.Contains("ROWSTAT"))
                    pageLogTable.Columns.Remove("ROWSTAT");
                if (pageLogTable.Rows.Count == 1)
                    pageLogTable.ImportRow(pageLogTable.Rows[0]);
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }
        public ActionResult SessionAnalysisDetails(AnalysisSearchViewModel searchViewModel, string SessionID, int? index)
        {
            ViewBag.VMemberPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(1, "Member");
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AnalysisSearchViewModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AnalysisSearchViewModel>();
                    if (prevSearch != null)
                        searchViewModel = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(searchViewModel);
            }
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            bool IsShowLabelLine = GetCustomLableCookie();
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            WorkV3.Common.DateRange rangeType = DateRange.DoubleWeeks;
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
            }
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;
            int totalRecord = 0;
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };
            List<AnalysisMemberGroupViewModel> GroupMembers = new List<AnalysisMemberGroupViewModel>();
            var groups = Models.GroupDAO.GetItems();
            foreach (Models.GroupModels group in groups)
            {
                AnalysisMemberGroupViewModel groupViewModel = new AnalysisMemberGroupViewModel()
                {
                    GroupModel = group
                };
                int memberCount = 0;
                groupViewModel.GroupMembers = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount, GroupID: group.Id.ToString());
                GroupMembers.Add(groupViewModel);
            }
            ViewBag.GroupMembers = GroupMembers;
            ViewBag.UploadUrl = uploadUrl;
            AnalysisSessionLogViewModel sessionLogModel = Models.DataAccess.PagesView_LogDAO.GetWebSessionLogStatistics(SearchStartDate, SearchEndDate, SelectMembers, orderBy,
                pagination.PageSize, pagination.PageIndex, out totalRecord, SessionID);
            if (sessionLogModel != null && sessionLogModel.LogSessionList != null && sessionLogModel.LogSessionList.Count() > 0)
            {
                foreach (AnalysisPageViewSessionViewModel item in sessionLogModel.LogSessionList)
                {
                    
                    item.Url = Url.Action("Index", "Home", new { SiteSN = item.SiteSN, PageSN = item.SN, area = string.Empty });
                }
            }
                List<AnalysisLogCustomLabelLineViewModel> LabelLineList = new List<AnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.LogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (LogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.AnalysisLogCustomLabelLineViewModel()
                    {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = model.LabelColor
                    });
                }
            }
            sessionLogModel.SearchStartDate = SearchStartDate;
            sessionLogModel.SearchEndDate = SearchEndDate;
            sessionLogModel.RangeType = rangeType;
            sessionLogModel.SelectMembers = SelectMembers;
            sessionLogModel.IsShowCustomLableLine = IsShowLabelLine;
            sessionLogModel.LabelLineList = LabelLineList;
            sessionLogModel.OrderBy = orderBy;
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            SetCustomLableCookie(IsShowLabelLine);
            return View(sessionLogModel);
        }
        public ActionResult MachineAnalysisDetails(AnalysisSearchViewModel searchViewModel, string Machine, int? index)
        {
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AnalysisSearchViewModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AnalysisSearchViewModel>();
                    if (prevSearch != null)
                        searchViewModel = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(searchViewModel);
            }
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            bool IsShowLabelLine = GetCustomLableCookie();
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            WorkV3.Common.DateRange rangeType = DateRange.DoubleWeeks;
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
            }
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;
            int totalRecord = 0;
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };
            List<AnalysisMemberGroupViewModel> GroupMembers = new List<AnalysisMemberGroupViewModel>();
            var groups = Models.GroupDAO.GetItems();
            foreach (Models.GroupModels group in groups)
            {
                AnalysisMemberGroupViewModel groupViewModel = new AnalysisMemberGroupViewModel()
                {
                    GroupModel = group
                };
                int memberCount = 0;
                groupViewModel.GroupMembers = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount, GroupID: group.Id.ToString());
                GroupMembers.Add(groupViewModel);
            }
            ViewBag.GroupMembers = GroupMembers;
            ViewBag.UploadUrl = uploadUrl;
            AnalysisMachineLogViewModel machineLogModel = Models.DataAccess.PagesView_LogDAO.GetWebMachineLogStatistics(SearchStartDate, SearchEndDate, SelectMembers, orderBy,
                pagination.PageSize, pagination.PageIndex, out totalRecord, Machine);
            if (machineLogModel != null && machineLogModel.LogSessionList != null && machineLogModel.LogSessionList.Count() > 0)
            {
                foreach (AnalysisPageViewSessionViewModel item in machineLogModel.LogSessionList)
                {

                    item.Url = Url.Action("Index", "Home", new { SiteSN = item.SiteSN, PageSN = item.SN, area = string.Empty });
                }
            }
            List<AnalysisLogCustomLabelLineViewModel> LabelLineList = new List<AnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.LogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (LogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.AnalysisLogCustomLabelLineViewModel()
                    {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = model.LabelColor
                    });
                }
            }
            machineLogModel.Machine = Machine;
            machineLogModel.SearchStartDate = SearchStartDate;
            machineLogModel.SearchEndDate = SearchEndDate;
            machineLogModel.RangeType = rangeType;
            machineLogModel.SelectMembers = SelectMembers;
            machineLogModel.IsShowCustomLableLine = IsShowLabelLine;
            machineLogModel.LabelLineList = LabelLineList;
            machineLogModel.OrderBy = orderBy;
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            SetCustomLableCookie(IsShowLabelLine);
            return View(machineLogModel);
        }
        public ActionResult MemberAnalysisDetails(AnalysisSearchViewModel searchViewModel, string MemberID, int? index)
        {
            if (Request.HttpMethod == "GET")
            {
                if (index == null)
                    WorkV3.Common.Utility.ClearSearchValue();
                else
                {
                    AnalysisSearchViewModel prevSearch = WorkV3.Common.Utility.GetSearchValue<AnalysisSearchViewModel>();
                    if (prevSearch != null)
                        searchViewModel = prevSearch;
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                WorkV3.Common.Utility.SetSearchValue(searchViewModel);
            }
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            bool IsShowLabelLine = GetCustomLableCookie();
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            WorkV3.Common.DateRange rangeType = DateRange.DoubleWeeks;
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
            }
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SelectMembers))
            {
                SelectMembers = searchViewModel.SelectMembers;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;
            int totalRecord = 0;
            Pagination pagination = new Pagination
            {
                PageIndex = index ?? 1,
                PageSize = WebInfo.PageSize
            };
            List<AnalysisMemberGroupViewModel> GroupMembers = new List<AnalysisMemberGroupViewModel>();
            var groups = Models.GroupDAO.GetItems();
            foreach (Models.GroupModels group in groups)
            {
                AnalysisMemberGroupViewModel groupViewModel = new AnalysisMemberGroupViewModel()
                {
                    GroupModel = group
                };
                int memberCount = 0;
                groupViewModel.GroupMembers = Models.DataAccess.ManagerDAO.GetItems(99999, 1, out memberCount, GroupID: group.Id.ToString());
                GroupMembers.Add(groupViewModel);
            }
            ViewBag.GroupMembers = GroupMembers;
            ViewBag.UploadUrl = uploadUrl;
            AnalysisMemberLogViewModel memberLogModel = Models.DataAccess.PagesView_LogDAO.GetWebMemberLogStatistics(SearchStartDate, SearchEndDate, SelectMembers, orderBy,
                pagination.PageSize, pagination.PageIndex, out totalRecord, MemberID);
            if (memberLogModel != null && memberLogModel.LogSessionList != null && memberLogModel.LogSessionList.Count() > 0)
            {
                foreach (AnalysisPageViewSessionViewModel item in memberLogModel.LogSessionList)
                {

                    item.Url = Url.Action("Index", "Home", new { SiteSN = item.SiteSN, PageSN = item.SN, area = string.Empty });
                }
            }
            List<AnalysisLogCustomLabelLineViewModel> LabelLineList = new List<AnalysisLogCustomLabelLineViewModel>();
            var EnabledLabelLineModels = Models.DataAccess.LogStatisticLabelDAO.GetShowLabelLine(SearchStartDate, SearchEndDate);
            if (EnabledLabelLineModels != null && EnabledLabelLineModels.Count() > 0)
            {
                foreach (LogStatisticLabelModels model in EnabledLabelLineModels)
                {
                    LabelLineList.Add(new ViewModels.AnalysisLogCustomLabelLineViewModel()
                    {
                        Title = model.Title,
                        LabelDate = model.LabelDate.ToString("MM/dd"),
                        LabelColor = model.LabelColor
                    });
                }
            }
            memberLogModel.SearchStartDate = SearchStartDate;
            memberLogModel.SearchEndDate = SearchEndDate;
            memberLogModel.RangeType = rangeType;
            memberLogModel.SelectMembers = SelectMembers;
            memberLogModel.IsShowCustomLableLine = IsShowLabelLine;
            memberLogModel.LabelLineList = LabelLineList;
            memberLogModel.OrderBy = orderBy;
            pagination.TotalRecord = totalRecord;
            ViewBag.Pagination = pagination;
            SetCustomLableCookie(IsShowLabelLine);
            return View(memberLogModel);
        }
        public void ExportSessionsToXLS(string SessionID, string startDate, string endDate, string SelectMembers)
        {
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(startDate))
            {
                SearchStartDate = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                SearchEndDate = DateTime.Parse(endDate);
            }
            int totalRecord = 0;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return;
                }
            }
            DataTable pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebSessionLogTable(SearchStartDate, SearchEndDate, orderBy, SelectMembers, 99999, 1, out totalRecord, SessionID);

            string pageTitle = "Session_"+SessionID;
            if (pageLogTable != null)
            {
                string sheetName = string.Format("{0}_{1}~{2}明細表",
                    pageTitle,
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                

                pageLogTable.Columns["Title"].ColumnName = "目標網址";
                pageLogTable.Columns["Name"].ColumnName = "創建者";
                pageLogTable.Columns["AddTime"].ColumnName = "進入時間";
                pageLogTable.Columns["ReferrerUrl"].ColumnName = "來源網址";
                pageLogTable.Columns["StaySeconds"].ColumnName = "停留秒數";
                pageLogTable.Columns["IP"].ColumnName = "IP";
                if (pageLogTable.Columns.Contains("No"))
                    pageLogTable.Columns.Remove("No");
                if (pageLogTable.Columns.Contains("MenuTitle"))
                    pageLogTable.Columns.Remove("MenuTitle");
                if (pageLogTable.Columns.Contains("SN"))
                    pageLogTable.Columns.Remove("SN");
                if (pageLogTable.Columns.Contains("Lang"))
                    pageLogTable.Columns.Remove("Lang");
                if (pageLogTable.Columns.Contains("Browser"))
                    pageLogTable.Columns.Remove("Browser");
                if (pageLogTable.Columns.Contains("UserAgent"))
                    pageLogTable.Columns.Remove("UserAgent");
                if (pageLogTable.Columns.Contains("SiteID"))
                    pageLogTable.Columns.Remove("SiteID");
                if (pageLogTable.Columns.Contains("SessionID"))
                    pageLogTable.Columns.Remove("SessionID");
                if (pageLogTable.Columns.Contains("MemberID"))
                    pageLogTable.Columns.Remove("MemberID");
                if (pageLogTable.Columns.Contains("IPNum"))
                    pageLogTable.Columns.Remove("IPNum");
                if (pageLogTable.Columns.Contains("PagesNo"))
                    pageLogTable.Columns.Remove("PagesNo");
                if (pageLogTable.Columns.Contains("ID"))
                    pageLogTable.Columns.Remove("ID");
                if (pageLogTable.Columns.Contains("ROWSTAT"))
                    pageLogTable.Columns.Remove("ROWSTAT");
                if (pageLogTable.Columns.Contains("ReferrerUrlTitle"))
                    pageLogTable.Columns.Remove("ReferrerUrlTitle");
                if (pageLogTable.Rows.Count == 1)
                    pageLogTable.ImportRow(pageLogTable.Rows[0]);
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }
        public void ExportMachineToXLS(string Machine, string startDate, string endDate, string SelectMembers)
        {
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(startDate))
            {
                SearchStartDate = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                SearchEndDate = DateTime.Parse(endDate);
            }
            int totalRecord = 0;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return;
                }
            }
            DataTable pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebMachineLogTable(SearchStartDate, SearchEndDate, orderBy, SelectMembers, 99999, 1, out totalRecord, Machine);

            string pageTitle = "Machine_" + Machine;
            if (pageLogTable != null)
            {
                string sheetName = string.Format("{0}_{1}~{2}明細表",
                    pageTitle,
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));
                

                pageLogTable.Columns["Title"].ColumnName = "目標網址";
                pageLogTable.Columns["Name"].ColumnName = "創建者";
                pageLogTable.Columns["AddTime"].ColumnName = "進入時間";
                pageLogTable.Columns["StaySeconds"].ColumnName = "停留秒數";
                pageLogTable.Columns["ReferrerUrl"].ColumnName = "來源網址";
                pageLogTable.Columns["IP"].ColumnName = "IP";
                if (pageLogTable.Columns.Contains("No"))
                    pageLogTable.Columns.Remove("No");
                if (pageLogTable.Columns.Contains("MenuTitle"))
                    pageLogTable.Columns.Remove("MenuTitle");
                if (pageLogTable.Columns.Contains("SN"))
                    pageLogTable.Columns.Remove("SN");
                if (pageLogTable.Columns.Contains("Lang"))
                    pageLogTable.Columns.Remove("Lang");
                if (pageLogTable.Columns.Contains("Browser"))
                    pageLogTable.Columns.Remove("Browser");
                if (pageLogTable.Columns.Contains("UserAgent"))
                    pageLogTable.Columns.Remove("UserAgent");
                if (pageLogTable.Columns.Contains("SiteID"))
                    pageLogTable.Columns.Remove("SiteID");
                if (pageLogTable.Columns.Contains("SessionID"))
                    pageLogTable.Columns.Remove("SessionID");
                if (pageLogTable.Columns.Contains("MemberID"))
                    pageLogTable.Columns.Remove("MemberID");
                if (pageLogTable.Columns.Contains("IPNum"))
                    pageLogTable.Columns.Remove("IPNum");
                if (pageLogTable.Columns.Contains("PagesNo"))
                    pageLogTable.Columns.Remove("PagesNo");
                if (pageLogTable.Columns.Contains("ID"))
                    pageLogTable.Columns.Remove("ID");
                if (pageLogTable.Columns.Contains("ReferrerUrlTitle"))
                    pageLogTable.Columns.Remove("ReferrerUrlTitle");
                if (pageLogTable.Columns.Contains("ROWSTAT"))
                    pageLogTable.Columns.Remove("ROWSTAT");
                if (pageLogTable.Rows.Count == 1)
                    pageLogTable.ImportRow(pageLogTable.Rows[0]);
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }
        public void ExportMemberToXLS(string MemberID, string startDate, string endDate, string SelectMembers)
        {
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            if (!string.IsNullOrEmpty(startDate))
            {
                SearchStartDate = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                SearchEndDate = DateTime.Parse(endDate);
            }
            int totalRecord = 0;
            AnalysisOrderByViewModel orderBy = new AnalysisOrderByViewModel()
            {
                SortColumn = SortColumn.AddTime,
                SortDesc = SortDesc.Desc
            };
            if (!string.IsNullOrEmpty(Request.QueryString["OrderColumn"]))
            {
                try
                {
                    SortColumn sortColumn = (SortColumn)int.Parse(Request.QueryString["OrderColumn"]);
                    orderBy.SortColumn = sortColumn;
                }
                catch
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrderSort"]))
            {
                try
                {
                    SortDesc sortDesc = (SortDesc)int.Parse(Request.QueryString["OrderSort"]);
                    orderBy.SortDesc = sortDesc;
                }
                catch
                {
                    return;
                }
            }
            DataTable pageLogTable = Models.DataAccess.PagesView_LogDAO.GetWebMemberLogTable(SearchStartDate, SearchEndDate, orderBy, SelectMembers, 99999, 1, out totalRecord, MemberID);

            string pageTitle = "Member_" + MemberID;
            if (pageLogTable != null)
            {
                string sheetName = string.Format("{0}_{1}~{2}明細表",
                    pageTitle,
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"));


                pageLogTable.Columns["Title"].ColumnName = "目標網址";
                pageLogTable.Columns["Name"].ColumnName = "創建者";
                pageLogTable.Columns["AddTime"].ColumnName = "進入時間";
                pageLogTable.Columns["StaySeconds"].ColumnName = "停留秒數";
                pageLogTable.Columns["ReferrerUrl"].ColumnName = "來源網址";
                pageLogTable.Columns["IP"].ColumnName = "IP";
                if (pageLogTable.Columns.Contains("No"))
                    pageLogTable.Columns.Remove("No");
                if (pageLogTable.Columns.Contains("MenuTitle"))
                    pageLogTable.Columns.Remove("MenuTitle");
                if (pageLogTable.Columns.Contains("SN"))
                    pageLogTable.Columns.Remove("SN");
                if (pageLogTable.Columns.Contains("Lang"))
                    pageLogTable.Columns.Remove("Lang");
                if (pageLogTable.Columns.Contains("Browser"))
                    pageLogTable.Columns.Remove("Browser");
                if (pageLogTable.Columns.Contains("UserAgent"))
                    pageLogTable.Columns.Remove("UserAgent");
                if (pageLogTable.Columns.Contains("SiteID"))
                    pageLogTable.Columns.Remove("SiteID");
                if (pageLogTable.Columns.Contains("SessionID"))
                    pageLogTable.Columns.Remove("SessionID");
                if (pageLogTable.Columns.Contains("MemberID"))
                    pageLogTable.Columns.Remove("MemberID");
                if (pageLogTable.Columns.Contains("IPNum"))
                    pageLogTable.Columns.Remove("IPNum");
                if (pageLogTable.Columns.Contains("PagesNo"))
                    pageLogTable.Columns.Remove("PagesNo");
                if (pageLogTable.Columns.Contains("ID"))
                    pageLogTable.Columns.Remove("ID");
                if (pageLogTable.Columns.Contains("ReferrerUrlTitle"))
                    pageLogTable.Columns.Remove("ReferrerUrlTitle");
                if (pageLogTable.Columns.Contains("ROWSTAT"))
                    pageLogTable.Columns.Remove("ROWSTAT");
                if (pageLogTable.Rows.Count == 1)
                    pageLogTable.ImportRow(pageLogTable.Rows[0]);
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }
    }
}