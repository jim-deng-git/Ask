using System;
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
using System.Data;
namespace WorkV3.Areas.Backend.Controllers
{
    public class StatisticController : Controller
    {
        string IdentityType = CategoryType.Identity.ToString();
        string FavorityType = CategoryType.Favority.ToString();
        string CareerType = CategoryType.Career.ToString();
        string EducationType = CategoryType.Education.ToString();
        string MarriageType = CategoryType.Marriage.ToString();
        string uploadUrl = GetItem.ViewUpdUrl().TrimEnd('/') + "/Manager/";
        // GET: Backend/Statistic
        public ActionResult ListWeb(long siteId, AnalysisSearchViewModel searchViewModel)
        {
            bool IsShowLabelLine = GetCustomLableCookie();
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
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
            }
            if (searchViewModel != null && searchViewModel.IsShowCustomLableLine.HasValue)
                IsShowLabelLine = searchViewModel.IsShowCustomLableLine.Value;

            StatisticViewModel viewModel = new StatisticViewModel();
            //StatisticViewModel siteLogModel = Models.DataAccess.PagesView_LogDAO.GetWebSiteLogStatistics(SearchStartDate, SearchEndDate, SelectMembers, orderBy,
            //    pagination.PageSize, pagination.PageIndex, out totalRecord);
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
            viewModel.SearchStartDate = SearchStartDate;
            viewModel.SearchEndDate = SearchEndDate;
            viewModel.RangeType = rangeType;
            viewModel.IsShowCustomLableLine = IsShowLabelLine;
            viewModel.LabelLineList = LabelLineList;
            viewModel.StatisticConditionList = Models.DataAccess.StatisticConditionDAO.GetItems();
            if (viewModel.StatisticConditionList != null && viewModel.StatisticConditionList.Count() > 0)
            {
                foreach (Models.StatisticConditionModels model in viewModel.StatisticConditionList)
                {
                    model.StatisticValue = Models.DataAccess.StatisticConditionDAO.GetStatistionValue(siteId, model, SearchStartDate, SearchEndDate);
                    if(model.ShowStatus)
                        model.LogDailyList = Models.DataAccess.StatisticConditionDAO.GetDailyStatistionValue(siteId, model, SearchStartDate, SearchEndDate);
                }
            }
            ViewBag.SiteId = siteId;
            SetCustomLableCookie(IsShowLabelLine);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult StatisticConditionEdit(long siteId, long? id)
        {
            SetDefaultConditionCodes( siteId, id);
            StatisticConditionModels item = null;
            if (!id.HasValue)
            {
                id = WorkLib.GetItem.NewSN();
            }
            item = Models.DataAccess.StatisticConditionDAO.GetItem(id.Value);
            if (item == null)
            {
                item = new StatisticConditionModels();
                item.ID = id.Value;
            }
            ViewBag.SiteId = siteId;
            return View(item);
        }
        [HttpPost]
        public ActionResult StatisticConditionEdit(long siteId, long? id, StatisticConditionModels item)
        {
            SetDefaultConditionCodes(siteId, id);
            bool result = Models.DataAccess.StatisticConditionDAO.SetItem(item);
            if (result)
            {
                ViewBag.Exit = true;
            }
            return View(item);
        }
        [HttpGet]
        public ActionResult StatisticConditionDetailEdit(long siteId, long ConditionID, long? DetailID)
        {
            SetDefaultConditionDetailCodes(siteId, ConditionID, DetailID);
            StatisticConditionDetailModels item = null;

            if (DetailID.HasValue)
            {
                item = StatisticConditionDAO.GetDetailItem(DetailID.Value);
            }

            return View(item);
        }
        private void SetDefaultConditionDetailCodes(long siteId, long ConditionID, long? DetailID)
        {
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
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.GroupMembers = GroupMembers;
            ViewBag.IdentityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(IdentityType);
            ViewBag.FavorityCategories = Backend.Models.DataAccess.CategoryDAO.GetItems(FavorityType);
            ViewBag.CareerCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(CareerType);
            ViewBag.MarriageCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(MarriageType);
            ViewBag.EducationCategories = WorkV3.Areas.Backend.Models.DataAccess.CategoryDAO.GetItems(EducationType);
            Areas.Backend.Models.MemberShipRegSetModels MemberSetModel = Areas.Backend.Models.DataAccess.MemberShipRegSetDAO.GetItem(siteId);
            ViewBag.MemberSetModel = MemberSetModel;
            ViewBag.SiteId = siteId;
            ViewBag.ConditionID = ConditionID;

            ViewBag.HasDetailData = false;
            var StatisticConditionList = Models.DataAccess.StatisticConditionDAO.GetDetailItems(siteId, ConditionID);
            StatisticConditionModels mainModel = Areas.Backend.Models.DataAccess.StatisticConditionDAO.GetItem(ConditionID);
            if (StatisticConditionList != null && StatisticConditionList.Count() > 0)
            {
                ViewBag.HasDetailData = true;
            }
        }

