using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WorkLib;
using System.Data;
using WorkV3.Common;
using Dapper;

namespace WorkV3.Models
{
    public class FormItemDAO
    {
        public static FormItem GetItem(long id) {
            return DataAccess.CommonDA.GetItem<FormItem>("FormItem", id);
        }

        public static IEnumerable<FormItem> GetItems(long formId) {
            return GetItems(new FormItemSearch { FormID = formId });
        }

        public static IEnumerable<FormItem> GetItems(IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return new List<FormItem>();

            string sql = $"Select * From FormItem Where ID IN ({ string.Join(", ", ids) })";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormItem>(sql);
            }
        }

        public static IEnumerable<FormItem> GetItems(long menuId, string email) {
            string sql = "SELECT * FROM FormItem WHERE {0} Order By CreateDate Desc";
            List<string> where = new List<string>();
            where.Add($"FormID IN (SELECT ID FROM Form WHERE SourceID IN (SELECT ID FROM [Events] WHERE MenuID = { menuId }))");
            where.Add($"Email = N'{ email.Replace("'", "''") }'");

            sql = string.Format(sql, string.Join(" AND ", where));
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormItem>(sql);
            }
        }

        public static void SetItem(FormItem item, bool hasCreateDate = false) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("FormItem");
            tableObj.GetDataFromObject(item);

            DateTime now = DateTime.Now;
            tableObj.Add("ModifyDate", now);

