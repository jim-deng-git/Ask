using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using Dapper;
using WorkV3.Common;
using System.Data.SqlClient;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    internal static class CommonDA {
        public static T GetItem<T>(string tableName, long id) where T: new() {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From " + tableName + " Where ID = " + id;
                return conn.Query<T>(sql).SingleOrDefault();
            }
        }

        public static IEnumerable<T> GetAllItem<T>(string tableName) where T : new()
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select * From " + tableName;
                return conn.Query<T>(sql);
            }
        }

        // 20180706 neil 因原本 DataBase.GetPageData 需從 DataTable 轉型至 Model 很不方便，故新增取得分頁資料方式
        /// <summary>
        /// 取得分頁資料
        /// </summary>
        /// <typeparam name="T">取得資料類型</typeparam>
        /// <param name="sql">Sql Statement</param>
        /// <param name="pageSize">每頁資料數</param>
        /// <param name="pageIndex">頁數</param>
        /// <param name="recordCount">總筆數</param>
        /// <param name="param">Sql Statement 的參數</param>
        /// <param name="orderClause">排序語法</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageData<T>(string sql, int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param = null, string orderClause = "")
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                param = param ?? new Dictionary<string, object>();
                orderClause = string.IsNullOrEmpty(orderClause) ? " ORDER BY (SELECT NULL) " : orderClause;
                string recordCountSql = $@" SELECT SelectedRowNum 
                                            FROM (SELECT ROW_NUMBER() OVER ({orderClause}) as SelectedRowNum, * 
                                                  FROM ( {sql} ) AS result 
                                                  ) AS result2
";
                WorkLib.WriteLog.Write(true, recordCountSql);
                recordCount = conn.Query<T>(recordCountSql, param).Count();
                int startPos = (pageIndex - 1) * pageSize + 1;
                int endPos = pageIndex * pageSize;

                string fullSql = $@" SELECT * 
                                     FROM (SELECT ROW_NUMBER() OVER ({orderClause}) as SelectedRowNum, * 
                                           FROM ( {sql} ) AS result 
                                           ) AS result2
                                     WHERE SelectedRowNum >= {startPos} AND SelectedRowNum <= {endPos} 
";
                WorkLib.WriteLog.Write(true, fullSql);
                return conn.Query<T>(fullSql, param);
            }
        }
        // 20180706 neil 因原本 DataBase.GetPageData 需從 DataTable 轉型至 Model 很不方便，故新增取得分頁資料方式
        /// <summary>
        /// 取得分頁資料
        /// </summary>
        /// <typeparam name="T">取得資料類型</typeparam>
        /// <param name="sql">Sql Statement</param>
        /// <param name="pageSize">每頁資料數</param>
        /// <param name="pageIndex">頁數</param>
        /// <param name="recordCount">總筆數</param>
        /// <param name="param">Sql Statement 的參數</param>
        /// <param name="orderClause">排序語法</param>
        /// <returns></returns>
        public static System.Data.DataTable GetPageDataTable(string sql, int pageSize, int pageIndex, out int recordCount, Dictionary<string, object> param = null, string orderClause = "")
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                param = param ?? new Dictionary<string, object>();
                orderClause = string.IsNullOrEmpty(orderClause) ? " ORDER BY (SELECT NULL) " : orderClause;
                string recordCountSql = $@" SELECT Count(1) FROM ({sql}) as counts ";

                recordCount = 0;
                System.Data.DataTable count_dt = new System.Data.DataTable();
                SqlCommand count_scmd = new SqlCommand(recordCountSql, conn);
                foreach (string parakey in param.Keys)
                {
                    count_scmd.Parameters.Add(new SqlParameter(parakey, param[parakey]));
                }
                SqlDataAdapter count_sda = new SqlDataAdapter(count_scmd);
                count_sda.Fill(count_dt);
                if (count_dt != null && count_dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(count_dt.Rows[0][0].ToString()))
                        recordCount = int.Parse(count_dt.Rows[0][0].ToString());
                }
                int offset = (pageIndex - 1) * pageSize;

                string fullSql = $@" SELECT * 
                                     FROM (SELECT ROW_NUMBER() OVER ({orderClause}) as SelectedRowNum, * 
                                           FROM ( {sql} ) AS result 
                                           ) AS result2
                                     ORDER BY SelectedRowNum OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY
";
                System.Data.DataTable dt = new System.Data.DataTable();
                SqlCommand scmd = new SqlCommand(fullSql, conn);
                foreach (string parakey in param.Keys)
                {
                    scmd.Parameters.Add(new SqlParameter(parakey, param[parakey]));
                }
                SqlDataAdapter sda = new SqlDataAdapter(scmd);
                sda.Fill(dt);
                return dt;
            }
        }

        public static int Delete(string tableName, IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Delete " + tableName + " Where ID In ({0})";
                return conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
        }

        public static int Delete(string tableName, string keyColumnName, IEnumerable<long> ids)
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Delete " + tableName + " Where " + keyColumnName + " In ({0})";
                return conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
        }

        public static void Sort(string tableName, IEnumerable<SortItem> items, string where, string specifyColumnName = "ID", string sortColumnName = "Sort")
        {
            if (items == null || items.Count() == 0)
                return;

            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"Select {specifyColumnName} From {tableName} {{0}} Order By IsNull({sortColumnName}, {{1}})";
                where = string.IsNullOrWhiteSpace(where) ? string.Empty : "Where " + where;
                IEnumerable<long> ids = conn.Query<long>(string.Format(sql, where, int.MaxValue));

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = $"Update {tableName} Set {sortColumnName} = {{0}} Where {specifyColumnName} = {{1}}\n";
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

        public static bool IsValueExists(string tableName, string columnName, string value)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $@" SELECT IsNull(Sum(1), 0) count 
                                 FROM {tableName} 
                                 WHERE {columnName} = @Value ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@Value", value);

                int retValue = conn.ExecuteScalar<int>(sql, param);
                return retValue > 0;
            }
        }

        public static void ToggleIssue(string tableName, long id) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Update " + tableName + " Set IsIssue = 1 - IsIssue Where ID = " + id;
                conn.Execute(sql);
            }
        }
    }
}