using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ResourceVoicesDAO
    {
        public static ResourceVoicesModels GetItem(long id) {
            return CommonDA.GetItem<ResourceVoicesModels>("ResourceVoices", id);            
        }

        public static void SetItem(ResourceVoicesModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ResourceVoices");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ResourceVoices Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["MimeType"] = uMimeType.GetMimeType(item.Path);
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                string[] removeFields = { "ID", "SiteID", "SourceNo", "SourceType", "Ver", "AreaID", "Code", "Path", "MimeType", "Creator", "CreateTime" };
                foreach (string f in removeFields)
                    tableObj.Remove(f);

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@ID", item.ID);
                keys.Add("@SiteID", item.SiteID);
                keys.Add("@SourceNo", item.SourceNo);
                keys.Add("@SourceType", item.SourceType);
                keys.Add("@Ver", item.Ver);
                keys.Add("@AreaID", item.AreaID);
                
                tableObj.Update(keys);
            }
        }

        public static IEnumerable<ResourceVoicesModels> GetItems(long sourceNo, string code = null) {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ResourceVoices Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceVoicesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }
        
        public static int DeleteItemsExcept(long sourceNo, string code, IEnumerable<long> exceptIds = null) {
            string sql = "Delete ResourceVoices Where {0}";

            List<string> where = new List<string>();
            where.Add("SourceNo = " + sourceNo);

            if (!string.IsNullOrWhiteSpace(code))
                where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

            if (exceptIds != null && exceptIds.Count() > 0)
                where.Add(string.Format("ID Not In ({0})", string.Join(", ", exceptIds)));

            sql = string.Format(sql, string.Join(" And ", where));

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                return conn.Execute(sql);
            }
        }

        public static int DeleteItems(long sourceNo, string code) {
            return DeleteItemsExcept(sourceNo, code);
        }
    }
}