            string sql = "Select ID, CheckStatus From FormItem Where ID = " + item.ID;
            SQLData.SelectObject selectObj = db.GetSelectObject(sql);
            bool isNew = selectObj.Count == 0;
            if (isNew) {
                if(!hasCreateDate)
                    tableObj["CreateDate"] = now;

                if (item.CheckStatus != (byte)FormCheckStatus.待審核)
                    tableObj["CheckDate"] = DateTime.Now;

                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("FormID");
                tableObj.Remove("CreateDate");
                tableObj.Remove("SN");
                tableObj.Remove("IsTemp");
                tableObj.Remove("IsBack");
                tableObj.Remove("CheckDate");
                tableObj.Remove("CheckInDate");

                if (item.CheckStatus != (byte)FormCheckStatus.待審核 && (byte)selectObj["CheckStatus"] != item.CheckStatus)
                    tableObj.Add("CheckDate", DateTime.Now);

                tableObj.Update(item.ID);
            }
        }

        public static IEnumerable<FormItem> GetItems(FormItemSearch search, int pageSize, int pageIndex, out int recordCount) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(GetSearchSql(search), pageSize, pageIndex, out recordCount);

            List<FormItem> items = new List<FormItem>();
            if (datas == null || datas.Rows.Count == 0)
                return items;

            foreach (DataRow dr in datas.Rows) {
                items.Add(new FormItem {
                    ID = (long)dr["ID"], FormID = search.FormID, CreateDate = (DateTime)dr["CreateDate"], CheckStatus = (byte)dr["CheckStatus"], CheckDate = dr["CheckDate"] as DateTime?,
                    IsBack = (bool)dr["IsBack"], SN = dr["SN"] as int?, Remark = dr["Remark"].ToString().Trim(), Email = dr["Email"].ToString().Trim(), Phone = dr["Phone"].ToString().Trim(),
                    Mobile = dr["Mobile"].ToString().Trim(), IDCard = dr["IDCard"].ToString().Trim(), CheckInDate = dr["CheckInDate"] as DateTime?,
                    IsProcess = dr["IsProcess"] == DBNull.Value? false: (bool)dr["IsProcess"], ProcessRemark = dr["ProcessRemark"].ToString(), ProcessTime = dr["ProcessTime"] as DateTime?
                });
            }

            return items;
        }

        public static IEnumerable<FormItem> GetItems(long formId, IEnumerable<IDValue> fieldValues) {
            string sql = "Select * From FormItem Where {0} Order By CreateDate Desc";

            List<string> where = new List<string>();
            where.Add($"FormID = { formId } AND IsTemp = 0");

            if(fieldValues?.Count() > 0) {
                List<string> orSql = new List<string>();
                foreach (IDValue item in fieldValues) {
                    if (!string.IsNullOrWhiteSpace(item.Value)) {
                        orSql.Add($"(FieldID = { item.ID } AND Value = N'{ item.Value.Replace("'", "''") }')");
                    }
                }
                
                where.Add($"ID IN (Select FormItemID From FieldValue Where ({ string.Join(" OR ", orSql) }))");
            }
            sql = string.Format(sql, string.Join(" AND ", where));

            using(SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FormItem>(sql);
            }
        }

        public static List<string> GetItemsAll(FormItemSearch search)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(GetSearchSql(search));

            List<string> items = new List<string>();
            if (datas == null || datas.Rows.Count == 0)
                return items;

            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(dr["ID"].ToString());
                }
            }

            return items;
        }
        public static IEnumerable<FormItem> GetItems(FormItemSearch search) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(GetSearchSql(search));

            List<FormItem> items = new List<FormItem>();
            foreach (DataRow dr in datas.Rows) {
                items.Add(new FormItem {
                    ID = (long)dr["ID"], FormID = search.FormID, CreateDate = (DateTime)dr["CreateDate"], CheckStatus = (byte)dr["CheckStatus"], CheckDate = dr["CheckDate"] as DateTime?,
                    IsBack = (bool)dr["IsBack"], SN = dr["SN"] as int?, Remark = dr["Remark"].ToString().Trim(), Email = dr["Email"].ToString().Trim(), Phone = dr["Phone"].ToString().Trim(),
                    Mobile = dr["Mobile"].ToString().Trim(), IDCard = dr["IDCard"].ToString().Trim(), CheckInDate = dr["CheckInDate"] as DateTime?
                });
            }

            return items;
        }

        private static string GetSearchSql(FormItemSearch search) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "SELECT ID, CreateDate, CheckStatus, CheckDate, IsBack, SN, Remark, Email, Phone, Mobile, IDCard, CheckInDate, IsProcess, ProcessRemark, ProcessTime FROM FormItem I WHERE {0} ORDER BY {1}";

            List<string> where = new List<string>();
            where.Add("FormID = " + search.FormID);
            where.Add("IsTemp = 0");

            if (!string.IsNullOrWhiteSpace(search.Key)) {
                string key = $"N'%{ search.Key.Replace("'", "''") }%'";

                List<string> orSql = new List<string>();
                orSql.Add($"Exists(Select 1 From FieldValue Where FormItemID = I.ID AND Value Like { key })");

                string query = $"Select UserNo From UserFlag Where Flag Like { key } AND UserNo <> '' AND SiteID = (Select SiteID From Form Where ID = { search.FormID })";
                IEnumerable<string> userNo = db.GetDataTable(query).AsEnumerable().Select(dr => $"N'{ dr["UserNo"].ToString().Trim() }'");
                if(userNo?.Count() > 0) {                    
                    orSql.Add(string.Format("Email In ({0}) OR Mobile In ({0}) OR Phone In ({0}) OR IDCard In ({0})", string.Join(", ", userNo)));
                }

                where.Add($"({ string.Join(" OR ", orSql) })");
            }

            if (search.CheckStatusList?.Length > 0)
                where.Add($"CheckStatus IN ({ string.Join(", ", search.CheckStatusList.Select(s => (int)s)) })");

            if (search.FillModes?.Count() == 1)
                where.Add($"IsBack = { search.FillModes[0] }");

            if (search.Start != null)
                where.Add($"CreateDate >= '{ search.Start.ToString("yyyy/MM/dd HH:mm") }'");

            if (search.End != null)
                where.Add($"CreateDate <= '{ search.End.ToString("yyyy/MM/dd HH:mm") }'");

            string[] allowOrders = { "CheckStatus", "CreateDate", "CreateDate Desc" };
            string order = allowOrders.Contains(search.Order) ? search.Order : "CheckStatus";

            return string.Format(sql, string.Join(" AND ", where), order);
        }

        public static int Delete(IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return 0;

            string sql =
                    "DELETE FieldValue WHERE FormItemID IN ({0})\n" +
                    "DELETE FormItem WHERE ID IN ({0})\n";

            int num = 0;
            using (SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static void Complete(long id, FormCheckStatus status) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"Update FormItem Set IsTemp = 0, CheckStatus = { (int)status } Where ID = { id }";
            db.ExecuteNonQuery(sql);
        }

        public static void SetCheckStatus(IEnumerable<long> ids, FormCheckStatus status) {
            if (ids == null || ids.Count() == 0)
                return;

            string sql = $"Update FormItem Set CheckStatus = { (int)status }, CheckDate = GetDate() Where ID IN ({ string.Join(", ", ids) })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static void CheckIn(long formId, IEnumerable<long> ids) {
            if (ids == null || ids.Count() == 0)
                return;

            string sql = $"Update FormItem Set CheckInDate = GetDate() Where FormID = { formId } AND ID IN ({ string.Join(", ", ids) })";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            db.ExecuteNonQuery(sql);
        }

        public static void Process(long[] ids, bool isProcess, string remark)
        {
            if (ids == null || ids.Length == 0)
                return;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = $"Update FormItem Set IsProcess = { (isProcess ? 1 : 0) }, ProcessTime = GetDate(), ProcessRemark = N'{ (remark ?? string.Empty).Replace("'", "''") }' Where ID IN ({ string.Join(", ", ids) })";
            db.ExecuteNonQuery(sql);
        }
    }

    public class FieldValueDAO 
    {
        public static IEnumerable<FieldValue> GetItems(long formItemId) {
            string sql = "Select * From FieldValue Where FormItemID = " + formItemId;

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FieldValue>(sql);
            }
        }

        public static IEnumerable<FieldValue> GetItemsByFormID(long formId) {
            string sql = $"Select * From FieldValue Where FormItemID IN (Select ID From FormItem Where FormID = { formId })";

            using (SqlConnection conn = new SqlConnection(WebInfo.Conn)) {
                return conn.Query<FieldValue>(sql);
            }
        }

        public static void SetItem(FieldValue item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("FieldValue");
            tableObj.GetDataFromObject(item);

            string sql = $"Select 1 From FieldValue Where FormItemID = { item.FormItemID } AND FieldID = { item.FieldID }";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                tableObj.Remove("FormItemID");
                tableObj.Remove("FieldID");

                SQLData.ParameterCollection paras = new SQLData.ParameterCollection();
                paras.Add("@FormItemID", item.FormItemID);
                paras.Add("@FieldID", item.FieldID);

                tableObj.Update(paras);
            }
        }

        public static bool IsExist(long formId, long fieldId, string value, long? formItemId) {
            string sql = $"Select Top(1) 1 From FieldValue Where FieldID = { fieldId } AND Value = N'{ (value ?? string.Empty).Replace("'", "''") }' " +
                $"AND FormItemID IN (Select ID From FormItem Where FormID = { formId })";
            if (formItemId != null)
                sql += " AND FormItemID <> " + formItemId;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return db.GetFirstValue(sql) != null;
        }

        public static int GetItemCount(long formId, long fieldId, string value, long? formItemId) {
            string sql = $"Select Count(*) From FieldValue Where FieldID = { fieldId } AND Value = N'{ (value ?? string.Empty).Replace("'", "''") }' " +
                $"AND FormItemID IN (Select ID From FormItem Where FormID = { formId })";
            if (formItemId != null)
                sql += " AND FormItemID <> " + formItemId;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            return (int)db.GetFirstValue(sql);
        }
    }
}