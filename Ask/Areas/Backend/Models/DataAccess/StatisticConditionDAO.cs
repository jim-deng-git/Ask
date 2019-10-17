using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;
using System.Data.SqlClient;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class StatisticConditionDAO
    {
        /// <summary>
        /// 取得 model 內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StatisticConditionModels GetItem(long id)
        {
            return CommonDA.GetItem<StatisticConditionModels>("StatisticCondition", id);
        }
        /// <summary>
        /// 取得 model 內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StatisticConditionDetailModels GetDetailItem(long id)
        {
            StatisticConditionDetailModels item = null;
            string sql = $"Select * from [StatisticConditionDetails] Where ID={id}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                DataRow dr = datas.Rows[0];
                item = new StatisticConditionDetailModels()
                {
                    ID = dr["ID"].ToString(),
                    ConditionID = dr["ConditionID"].ToString(),
                    LogicType = (LogicType)((int)dr["LogicType"]),
                    AnalysisType = (AnalysisType)((int)dr["AnalysisType"]),
                    AnalysisItems = dr["AnalysisItems"].ToString(),
                    Sort = (int)dr["Sort"],
                    Modifier = (long)dr["ID"],
                    ModifyTime = (DateTime)dr["ModifyTime"]
                };
            }
            return item;
        }
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<StatisticConditionModels> GetItems()
        {
            List<StatisticConditionModels> items = new List<StatisticConditionModels>();

            string sql = "Select * from [StatisticCondition] Order By Sort Asc";

            List<string> where = new List<string>();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new StatisticConditionModels
                    {
                        ID = (long)dr["ID"],
                        Title = dr["Title"].ToString(),
                        LabelColor = dr["LabelColor"].ToString(),
                        StatisticType = (StatisticType)((int)dr["StatisticType"]),
                        ShowStatus = (bool)dr["ShowStatus"],
                        Creator = (long)dr["ID"],
                        CreateTime = (DateTime)dr["CreateTime"],
                        Modifier = (long)dr["ID"],
                        ModifyTime = (DateTime)dr["ModifyTime"]
                    });
                }
            }


            return items;
        }
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<StatisticConditionDetailModels> GetDetailItems(long siteID, long ConditionID)
        {
            List<StatisticConditionDetailModels> items = new List<StatisticConditionDetailModels>();

            string sql = $"Select * from [StatisticConditionDetails] WHERE ConditionID={ConditionID} Order By Sort Asc";

            List<string> where = new List<string>();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    StatisticConditionDetailModels detailModel = new StatisticConditionDetailModels()
                    {
                        ID = dr["ID"].ToString(),
                        ConditionID = dr["ConditionID"].ToString(),
                        LogicType = (LogicType)((int)dr["LogicType"]),
                        AnalysisType = (AnalysisType)((int)dr["AnalysisType"]),
                        AnalysisItems = dr["AnalysisItems"].ToString(),
                        Sort = (int)dr["Sort"],
                        Modifier = (long)dr["ID"],
                        ModifyTime = (DateTime)dr["ModifyTime"]
                    };
                    detailModel.AnalysisTypeName = detailModel.GetAnalysisTypeName(detailModel.AnalysisType);
                    detailModel.LogicTypeName = detailModel.GetLogicTypeName(detailModel.LogicType);
                    detailModel.AnalysisItemsContent = detailModel.GetAnalysisItemsContent(siteID, detailModel.AnalysisType, detailModel.AnalysisItems);
                    items.Add(detailModel);
                }
            }


            return items;
        }

        public static bool SetItem(StatisticConditionModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("StatisticCondition");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM StatisticCondition WHERE  ID='{ item.ID }' ";
            string sql2 = $"SELECT Count(1) FROM StatisticCondition  ";

            bool isNew = db.GetFirstValue(sql) == null;
            int totalObjectCount = (int)db.GetFirstValue(sql2);
            if (isNew)
            {
                tableObj["Sort"] = totalObjectCount + 1;
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.ID);
            }
            return true;
        }
        public static bool SetDetailItem(StatisticConditionDetailModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("StatisticConditionDetails");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM StatisticConditionDetails WHERE  ID='{ item.ID }' ";
            bool isNew = db.GetFirstValue(sql) == null;

            if (isNew)
            {
                tableObj["ID"] = item.ID;
                tableObj["AnalysisItems"] = string.IsNullOrEmpty(item.AnalysisItems) ? "" : item.AnalysisItems;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj["AnalysisItems"] = string.IsNullOrEmpty(item.AnalysisItems) ? "" : item.AnalysisItems;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.ID);
            }
            return true;
        }
        public static void SetItemStatus(long ID, bool ShowStatus)
        {
            StatisticConditionModels item = new StatisticConditionModels();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@ShowStatus", ShowStatus);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string updateSQL = " UPDATE StatisticCondition SET ShowStatus=@ShowStatus WHERE ID=@ID ";
            db.ExecuteNonQuery(updateSQL, paraList);
        }
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ViewModels.MenuStructure> GetMenuORPages(long? parentID)
        {
            List<ViewModels.MenuStructure> items = new List<ViewModels.MenuStructure>();

            string cond = "";
            if (parentID.HasValue)
            {
                cond = " AND ParentID=" + parentID.Value.ToString();

            }
            else
                cond = " AND ParentID=0";
            string sql = string.Format("Select * from [Menus] WHERE AreaID=2 AND ShowStatus=1 {0} Order By Sort Asc", cond);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new ViewModels.MenuStructure
                    {
                        ID = dr["ID"].ToString(),
                        Title = dr["Title"].ToString(),
                        ParentID = parentID.HasValue ? parentID.Value.ToString() : "",
                        Type = ViewModels.StructureType.Menu
                    });
                }
            }

            if (parentID.HasValue)
            {
                sql = string.Format(@"Select [Pages].No AS ID, [Pages].Title, [Article].Sort from[Article]
                                        JOIN [Cards] ON[Cards].No =[Article].CardNo
                                        JOIN [Zones] ON[Zones].No =[Cards].ZoneNo
                                        JOIN [Pages] ON[Pages].No =[Zones].PageNo
                                        JOIN Menus ON Menus.ID = Pages.MenuID
                                        WHERE [Article].IsIssue = 1 AND Menus.ShowStatus = 1
										    AND [Article].MenuID={0}
                                        UNION
                                        Select [Pages].No AS ID, [Pages].Title, [Events].Sort from[Events]
                                        JOIN [Cards] ON[Cards].No =[Events].CardNo
                                        JOIN [Zones] ON[Zones].No =[Cards].ZoneNo
                                        JOIN [Pages] ON[Pages].No =[Zones].PageNo
                                        JOIN Menus ON Menus.ID = Pages.MenuID
                                        WHERE [Events].IsIssue = 1 AND Menus.ShowStatus = 1
                                            AND [Events].MenuID={0}", parentID.Value.ToString());
                datas = db.GetDataTable(sql);
                if (datas != null)
                {
                    foreach (DataRow dr in datas.Rows)
                    {
                        items.Add(new ViewModels.MenuStructure
                        {
                            ID = dr["ID"].ToString(),
                            Title = dr["Title"].ToString(),
                            ParentID = parentID.Value.ToString(),
                            Type = ViewModels.StructureType.Page
                        });
                    }
                }
            }
            return items;
        }
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ViewModels.MenuStructure> GetMenuORPages(long siteID, long? parentID)
        {
            List<ViewModels.MenuStructure> items = new List<ViewModels.MenuStructure>();

            string cond = " AND SiteID="+siteID.ToString();
            if (parentID.HasValue)
            {
                cond += " AND ParentID=" + parentID.Value.ToString();
            }
            else
                cond += " AND ParentID=0";
            string sql = string.Format("Select * from [Menus] WHERE AreaID IN (0,1,2,3) AND DataType IS NOT NULL AND ShowStatus=1 {0} Order By Sort Asc", cond);

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new ViewModels.MenuStructure
                    {
                        ID = dr["ID"].ToString(),
                        Title = dr["Title"].ToString(),
                        ParentID = parentID.HasValue ? parentID.Value.ToString() : "",
                        Type = ViewModels.StructureType.Menu
                    });
                }
            }

            if (parentID.HasValue)
            {
                sql = string.Format(@"Select [Pages].No AS ID, [Pages].Title, [Article].Sort from[Article]
                                        JOIN [Cards] ON[Cards].No =[Article].CardNo
                                        JOIN [Zones] ON[Zones].No =[Cards].ZoneNo
                                        JOIN [Pages] ON[Pages].No =[Zones].PageNo AND [Pages].No!=[Pages].MenuID
                                        JOIN Menus ON Menus.ID = Pages.MenuID
                                        WHERE [Article].IsIssue = 1 AND Menus.ShowStatus = 1 
										    AND [Article].MenuID={0}
                                        UNION
                                        Select [Pages].No AS ID, [Pages].Title, [Events].Sort from[Events]
                                        JOIN [Cards] ON[Cards].No =[Events].CardNo
                                        JOIN [Zones] ON[Zones].No =[Cards].ZoneNo
                                        JOIN [Pages] ON[Pages].No =[Zones].PageNo AND [Pages].No!=[Pages].MenuID
                                        JOIN Menus ON Menus.ID = Pages.MenuID
                                        WHERE [Events].IsIssue = 1 AND Menus.ShowStatus = 1
                                            AND [Events].MenuID={0}", parentID.Value.ToString());
                datas = db.GetDataTable(sql);
                if (datas != null)
                {
                    foreach (DataRow dr in datas.Rows)
                    {
                        items.Add(new ViewModels.MenuStructure
                        {
                            ID = dr["ID"].ToString(),
                            Title = dr["Title"].ToString(),
                            ParentID = parentID.Value.ToString(),
                            Type = ViewModels.StructureType.Page
                        });
                    }
                }
            }
            return items;
        }

        public static int DeleteStatisticConditionDetail(long id)
        {

            string sql =
                "Delete [StatisticConditionDetails] Where ID In ({0})";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, id.ToString()));
            }

            return num;
        }
        public static MenusModels GetMenuInfo(long Id)
        {

            string sql = $"SELECT * FROM Menus WHERE ID={Id}";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                MenusModels menuModel = new MenusModels()
                {
                    ID = long.Parse(datas.Rows[0]["Id"].ToString()),
                    Title = datas.Rows[0]["Title"].ToString(),
                    ParentID = long.Parse(datas.Rows[0]["ParentID"].ToString())
                };
                return menuModel;
            }
            return null;
        }

        public static int DeleteCondition(IEnumerable<long> ids)
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            string sql =
                    @"  Delete StatisticCondition Where ID In ({0});
                        Delete StatisticConditionDetails WHERE ConditionID In ({0});
                    ";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static void SortCondition(IEnumerable<SortItem> items)
        {
            if (items == null || items.Count() == 0)
                return;

            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();

            string sql = $"Select ID From StatisticCondition Order By Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                IEnumerable<long> ids = conn.Query<long>(sql);

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = "Update StatisticCondition Set Sort = {0} Where ID = {1}\n";
                foreach (long id in ids)
                {
                    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
                    foreach (SortItem item in updateItems)
                    {
                        sortSql.AppendFormat(fmt, index++, item.ID);
                        itemList.Remove(item);
                    }

                    if (!sortIds.Contains(id))
                    {
                        sortSql.AppendFormat(fmt, index++, id);
                    }
                }

                if (itemList.Count > 0)
                {
                    foreach (SortItem item in itemList.OrderBy(item => item.Index))
                        sortSql.AppendFormat(fmt, index++, item.ID);
                }

                conn.Execute(sortSql.ToString());
            }
        }
        private static string GetStatisticCondition(long SiteID, Models.StatisticConditionModels sModel)
        {
            string statisticConditionStr = "";
            var detailConditions = DataAccess.StatisticConditionDAO.GetDetailItems(SiteID, sModel.ID);
            if (detailConditions != null && detailConditions.Count() > 0)
            {
                foreach (Models.StatisticConditionDetailModels detailCondition in detailConditions)
                {
                    if (detailCondition.AnalysisType == AnalysisType.Page ||
                        detailCondition.AnalysisType == AnalysisType.Manager ||
                        detailCondition.AnalysisType == AnalysisType.Machine ||
                        detailCondition.AnalysisType == AnalysisType.Browser ||
                        detailCondition.AnalysisType == AnalysisType.Sex ||
                        detailCondition.AnalysisType == AnalysisType.Age ||
                        detailCondition.AnalysisType == AnalysisType.Marriage ||
                        detailCondition.AnalysisType == AnalysisType.Career ||
                        detailCondition.AnalysisType == AnalysisType.Education ||
                        detailCondition.AnalysisType == AnalysisType.Identity ||
                        detailCondition.AnalysisType == AnalysisType.Favority ||
                        detailCondition.AnalysisType == AnalysisType.OrderEpaper)
                    {
                        if (detailCondition.LogicType != LogicType.None)
                            statisticConditionStr += " " + detailCondition.LogicType.ToString();
                    }
                    string analysisItem = detailCondition.AnalysisItems;
                    if (detailCondition.AnalysisType == AnalysisType.Page)
                    {
                        #region 選單/網頁
                        if (string.IsNullOrEmpty(analysisItem)) // 全部選單, 理論上與全站是一樣的意思...
                        {
                            statisticConditionStr += $" ( 1=1 )";
                        }
                        else
                        {
                            string[] pagePath = analysisItem.Split(';'); // 理論上只需取最後一個項目即可
                            long objectNo = long.Parse(pagePath[pagePath.Length - 1]);
                            MenusModels menu = DataAccess.StatisticConditionDAO.GetMenuInfo(objectNo);
                            if (menu == null)
                            {
                                PagesModels page = DataAccess.PagesDAO.GetPageInfo(SiteID, objectNo);
                                if (page != null)
                                {
                                    statisticConditionStr += $" ( PagesNo={objectNo} )";
                                }
                            }
                            else
                                statisticConditionStr += $" ( PagesNo IN (SELECT No FROM Pages where MenuID ={objectNo}) )";
                        }
                        #endregion
                    }

                    if (detailCondition.AnalysisType == AnalysisType.Manager)
                    {
                        #region 管理者
                        if (string.IsNullOrEmpty(analysisItem)) // 沒有選管理者, 理論上應不會有這樣的資訊
                        {
                            statisticConditionStr += $" ( 1=1 )";
                        }
                        else
                        {
                            statisticConditionStr += " PagesNo IN ( SELECT PagesNo FROM Pages WHERE Creator IN (" + analysisItem.Replace(";", ",") + ") )";
                        }
                        #endregion
                    }
                    if (detailCondition.AnalysisType == AnalysisType.Machine)
                    {
                        #region 裝置
                        string pageCondition = "";
                        string[] machineObjects = analysisItem.Split(';');
                        foreach (string machineObject in machineObjects)
                        {
                            string ltype = "";
                            if (!string.IsNullOrEmpty(pageCondition))
                                ltype = " OR ";
                            pageCondition += ViewModels.AnalysisPageLogViewModel.GetMachineUserAgentCondition(machineObject, ltype);
                        }
                        statisticConditionStr += pageCondition;
                        #endregion
                    }
                    if (detailCondition.AnalysisType == AnalysisType.Browser)
                    {
                        #region Browser
                        string pageCondition = "";
                        string[] browserObjects = analysisItem.Split(';');
                        foreach (string browserObject in browserObjects)
                        {
                            string ltype = "";
                            if (!string.IsNullOrEmpty(pageCondition))
                                ltype = " OR ";
                            pageCondition += ViewModels.AnalysisPageLogViewModel.GetBrowserUserAgentCondition(browserObject, ltype);
                        }
                        statisticConditionStr += " ( " + pageCondition + " ) ";
                        #endregion
                    }
                    if (detailCondition.AnalysisType == AnalysisType.Sex)
                    {
                        #region 性別
                        string pageCondition = "";
                        string[] genderObjects = analysisItem.Split(';');
                        if (string.IsNullOrEmpty(analysisItem)) // 全部性別, 理論上與全站是一樣的意思...
                        {
                            statisticConditionStr += " MemberID IS NOT NULL ";
                        }
                        else
                        {
                            foreach (string genderObject in genderObjects)
                            {
                                if (!string.IsNullOrEmpty(pageCondition))
                                    pageCondition += ",";
                                pageCondition += string.Format("'{0}'", genderObject);
                            }
                            if (!string.IsNullOrEmpty(pageCondition))
                                statisticConditionStr += $" MemberID IN ( SELECT ID FROM MemberShip WHERE Sex IN ( { pageCondition } ) )";
                        }
                        #endregion
                    }

                    if (detailCondition.AnalysisType == AnalysisType.Age)
                    {
                        #region 年紀
                        string pageCondition = "";
                        int sAge = 0, eAge = 0;
                        if (analysisItem.IndexOf("other") > 0)
                        {
                            sAge = int.Parse(analysisItem.Split(':')[1].Split('-')[0]);
                            eAge = int.Parse(analysisItem.Split(':')[1].Split('-')[1]);
                        }
                        else
                        {
                            if (ViewModels.AnalysisPageLogViewModel.Ages.Keys.Contains(analysisItem))
                            {
                                sAge = ViewModels.AnalysisPageLogViewModel.Ages[analysisItem][0];
                                eAge = ViewModels.AnalysisPageLogViewModel.Ages[analysisItem][1];
                            }
                        }
                        pageCondition = " Birthday<>'' AND Birthday IS NOT NULL ";
                        if (sAge != 0)
                            pageCondition += " AND year(convert(datetime, Birthday))<=year(getdate())-" + sAge.ToString();
                        if (eAge != 99)
                            pageCondition += " AND year(convert(datetime, Birthday))>=year(getdate())-" + eAge.ToString();

                        if (!string.IsNullOrEmpty(pageCondition))
                            statisticConditionStr += $" MemberID IN ( SELECT ID FROM MemberShip WHERE  {pageCondition}  ) ";
                        #endregion
                    }
                    if (detailCondition.AnalysisType == AnalysisType.Marriage ||
                        detailCondition.AnalysisType == AnalysisType.Career ||
                        detailCondition.AnalysisType == AnalysisType.Education ||
                        detailCondition.AnalysisType == AnalysisType.Identity ||
                        detailCondition.AnalysisType == AnalysisType.Favority)
                    {
                        #region 婚姻、學歷、職業、身份、喜好
                        if (string.IsNullOrEmpty(analysisItem)) // 沒有選類別, 理論上應不會有這樣的資訊
                        {
                            statisticConditionStr += $" ( MemberID IS NOT NULL )";
                        }
                        else
                        {
                            statisticConditionStr += " MemberID IN ( SELECT MemberShipID FROM MemberShipSetting WHERE CategoryID IN (" + analysisItem.Replace(";", ",") + ") )";
                        }
                        #endregion
                    }
                    if (detailCondition.AnalysisType == AnalysisType.OrderEpaper)
                    {
                        #region 訂閱電子報
                        if (string.IsNullOrEmpty(analysisItem)) // 沒有選類別, 理論上應不會有這樣的資訊
                        {
                            statisticConditionStr += $" ( MemberID IS NOT NULL )";
                        }
                        else
                        {
                            statisticConditionStr += @" ( MemberID IN ( SELECT Member_ID FROM EpaperSubscriber WHERE ID IN 
                                                                        (SELECT EpaperSubscriber_ID FROM EpaperSubscriberDetail WHERE EpaperType_ID IN
                                                                            (" + analysisItem.Replace(";", ",") + @")
                                                                        ) AND Member_ID IS NOT NULL  ) )";
                        }
                        #endregion
                    }
                }
                if (!string.IsNullOrEmpty(statisticConditionStr))
                    statisticConditionStr = string.Format(" AND ( {0} ) ", statisticConditionStr);
            }
            return statisticConditionStr;
        }
        public static long GetStatistionValue(long SiteID, Models.StatisticConditionModels sModel, DateTime startDate, DateTime endDate)
        {
            string statisticConditionStr = GetStatisticCondition(SiteID, sModel);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", DateTime.Parse(startDate.ToString("yyyy-MM-dd 00:00")));
            paraList.Add("@EndDate", DateTime.Parse(endDate.ToString("yyyy-MM-dd 23:59")));

            string selectSampleSQL = @" SELECT Count(1) AS TotalHits, 
                                    (
	                                    SELECT Count(1) FROM
	                                    (
		                                    SELECT Convert(nvarchar(10), [AddTime], 111) AS LogDate, [SessionID] FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0}
                                            GROUP BY [SessionID], Convert(nvarchar(10), [AddTime], 111)
	                                    ) 
	                                    AS PersonSumCounts
                                    ) AS PersonCount, 
                                    SUM(CASE WHEN MemberID IS NOT NULL THEN 1 ELSE 0 END) AS MemberHits
                                    FROM [{1}] 
                                            Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0} ";

            int totalViewHits = 0, memberViewHits = 0, totalUserCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            while (curDate < endDate)
            {
                if (PagesView_LogDAO.CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = PagesView_LogDAO.GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string SumSql = string.Format(selectSampleSQL, statisticConditionStr, tableName);
                    SQLData.SelectObject selObj = db.GetSelectObject(SumSql, paraList);
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
            string currentSumSql = string.Format(selectSampleSQL, statisticConditionStr, "PagesView_Log");
            SQLData.SelectObject currentSelObj = db.GetSelectObject(currentSumSql, paraList);
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
            if (sModel.StatisticType == StatisticType.SummaryViewCount)
                return totalViewHits;
            if (sModel.StatisticType == StatisticType.DailyViewCount)
                return totalUserCount;
            if (sModel.StatisticType == StatisticType.MemberViewCount)
                return memberViewHits;
            return 0;
        }
        public static DataTable  GetStatistionDetailTable(long SiteID, Models.StatisticConditionModels sModel, DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string statisticConditionStr = GetStatisticCondition(SiteID, sModel);
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

            recordCount = 0;
            string selectSampleSQL = @" SELECT {3}.* FROM {3}
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            DataTable dt = null;
            while (curDate < endDate)
            {
                if (PagesView_LogDAO.CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = PagesView_LogDAO.GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string mySQL = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                        , statisticConditionStr, tableName);
                    Sql += (Sql == "" ? "" : " UNION ") + mySQL;
                }

                curDate = curDate.AddMonths(1);
            }
            string currentSQL = string.Format(selectSampleSQL
                                        , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                        , statisticConditionStr, "PagesView_Log");
            Sql += (Sql == "" ? "" : " UNION ") + currentSQL;
            //Sql = string.Format(@" SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[SN], [Sites].[SN] AS SiteSN, [Member].Name, Results.*,  
            //                                CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo,  
            //                                [MemberShip].Photo ,CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
            //                                AS Title FROM ({0}) AS Results
            //                                LEFT JOIN [Pages] ON [Pages].[No]=[Results].[PagesNo]
            //                                LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                LEFT JOIN [MemberShip] ON [MemberShip].ID = [Results].[MemberID] 
            //                                LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID]
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = [Pages].[No]
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = [Pages].[No]
            //                                LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = [Pages].[No]
            //                                ", Sql);
            dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null, OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    dt.Columns.Add(new DataColumn("MenuTitle"));
            //    dt.Columns.Add(new DataColumn("No"));
            //    dt.Columns.Add(new DataColumn("Title"));
            //    dt.Columns.Add(new DataColumn("SN"));
            //    dt.Columns.Add(new DataColumn("SiteSN"));
            //    dt.Columns.Add(new DataColumn("Name"));
            //    dt.Columns.Add(new DataColumn("MemberInfo"));
            //    dt.Columns.Add(new DataColumn("Photo"));
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        long siteID = (long)dr["SiteID"];
            //        long pagesNo = (long)dr["PagesNo"];
            //        var pageModel = PagesDAO.GetPageInfo(siteID, pagesNo);
            //        if (pageModel != null)
            //        {
            //            var siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteID);
            //            var menuModel = MenusDAO.GetInfo(siteID, pageModel.MenuID);
            //            dr["No"] = pageModel.No;
            //            dr["SN"] = pageModel.SN;
            //            dr["Title"] = pageModel.Title;
            //            if (menuModel != null)
            //                dr["MenuTitle"] = menuModel.Title;
            //            if (siteModel != null)
            //                dr["SiteSN"] = siteModel.SN;
            //        }
            //        if (!string.IsNullOrEmpty(dr["MemberID"].ToString()))
            //        {
            //            long memberID = (long)dr["MemberID"];
            //            var memberModel = WorkV3.Models.DataAccess.MemberShipDAO.GetItem(memberID);
            //            if (memberModel != null)
            //            {
            //                dr["Name"] = memberModel.Name;
            //                dr["MemberInfo"] = memberModel.Account + '/' + memberModel.Name;
            //                dr["Photo"] = memberModel.Photo;
            //            }
            //        }
            //    }
            //}
            // recordCount = PagesView_LogDAO.GetSearchRecord(db, Sql);
            // Sql = string.Format(@" SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[Title], [Pages].[SN], [Pages].[SiteID], [Sites].[SN] AS SiteSN, [Member].Name, Results.*,  
            //                                 CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo,  
            //                                 [MemberShip].Photo ,CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
            //                                 AS Title FROM ({0}) AS Results
            //                                 LEFT JOIN [Pages] ON [Pages].[No]=[Results].[PagesNo]
            //                                 LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                 LEFT JOIN [MemberShip] ON [MemberShip].ID = [Results].[MemberID] 
            //                                 LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                 LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = [Pages].[No]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = [Pages].[No]
            //LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = [Pages].[No]
            //                                 {1} ", Sql, OrderByStr) ;
            // dt = PagesView_LogDAO.GetSearchTable(db, Sql, pageSize, pageIndex);
            return dt;
        }
