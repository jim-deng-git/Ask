using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using Dapper;
using WorkV3.Common;
using System.Data.SqlClient;

namespace WorkV3.Models.DataAccess
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

        public static int Delete(string tableName, IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Delete " + tableName + " Where ID In ({0})";
                return conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }
        }

        public static void Sort(string tableName, IEnumerable<SortItem> items, string where) {
            if (items == null || items.Count() == 0)
                return;

            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select ID From " + tableName + " {0} Order By IsNull(Sort, {1})";
                where = string.IsNullOrWhiteSpace(where) ? string.Empty : "Where " + where;
                IEnumerable<long> ids = conn.Query<long>(string.Format(sql, where, int.MaxValue));

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = "Update " + tableName + " Set Sort = {0} Where ID = {1}\n";
                foreach(long id in ids) {
                    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
                    foreach(SortItem item in updateItems) {
                        sortSql.AppendFormat(fmt, index++, item.ID);
                        itemList.Remove(item);
                    }

                    if (!sortIds.Contains(id)) {
                        sortSql.AppendFormat(fmt, index++, id);
                    }
                }

                conn.Execute(sortSql.ToString());
            }
        }

        //public static void Sort(string tableName, IEnumerable<SortItem> items, string where)
        //{
        //    if (items == null || items.Count() == 0)
        //        return;

        //    IEnumerable<long> sortIds = items.Select(item => item.ID);
        //    List<SortItem> itemList = items.ToList();

        //    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
        //    {

        //        string sql = "Select ID, Sort AS Index  From " + tableName + " {0} Order By IsNull(Sort, {1})";
        //        where = string.IsNullOrWhiteSpace(where) ? string.Empty : "Where " + where;
        //        var SortItemList = conn.Query<SortItem>(string.Format(sql, where, int.MaxValue));
        //        int index = 1;
        //        System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
        //        string fmt = "Update " + tableName + " Set Sort = {0} Where ID = {1}\n";
        //        foreach (SortItem item in items)
        //        {
        //            var updateItem = SortItemList.Where(p => p.ID == item.ID);
        //            if (updateItem != null && updateItem.Count() > 0)
        //                updateItem.ElementAt(0).Index = item.Index;
        //        }
        //        var newSortItemList = SortItemList.OrderBy(item => item.Index);
        //        foreach (SortItem item in SortItemList)
        //        {
        //            if (item.Index != index)
        //            {
        //                sortSql.AppendFormat(fmt, index, item.ID);
        //                index++;
        //            }
        //        }
        //        //foreach (SortItem NewSortItem in SortItemList) {
        //        //    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
        //        //    foreach(SortItem item in updateItems) {
        //        //        sortSql.AppendFormat(fmt, index++, item.ID);
        //        //        itemList.Remove(item);
        //        //    }

        //        //    if (!sortIds.Contains(id)) {
        //        //        sortSql.AppendFormat(fmt, index++, id);
        //        //    }
        //        //}

        //        WorkLib.WriteLog.Write(true, sortSql.ToString());
        //        conn.Execute(sortSql.ToString());
        //    }
        //}
        public static void ToggleIssue(string tableName, long id) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Update " + tableName + " Set IsIssue = 1 - IsIssue Where ID = " + id;
                conn.Execute(sql);
            }
        }
    }
}