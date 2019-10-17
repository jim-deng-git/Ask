using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ResourceLightBoxDAO
    {
        public static ResourceLightBoxModels GetItem(long sourceNo, string code)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "Select Top(1) * From ResourceLightBox Where {0}";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                return conn.Query<ResourceLightBoxModels>(string.Format(sql, string.Join(" And ", where))).SingleOrDefault();
            }
        }

        public static void SetItem(ResourceLightBoxModels item)
        {
            item.Spec = item.Spec ?? string.Empty;            

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ResourceLightBox");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ResourceLightBox Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            }
            else
            {
                string[] removeFields = { "ID", "SiteID", "SourceNo", "SourceType", "Ver", "AreaID", "Code", "Creator", "CreateTime" };
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
    }
}