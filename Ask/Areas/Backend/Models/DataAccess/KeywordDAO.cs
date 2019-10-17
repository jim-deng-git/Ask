using Dapper;
using System;
//using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Areas.Backend.ViewModels;
using Newtonsoft.Json.Linq;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class KeywordDAO
    {
        private SQLData.Database db;
        private System.Data.SqlClient.SqlConnection conn;

        public KeywordDAO()
        {
            db = new SQLData.Database(WebInfo.Conn);
            conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn);
        }

        /// <summary>
        /// 以 id 取得已設定的關鍵字
        /// </summary>
        /// <param name="keywordId"></param>
        /// <returns></returns>
        public KeywordModels GetKeywordItem(long keywordId)
        {
            string strSql = " SELECT * FROM Keywords WHERE ID = @id ";
            return conn.Query<KeywordModels>(strSql, new { @id = keywordId }).SingleOrDefault();
        }

        /// <summary>
        /// 以關鍵字全文取得已設定的關鍵字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public KeywordModels GetKeywordItemByText(string str)
        {
            KeywordQueriedModels search = GetKeywordQueriedItemByText(str);
            return GetKeywordItemByQueriedId(search.ID);
        }

        /// <summary>
        /// 使用查詢關鍵字 id 取得已設定的關鍵字
        /// </summary>
        /// <param name="keywordQueriedId"></param>
        /// <returns></returns>
        public KeywordModels GetKeywordItemByQueriedId(long keywordQueriedId)
        {
            string strSql = " SELECT * FROM Keywords WHERE KeywordQueriedID = @queriedId ";
            return conn.Query<KeywordModels>(strSql, new { @queriedId = keywordQueriedId }).SingleOrDefault();
        }

        /// <summary>
        /// 使用 id 取得查詢關鍵字
        /// </summary>
        /// <param name="keywordQueriedId"></param>
        /// <returns></returns>
        public KeywordQueriedModels GetKeywordQueriedItem(long keywordQueriedId)
        {
            string strSql = " SELECT * FROM KeywordQueried WHERE ID = @id ";
            return conn.Query<KeywordQueriedModels>(strSql, new { @id = keywordQueriedId }).SingleOrDefault();
        }

        public KeywordQueriedModels GetKeywordQueriedItemByText(string str)
        {
            string strSql = " SELECT * FROM KeywordQueried WHERE Text = @text ";
            return conn.Query<KeywordQueriedModels>(strSql, new { text = str }).SingleOrDefault();
        }

        public KeywordQueriedModels GetKeywordQueriedItemByKeywordId(long keywordId)
        {
            KeywordModels keywordObj = GetKeywordItem(keywordId);
            long queriedItemId = keywordObj.KeywordQueriedID;

            return GetKeywordQueriedItem(queriedItemId);
        }

        public KeywordQueriedLogModels GetLastetKeywordQueriedLogItem()
        {
            string strSql = " SELECT TOP 1 * FROM KeywordQueriedLogs ORDER BY QueryTime DESC ";
            return conn.Query<KeywordQueriedLogModels>(strSql).SingleOrDefault();
        }

        public SQLData.TableObject GetTableObj(string strTableName)
        {
            return db.GetTableObject(strTableName);
        }

        public bool EditKeyword(KeywordSaveViewModel item)
        {
            KeywordQueriedModels queriedObj = GetKeywordQueriedItemByText(item.Text);

            if (queriedObj == null)
            {
                AddSingleKeywordQueried(item.Text, 0);
                queriedObj = GetKeywordQueriedItemByText(item.Text);
            }

            KeywordModels keywordExist = GetKeywordItemByText(item.Text);

            if (keywordExist != null && keywordExist.ID != item.ID)
                return false;

            KeywordModels keywordObj = GetKeywordItem(item.ID??0);
            SQLData.TableObject tableObj = GetTableObj("Keywords");
            tableObj.GetDataFromObject(keywordObj);

            tableObj["KeywordQueriedID"] = queriedObj.ID;
            tableObj["Modifier"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
            tableObj["ModifyTime"] = DateTime.Now;

            tableObj.Update(keywordObj.ID);
            return true;
        }

        // 新增自定關鍵字
        public bool AddKeyword(KeywordSaveViewModel item)
        {
            SQLData.TableObject keywordTableObj = GetTableObj("Keywords");
            SQLData.TableObject queriedTableObj = GetTableObj("KeywordQueried");

            KeywordQueriedModels keywordQueriedObj = GetKeywordQueriedItemByText(item.Text);
            bool isNew = keywordQueriedObj == null;

            // 如果 KeywordQueried 沒有的話，Keywords、KeywordQueried 各 insert 一筆
            if(isNew)
            {
                queriedTableObj["Text"] = item.Text;
                queriedTableObj["Count"] = 0;
                queriedTableObj.Insert();

                KeywordQueriedModels keyword = GetKeywordQueriedItemByText(item.Text);
                long keywordId = keyword.ID;

                int keywordCount = (int)db.GetFirstValue(" SELECT count(*) FROM Keywords ");
                int maxKeywordSort = keywordCount == 0? 1: (int)db.GetFirstValue("SELECT Sort FROM Keywords ORDER BY Sort Desc");

                keywordTableObj["KeywordQueriedID"] = keywordId;
                keywordTableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                keywordTableObj["CreateTime"] = DateTime.Now;
                keywordTableObj["IsIssue"] = item.IsIssue;
                keywordTableObj["Sort"] = maxKeywordSort+1;
                keywordTableObj.Insert();

                return true;
            }
            else
            {
                // 如果有的話，判斷是不是已經加入關鍵字
                KeywordModels keywordObj = GetKeywordItemByText(item.Text);

                // 已經加入的話就不加
                if(keywordObj != null)
                    return false;

                int keywordCount = (int)db.GetFirstValue(" SELECT count(*) FROM Keywords ");
                int maxKeywordSort = keywordCount == 0 ? 1 : (int)db.GetFirstValue("SELECT Sort FROM Keywords ORDER BY Sort Desc");

                keywordTableObj["KeywordQueriedID"] = keywordQueriedObj.ID;
                keywordTableObj["Modifier"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                keywordTableObj["ModifyTime"] = DateTime.Now;
                keywordTableObj["IsIssue"] = item.IsIssue;
                keywordTableObj["Sort"] = maxKeywordSort+1;
                keywordTableObj.Insert();

                return true;
            }
        }

        public void RemoveKeyword(long id)
        {
            SQLData.TableObject keywordTableObj = GetTableObj("Keywords");

            db.ExecuteNonQuery(" DELETE FROM Keywords WHERE ID = @ID ", new SQLData.ParameterCollection("@ID", id));
        }

        public List<KeywordViewModel> GetAllKeywords(bool filterIsIssue = false)
        {
            List<KeywordViewModel> item = new List<KeywordViewModel>();
            string filterClause = filterIsIssue ? "WHERE IsIssue = 1" : "";
            string Sql = $@"SELECT a.Id, b.Text, a.IsIssue, a.Sort, b.Count
                           FROM Keywords a 
                           JOIN KeywordQueried b ON a.KeywordQueriedID = b.ID
                           {filterClause}
                           ORDER BY Sort DESC ";

            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<KeywordViewModel>(Sql, new {});
                item = res.ToList();
            }
            return item;
        }

        public List<KeywordModels> GetAllKeywordQueried()
        {
            List<KeywordModels> item = new List<KeywordModels>();
            string Sql = @"SELECT a.*, b.Text 
                           FROM KeywordQueried 
                           ORDER BY ID DESC ";

            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<KeywordModels>(Sql, new {});
                item = res.ToList();
            }
            return item;
        }

        public void AddKeywordQueried(string strSearches)
        {
            Regex trimmer = new Regex(@"\s\s+");

            strSearches = trimmer.Replace(strSearches, " ");

            strSearches = strSearches.Trim();
            string[] searches = strSearches.Split(' ');

            foreach(var search in searches)
            {
                long queriedId = AddSingleKeywordQueried(search);
                AddKeywordQueriedLog(queriedId);
            }
        }

        /// <summary>
        /// 新增一組使用者搜尋字串
        /// </summary>
        /// <param name="strSearch">搜尋字串</param>
        /// <returns>新增後的 ID ，若已存在也回傳 ID </returns>
        public long AddSingleKeywordQueried(string strSearch, int count = 1)
        {
            KeywordQueriedModels keywordQueriedObj = GetKeywordQueriedItemByText(strSearch);
            SQLData.TableObject keywordQueriedTableObj = GetTableObj("KeywordQueried");

            if (keywordQueriedObj == null)
            {
                keywordQueriedTableObj["Text"] = strSearch;
                keywordQueriedTableObj["Count"] = count;
                keywordQueriedTableObj.Insert();

                keywordQueriedObj = GetKeywordQueriedItemByText(strSearch);
                return keywordQueriedObj.ID;
            }
            else
            {
                keywordQueriedTableObj.GetDataFromObject(keywordQueriedObj);
                keywordQueriedTableObj["Count"] = keywordQueriedObj.Count+1;
                keywordQueriedTableObj.Update(keywordQueriedObj.ID);

                return keywordQueriedObj.ID;
            }
        }

        /// <summary>
        /// 新增搜尋的 log
        /// </summary>
        /// <param name="KeywordQueriedID">使用者搜尋字串ID</param>
        /// <returns>新增後的 ID</returns>
        public long AddKeywordQueriedLog(long KeywordQueriedID)
        {
            SQLData.TableObject logObj = GetTableObj("KeywordQueriedLogs");
            logObj["KeywordQueriedID"] = KeywordQueriedID;
            logObj["QueryTime"] = DateTime.Now;
            logObj.Insert();

            KeywordQueriedLogModels log = GetLastetKeywordQueriedLogItem();
            return log.ID;
        }

        public void ToggleIssue(long keywordId)
        {
            CommonDA.ToggleIssue("Keywords", keywordId);
        }

        public void DeleteKeyword(IEnumerable<long> IDs)
        {
            CommonDA.Delete("Keywords", IDs);
        }

        public void SortKeywords(IEnumerable<SortItem> items)
        {
            CommonDA.Sort("Keywords", items, "");
        }

        public int GetKeywordQueriedRowCounts()
        {
            string sql = " SELECT COUNT(*) FROM KeywordQueried ";
            return (int)db.GetFirstValue(sql);
        }

        /// <summary>
        /// 撈出分頁資料
        /// 撈資料邏輯：
        /// 沒有設定搜尋時間及熱門關鍵字：直接從 KeywordQueried 撈資料
        /// 有設定搜尋時間：從 KeywordQueriedLogs 撈出符合條件的 id，在 KeywordQueried 做 in 條件查詢
        /// 有設定熱門關鍵字：從 Keyword 撈出所有 KeywordQueriedID，在 KeywordQueried 做 in 或 not in 條件查詢，再將這筆資料設為資料來源
        /// </summary>
        /// <param name="index"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<KeywordQueriedViewModel> GetKeywordQueriedPagedItems(int index, JObject search, int countPerPage = 20)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
               
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    List<string> where = new List<string>();

                    int startRow = (index - 1) * countPerPage + 1;
                    int endRow = (index - 1) * countPerPage + countPerPage;
                    int offset = (index - 1) * countPerPage;

                    if (search["search"] != null && search["search"].ToString() != "")
                    {
                        where.Add("([Text] like '%'+@search+'%')");
                        param.Add("@search", search["search"].ToString());
                    }
                    if (search["leastSearch"] != null && search["leastSearch"].ToString() != "")
                    {
                        where.Add("[Count] >= @LeastCount");
                        param.Add("@LeastCount", search["leastSearch"].ToString());
                    }
                    if (search["maxSearch"] != null && search["maxSearch"].ToString() != "")
                    {
                        where.Add("[Count] <= @MaxCount");
                        param.Add("@MaxCount", search["maxSearch"].ToString());
                    }

                    string strSource = "";
                    if ((search["isSet"] != null && search["isSet"].ToString() != "false") ^ (search["isNotSet"] != null && search["isNotSet"].ToString() != "false"))
                    {
                        if (search["isSet"] != null && search["isSet"].ToString() != "false")
                            strSource = @" (SELECT * FROM KeywordQueried WHERE ID IN ( SELECT KeywordQueriedID FROM Keywords)) ";
                        else
                            strSource = @" (SELECT * FROM KeywordQueried WHERE ID NOT IN ( SELECT KeywordQueriedID FROM Keywords)) ";
                    }
                    else
                    {
                        strSource = "[KeywordQueried]";
                    }

                    StringBuilder sql = new StringBuilder();
                    if ((search["searchStart"] != null && search["searchStart"].ToString() != "") || (search["searchEnd"] != null && search["searchEnd"].ToString() != ""))
                    {
                        List<string> joinWhere = new List<string>();
                        if (search["searchStart"] != null && search["searchStart"].ToString() != "")
                        {
                            joinWhere.Add("[QueryTime] >= @searchStart");
                            param.Add("@searchStart", search["searchStart"].ToString());
                        }
                        if (search["searchEnd"] != null && search["searchEnd"].ToString() != "")
                        {
                            joinWhere.Add("[QueryTime] <= @searchEnd");
                            param.Add("@searchEnd", search["searchEnd"].ToString());
                        }

                        sql.Append($@"SELECT ROW_NUMBER() OVER ( ORDER BY Count DESC ) AS RowNum, a.*, b.IsIssue, b.KeywordQueriedID 
                                 FROM (SELECT * 
                                       FROM {strSource} 
                                       WHERE ID IN (SELECT ID FROM KeywordQueriedLogs WHERE { string.Join(" AND ", joinWhere) })) a 
                                 LEFT JOIN Keywords b on a.id = b.KeywordQueriedID ");
                        sql.Append($"WHERE { string.Join(" AND ", where) } ");
                        sql.Append($@"ORDER BY a.Count DESC 
                                  OFFSET {offset} ROWS
                                  FETCH NEXT {countPerPage} ROWS ONLY");
                    }
                    else
                    {
                        sql.Append($"SELECT ROW_NUMBER() OVER ( ORDER BY Count DESC ) AS RowNum, a.*, b.IsIssue, b.KeywordQueriedID FROM {strSource} a left join Keywords b on a.id = b.KeywordQueriedID ");
                        if (where.Count > 0)
                            sql.Append($"WHERE { string.Join(" AND ", where) } ");

                        sql.Append($@"ORDER BY a.Count DESC 
                                  OFFSET {offset} ROWS
                                  FETCH NEXT {countPerPage} ROWS ONLY");
                    }
                try
                {
                    //WorkLib.WriteLog.Write(true, sql.ToString());
                    return cn.Query<KeywordQueriedViewModel>(sql.ToString(), param).ToList();
                }
                catch (Exception exp)
                {
                    //WorkLib.WriteLog.Write(true, exp.Message.ToString());
                    //WorkLib.WriteLog.Write(true, sql.ToString());
                    return new List<KeywordQueriedViewModel>();
                }
            }
        }
        public int GetKeywordQueriedPagedItemCount(int index, JObject search)
        {
            using (var cn = new SqlConnection(WebInfo.Conn))
            {
                SQLData.ParameterCollection param = new SQLData.ParameterCollection();
                List<string> where = new List<string>();

                if (search["search"] != null && search["search"].ToString() != "")
                {
                    where.Add("([Text] like '%'+@search+'%')");
                    param.Add("@search", search["search"].ToString());
                }
                if (search["leastSearch"] != null && search["leastSearch"].ToString() != "")
                {
                    where.Add("[Count] >= @LeastCount");
                    param.Add("@LeastCount", search["leastSearch"].ToString());
                }
                if (search["maxSearch"] != null && search["maxSearch"].ToString() != "")
                {
                    where.Add("[Count] <= @MaxCount");
                    param.Add("@MaxCount", search["maxSearch"].ToString());
                }

                string strSource = "";
                if ((search["isSet"] != null && search["isSet"].ToString() != "false") ^ (search["isNotSet"] != null && search["isNotSet"].ToString() != "false"))
                {
                    if (search["isSet"] != null && search["isSet"].ToString() != "false")
                        strSource = @" (SELECT * FROM KeywordQueried WHERE ID IN ( SELECT KeywordQueriedID FROM Keywords)) ";
                    else
                        strSource = @" (SELECT * FROM KeywordQueried WHERE ID NOT IN ( SELECT KeywordQueriedID FROM Keywords)) ";
                }
                else
                {
                    strSource = "[KeywordQueried]";
                }

                StringBuilder sql = new StringBuilder();
                if ((search["searchStart"] != null && search["searchStart"].ToString() != "") || (search["searchEnd"] != null && search["searchEnd"].ToString() != ""))
                {
                    List<string> joinWhere = new List<string>();
                    if (search["searchStart"] != null && search["searchStart"].ToString() != "")
                    {
                        joinWhere.Add("[QueryTime] >= @searchStart");
                        param.Add("@searchStart", search["searchStart"].ToString());
                    }
                    if (search["searchEnd"] != null && search["searchEnd"].ToString() != "")
                    {
                        joinWhere.Add("[QueryTime] <= @searchEnd");
                        param.Add("@searchEnd", search["searchEnd"].ToString());
                    }

                    sql.Append($@"SELECT Count(*)
                                 FROM (SELECT * 
                                       FROM {strSource} 
                                       WHERE ID IN (SELECT ID FROM KeywordQueriedLogs WHERE { string.Join(" AND ", joinWhere) })) a 
                                 LEFT JOIN Keywords b on a.id = b.KeywordQueriedID ");
                    sql.Append($"WHERE { string.Join(" AND ", where) } ");
                }
                else
                {
                    sql.Append($"SELECT Count(*) FROM {strSource} a left join Keywords b on a.id = b.KeywordQueriedID ");
                    if (where.Count > 0)
                        sql.Append($"WHERE { string.Join(" AND ", where) } ");
                }

                return (int)db.GetFirstValue(sql.ToString(), param);
            }
        }
    }
}