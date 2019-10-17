using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class PageCommentsDAO
    {
        public static void SetItem(PageCommentsModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("PageComments");
            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            string sql = "Select 1 From PageComments Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("PageSN");
                tableObj.Remove("ParentID");
                tableObj.Remove("PostDate");
                tableObj.Remove("ShowStatus");
                tableObj.Remove("MemberShipID");

                Common.Member curUser = Common.Member.Current;
                tableObj["Modifier"] = curUser == null ? Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id : curUser.ID;

                tableObj["ModifyTime"] = now;

                tableObj.Update(item.ID);
            }
        }

        public static bool DeleteItem(long ID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"DELETE PageComments Where ID = {ID}" ;
            return db.ExecuteNonQuery(sql) >0;
        }
        public static List<PageCommentsModels> GetItems(long SiteID, string PageSN, int pageIndex, int rowCount, out int totalRecord)
        {
            List<PageCommentsModels> itemList = new List<PageCommentsModels>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            string query = $"Select Count(1) From PageComments Where PageSN='{PageSN.Replace("'", "")}' AND ShowStatus=1 AND ParentID IS NULL ";
            DataTable countTable = db.GetDataTable(query);
            totalRecord = 0;
            if (countTable == null || countTable.Rows.Count <= 0)
            {
                return itemList;
            }
            if(string.IsNullOrEmpty(countTable.Rows[0][0].ToString()))
            {
                return itemList;
            }
            totalRecord = int.Parse(countTable.Rows[0][0].ToString());
            string cond = $" PageSN='{PageSN.Replace("'", "")}' AND ShowStatus=1  AND ParentID IS NULL ";
            string orderby = " Order By PostDate ASC ";
            string sql = 
                $"Select TOP {rowCount} * From PageComments Where {cond} AND ID NOT IN (SELECT TOP {pageIndex * rowCount} ID FROM PageComments Where {cond} {orderby})  {orderby} ";
            DataTable datas = db.GetDataTable(sql);

            itemList = GetListItems(SiteID, datas);
            return itemList;
        }
        public static PageCommentsModels GetSingleItem(long SiteID, string ID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string cond = $" ID='{ID.Replace("'", "")}' AND ShowStatus=1  ";
            string sql =
                $"Select * From PageComments Where {cond} ";
            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    PageCommentsModels item = new PageCommentsModels();
                    item.ID = long.Parse(dr["ID"].ToString());
                    item.CommentID = dr["ID"].ToString();
                    item.PageSN = dr["PageSN"].ToString();
                    item.ParentID = string.IsNullOrEmpty(dr["ParentID"].ToString()) ? (long?)null : (long)dr["ParentID"];
                    item.Title = dr["Title"].ToString().Trim();
                    item.PosterName = dr["PosterName"].ToString().Trim();
                    item.PostDate = (DateTime)dr["PostDate"];
                    item.PostDateDiff = GetDateDiffNow(item.PostDate);
                    item.ShowStatus = Convert.ToBoolean(dr["ShowStatus"].ToString());
                    item.MemberShipID = string.IsNullOrEmpty(dr["MemberShipID"].ToString()) ? (long?)null : (long)dr["MemberShipID"];
                    item.CommentContent = dr["CommentContent"].ToString().Trim();
                    item.IP = dr["IP"].ToString().Trim();
                    item.IPNum = dr["IPNum"].ToString().Trim();
                    item.GoodCount = int.Parse(dr["GoodCount"].ToString());
                    item.Modifier = string.IsNullOrEmpty(dr["Modifier"].ToString()) ? (long?)null : (long)dr["Modifier"];
                    item.ModifyTime = (DateTime)dr["ModifyTime"];
                    item.ChildCommentList = GetChildItems(SiteID, item.ID);
                    return item;
                }
            }
            return null;
        }
        public static List<PageCommentsModels> GetChildItems(long SiteID, long ParentID)
        {
            List<PageCommentsModels> itemList = new List<PageCommentsModels>();
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
           
            string sql =
                $"Select * From PageComments Where ParentID={ParentID} AND ShowStatus=1 Order By PostDate ASC  ";
            DataTable datas = db.GetDataTable(sql);

            itemList = GetListItems(SiteID, datas);
            return itemList;
        }

        private static List<PageCommentsModels> GetListItems(long SiteID, DataTable datas)
        {
            long? memberID = null;
            Member curUser = Member.Current;
            if (curUser != null)
            {
                memberID = curUser.ID;
            }
            int rowIndex = 0;
            string vPath = WorkV3.Golbal.UpdFileInfo.GetVPathBySiteID(SiteID, "Member");
            MemberShipSearchModels searchModel = new MemberShipSearchModels();
            searchModel.SiteID = SiteID;
            var memberList = Models.DataAccess.MemberShipDAO.GetItems(searchModel, 99999, 0, out rowIndex);
            List<PageCommentsModels> items = new List<PageCommentsModels>();
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    PageCommentsModels item = new PageCommentsModels();
                    item.ID = long.Parse( dr["ID"].ToString());
                    item.CommentID = dr["ID"].ToString();
                    item.PageSN = dr["PageSN"].ToString();
                    item.ParentID = string.IsNullOrEmpty(dr["ParentID"].ToString())? (long?)null:(long)dr["ParentID"];
                    item.Title = dr["Title"].ToString().Trim();
                    item.PosterName =  dr["PosterName"].ToString().Trim();
                    item.PostDate = (DateTime)dr["PostDate"];
                    item.PostDateDiff = GetDateDiffNow(item.PostDate);
                    item.ShowStatus = Convert.ToBoolean(dr["ShowStatus"].ToString());
                    item.MemberShipID = string.IsNullOrEmpty(dr["MemberShipID"].ToString()) ? (long?)null : (long)dr["MemberShipID"];
                    if (item.MemberShipID.HasValue)
                    {
                        var m = memberList.Where(p => p.ID == item.MemberShipID.Value);
                        if (m != null && m.Count() > 0)
                        {
                            item.PosterName = m.First().Name;
                            if (!string.IsNullOrEmpty(m.First().Photo))
                            {
                                if(m.First().Photo.StartsWith("http"))
                                    item.Photo = m.First().Photo;
                                else
                                    item.Photo = vPath + m.First().Photo;
                            }
                            item.MemberType = m.First().MemberType.ToString();
                        }
                    }
                    //item.PosterName = memberList.Count().ToString();
                    item.CommentContent =  dr["CommentContent"].ToString().Trim();
                    item.IP =  dr["IP"].ToString().Trim();
                    item.IPNum =  dr["IPNum"].ToString().Trim();
                    item.GoodCount =  int.Parse(dr["GoodCount"].ToString());
                    item.Modifier = string.IsNullOrEmpty(dr["Modifier"].ToString()) ? (long?)null : (long)dr["Modifier"];
                    item.ModifyTime = (DateTime)dr["ModifyTime"];
                    item.ChildCommentList = GetChildItems(SiteID, item.ID);
                    item.IsAddedGood = false;
                    if (memberID.HasValue)
                    {
                        PageCommentLogsModels logModel = PageCommentsDAO.GetGoodLogs((long)dr["ID"], ViewModels.CommentLogType.Good, memberID.Value);
                        if (logModel != null)
                        {
                            item.IsAddedGood = true;
                        }
                    }
                    items.Add(item);
                }
            }

            return items;
        }
        public static int AddGoodLogs(long commentID, ViewModels.CommentLogType LogType, string Browser, string UserAgent, long? MemberID)
        {
            int goodCountResult = 0;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string AddCol = "", AddColPara = "";
            SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
            paras.Add("@ID", commentID);
            paras.Add("@NewLogID", WorkLib.GetItem.NewSN());
            paras.Add("@LogType", (int)LogType);
            paras.Add("@Browser", Browser);
            paras.Add("@UserAgent", UserAgent);
            paras.Add("@IP", WorkLib.GetItem.IPAddr());
            paras.Add("@IPNum", WorkLib.GetItem.GetIPNum().ToString());
            if (MemberID.HasValue)
            {
                AddCol = ", MemberShipID";
                AddColPara = ", @MemberShipID";
                paras.Add("@MemberShipID", MemberID.Value);
            }
            string sql = string.Format(@" UPDATE PageComments SET GoodCount=GoodCount+1 WHERE ID=@ID;
                                INSERT PageCommentLogs (ID, CommentID{0}, LogType, Browser, UserAgent, IP, IPNum, AddTime) 
                                        VALUES (@NewLogID, @ID{1}, @LogType, @Browser, @UserAgent, @IP, @IPNum, GETDATE());
                                SELECT GoodCount FROM PageComments WHERE ID=@ID", AddCol, AddColPara);
           DataTable logTable =  db.GetDataTable(sql, paras);
            if (logTable != null && logTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(logTable.Rows[0][0].ToString()))
                {
                    goodCountResult = int.Parse(logTable.Rows[0][0].ToString());
                }
            }
            return goodCountResult;
        }
        public static Models.PageCommentLogsModels GetGoodLogs(long commentID, ViewModels.CommentLogType LogType, long MemberID)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
            paras.Add("@CommentID", commentID);
            paras.Add("@MemberShipID", MemberID);
            paras.Add("@LogType", (int)LogType);
          
            string sql = @" SELECT * FROM PageCommentLogs WHERE CommentID=@CommentID AND MemberShipID=@MemberShipID AND LogType=@LogType";
            DataTable logTable = db.GetDataTable(sql, paras);
            if (logTable != null && logTable.Rows.Count > 0)
            {
                DataRow dr = logTable.Rows[0];
                PageCommentLogsModels item = new PageCommentLogsModels();
                item.ID = long.Parse(dr["ID"].ToString());
                item.CommentID = long.Parse(dr["CommentID"].ToString());
                item.MemberShipID = long.Parse(dr["MemberShipID"].ToString());
                item.LogType = int.Parse(dr["LogType"].ToString());
                item.Browser = dr["Browser"].ToString().Trim();
                item.UserAgent = dr["UserAgent"].ToString().Trim();
                item.IP = dr["IP"].ToString().Trim();
                item.IPNum = dr["IPNum"].ToString().Trim();
                item.AddTime = (DateTime)dr["AddTime"];
                return item;
            }
            return null;
        }
        public static int DeleteGoodLogs(long commentID, long logID)
        {
            int goodCountResult = 0;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string AddCol = "", AddColPara = "";
            SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
            paras.Add("@ID", commentID);
            paras.Add("@LogID", logID);
            string sql = string.Format(@" UPDATE PageComments SET GoodCount=GoodCount-1 WHERE ID=@ID;
                                DELETE PageCommentLogs WHERE ID=@LogID;
                                SELECT GoodCount FROM PageComments WHERE ID=@ID", AddCol, AddColPara);
            DataTable logTable = db.GetDataTable(sql, paras);
            if (logTable != null && logTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(logTable.Rows[0][0].ToString()))
                {
                    goodCountResult = int.Parse(logTable.Rows[0][0].ToString());
                }
            }
            return goodCountResult;
        }
        /// <summary>
        /// 檢查是否已發文過, 依 IP 及 MemberID (若非 NULL 時) 目前限制為 10 分鐘, return: true 已發文過, 時效未過 | false: 未發文過或時效已過
        /// </summary>
        /// <param name="pageSN"></param>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public static bool IsPostCommentOverTime(string pageSN, long? MemberID)
        {
            //因應 cc 所提, 先不限制時間, 故 always false 2018-04-18 charlie_shan
            return false;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string AddCol = "";
            SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
            paras.Add("@PageSN", pageSN);
            paras.Add("@IP", WorkLib.GetItem.IPAddr());
            if (MemberID.HasValue)
            {
                AddCol = " AND MemberShipID=@MemberShipID ";
                paras.Add("@MemberShipID", MemberID.Value);
            }
            string sql = string.Format(@" SELECT TOP 1 * FROM  PageComments WHERE PageSN=@PageSN AND IP=@IP {0} Order By PostDate DESC "
                                            , AddCol);
            DataTable logTable = db.GetDataTable(sql, paras);
            if (logTable == null || logTable.Rows.Count <= 0)
                return false;

            DateTime LastAddTime = DateTime.Parse(logTable.Rows[0]["PostDate"].ToString());
            if (DateTime.Now.Subtract(LastAddTime).TotalMinutes > 10)
                return false;
            return true;
        }
        /// <summary>
        /// 檢查是否已投過票, 依 IP 及 MemberID (若非 NULL 時) 目前限制為 10 分鐘, return: true 已投過, 時效未過 | false: 未投過或時效已過
        /// </summary>
        /// <param name="commentID"></param>
        /// <param name="LogType"></param>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public static bool IsAddGoodLogOverTime(long commentID, ViewModels.CommentLogType LogType, long? MemberID)
        {
            //因應 cc 所提, 先不限制時間, 故 always false 2018-04-18 charlie_shan
            return false;
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string AddCol = "";
            SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
            paras.Add("@CommentID", commentID);
            paras.Add("@LogType", (int)LogType);
            paras.Add("@IP", WorkLib.GetItem.IPAddr());
            if (MemberID.HasValue)
            {
                AddCol = " AND MemberShipID=@MemberShipID ";
                paras.Add("@MemberShipID", MemberID.Value);
            }
            string sql = string.Format(@" SELECT TOP 1 * FROM  PageCommentLogs WHERE CommentID=@CommentID AND LogType=@LogType AND IP=@IP {0} Order By AddTime DESC "
                                            , AddCol);
            DataTable logTable = db.GetDataTable(sql, paras);
            if (logTable == null || logTable.Rows.Count <= 0)
                return false;

            DateTime LastAddTime = DateTime.Parse(logTable.Rows[0]["AddTime"].ToString());
            if (DateTime.Now.Subtract(LastAddTime).TotalMinutes > 10)
                return false;
            return true;
        }
        private static string GetDateDiffNow(DateTime pDate)
        {
            string dateDiff = "";
            DateTime dateNow = DateTime.Now;
            int total_minutes = (int)dateNow.Subtract(pDate).TotalMinutes;
            int days = 0, hours = 0, minutes = 0;
            if (total_minutes > 60)
            {
                hours = (int)(total_minutes / 60);
                if (hours > 24)
                    days = (int)(hours / 24);
            }
            hours = hours % 24;
            minutes = total_minutes % 60;
            if (days > 1)
            {
                dateDiff += string.Format("{0}天", days.ToString());
            }
            if (hours > 1)
            {
                dateDiff += string.Format("{0}小時", hours.ToString());
            }
            dateDiff += string.Format("{0}分鐘", minutes.ToString());
           // dateDiff += "("+total_minutes.ToString()+")";
            return dateDiff;
        }
    }
}