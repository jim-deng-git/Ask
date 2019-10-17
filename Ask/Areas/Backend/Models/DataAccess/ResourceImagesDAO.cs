using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ResourceImagesDAO
    {
        public static ResourceImagesModels GetItem(long id) {
            return CommonDA.GetItem<ResourceImagesModels>("ResourceImages", id);            
        }

        public static void SetItem(ResourceImagesModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ResourceImages");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ResourceImages Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj.Insert();
            } else {
                string[] removeFields = { "ID", "SiteID", "SourceNo", "SourceType", "Ver", "AreaID", "Creator", "CreateTime" };
                foreach (string f in removeFields)
                    tableObj.Remove(f);

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

        public static IEnumerable<ResourceImagesModels> GetItems(long sourceNo, string code = null) {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ResourceImages Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceImagesModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
            }
        }

        public static int DeleteItems(long sourceNo, string code) {
            return DeleteItemsExcept(sourceNo, code);
        }

        public static int DeleteItemsExcept(long sourceNo, string code, IEnumerable<long> exceptIds = null) {
            string sql = "Delete ResourceImages Where {0}";

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
    }
}