        private void SetDefaultConditionCodes(long siteId, long? ConditionID)
        {
            ViewBag.UploadUrl = uploadUrl;
            ViewBag.SiteId = siteId;
        }
        [HttpPost]
        public ActionResult StatisticConditionDetailEdit(long siteId, long ConditionID, StatisticConditionDetailModels item
            , string[] Pages, string[] Manager, string[] MachineNames, string[] Browsers, string[] Sex
            , string[] ageFilter, string ageFilter_customStart, string ageFilter_customEnd, string Regions, string Address, string[] Marriage
            , string[] Education, string[] Career, string[] Identity, string[] Favority, string[] EpeprOrder)
        {
            string AnalysisItems = "";
            if (item.AnalysisType == AnalysisType.Page)
            {
                if (Pages != null)
                {
                    foreach (string page in Pages)
                    {
                        if (!string.IsNullOrEmpty(page))
                            AnalysisItems += page + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Manager)
            {
                if (Manager != null)
                {
                    foreach (string manager in Manager)
                    {
                        if (!string.IsNullOrEmpty(manager))
                            AnalysisItems += manager + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Machine)
            {
                if (MachineNames != null)
                {
                    foreach (string MachineName in MachineNames)
                    {
                        if (!string.IsNullOrEmpty(MachineName))
                            AnalysisItems += MachineName + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Browser)
            {
                if (Browsers != null)
                {
                    foreach (string Browser in Browsers)
                    {
                        if (!string.IsNullOrEmpty(Browser))
                            AnalysisItems += Browser + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Sex)
            {
                if (Sex != null)
                {
                    foreach (string sex in Sex)
                    {
                        if (!string.IsNullOrEmpty(sex))
                            AnalysisItems += sex + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Age)
            {
                if (ageFilter != null)
                {
                    foreach (string age in ageFilter)
                    {
                        if (!string.IsNullOrEmpty(age))
                        {
                            if (age.Contains( "other"))
                                AnalysisItems += age + ":" + ageFilter_customStart + "-" + ageFilter_customEnd + ";";
                            else
                                AnalysisItems += age + ";";
                        }
                        
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Location)
            {
                if (!string.IsNullOrEmpty(Regions))
                    AnalysisItems += Regions + ";";
                if (!string.IsNullOrEmpty(Address))
                    AnalysisItems += Address + ";";
            }
            if (item.AnalysisType == AnalysisType.Marriage)
            {
                if (Marriage != null)
                {
                    foreach (string marriage in Marriage)
                    {
                        if (!string.IsNullOrEmpty(marriage))
                            AnalysisItems += marriage + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Education)
            {
                if (Education != null)
                {
                    foreach (string education in Education)
                    {
                        if (!string.IsNullOrEmpty(education))
                            AnalysisItems += education + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Career)
            {
                if (Career != null)
                {
                    foreach (string career in Career)
                    {
                        if (!string.IsNullOrEmpty(career))
                            AnalysisItems += career + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Identity)
            {
                if (Identity != null)
                {
                    foreach (string identity in Identity)
                    {
                        if (!string.IsNullOrEmpty(identity))
                            AnalysisItems += identity + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.Favority)
            {
                if (Favority != null)
                {
                    foreach (string favority in Favority)
                    {
                        if (!string.IsNullOrEmpty(favority))
                            AnalysisItems += favority + ";";
                    }
                }
            }
            if (item.AnalysisType == AnalysisType.OrderEpaper)
            {
                if (EpeprOrder != null)
                {
                    foreach (string epeprOrder in EpeprOrder)
                    {
                        if (!string.IsNullOrEmpty(epeprOrder))
                            AnalysisItems += epeprOrder + ";";
                    }
                }
            }
            item.AnalysisItems = AnalysisItems.Trim(';');
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
                item.ID = WorkLib.GetItem.NewSN().ToString();
            bool result = Models.DataAccess.StatisticConditionDAO.SetDetailItem(item);
            if (result)
            {
                ViewBag.Exit = true;
            }
            SetDefaultConditionDetailCodes(siteId, ConditionID,long.Parse( item.ID));
            ViewBag.HasDetailData = true;
            return View(item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult StatisticConditionChangeStatus(string ID, bool ShowStatus)
        {
            Models.DataAccess.StatisticConditionDAO.SetItemStatus(long.Parse(ID), ShowStatus);
            return Json("success");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DeleteStatisticCondition(string ID)
        {
            Models.DataAccess.StatisticConditionDAO.DeleteStatisticConditionDetail(long.Parse(ID));
            return Json("success");
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
        public ActionResult SelectMenus(long SiteID, long? parentID)
        {
            IEnumerable<MenuStructure> structureList = Models.DataAccess.StatisticConditionDAO.GetMenuORPages(SiteID, parentID);
            
            return Json(structureList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailList(long siteId, long ConditionID)
        {
            IEnumerable<StatisticConditionDetailModels> modelList = Models.DataAccess.StatisticConditionDAO.GetDetailItems(siteId, ConditionID);

            return Json(modelList);
        }

        [HttpPost]
        public void StatisticConditionDel(long siteId, IEnumerable<long> ids)
        {
            Models.DataAccess.StatisticConditionDAO.DeleteCondition(ids);
        }
        [HttpPost]
        public void StatisticConditionSort(long siteId, IEnumerable<SortItem> items)
        {
            Models.DataAccess.StatisticConditionDAO.SortCondition(items);
        }


        public ActionResult PageAnalysisDetails(long ID, AnalysisSearchViewModel searchViewModel, int? index)
        {
            long siteId = 1;
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
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
            }
            if (searchViewModel != null)
            {
                rangeType = searchViewModel.RangeType;
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

            var StatisticConditionModel = Models.DataAccess.StatisticConditionDAO.GetItem(ID);
            AnalysisPageStatLogViewModel pageLogModel = new AnalysisPageStatLogViewModel();
            //AnalysisPageStatLogViewModel pageLogModel = Models.DataAccess.PagesView_LogDAO.GetWebPageStatLogStatistics(PageID, qType, SearchStartDate, SearchEndDate, orderBy,
            //        pagination.PageSize, pagination.PageIndex, out totalRecord);
            pageLogModel.TotalViewCount = Models.DataAccess.StatisticConditionDAO.GetStatistionValue(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate);
            if(StatisticConditionModel.StatisticType == StatisticType.SummaryViewCount)
                pageLogModel.SessionList  = Models.DataAccess.StatisticConditionDAO.GeViewLogs(siteId, SearchStartDate, SearchEndDate, StatisticConditionModel, orderBy,
                        pagination.PageSize, pagination.PageIndex, out totalRecord);
            if (StatisticConditionModel.StatisticType == StatisticType.MemberViewCount)
                pageLogModel.SessionList = Models.DataAccess.StatisticConditionDAO.GeMemberViewLogs(siteId, SearchStartDate, SearchEndDate, StatisticConditionModel, orderBy,
                        pagination.PageSize, pagination.PageIndex, out totalRecord);
            //pageLogModel.LogDailyList = Models.DataAccess.StatisticConditionDAO.GetDailyStatistionValue(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate);
            if (StatisticConditionModel.StatisticType == StatisticType.DailyViewCount)
                pageLogModel.SessionList = Models.DataAccess.StatisticConditionDAO.GetUserViewLogs(siteId, SearchStartDate, SearchEndDate, StatisticConditionModel, orderBy,
                        pagination.PageSize, pagination.PageIndex, out totalRecord);
            if (pageLogModel != null && pageLogModel.SessionList != null && pageLogModel.SessionList.Count() > 0)
            {
                foreach (AnalysisPageViewSessionViewModel item in pageLogModel.SessionList)
                {

                    item.Url = Url.Action("Index", "Home", new { SiteSN = item.SiteSN, PageSN = item.SN, area = string.Empty });
                }
            }
            pageLogModel.LogDailyList = Models.DataAccess.StatisticConditionDAO.GetDailyStatistionValue(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate);
            pageLogModel.Title = StatisticConditionModel.Title;
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
            pageLogModel.PageID = ID.ToString();
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
        public void ExportToXLS(long ID, string Type, AnalysisSearchViewModel searchViewModel)
        {
            DateTime SearchStartDate = DateTime.Now.AddDays(-14);
            DateTime SearchEndDate = DateTime.Now;
            string SelectMembers = "";
            if (!string.IsNullOrEmpty(searchViewModel.SearchStartDate))
            {
                SearchStartDate = DateTime.Parse(searchViewModel.SearchStartDate);
            }
            if (!string.IsNullOrEmpty(searchViewModel.SearchEndDate))
            {
                SearchEndDate = DateTime.Parse(searchViewModel.SearchEndDate);
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
            long siteId = 1;

            var StatisticConditionModel = Models.DataAccess.StatisticConditionDAO.GetItem(ID);
            DataTable pageLogTable = new DataTable();
            if(string.IsNullOrEmpty(Type) || Type == "main")
                pageLogTable = Models.DataAccess.StatisticConditionDAO.GetStatistionDetailTable(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate, orderBy, 99999, 1, out totalRecord);
            else if (Type == "user")
                pageLogTable = Models.DataAccess.StatisticConditionDAO.GetStatistionUserDetailTable(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate, orderBy, 99999, 1, out totalRecord);
            else if (Type == "member")
                pageLogTable = Models.DataAccess.StatisticConditionDAO.GetStatistionMemberDetailTable(siteId, StatisticConditionModel, SearchStartDate, SearchEndDate, orderBy, 99999, 1, out totalRecord);
           
            if (pageLogTable != null)
            {
                pageLogTable.Columns.Add("裝置");
                pageLogTable.Columns.Add("目標網址");
                foreach (DataRow pageRow in pageLogTable.Rows)
                {
                    pageRow["裝置"] = WorkV3.Areas.Backend.ViewModels.AnalysisPageLogViewModel.GetOSNumber(pageRow["UserAgent"].ToString());
                    pageRow["目標網址"] = Url.Action("Index", "Home", new { SiteSN = pageRow["SiteSN"].ToString(), PageSN = pageRow["SN"].ToString(), area = string.Empty });
                }
                string[] removeColumns = "MenuTitle,No,SN,SiteID,SiteSN,ID,PagesNo,Lang,Browser,UserAgent,SiteID1,MemberID,IPNum,ReferrerUrlPageNo,Photo,ROWSTAT".Split(',') ;
                foreach (string removeColumn in removeColumns)
                {
                    if (pageLogTable.Columns.Contains(removeColumn))
                        pageLogTable.Columns.Remove(removeColumn);
                }
                pageLogTable.Columns["ReferrerUrl"].ColumnName = "來源網址";
                pageLogTable.Columns["ReferrerUrlTitle"].ColumnName = "來源網址名稱";
                pageLogTable.Columns["Title"].ColumnName = "目標網址名稱";
                pageLogTable.Columns["StaySeconds"].ColumnName = "停留秒數";
                pageLogTable.Columns["MemberInfo"].ColumnName = "會員姓名 / 帳號";
                pageLogTable.Columns["Name"].ColumnName = "創建者";
                pageLogTable.Columns["AddTime"].ColumnName = "進入時間";
                pageLogTable.Columns["來源網址"].SetOrdinal(0);
                pageLogTable.Columns["來源網址名稱"].SetOrdinal(1);
                pageLogTable.Columns["目標網址"].SetOrdinal(2);
                pageLogTable.Columns["目標網址名稱"].SetOrdinal(3);
                pageLogTable.Columns["創建者"].SetOrdinal(4);
                pageLogTable.Columns["進入時間"].SetOrdinal(5);
                pageLogTable.Columns["停留秒數"].SetOrdinal(6);
                pageLogTable.Columns["SessionID"].SetOrdinal(7);
                pageLogTable.Columns["裝置"].SetOrdinal(8);
                pageLogTable.Columns["會員姓名 / 帳號"].SetOrdinal(9);
                pageLogTable.Columns["IP"].SetOrdinal(10);
                string sheetName = string.Format("條件{2}-{0}~{1}比較分析表",
                    SearchStartDate.ToString("yyyyMMdd"), SearchEndDate.ToString("yyyyMMdd"), StatisticConditionModel.Title);
                WorkLib.uXls.DTExpExcel(pageLogTable, string.Format("{0}", sheetName), sheetName);
            }
        }
    }
}