public static DataTable GetStatistionMemberDetailTable(long SiteID, Models.StatisticConditionModels sModel, DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string statisticConditionStr = GetStatisticCondition(SiteID, sModel);
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
            string selectSampleSQL = @" SELECT * FROM {3}
                                            WHERE MemberID IS NOT NULL AND [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            DataTable dt = null;
            while (curDate < endDate)
            {
                if (PagesView_LogDAO.CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = PagesView_LogDAO.GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string mySQL = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                , statisticConditionStr, tableName);
                    Sql += (Sql == "" ? "" : " UNION ") + mySQL;
                }
                curDate = curDate.AddMonths(1);
            }
            // string Sql = string.Format(@" SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[Title], [Pages].[SN], [Pages].[SiteID], [Sites].[SN] AS SiteSN, [Member].Name, PagesView_Log.*,  
            //                                 CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo,  
            //                                 [MemberShip].Photo ,CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
            //                                 AS Title FROM PagesView_Log
            //                                 LEFT JOIN [Pages] ON [Pages].[No]=[PagesView_Log].[PagesNo]
            //                                 LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                 LEFT JOIN [MemberShip] ON [MemberShip].ID = [PagesView_Log].[MemberID] 
            //                                 LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                 LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = [Pages].[No]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = [Pages].[No]
            //LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = [Pages].[No]
            //                                 WHERE MemberID IS NOT NULL AND [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
            //                                 {3}  "
            //                     , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
            //                     , statisticConditionStr, OrderByStr);
            string currentSql = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                , statisticConditionStr, "PagesView_Log");
            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;
            //DataTable dt = db.GetPageData(Sql, pageSize, pageIndex, out recordCount);
            dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null, OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    dt.Columns.Add(new DataColumn("MenuTitle"));
            //    dt.Columns.Add(new DataColumn("No"));
            //    dt.Columns.Add(new DataColumn("Title"));
            //    dt.Columns.Add(new DataColumn("SN"));
            //    dt.Columns.Add(new DataColumn("SiteSN"));
            //    dt.Columns.Add(new DataColumn("Name"));
            //    dt.Columns.Add(new DataColumn("MemberInfo"));
            //    dt.Columns.Add(new DataColumn("Photo"));
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        long siteID = (long)dr["SiteID"];
            //        long pagesNo = (long)dr["PagesNo"];
            //        var pageModel = PagesDAO.GetPageInfo(siteID, pagesNo);
            //        if (pageModel != null)
            //        {
            //            var siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteID);
            //            var menuModel = MenusDAO.GetInfo(siteID, pageModel.MenuID);
            //            dr["No"] = pageModel.No;
            //            dr["SN"] = pageModel.SN;
            //            dr["Title"] = pageModel.Title;
            //            if (menuModel != null)
            //                dr["MenuTitle"] = menuModel.Title;
            //            if (siteModel != null)
            //                dr["SiteSN"] = siteModel.SN;
            //        }
            //        if (!string.IsNullOrEmpty(dr["MemberID"].ToString()))
            //        {
            //            long memberID = (long)dr["MemberID"];
            //            var memberModel = WorkV3.Models.DataAccess.MemberShipDAO.GetItem(memberID);
            //            if (memberModel != null)
            //            {
            //                dr["Name"] = memberModel.Name;
            //                dr["MemberInfo"] = memberModel.Account + '/' + memberModel.Name;
            //                dr["Photo"] = memberModel.Photo;
            //            }
            //        }
            //    }
            //}
            return dt;
        }
        public static DataTable GetStatistionUserDetailTable(long SiteID, Models.StatisticConditionModels sModel, DateTime startDate, DateTime endDate, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            string statisticConditionStr = GetStatisticCondition(SiteID, sModel);
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
            string selectSampleSQL = @" SELECT  * FROM {3}
                                            WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2} ";
            recordCount = 0;
            DateTime curDate = new DateTime(startDate.Year, startDate.Month, 1);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "";
            while (curDate < endDate)
            {
                if (PagesView_LogDAO.CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = PagesView_LogDAO.GetPageHistoryTableName(curDate.Year, curDate.Month);
                    string mySQL = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                , statisticConditionStr, tableName);
                    Sql += (Sql == "" ? "" : " UNION ") + mySQL;
                }

                curDate = curDate.AddMonths(1);
            }

            string currentSql = string.Format(selectSampleSQL
                                , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
                                , statisticConditionStr, "PagesView_Log");
            Sql += (string.IsNullOrEmpty(Sql) ? "" : " UNION ") + currentSql;

            DataTable dt = CommonDA.GetPageDataTable(Sql, pageSize, pageIndex, out recordCount, null, OrderByStr);
            dt = GetPageAndMemberInfo(dt);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    dt.Columns.Add(new DataColumn("MenuTitle"));
            //    dt.Columns.Add(new DataColumn("No"));
            //    dt.Columns.Add(new DataColumn("Title"));
            //    dt.Columns.Add(new DataColumn("SN"));
            //    dt.Columns.Add(new DataColumn("SiteSN"));
            //    dt.Columns.Add(new DataColumn("Name"));
            //    dt.Columns.Add(new DataColumn("MemberInfo"));
            //    dt.Columns.Add(new DataColumn("Photo"));
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        long siteID = (long)dr["SiteID"];
            //        long pagesNo = (long)dr["PagesNo"];
            //        var pageModel = PagesDAO.GetPageInfo(siteID, pagesNo);
            //        if (pageModel != null)
            //        {
            //            var siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteID);
            //            var menuModel = MenusDAO.GetInfo(siteID, pageModel.MenuID);
            //            dr["No"] = pageModel.No;
            //            dr["SN"] = pageModel.SN;
            //            dr["Title"] = pageModel.Title;
            //            if (menuModel != null)
            //                dr["MenuTitle"] = menuModel.Title;
            //            if (siteModel != null)
            //                dr["SiteSN"] = siteModel.SN;
            //        }
            //        if (!string.IsNullOrEmpty(dr["MemberID"].ToString()))
            //        {
            //            long memberID = (long)dr["MemberID"];
            //            var memberModel = WorkV3.Models.DataAccess.MemberShipDAO.GetItem(memberID);
            //            if (memberModel != null)
            //            {
            //                dr["Name"] = memberModel.Name;
            //                dr["MemberInfo"] = memberModel.Account + '/' + memberModel.Name;
            //                dr["Photo"] = memberModel.Photo;
            //            }
            //        }
            //    }
            //}
            // string Sql = string.Format(@" SELECT [Menus].[Title] AS MenuTitle, [Pages].[No], [Pages].[Title], [Pages].[SN], [Pages].[SiteID], [Sites].[SN] AS SiteSN, [Member].Name, PagesView_Log.*,  
            //                                 CASE WHEN [MemberShip].Account IS NOT NULL THEN ([MemberShip].Account+'/'+[MemberShip].Name) ELSE '' END AS MemberInfo,  
            //                                 [MemberShip].Photo ,CASE WHEN [ArticleInfo].[Title] IS NOT NULL THEN [ArticleInfo].[Title]  WHEN [EventsInfo].[Title] IS NOT NULL THEN [EventsInfo].[Title]  WHEN [SeekInfo].[Title] IS NOT NULL THEN [SeekInfo].[Title]  ELSE [Pages].[Title] END 
            //                                 AS Title FROM PagesView_Log
            //                                 LEFT JOIN [Pages] ON [Pages].[No]=[PagesView_Log].[PagesNo]
            //                                 LEFT JOIN [Menus] ON [Menus].ID = [Pages].[MenuID] 
            //                                 LEFT JOIN [MemberShip] ON [MemberShip].ID = [PagesView_Log].[MemberID] 
            //                                 LEFT JOIN [Member] ON [Member].ID = [Pages].[Creator] 
            //                                 LEFT JOIN [Sites] ON [Sites].[ID]=[Pages].[SiteID]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Seek.* FROM Seek  JOIN Cards ON Cards.No=Seek.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS SeekInfo ON SeekInfo.PageNo = [Pages].[No]
            //                                 LEFT JOIN ( SELECT Pages.No AS PageNo, Article.* FROM Article  JOIN Cards ON Cards.No=Article.CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS ArticleInfo ON ArticleInfo.PageNo = [Pages].[No]
            //LEFT JOIN ( SELECT Pages.No AS PageNo, [Events].* FROM [Events] JOIN Cards ON Cards.No=[Events].CardNo JOIN Zones ON Zones.No=Cards.ZoneNo JOIN Pages ON Pages.No=Zones.PageNo ) AS EventsInfo ON EventsInfo.PageNo = [Pages].[No]
            //                                 WHERE [AddTime]>='{0}' AND [AddTime]<='{1}' {2}
            //                                 {3}  "
            //                     , startDate.ToString("yyyy-MM-dd 00:00"), endDate.ToString("yyyy-MM-dd 23:59")
            //                     , statisticConditionStr, OrderByStr);
            //DataTable dt = db.GetPageData(Sql, pageSize, pageIndex, out recordCount);
            return dt;
        }

        /// <summary>
        /// 取得 Session 統計資料
        /// </summary>
        /// <returns></returns>
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GeViewLogs(long SiteID, DateTime startDate, DateTime endDate, Models.StatisticConditionModels sModel, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetStatistionDetailTable(SiteID, sModel, startDate, endDate, orderBy, pageSize, pageIndex, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        Title = row["Title"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                       // Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
                        MemberID = row["MemberID"].ToString(),
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
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GeMemberViewLogs(long SiteID, DateTime startDate, DateTime endDate, Models.StatisticConditionModels sModel, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetStatistionMemberDetailTable(SiteID, sModel, startDate, endDate, orderBy, pageSize, pageIndex, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        Title = row["Title"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                       // Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
                        MemberID = row["MemberID"].ToString(),
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
        public static List<ViewModels.AnalysisPageViewSessionViewModel> GetUserViewLogs(long SiteID, DateTime startDate, DateTime endDate, Models.StatisticConditionModels sModel, ViewModels.AnalysisOrderByViewModel orderBy, int pageSize, int pageIndex, out int recordCount)
        {
            List<ViewModels.AnalysisPageViewSessionViewModel> logSessionModelList = new List<ViewModels.AnalysisPageViewSessionViewModel>();
            DataTable dt = GetStatistionUserDetailTable(SiteID, sModel, startDate, endDate, orderBy, pageSize, pageIndex, out recordCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    logSessionModelList.Add(new ViewModels.AnalysisPageViewSessionViewModel()
                    {
                        No = row["PagesNo"].ToString(),
                        Title = row["Title"].ToString(),
                        SiteSN = row["SiteSN"].ToString(),
                        SiteID = row["SiteID"].ToString(),
                        PageCreator = row["Name"].ToString(),
                        UserAgent = row["UserAgent"].ToString(),
                        RefererUrl = row["ReferrerUrl"].ToString(),
                        RefererTitle = row["ReferrerUrlTitle"].ToString(),
                        RefererPageNo = row["ReferrerUrlPageNo"].ToString(),
                        SN = row["SN"].ToString(),
                        // Title = row["Title"].ToString(),
                        SessionID = row["SessionID"].ToString(),
                        LogDate = DateTime.Parse(row["AddTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                        StaySeconds = (double)row["StaySeconds"],
                        Browser = row["Browser"].ToString(),
                        MemberInfo = row["MemberInfo"].ToString(),
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
        public static List<ViewModels.AnalysisDailyLogViewModel> GetDailyStatistionValue(long SiteID, Models.StatisticConditionModels sModel, DateTime startDate, DateTime endDate)
        {
            List<ViewModels.AnalysisDailyLogViewModel> logDailyPageModellList = new List<ViewModels.AnalysisDailyLogViewModel>();
            string statisticConditionStr = GetStatisticCondition(SiteID, sModel);
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@StartDate", startDate.ToString("yyyy/MM/dd 00:00"));
            paraList.Add("@EndDate", endDate.ToString("yyyy/MM/dd 23:59"));
            string selectSampleSQL = @"SELECT [Statistic01].AddDate, [Statistic01].TotalHits, CASE WHEN [Statistic02].PersonCounts IS NOT NULL THEN [Statistic02].PersonCounts  ELSE 0 END AS PersonCounts, [Statistic01].MemberHits FROM
                                            (SELECT Convert(nvarchar(10), [AddTime], 111) AS AddDate, Count(1) AS TotalHits, SUM(CASE WHEN MemberID IS NOT NULL THEN 1 ELSE 0 END) AS MemberHits
                                            FROM [{1}]  Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0} GROUP BY Convert(nvarchar(10), [AddTime], 111) ) AS Statistic01
                                            LEFT JOIN 
                                            (SELECT AddDate, Count(1) AS PersonCounts FROM ( SELECT Convert(nvarchar(10), [AddTime], 111) AS AddDate, SessionID FROM [{1}] 
                                                                                    Where  [AddTime]>=@StartDate AND [AddTime]<=@EndDate {0} GROUP BY Convert(nvarchar(10), [AddTime], 111), SessionID 
	                                                                            ) AS PersonSumCounts GROUP BY AddDate )
	                                                                            AS Statistic02 ON  Statistic01.AddDate=Statistic02.AddDate
                                            ORDER BY [Statistic01].AddDate ASC   ";
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
                if (PagesView_LogDAO.CheckPageHistoryTableExist(curDate.Year, curDate.Month))
                {
                    string tableName = PagesView_LogDAO.GetPageHistoryTableName(curDate.Year, curDate.Month);

                    string SumSql = string.Format(selectSampleSQL, statisticConditionStr, tableName);
                    dt = db.GetDataTable(SumSql, paraList);
                }
                for (DateTime logDate = beginDate; logDate <= lastDate; logDate = logDate.AddDays(1))
                {
                    int totalViewHits = 0, memberViewHits = 0, totalUserCount = 0;
                    ViewModels.AnalysisDailyLogViewModel viewModel = new ViewModels.AnalysisDailyLogViewModel()
                    {
                        LogDate = logDate.ToString("MM/dd"),
                        TotalViewCount = 0,
                        TotalUserCount = 0,
                        TotalMemberViewCount = 0
                    };
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] selLogDate = dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                        if (selLogDate != null && selLogDate.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(selLogDate[0]["TotalHits"].ToString()))
                            {
                                totalViewHits = int.Parse(selLogDate[0]["TotalHits"].ToString());
                            }
                            if (!string.IsNullOrEmpty(selLogDate[0]["PersonCounts"].ToString()))
                            {
                                totalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                            }
                            if (!string.IsNullOrEmpty(selLogDate[0]["MemberHits"].ToString()))
                            {
                                memberViewHits = int.Parse(selLogDate[0]["MemberHits"].ToString());
                            }
                        }
                    }
                    if (sModel.StatisticType == StatisticType.SummaryViewCount)
                        viewModel.TotalViewCount = totalViewHits;
                    if (sModel.StatisticType == StatisticType.DailyViewCount)
                        viewModel.TotalViewCount = totalUserCount;
                    if (sModel.StatisticType == StatisticType.MemberViewCount)
                        viewModel.TotalViewCount = memberViewHits;
                    logDailyPageModellList.Add(viewModel);
                }
                curDate = curDate.AddMonths(1);
            }

            string currentSumSql = string.Format(selectSampleSQL, statisticConditionStr, "PagesView_Log");
            DataTable current_dt = db.GetDataTable(currentSumSql, paraList);
            for (DateTime logDate = startDate; logDate <= endDate; logDate = logDate.AddDays(1))
            {
                int totalViewHits = 0, memberViewHits = 0, totalUserCount = 0;
                if (current_dt != null && current_dt.Rows.Count > 0)
                {
                    DataRow[] selLogDate = current_dt.Select("AddDate=#" + logDate.ToString("yyyy/MM/dd") + "#");
                    if (selLogDate != null && selLogDate.Length > 0)
                    {
                        totalViewHits = int.Parse(selLogDate[0]["TotalHits"].ToString());
                        totalUserCount = int.Parse(selLogDate[0]["PersonCounts"].ToString());
                        memberViewHits = int.Parse(selLogDate[0]["MemberHits"].ToString());
                    }
                    var currentLogDate = logDailyPageModellList.Where(p => p.LogDate == logDate.ToString("MM/dd"));
                    if (currentLogDate != null && currentLogDate.Count() > 0)
                    {
                        currentLogDate.ElementAt(0).TotalViewCount += totalViewHits;
                        currentLogDate.ElementAt(0).TotalUserCount += totalUserCount;
                        currentLogDate.ElementAt(0).TotalMemberViewCount += totalUserCount;
                    }
                    else
                    {
                        ViewModels.AnalysisDailyLogViewModel viewModel = new ViewModels.AnalysisDailyLogViewModel()
                        {
                            LogDate = logDate.ToString("MM/dd"),
                            TotalViewCount = totalViewHits,
                            TotalUserCount = totalUserCount,
                            TotalMemberViewCount = totalUserCount
                        };
                        logDailyPageModellList.Add(viewModel);
                    }
                }
            }
            return logDailyPageModellList;
        }
        public static DataTable GetPageAndMemberInfo(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add(new DataColumn("MenuTitle"));
                dt.Columns.Add(new DataColumn("No"));
                dt.Columns.Add(new DataColumn("Title"));
                dt.Columns.Add(new DataColumn("SN"));
                dt.Columns.Add(new DataColumn("SiteSN"));
                dt.Columns.Add(new DataColumn("Name"));
                dt.Columns.Add(new DataColumn("MemberInfo"));
                dt.Columns.Add(new DataColumn("Photo"));
                foreach (DataRow dr in dt.Rows)
                {
                    long siteID = (long)dr["SiteID"];
                    long pagesNo = (long)dr["PagesNo"];
                    var pageModel = PagesDAO.GetPageInfo(siteID, pagesNo);
                    if (pageModel != null)
                    {
                        var siteModel = WorkV3.Models.DataAccess.SitesDAO.GetInfo(siteID);
                        var menuModel = MenusDAO.GetInfo(siteID, pageModel.MenuID);
                        dr["No"] = pageModel.No;
                        dr["SN"] = pageModel.SN;
                        dr["Title"] = pageModel.Title;
                        if (menuModel != null)
                            dr["MenuTitle"] = menuModel.Title;
                        if (siteModel != null)
                            dr["SiteSN"] = siteModel.SN;
                    }
                    if (!string.IsNullOrEmpty(dr["MemberID"].ToString()))
                    {
                        long memberID = (long)dr["MemberID"];
                        var memberModel = WorkV3.Models.DataAccess.MemberShipDAO.GetItem(memberID);
                        if (memberModel != null)
                        {
                            dr["Name"] = memberModel.Name;
                            dr["MemberInfo"] = memberModel.Account + '/' + memberModel.Name;
                            dr["Photo"] = memberModel.Photo;
                        }
                    }
                }
            }
            return dt;
        }
    }
}