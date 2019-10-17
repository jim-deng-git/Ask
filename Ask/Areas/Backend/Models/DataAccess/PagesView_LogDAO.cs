using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class PagesView_LogDAO
    {
        private static string pageLogTableFormat = "PagesView_Log_{YM}";
        public static string GetPageHistoryTableName(int YY, int MM)
        {
            return pageLogTableFormat.Replace("{YM}", YY.ToString("0000") +MM.ToString("00"));
        }
        public static bool CheckPageHistoryTableExist(int YY, int MM)
        {
            string TableName = GetPageHistoryTableName(YY, MM);
            string Sql = @" SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@TableName", TableName);
            SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
            //WorkLib.WriteLog.Write(true, Sql + ";@TableName:" + TableName);
            if (selObj["TABLE_NAME"]!= null && !string.IsNullOrEmpty(selObj["TABLE_NAME"].ToString()))
                return true;
            return false;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.AnalysisSiteLogViewModel GetWebSiteLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, string Keywords, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string SelectMenberCond = "";
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", DateTime.Parse(startDate.ToString("yyyy-MM-dd 00:00")));
            paraList.Add("@EndDate", DateTime.Parse(endDate.ToString("yyyy-MM-dd 23:59")) );
            if(!string.IsNullOrEmpty(SelectMembers))
            {
                SelectMenberCond = " AND PagesNo IN (SELECT No FROM　Pages WHERE Creator IN ("+string.Join(",",SelectMembers.Split(';'))+") ) ";
            }
            if (!string.IsNullOrEmpty(Keywords))
            {
                SelectMenberCond += " AND PagesNo IN (SELECT No FROM　Pages WHERE Title LIKE N'%"+Keywords.Replace("'", "")+"%' ) ";
            }
            ViewModels.AnalysisSiteLogViewModel viewLogModel = new ViewModels.AnalysisSiteLogViewModel();

            int totalViewHits = 0, memberViewHits = 0, totalUserCount = 0;
            DateTime curDate = new DateTime( startDate.Year, startDate.Month,1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            while (curDate<endDate)
            {
                if (!CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    curDate = curDate.AddMonths(1);
                    continue;
                }
                string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                string Sql = string.Format(@" SELECT Count(1) AS TotalHits, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS SumPersons FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0}
                                            GROUP BY [SessionID]
	                                    ) 
	                                    AS PersonSumCounts
                                    ) AS PersonCount, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  AND MemberID IS NOT NULL AND MemberID IN  (SELECT ID FROM MemberShip)  {0}
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                    ) AS MemberHits
                            FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  {0} ", SelectMenberCond, tableName);
                SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
                
                if (!string.IsNullOrEmpty(selObj["TotalHits"].ToString()))
                {
                    totalViewHits += int.Parse(selObj["TotalHits"].ToString());
                }
                if (!string.IsNullOrEmpty(selObj["PersonCount"].ToString()))
                {
                    totalUserCount += int.Parse(selObj["PersonCount"].ToString());
                }
                if (!string.IsNullOrEmpty(selObj["MemberHits"].ToString()))
                {
                    memberViewHits += int.Parse(selObj["MemberHits"].ToString());
                }
                //WorkLib.WriteLog.Write(true, tableName);
                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(@" SELECT Count(1) AS TotalHits, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS SumPersons FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0}
                                            GROUP BY [SessionID]
	                                    ) 
	                                    AS PersonSumCounts
                                    ) AS PersonCount, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  AND MemberID IS NOT NULL AND MemberID IN  (SELECT ID FROM MemberShip)  {0}
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                    ) AS MemberHits
                            FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  {0} ", SelectMenberCond, "PagesView_Log");
            //WorkLib.WriteLog.Write(true, "PagesView_Log");
            SQLData.SelectObject currentSelObj = db.GetSelectObject(currentSql, paraList);

            if (!string.IsNullOrEmpty(currentSelObj["TotalHits"].ToString()))
            {
                totalViewHits += int.Parse(currentSelObj["TotalHits"].ToString());
            }
            if (!string.IsNullOrEmpty(currentSelObj["PersonCount"].ToString()))
            {
                totalUserCount += int.Parse(currentSelObj["PersonCount"].ToString());
            }
            if (!string.IsNullOrEmpty(currentSelObj["MemberHits"].ToString()))
            {
                memberViewHits += int.Parse(currentSelObj["MemberHits"].ToString());
            }
            viewLogModel.TotalViewCount = totalViewHits;
            viewLogModel.TotalUserCount = totalUserCount;
            viewLogModel.TotalMemberViewCount = memberViewHits;
            viewLogModel.LogDailyList = GetDailyPageLogStatistics(startDate, endDate, SelectMembers, Keywords);
            viewLogModel.LogPageList = GetWebPageLogStatistics(startDate, endDate, orderBy, SelectMembers, Keywords, pageSize, pageIndex, out recordCount);
            return viewLogModel;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.AnalysisPageStatLogViewModel GetWebPageStatLogStatistics(long? PageID, string qType, DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string Cond = "";
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", DateTime.Parse(startDate.ToString("yyyy-MM-dd 00:00:00")));
            paraList.Add("@EndDate", DateTime.Parse(endDate.ToString("yyyy-MM-dd 23:59:59")));
            if (PageID.HasValue)
            {
                paraList.Add("@PageID", PageID);
                Cond = " AND PagesNo=@PageID ";
            }
            string selectSampleSQL = @" SELECT Count(1) AS TotalHits, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS SumPersons FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  {0}
                                            GROUP BY [SessionID]
	                                    ) 
	                                    AS PersonSumCounts
                                    ) AS PersonCount, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  AND MemberID IS NOT NULL  {0}
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                    ) AS MemberHits
                            FROM [{1}] Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0} ";
            ViewModels.AnalysisPageStatLogViewModel viewLogModel = new ViewModels.AnalysisPageStatLogViewModel();
            int totalViewHits = 0, memberViewHits = 0, totalUserCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL, Cond, tableName);
                    SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
                    if (!string.IsNullOrEmpty(selObj["TotalHits"].ToString()))
                    {
                        totalViewHits += int.Parse(selObj["TotalHits"].ToString());
                    }
                    if (!string.IsNullOrEmpty(selObj["PersonCount"].ToString()))
                    {
                        totalUserCount += int.Parse(selObj["PersonCount"].ToString());
                    }
                    if (!string.IsNullOrEmpty(selObj["MemberHits"].ToString()))
                    {
                        memberViewHits += int.Parse(selObj["MemberHits"].ToString());
                    }
                }
                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL, Cond, "PagesView_Log");
            SQLData.SelectObject selCurrentObj = db.GetSelectObject(currentSql, paraList);
            if (!string.IsNullOrEmpty(selCurrentObj["TotalHits"].ToString()))
            {
                totalViewHits += int.Parse(selCurrentObj["TotalHits"].ToString());
            }
            if (!string.IsNullOrEmpty(selCurrentObj["PersonCount"].ToString()))
            {
                totalUserCount += int.Parse(selCurrentObj["PersonCount"].ToString());
            }
            if (!string.IsNullOrEmpty(selCurrentObj["MemberHits"].ToString()))
            {
                memberViewHits += int.Parse(selCurrentObj["MemberHits"].ToString());
            }
            viewLogModel.TotalViewCount = totalViewHits;
            viewLogModel.TotalUserCount = totalUserCount;
            viewLogModel.TotalMemberViewCount = memberViewHits;
            recordCount = 0;
            viewLogModel.LogDailyList = GetDailyPageSessionLogStatistics(startDate, endDate, PageID);
            if(string.IsNullOrEmpty(qType) || qType == "main")
                viewLogModel.SessionList = GetWebPageSessionLogs(startDate, endDate, PageID, orderBy, pageSize, pageIndex, out recordCount);
            else if (qType == "user")
                viewLogModel.UserList = GetWebPageUserLogs(startDate, endDate, PageID, orderBy, pageSize, pageIndex, out recordCount);
            else if (qType == "member")
                viewLogModel.MemberList = GetWebPageMemberLogs(startDate, endDate, PageID, orderBy, pageSize, pageIndex, out recordCount);
            return viewLogModel;
        }

        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageLogViewModel> GetWebPageLogStatistics(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, string Keywords, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageLogViewModel> logPagesModelList = new List<ViewModels.AnalysisPageLogViewModel>();
            DataTable dt = GetWebPageLogStatisticsTable(startDate, endDate, orderBy, SelectMembers, Keywords, pageSize, pageIndex, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logPagesModelList.Add(new ViewModels.AnalysisPageLogViewModel() {
                        SiteID = row["SiteID"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        No = row["No"].ToString(),
                        Title = string.Format("{0} > {1}", row["MenuTitle"].ToString(), row["Title"].ToString()),
                        SN = row["SN"].ToString(),
                        Creator = row["Name"].ToString(),
                        TotalViewCount = int.Parse(row["TotalHits"].ToString()),
                        TotalUserCount = int.Parse(row["PersonCounts"].ToString()),
                        TotalMemberViewCount = int.Parse(row["MemberHits"].ToString())
                    });
                }
            }
            return logPagesModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetSessionLogList(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string SessionID)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebSessionLogTable(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, SessionID);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                        Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = "",
                        MemberID = row["MemberID"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetMachineLogList(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string Machine)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebMachineLogTable(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, Machine);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                        Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = "",
                        MemberID = row["MemberID"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetMemberLogList(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string MemberID)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebMemberLogTable(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, MemberID);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                        Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = "",
                        MemberID = row["MemberID"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
       /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebPageLogStatisticsTable(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, string Keywords, int pageSize, int pageIndex, out int recordCount)
        {
            string OrderByStr = " Order By [TotalHits] DESC ";
            switch (orderBy.SortColumn)
            {
                case ViewModels.SortColumn.TotalViewCount:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By [TotalHits] DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By [TotalHits] ASC ";
                    }
                    break;
                case ViewModels.SortColumn.TotalUserCount:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By [PersonCounts] DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By [PersonCounts] ASC ";
                    }
                    break;
                case ViewModels.SortColumn.TotalMemberViewCount:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By [MemberHits] DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By [MemberHits] ASC ";
                    }
                    break;
            }
            string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = "Creator IN (" + SelectMembers.Replace(";", ",") + ")";
            }
            if (!string.IsNullOrEmpty(Keywords))
            {
                if (!string.IsNullOrEmpty(pageCreatorCondition))
                    pageCreatorCondition += " AND ";
                pageCreatorCondition += "Title  LIKE N'%"+ Keywords.Replace("'", "")+ "%'";
            }
            if (!string.IsNullOrEmpty(pageCreatorCondition))
            {
                pageCreatorCondition = " AND [PagesView_Log].PagesNo IN ( SELECT No FROM Pages WHERE "+ pageCreatorCondition+")";
            }
            //SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            //paraList.Add("@StartDate", startDate);
            //paraList.Add("@EndDate", endDate);
            string selectSampleSQL = @" 
                                                SELECT [SummaryPageLogs].[PagesNo], [SummaryPageLogs].[TotalHits],  [PersonHitsCount].[PersonCounts], [SummaryPageLogs].[MemberHits]  
                                                        FROM  ( SELECT [{3}].[PagesNo], Count(1) AS TotalHits, 
                                                                    CASE WHEN TmpMemberHits.MemberHits IS NOT NULL THEN TmpMemberHits.MemberHits ELSE 0 END AS MemberHits FROM [{3}]  
                                                                    LEFT JOIN  
                                                                        (  SELECT [PagesNo], Count(1) AS MemberHits FROM    
                                                                            ( SELECT [PagesNo], [MemberID], SUM(1) AS MemberHits FROM [{3}]  
                                                                                Where  MemberID IS NOT NULL AND [AddTime]>='{0}' AND [AddTime]<='{1}' {2} GROUP BY [MemberID], [PagesNo]
                                                                            )  AS MemberHitsData GROUP BY [PagesNo] 
                                                                        ) AS TmpMemberHits ON TmpMemberHits.[PagesNo]=[{3}].[PagesNo]
                                                                 WHERE [AddTime]>='{0}' AND [AddTime]<='{1}'  {2}  GROUP BY [{3}].[PagesNo], TmpMemberHits.MemberHits 
                                                            )  AS SummaryPageLogs 
                                                    LEFT JOIN  
                                                        ( SELECT [PagesNo], Count(1) AS PersonCounts FROM 
                                                            (   SELECT [PagesNo], SUM(1) AS SumPersons  FROM [{3}] WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} 
                                                                GROUP BY [SessionID], [PagesNo] ) AS PersonSumCounts GROUP BY [PagesNo] 
                                                            )  AS PersonHitsCount ON [PersonHitsCount].[PagesNo]=[SummaryPageLogs].[PagesNo]
                                                ";
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            DataTable dt = null;
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string mySQL = string.Format(selectSampleSQL
                           , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                           , pageCreatorCondition, tableName);
                    //         string mySQL = string.Format(@" SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[SN],[Pages].[SiteID], [Sites].[SN] AS SiteSN,  CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
                    //                                 AS Title, [Member].Name , [SummaryPageLogs].[PagesNo], [SummaryPageLogs].[TotalHits],  [PersonHitsCount].[PersonCounts], [SummaryPageLogs].[MemberHits]  FROM  ( SELECT [{3}].[PagesNo], Count(1) AS TotalHits, 
                    //                                      CASE WHEN TmpMemberHits.MemberHits IS NOT NULL THEN TmpMemberHits.MemberHits ELSE 0 END AS MemberHits FROM [{3}]  LEFT JOIN  (  SELECT [PagesNo], Count(1) AS MemberHits FROM ( SELECT [PagesNo], [MemberID], SUM(1) AS MemberHits FROM [{3}]  Where  MemberID IS NOT NULL AND [AddTime]>='{0}' AND [AddTime]<='{1}' {2} GROUP BY [MemberID], [PagesNo])  AS MemberHitsData GROUP BY [PagesNo] ) AS TmpMemberHits ON TmpMemberHits.[PagesNo]=[{3}].[PagesNo]
                    //                                     WHERE [AddTime]>='{0}' AND [AddTime]<='{1}'  {2}  GROUP BY [{3}].[PagesNo], TmpMemberHits.MemberHits )  AS SummaryPageLogs LEFT JOIN  ( SELECT [PagesNo], Count(1) AS PersonCounts FROM (   SELECT [PagesNo], SUM(1) AS SumPersons  FROM [{3}] WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} GROUP BY [SessionID], [PagesNo] ) AS PersonSumCounts GROUP BY [PagesNo] )  AS PersonHitsCount ON [PersonHitsCount].[PagesNo]=[SummaryPageLogs].[PagesNo]
                    //                                 LEFT JOIN [Pages] ON [Pages].[No]=[SummaryPageLogs].[PagesNo] LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID] LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID]  LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
                    //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = [SummaryPageLogs].[PagesNo]
                    //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = [SummaryPageLogs].[PagesNo]
                    //LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = [SummaryPageLogs].[PagesNo]
                    //                                 WHERE [Pages].[No] IS NOT NULL OR [Menus].[ID] IS NOT NULL "
                    //                         , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                    //                         , pageCreatorCondition, tableName);
                   // recordCount += GetSearchRecord(db, mySQL);
                    Sql += (Sql == "" ? "" : " UNION ") + mySQL;
                    //try
                    //{
                    //    DataTable mydt = db.GetDataTable(mySQL);
                    //    if (mydt != null)
                    //    {
                    //        if (dt == null)
                    //            dt = mydt;
                    //        else
                    //        {
                    //            foreach (DataRow mydr in mydt.Rows)
                    //                dt.ImportRow(mydr);
                    //        }
                    //    }
                    //}
                    //catch
                    //{
                    //    WorkLib.WriteLog.Write(true, mySQL);
                    //}
                }
                curDate = curDate.AddMonths(1);
            }
            // WorkLib.WriteLog.Write(true, Sql);
            //if (dt != null)
            //{
            //    DataView sortView = new DataView(dt);
            //    sortView.Sort = OrderByStr.Replace("Order By", "").Replace("[", "").Replace("]", "");
            //    dt = GetSearchTable(dt, pageSize, pageIndex);
            //}
            string currentSQL = string.Format(selectSampleSQL
                           , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                           , pageCreatorCondition, "PagesView_Log");
            Sql += (Sql == "" ? "" : " UNION ") + currentSQL;
            dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null, OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            //if (!string.IsNullOrEmpty(Sql))
            //{
            //    Sql = string.Format(@"SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[SN],[Pages].[SiteID], [Sites].[SN] AS SiteSN,  CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
            //                            AS Title, [Member].Name, Result02.[PagesNo], Result02.[TotalHits], Result02.[PersonCounts], Result02.[MemberHits] FROM (
            //                                SELECT [PagesNo], SUM([TotalHits]) AS [TotalHits], SUM([PersonCounts]) AS [PersonCounts], SUM([MemberHits]) AS [MemberHits] 
            //                                    FROM ({0})  AS Result01 GROUP BY [PagesNo] ) AS Result02
            //                                LEFT JOIN [Pages] ON [Pages].[No]=Result02.[PagesNo] LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID] LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID]  LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = Result02.[PagesNo]
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = Result02.[PagesNo]
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = Result02.[PagesNo]
            //                                ", Sql 
            //                                );
            //    recordCount = GetSearchRecord(db, Sql);
            //    Sql += OrderByStr;
            //    dt = GetSearchTable(db, Sql, pageSize, pageIndex);
            //}
            return dt;
        }
        public static DataTable GetPageAndMemberInfo( DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add(new DataColumn("MenuTitle"));
                if(!dt.Columns.Contains("SiteID"))
                    dt.Columns.Add(new DataColumn("SiteID"));
                dt.Columns.Add(new DataColumn("No"));
                dt.Columns.Add(new DataColumn("Title"));
                dt.Columns.Add(new DataColumn("SN"));
                dt.Columns.Add(new DataColumn("SiteSN"));
                dt.Columns.Add(new DataColumn("Name"));
                if (!dt.Columns.Contains("MemberID"))
                    dt.Columns.Add(new DataColumn("MemberID"));
                foreach (DataRow dr in dt.Rows)
                {
                    long pagesNo = (long)dr["PagesNo"];
                    var pageModel = PagesDAO.GetPageInfo( pagesNo);
                    if (pageModel != null)
                    {
                        dr["SiteID"] = pageModel.SiteID;
                        var siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(pageModel.SiteID);
                        var menuModel = MenusDAO.GetInfo(pageModel.SiteID, pageModel.MenuID);
                        dr["No"] = pageModel.No;
                        dr["SN"] = pageModel.SN;
                        dr["Title"] = pageModel.Title;
                        if (menuModel != null)
                            dr["MenuTitle"] = menuModel.Title;
                        if (siteModel != null)
                            dr["SiteSN"] = siteModel.SN;

                        dr["MemberID"] = pageModel.Creator;
                        var memberModel = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.GetItem(pageModel.Creator);
                        if (memberModel != null)
                        {
                            dr["Name"] = memberModel.Name;
                        }
                    }
                }
            }
            return dt;
        }
        public static int GetSearchRecord(SQLData.Database db, string Sql)
        {
            int recordCount = 0;
            string recordCountSql = $" SELECT Count(1) AS RecordCount FROM ({Sql}) as CountResult ";
            SQLData.SelectObject selObj = db.GetSelectObject(recordCountSql);

            if (selObj["RecordCount"] != null && !string.IsNullOrEmpty(selObj["RecordCount"].ToString()))
            {
                recordCount = int.Parse(selObj["RecordCount"].ToString());
            }
            return recordCount;
        }
        public static DataTable GetSearchTable(SQLData.Database db, string Sql, int pageSize, int pageIndex)
        {
            DataTable allTable = db.GetDataTable(Sql);
            DataTable dt = GetSearchTable(allTable, pageSize, pageIndex);
            return dt;
        }
        public static DataTable GetSearchTable(DataTable allTable, int pageSize, int pageIndex)
        {
            DataTable dt = allTable.Clone();
            int sIndex = (pageIndex - 1) * pageSize;
            int eIndex = pageIndex * pageSize;
            for (int i = sIndex; i < eIndex; i++)
            {
                if (i < allTable.Rows.Count)
                {
                    dt.ImportRow(allTable.Rows[i]);
                }
            }
            return dt;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebSessionLogTable(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string SessionID)
        {
            string OrderByStr = " Order By [AddTime] DESC ";
            OrderByStr = string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }
            if (!string.IsNullOrEmpty(SessionID))
            {
                pageCreatorCondition += " AND SessionID='"+ SessionID + "'";
            }
            //string selectSampleSQL = @" SELECT [Menus].[Title] AS MenuTitle, [Sites].[SN] AS SiteSN, [Pages].[No], [Pages].[SN], [Pages].[Title], [Member].Name , [{3}].*  FROM [{3}]
            //                                LEFT JOIN [Pages] ON [Pages].[No]=[{3}].[PagesNo]
            //                                LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                LEFT JOIN [Sites] ON [Sites].ID = [Pages].[SiteID] 
            //                                LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            string selectSampleSQL = @" SELECT  [{3}].*  FROM [{3}]
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            //SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            //paraList.Add("@StartDate", startDate);
            //paraList.Add("@EndDate", endDate);
            recordCount = 0;

            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string mySql = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00")
                                        , endDate.ToString("yyyy-MM-dd 23:59")
                                        , pageCreatorCondition
                                        , tableName);
                    Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + mySql;
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00")
                                        , endDate.ToString("yyyy-MM-dd 23:59")
                                        , pageCreatorCondition
                                        , "PagesView_Log");
            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;
            //recordCount = GetSearchRecord(db, Sql);
            //Sql += OrderByStr;
            //DataTable dt = GetSearchTable(db, Sql, pageSize, pageIndex);

            DataTable dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null, OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            return dt;
        }
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebMachineLogTable(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string Machine)
        {
            string OrderByStr = " Order By [AddTime] DESC ";
            OrderByStr = string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition += " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }

            pageCreatorCondition += ViewModels.AnalysisPageLogViewModel.GetMachineUserAgentCondition(Machine, " AND ");
            string selectSampleSQL = @" SELECT [{3}].*  FROM [{3}]
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  ";
            //string selectSampleSQL = @" SELECT [Menus].[Title] AS MenuTitle, [Sites].SN AS SiteSN, [Pages].[No], [Pages].[SN], [Pages].[Title], [Member].Name , [{3}].*  FROM [{3}]
            //                                LEFT JOIN [Pages] ON [Pages].[No]=[{3}].[PagesNo]
            //                                LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                LEFT JOIN [Sites] ON [Sites].ID = [Pages].[SiteID] 
            //                                LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  ";
            //SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            //paraList.Add("@StartDate", startDate);
            //paraList.Add("@EndDate", endDate);
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string mySql = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00")
                                        , endDate.ToString("yyyy-MM-dd 23:59")
                                        , pageCreatorCondition
                                        , tableName);
                    Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + mySql;
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                      , startDate.ToString("yyyy-MM-dd 00:00")
                                      , endDate.ToString("yyyy-MM-dd 23:59")
                                      , pageCreatorCondition
                                      , "PagesView_Log");
            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;
            //WorkLib.WriteLog.Write(true, Sql);
            //recordCount = GetSearchRecord(db, Sql);
            //Sql = string.Format(@"SELECT [Menus].[Title] AS MenuTitle, [Sites].SN AS SiteSN, [Pages].[No], [Pages].[SN], [Pages].[Title], [Member].Name , Results.*  FROM ({0}) AS Results
            //                                LEFT JOIN [Pages] ON [Pages].[No]=[Results].[PagesNo]
            //                                LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                LEFT JOIN [Sites] ON [Sites].ID = [Pages].[SiteID] 
            //                                LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] ", Sql);
            //Sql += OrderByStr;
            ////WorkLib.WriteLog.Write(true, Sql);
            //DataTable dt = GetSearchTable(db, Sql, pageSize, pageIndex);
            DataTable dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null,  OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            return dt;
        }
       /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebMemberLogTable(DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, string SelectMembers, int pageSize, int pageIndex, out int recordCount, string MemberID)
        {
            string OrderByStr = " Order By [AddTime] DESC ";
            OrderByStr = string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }

            pageCreatorCondition += " AND MemberID='"+MemberID+"'";
            string selectSampleSQL = @" SELECT [Menus].[Title] AS MenuTitle, [Sites].SN AS SiteSN, [Pages].SiteID, [Pages].[No], [Pages].[SN], [Pages].[Title], [Member].Name , [{3}].*  FROM [{3}]
                                            LEFT JOIN [Pages] ON [Pages].[No]=[{3}].[PagesNo]
                                            LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
                                            LEFT JOIN [Sites] ON [Sites].ID = [Pages].[SiteID] 
                                            LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            //SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            //paraList.Add("@StartDate", startDate);
            //paraList.Add("@EndDate", endDate);
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string mySql = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00")
                                        , endDate.ToString("yyyy-MM-dd 23:59")
                                        , pageCreatorCondition
                                        , tableName);
                    Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + mySql;
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                      , startDate.ToString("yyyy-MM-dd 00:00")
                                      , endDate.ToString("yyyy-MM-dd 23:59")
                                      , pageCreatorCondition
                                      , "PagesView_Log");
            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;
            recordCount = GetSearchRecord(db, Sql);
            Sql += OrderByStr;
            DataTable dt = GetSearchTable(db, Sql, pageSize, pageIndex);
            return dt;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebPageLogSessionTable(DateTime startDate, DateTime endDate, long? pageID, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string Cond = "";
            if (pageID.HasValue)
            {
                Cond = string.Format(" AND PagesNo={0} ", pageID.Value.ToString() );
            }
            string OrderByStr = " Order By [AddTime] DESC ";
            switch (orderBy.SortColumn)
            {
                default:
                case ViewModels.SortColumn.AddTime:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By [AddTime] DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By [AddTime] ASC ";
                    }
                    break;
                case ViewModels.SortColumn.StaySeconds:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By [StaySeconds] DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By [StaySeconds] ASC ";
                    }
                    break;
            }
            OrderByStr = string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            string selectSampleSQL = @" SELECT {3}.PagesNo, [Pages].SN AS PagesSN, {3}.AddTime, {3}.StaySeconds, {3}.SessionID, {3}.Browser, {3}.UserAgent,  
                                            CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo,  {3}.MemberID,
                                            [MemberShip].Photo, 
                                            {3}.IP   FROM {3}
                                            LEFT JOIN [Pages] ON [Pages].[No]=[{3}].[PagesNo]
                                            LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
                                            LEFT JOIN [MemberShip] ON [MemberShip].ID = [{3}].[MemberID] 
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
             recordCount = 0;

            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string mySql = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                        , Cond, tableName);

                    Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + mySql;
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                      , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                      , Cond, "PagesView_Log");

            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;
            recordCount = GetSearchRecord(db, Sql);
            //Sql += OrderByStr;
            DataTable dt = GetSearchTable(db, Sql, pageSize, pageIndex);
            return dt;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWebPageLogUserTable(DateTime startDate, DateTime endDate, long? pageID, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, bool IsMember, out int recordCount)
        {
            string OrderByStr = " Order By AddTime DESC ", Cond= "";
            switch (orderBy.SortColumn)
            {
                default:
                case ViewModels.SortColumn.AddTime:
                    if (orderBy.SortDesc == ViewModels.SortDesc.Desc)
                    {
                        OrderByStr = " Order By AddTime DESC ";
                    }
                    else
                    {
                        OrderByStr = " Order By AddTime ASC ";
                    }
                    break;
            }
            OrderByStr = string.Format(" Order By {0} {1}", orderBy.SortColumn, orderBy.SortDesc);
            if (IsMember)
            {
                Cond += " AND MemberID IS NOT NULL AND MemberID IN  (SELECT ID FROM MemberShip) ";
            }
            if (pageID.HasValue)
            {
                Cond += " AND PagesNo="+pageID.Value;
            }
            string selectSampleSQL = @" SELECT V.PagesNo, V.AddTime , SessionList.SessionID, V.Browser, V.UserAgent,VC.[MemberID],
                                                    CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo, V.IP,
                                                    [MemberShip].[Photo]
                                                    FROM 
                                                  (SELECT DISTINCT SessionID , 
										                first_value(id) over (partition by SessionID order by id) as [FirstID]
                                                    FROM {3} WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' 
                                                    {2} ) AS SessionList
                                                    LEFT JOIN  {3} V ON V.ID=SessionList.[FirstID]
                                                    LEFT JOIN (SELECT DISTINCT SessionID , 
										              first_value(id) over (partition by SessionID order by id) as [FirstMemberSsssionID]
                                                        FROM {3} WHERE MemberID Is Not Null) 
                                                        AS MemberSessionList ON MemberSessionList.SessionID=SessionList.SessionID
                                                    LEFT JOIN  {3} VC ON VC.ID=MemberSessionList.[FirstMemberSsssionID]
                                                    LEFT JOIN [MemberShip] ON [MemberShip].ID = VC.[MemberID]  ";
            string selectSampleSQL_Member = @" SELECT V.PagesNo, V.AddTime , V.SessionID, V.Browser, V.UserAgent,V.[MemberID],
                                            CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo, V.IP,
                                            [MemberShip].[Photo]
                                            FROM 
                                          (SELECT DISTINCT SessionID , 
										                first_value(id) over (partition by SessionID order by id) as [FirstID]
                                            FROM {3} WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' 
                                            {2} AND MemberID IS NOT NULL AND MemberID IN  (SELECT ID FROM MemberShip)  ) AS MemberList
                                            LEFT JOIN  {3} V ON V.ID=MemberList.[FirstID]
                                            LEFT JOIN [MemberShip] ON [MemberShip].ID = V.[MemberID]  ";
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable dt = new DataTable();
            string Sql = "";
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string mySql = "";
                    if (!IsMember)
                    {
                        mySql = string.Format(selectSampleSQL
                                            , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                            , Cond, tableName);
                    }
                    else
                    {
                         mySql = string.Format(selectSampleSQL_Member
                                             , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                             , Cond, tableName);
                    }
                    Sql += (string.IsNullOrEmpty(Sql) ? "" : "UNION") + mySql;
                }
                curDate = curDate.AddMonths(1);
            }
            string currentSql = "";
            if (!IsMember)
            {
                currentSql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                    , Cond, "PagesView_Log");
            }
            else
            {
                currentSql = string.Format(selectSampleSQL_Member
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                    , Cond, "PagesView_Log");
            }
            Sql += (string.IsNullOrEmpty(Sql) ? "" : "UNION") + currentSql;
            recordCount = GetSearchRecord(db, Sql);
            Sql += OrderByStr;
            dt = GetSearchTable(db, Sql, pageSize, pageIndex);
            //DataView sortView = new DataView(dt);
            //sortView.Sort = OrderByStr;
            //dt = sortView.ToTable();
            //dt = GetSearchTable(dt, pageSize, pageIndex);
            return dt;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailyPageLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, string Keywords)
        {
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }
            if (!string.IsNullOrEmpty(Keywords))
            {
                pageCreatorCondition += " AND PagesNo IN ( SELECT No FROM Pages WHERE Title LIKE N'%"+ Keywords .Replace("'","")+ "%')";
            }
            string selectSampleSQL = @" SELECT [DailyPagesView].[AddDate],
                            [DailyPagesView].[TotalHits],
                            [DailyPersonCounts].[PersonCount] AS PersonCounts,
                            CASE WHEN [MemberHitsData].[MemberHits] IS NOT NULL THEN [MemberHitsData].[MemberHits] ELSE 0 END AS MemberHits
                            FROM 
                            ( 
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) as AddDate,
	                            Count(1) AS TotalHits 
	                            FROM [{3}] 
                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
	                            GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111) 
                            ) AS [DailyPagesView]
                            LEFT JOIN 
							(
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) AS AddDate, SUM(1) AS MemberHits FROM [{3}] 
                                Where   MemberID IS NOT NULL AND [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
                                GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111), [MemberID]
                            )
	                        AS MemberHitsData
							ON [MemberHitsData].[AddDate]=[DailyPagesView].[AddDate] 
                            LEFT JOIN 
                            (
	                            SELECT AddDate, Count(1) AS PersonCount FROM
	                            (
		                            SELECT Convert(nvarchar(10), [AddTime], 111) as AddDate, SUM(1) AS SumPersons FROM [{3}] 
                                    WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
		                            GROUP BY [SessionID],
		                            Convert(nvarchar(10), [AddTime], 111)
	                            ) 
	                            AS DailyPersonSumCounts GROUP BY AddDate
                            ) AS DailyPersonCounts 
                            ON [DailyPersonCounts].[AddDate]=[DailyPagesView].[AddDate] 
                            ORDER BY [DailyPagesView].[TotalHits] DESC  ";
            List<ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            
            while (curDate < endDate)
            {
                DateTime beginDate = curDate;
                if (startDate > curDate)
                    beginDate = startDate;
                DateTime lastDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));
                if (endDate < lastDate)
                    lastDate = endDate;
                DataTable dt = null;
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, tableName);
                    dt = db.GetDataTable(Sql);
                }

                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                            TotalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                            TotalMemberViewCount = int.Parse(selLogDate[0]["MemberHits"].ToString());
                        }
                    }
                    logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = TotalViewCount,
                        TotalUserCount = TotalUserCount,
                        TotalMemberViewCount = TotalMemberViewCount
                    });
                }
                curDate = curDate.AddMonths(1);
            }

            string currentSql = string.Format(selectSampleSQL, startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, "PagesView_Log");
            DataTable current_dt = db.GetDataTable(currentSql);
            for (DateTime logDate = startDate; logDate <= endDate; logDate = logDate.AddDays(1))
            {
                int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                if (current_dt != null && current_dt.Rows.Count > 0)
                {
                    DataRow[] selLogDate = current_dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                    if (selLogDate != null && selLogDate.Length > 0)
                    {
                        TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        TotalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                        TotalMemberViewCount = int.Parse(selLogDate[0]["MemberHits"].ToString());
                    }
                    var currentLogDate = logDailyPageModellList.Where(p => p.LogDate == logDate.ToString("MM/dd"));
                    if (currentLogDate != null && currentLogDate.Count() > 0)
                    {
                        currentLogDate.ElementAt(0).TotalViewCount += TotalViewCount;
                        currentLogDate.ElementAt(0).TotalUserCount += TotalUserCount;
                        currentLogDate.ElementAt(0).TotalMemberViewCount += TotalMemberViewCount;
                    }
                    else
                    {
                        logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                        {
                            LogDate = logDate.ToString("MM/dd"),
                            TotalViewCount = TotalViewCount,
                            TotalUserCount = TotalUserCount,
                            TotalMemberViewCount = TotalMemberViewCount
                        });
                    }
                }
            }
            return logDailyPageModellList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailySessionLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, string SessionID)
        {
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition += " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }
            if (!string.IsNullOrEmpty(SessionID))
            {
                pageCreatorCondition += " AND SessionID='"+ SessionID+"'";
            }
            string selectSampleSQL = @" SELECT [DailyPagesView].[AddDate],
                            [DailyPagesView].[TotalHits]
                            FROM 
                            ( 
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) as AddDate,
	                            Count(1) AS TotalHits,
                                (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{3}] 
                                            Where [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  AND MemberID IS NOT NULL
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                    ) AS MemberHits
	                            FROM [{3}] 
                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
	                            GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111) 
                            ) AS [DailyPagesView]
                            LEFT JOIN 
                            (
	                            SELECT AddDate, Count(1) AS PersonCount FROM
	                            (
		                            SELECT Convert(nvarchar(10), [AddTime], 111) as AddDate, SUM(1) AS SumPersons FROM [{3}] 
                                    WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
		                            GROUP BY [SessionID],
		                            Convert(nvarchar(10), [AddTime], 111)
	                            ) 
	                            AS DailyPersonSumCounts GROUP BY AddDate
                            ) AS DailyPersonCounts 
                            ON [DailyPersonCounts].[AddDate]=[DailyPagesView].[AddDate] 
                            ORDER BY [DailyPagesView].[TotalHits] DESC  ";
            List <ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            while (curDate < endDate)
            {
                DataTable dt = null;
                DateTime beginDate = curDate;
                if (startDate > curDate)
                    beginDate = startDate;
                DateTime lastDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));
                if (endDate < lastDate)
                    lastDate = endDate;
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, tableName);
                    dt = db.GetDataTable(Sql);
                }

                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        }
                    }
                    logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = TotalViewCount,
                        TotalUserCount = TotalUserCount,
                        TotalMemberViewCount = TotalMemberViewCount
                    });
                }
                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, "PagesView_Log");
            DataTable current_dt = db.GetDataTable(currentSql);
            for (DateTime logDate = startDate; logDate <= endDate; logDate = logDate.AddDays(1))
            {
                int TotalViewCount = 0;
                if (current_dt != null && current_dt.Rows.Count > 0)
                {
                    DataRow[] selLogDate = current_dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                    if (selLogDate != null && selLogDate.Length > 0)
                    {
                        TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                    }
                    var currentLogDate = logDailyPageModellList.Where(p => p.LogDate == logDate.ToString("MM/dd"));
                    if (currentLogDate != null && currentLogDate.Count() > 0)
                    {
                        currentLogDate.ElementAt(0).TotalViewCount += TotalViewCount;
                    }
                    else
                    {
                        logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                        {
                            LogDate = logDate.ToString("MM/dd"),
                            TotalViewCount = TotalViewCount,
                            TotalUserCount = 0,
                            TotalMemberViewCount = 0
                        });
                    }
                }
            }
            return logDailyPageModellList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailyMachineLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, string Machine)
        {
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }
            
            pageCreatorCondition += ViewModels.AnalysisPageLogViewModel.GetMachineUserAgentCondition(Machine, " AND ");
            string selectSampleSQL = @" SELECT [DailyPagesView].[AddDate],
                            [DailyPagesView].[TotalHits]
                            FROM 
                            ( 
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) as AddDate,
	                            Count(1) AS TotalHits,
                                (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{3}] 
                                            Where  [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  AND MemberID IS NOT NULL
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                 ) AS MemberHits
	                            FROM [{3}] 
                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
	                            GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111) 
                            ) AS [DailyPagesView]
                            LEFT JOIN 
                            (
	                            SELECT AddDate, Count(1) AS PersonCount FROM
	                            (
		                            SELECT Convert(nvarchar(10), [AddTime], 111) as AddDate, SUM(1) AS SumPersons FROM [{3}] 
                                    WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
		                            GROUP BY [SessionID],
		                            Convert(nvarchar(10), [AddTime], 111)
	                            ) 
	                            AS DailyPersonSumCounts GROUP BY AddDate
                            ) AS DailyPersonCounts 
                            ON [DailyPersonCounts].[AddDate]=[DailyPagesView].[AddDate] 
                            ORDER BY [DailyPagesView].[TotalHits] DESC  ";
            List <ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();
            
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            while (curDate < endDate)
            {
                DataTable dt = null;
                   DateTime beginDate = curDate;
                if (startDate > curDate)
                    beginDate = startDate;
                DateTime lastDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));
                if (endDate < lastDate)
                    lastDate = endDate;
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, tableName);
                     dt = db.GetDataTable(Sql);
                }
                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        }
                    }
                    logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = TotalViewCount,
                        TotalUserCount = TotalUserCount,
                        TotalMemberViewCount = TotalMemberViewCount
                    });
                }
                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                   , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, "PagesView_Log");
            DataTable current_dt = db.GetDataTable(currentSql);
            for (DateTime logDate = startDate; logDate <= endDate; logDate = logDate.AddDays(1))
            {
                int TotalViewCount = 0;
                if (current_dt != null && current_dt.Rows.Count > 0)
                {
                    DataRow[] selLogDate = current_dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                    if (selLogDate != null && selLogDate.Length > 0)
                    {
                        TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                    }
                    var currentLogDate = logDailyPageModellList.Where(p => p.LogDate == logDate.ToString("MM/dd"));
                    if (currentLogDate != null && currentLogDate.Count() > 0)
                    {
                        currentLogDate.ElementAt(0).TotalViewCount += TotalViewCount;
                    }
                    else
                    {
                        logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                        {
                            LogDate = logDate.ToString("MM/dd"),
                            TotalViewCount = TotalViewCount,
                            TotalUserCount = 0,
                            TotalMemberViewCount = 0
                        });
                    }
                }
            }
            return logDailyPageModellList;
        }
          /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailyMmeberLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, string MemberID)
        {
            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }

            pageCreatorCondition = " AND MemberID='"+MemberID+"'";
            string selectSampleSQL = @" SELECT [DailyPagesView].[AddDate],
                            [DailyPagesView].[TotalHits]
                            FROM 
                            ( 
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) as AddDate,
	                            Count(1) AS TotalHits,
	                             (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{3}] 
                                            Where [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  AND MemberID IS NOT NULL
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                 ) AS MemberHits
	                            FROM [{3}] 
                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
	                            GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111) 
                            ) AS [DailyPagesView]
                            LEFT JOIN 
                            (
	                            SELECT AddDate, Count(1) AS PersonCount FROM
	                            (
		                            SELECT Convert(nvarchar(10), [AddTime], 111) as AddDate, SUM(1) AS SumPersons FROM [{3}] 
                                    WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
		                            GROUP BY [SessionID],
		                            Convert(nvarchar(10), [AddTime], 111)
	                            ) 
	                            AS DailyPersonSumCounts GROUP BY AddDate
                            ) AS DailyPersonCounts 
                            ON [DailyPersonCounts].[AddDate]=[DailyPagesView].[AddDate] 
                            ORDER BY [DailyPagesView].[TotalHits] DESC  ";
            List<ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();

            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            while (curDate < endDate)
            {
                DataTable dt = null;
                DateTime beginDate = curDate;
                if (startDate > curDate)
                    beginDate = startDate;
                DateTime lastDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));
                if (endDate < lastDate)
                    lastDate = endDate;
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, tableName);
                    dt = db.GetDataTable(Sql);
                }
                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        }
                    }
                    logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = TotalViewCount,
                        TotalUserCount = TotalUserCount,
                        TotalMemberViewCount = TotalMemberViewCount
                    });
                }
                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL
                                    , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, "PagesView_Log");
            DataTable current_dt = db.GetDataTable(currentSql);
            return logDailyPageModellList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailyPageSessionLogStatistics(DateTime startDate, DateTime endDate, long? PageNo)
        {
            string pageCreatorCondition = "";
            if (PageNo.HasValue)
            {
                pageCreatorCondition = string.Format(" AND PagesNo={0} ", PageNo.ToString());
            }
            string selectSampleSQL = @" SELECT [DailyPagesView].[AddDate],
                            [DailyPagesView].[TotalHits],
                            [DailyPersonCounts].[PersonCount] AS PersonCounts,
                            [DailyPagesView].[MemberHits]
                            FROM 
                            ( 
	                            SELECT Convert(nvarchar(10), [{3}].[AddTime], 111) as AddDate,
	                            Count(1) AS TotalHits,
	                            (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT SUM(1) AS MemberHits FROM [{3}] 
                                            Where   [AddTime]>='{0}' AND [AddTime]<='{1}' {2}  AND MemberID IS NOT NULL
                                            GROUP BY [MemberID]
	                                    ) 
	                                    AS MemberHitsData
                                    ) AS MemberHits
	                            FROM [{3}] 
                                WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
	                            GROUP BY Convert(nvarchar(10), [{3}].[AddTime], 111) 
                            ) AS [DailyPagesView]
                            LEFT JOIN 
                            (
	                            SELECT AddDate, Count(1) AS PersonCount FROM
	                            (
		                            SELECT Convert(nvarchar(10), [AddTime], 111) as AddDate, SUM(1) AS SumPersons FROM [{3}] 
                                    WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
		                            GROUP BY [SessionID],
		                            Convert(nvarchar(10), [AddTime], 111)
	                            ) 
	                            AS DailyPersonSumCounts GROUP BY AddDate
                            ) AS DailyPersonCounts 
                            ON [DailyPersonCounts].[AddDate]=[DailyPagesView].[AddDate] 
                            ORDER BY [DailyPagesView].[TotalHits] DESC  ";
            List <ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            
            while (curDate < endDate)
            {
                DateTime beginDate = curDate;
                if (startDate > curDate)
                    beginDate = startDate;
                DateTime lastDate = new DateTime(curDate.Year, curDate.Month, DateTime.DaysInMonth(curDate.Year, curDate.Month));
                if (endDate < lastDate)
                    lastDate = endDate;
                DataTable dt = null;
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string Sql = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, tableName);


                    dt = db.GetDataTable(Sql);
                }
                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int TotalViewCount = 0, TotalUserCount = 0, TotalMemberViewCount = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                            TotalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                            TotalMemberViewCount = int.Parse(selLogDate[0]["MemberHits"].ToString());
                        }
                    }
                    logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = TotalViewCount,
                        TotalUserCount = TotalUserCount,
                        TotalMemberViewCount = TotalMemberViewCount
                    });
                }

                curDate = curDate.AddMonths(1);
            }

            string currentSql = string.Format(selectSampleSQL
                        , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59"), pageCreatorCondition, "PagesView_Log");


            DataTable current_dt = db.GetDataTable(currentSql);
            for (DateTime logDate = startDate; logDate <= endDate; logDate = logDate.AddDays(1))
            {
                int TotalViewCount = 0, TotalUserCount=0, TotalMemberViewCount=0;
                if (current_dt != null && current_dt.Rows.Count > 0)
                {
                    DataRow[] selLogDate = current_dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                    if (selLogDate != null && selLogDate.Length > 0)
                    {
                        TotalViewCount = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        TotalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                        TotalMemberViewCount = int.Parse(selLogDate[0]["MemberHits"].ToString());
                    }
                    var currentLogDate = logDailyPageModellList.Where(p => p.LogDate == logDate.ToString("MM/dd"));
                    if (currentLogDate != null && currentLogDate.Count() > 0)
                    {
                        currentLogDate.ElementAt(0).TotalViewCount += TotalViewCount;
                        currentLogDate.ElementAt(0).TotalUserCount += TotalUserCount;
                        currentLogDate.ElementAt(0).TotalMemberViewCount += TotalMemberViewCount;
                    }
                    else
                    {
                        logDailyPageModellList.Add(new ViewModels.AnalysisDailyLogViewModel()
                        {
                        LogDate = logDate.ToString("MM/dd"),
                            TotalViewCount = TotalViewCount,
                            TotalUserCount = TotalUserCount,
                            TotalMemberViewCount = TotalMemberViewCount
                        });
                    }
                }
            }
            return logDailyPageModellList;
        }
        /// <summary>
        /// 取得 Session 統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetWebPageSessionLogs(DateTime startDate, DateTime endDate, long? PageID, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebPageLogSessionTable(startDate, endDate, PageID, orderBy, pageSize, pageIndex, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        SN = row["PagesSN"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
                        MemberID = row["MemberID"].ToString(),
                        MemberPhoto = row["Photo"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
        /// <summary>
        /// 取得 Session 統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetWebPageUserLogs(DateTime startDate, DateTime endDate, long? PageID, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebPageLogUserTable(startDate, endDate, PageID, orderBy, pageSize, pageIndex, false, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
                        MemberID = row["MemberID"].ToString(),
                        MemberPhoto = row["Photo"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
        /// <summary>
        /// 取得 Session 統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetWebPageMemberLogs(DateTime startDate, DateTime endDate, long? PageID, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetWebPageLogUserTable(startDate, endDate, PageID, orderBy, pageSize, pageIndex, true, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
                        MemberID = row["MemberID"].ToString(),
                        MemberPhoto = row["Photo"].ToString(),
                        IP = row["IP"].ToString()
                    });
                }
            }
            return logSessionModelList;
        }
        /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.AnalysisSessionLogViewModel GetWebSessionLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount, string SessionID)
        {
            string SelectMembersCond = "";
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", DateTime.Parse(startDate.ToString("yyyy-MM-dd 00:00")));
            paraList.Add("@EndDate", DateTime.Parse(endDate.ToString("yyyy-MM-dd 23:59")));
            paraList.Add("@SessionID", SessionID);
            if (!string.IsNullOrEmpty(SelectMembers))
                SelectMembersCond = " AND PagesNo IN (SELECT No From Pages WHERE Creator IN (" + string.Join(",", SelectMembers.Split(';'))+")) ";
            string selectSampleSQL = @" SELECT Count(1) AS TotalHits 
                            FROM [{1}] Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  AND SessionID=@SessionID {0} ";
            ViewModels.AnalysisSessionLogViewModel viewLogModel = new ViewModels.AnalysisSessionLogViewModel();
            int totalViewHits = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string Sql = string.Format(selectSampleSQL, SelectMembersCond, tableName);
                    SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
                    if (!string.IsNullOrEmpty(selObj["TotalHits"].ToString()))
                    {
                        totalViewHits += int.Parse(selObj["TotalHits"].ToString());
                    }
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL, SelectMembersCond, "PagesView_Log");
            SQLData.SelectObject selCurrentObj = db.GetSelectObject(currentSql, paraList);
            if (!string.IsNullOrEmpty(selCurrentObj["TotalHits"].ToString()))
            {
                totalViewHits += int.Parse(selCurrentObj["TotalHits"].ToString());
            }
            //DataTable dt = db.GetDataTable(Sql, paraList);
            viewLogModel.SessionID = SessionID;
            viewLogModel.TotalSessionViewCount = totalViewHits;
            viewLogModel.LogDailyList = GetDailySessionLogStatistics(startDate, endDate, SelectMembers, SessionID);
            viewLogModel.LogSessionList = GetSessionLogList(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, SessionID);
            return viewLogModel;
        }   /// <summary>
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.AnalysisMachineLogViewModel GetWebMachineLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount, string Machine)
        {
            string SelectMembersCond = "";
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate.ToString("yyyy/MM/dd 00:00"));
            paraList.Add("@EndDate", endDate.ToString("yyyy/MM/dd 23:59"));
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                SelectMembersCond += " AND PagesNo in (SELECT No FROM Pages WHERE Creator IN ("+string.Join(",",SelectMembers.Split(';'))+"))";
            }
            string cond = ViewModels.AnalysisPageLogViewModel.GetMachineUserAgentCondition(Machine, " AND ");
            string selectSampleSQL = @" SELECT Count(1) AS TotalHits 
                            FROM [{2}] Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  {0} {1}";
            ViewModels.AnalysisMachineLogViewModel viewLogModel = new ViewModels.AnalysisMachineLogViewModel();
            int totalViewHits = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql = string.Format(selectSampleSQL, cond, SelectMembersCond, tableName);
                    SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
                    if (!string.IsNullOrEmpty(selObj["TotalHits"].ToString()))
                    {
                        totalViewHits += int.Parse(selObj["TotalHits"].ToString());
                    }
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL, cond, SelectMembersCond, "PagesView_Log");
            SQLData.SelectObject selCurrentObj = db.GetSelectObject(currentSql, paraList);
            if (!string.IsNullOrEmpty(selCurrentObj["TotalHits"].ToString()))
            {
                totalViewHits += int.Parse(selCurrentObj["TotalHits"].ToString());
            }
            viewLogModel.Machine = Machine;
            viewLogModel.TotalViewCount = totalViewHits;
            viewLogModel.LogDailyList = GetDailyMachineLogStatistics(startDate, endDate, SelectMembers, Machine);
            viewLogModel.LogSessionList = GetMachineLogList(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, Machine);
            return viewLogModel;
        }
        /// 取得統計資料
        /// </summary>
        /// <returns></returns>
        public static ViewModels.AnalysisMemberLogViewModel GetWebMemberLogStatistics(DateTime startDate, DateTime endDate, string SelectMembers, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount, string MemberID)
        {
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate.ToString("yyyy/MM/dd 00:00"));
            paraList.Add("@EndDate", endDate.ToString("yyyy/MM/dd 23:59"));
            string cond = " AND MemberID='"+MemberID+"'";

            string pageCreatorCondition = "";
            if (!string.IsNullOrEmpty(SelectMembers))
            {
                pageCreatorCondition = " AND PagesNo IN ( SELECT No FROM Pages WHERE Creator IN (" + SelectMembers.Replace(";", ",") + ") )";
            }
            string selectSampleSQL = @" SELECT Count(1) AS TotalHits 
                            FROM [{2}] Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate  {0} {1} ";
            ViewModels.AnalysisMemberLogViewModel viewLogModel = new ViewModels.AnalysisMemberLogViewModel();
            int totalViewHits = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            while (curDate < endDate)
            {
                if (CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string Sql =string.Format(selectSampleSQL, cond, pageCreatorCondition, tableName);
                    SQLData.SelectObject selObj = db.GetSelectObject(Sql, paraList);
                    if (!string.IsNullOrEmpty(selObj["TotalHits"].ToString()))
                    {
                        totalViewHits += int.Parse(selObj["TotalHits"].ToString());
                    }
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSql = string.Format(selectSampleSQL, cond, pageCreatorCondition, "PagesView_Log");
            SQLData.SelectObject selCurrentObj = db.GetSelectObject(currentSql, paraList);
            if (!string.IsNullOrEmpty(selCurrentObj["TotalHits"].ToString()))
            {
                totalViewHits += int.Parse(selCurrentObj["TotalHits"].ToString());
            }
            viewLogModel.MemberID = MemberID;
            WorkV3.Models.MemberShipModels memberModel = WorkV3.Models.DataAccess.MemberShipDAO.GetItem(long.Parse(MemberID));
            if (memberModel != null)
            {
                viewLogModel.MemberName = memberModel.Name;
                viewLogModel.MemberEmail = memberModel.Email;
                viewLogModel.MemberAccount = memberModel.Account;
                viewLogModel.MemberPhoto = "";
                if (!string.IsNullOrEmpty(memberModel.Photo))
                {
                    if (memberModel.Photo.StartsWith("http"))
                        viewLogModel.MemberPhoto = memberModel.Photo;
                    else
                        viewLogModel.MemberPhoto = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(PageCache.SiteID, "Member")+ memberModel.Photo;
                }
            }
            viewLogModel.TotalViewCount = totalViewHits;
            viewLogModel.LogDailyList = GetDailyMmeberLogStatistics(startDate, endDate, SelectMembers, MemberID);
            viewLogModel.LogSessionList = GetMemberLogList(startDate, endDate, orderBy, SelectMembers, pageSize, pageIndex, out recordCount, MemberID);
            return viewLogModel;
        }
    }
}