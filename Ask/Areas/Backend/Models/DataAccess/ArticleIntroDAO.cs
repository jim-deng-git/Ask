using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ArticleIntroDAO
    {
        public static ArticleIntroModels GetItem(long menuId)
        {
            using(System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select Top(1) * From ArticleIntro Where MenuID = " + menuId;
                return conn.Query<ArticleIntroModels>(sql).SingleOrDefault();
            }
        }

        public static void SetItem(ArticleIntroModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ArticleIntro");

            item.Icon = item.Icon ?? string.Empty;
            item.RemarkText = item.RemarkText ?? string.Empty;

            tableObj.GetDataFromObject(item);
            string sql = "Select 1 From ArticleIntro Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                long cardNo = WorkV3.Models.DataAccess.MenusDAO.GetFirstCardNo(item.MenuID, "ArticleIntro") ?? 0;
                tableObj.Add("CardNo", cardNo);

                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("MenuID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                
                tableObj.Update(item.ID);
            }
        }
        public static int Delete(long id)
        {
            string sql =
                "Delete Pages Where NO IN (SELECT PageNo From Zones Where No IN (SELECT ZoneNo From Cards WHERE No IN (Select CardNo From ArticleIntro WHERE  ID={0})))\n" +
                "Delete Zones Where No IN (SELECT ZoneNo From Cards WHERE No IN (Select CardNo From ArticleIntro WHERE  ID={0}))\n" +
                "Delete Cards WHERE No IN (Select CardNo From Article WHERE ID={0}) \n" +
                "Delete ArticleIntro Where ID={0} \n";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, id.ToString()));
            }
            long[] ids = new long[1] { id };
            ParagraphDAO.DeleteBySourceNo(ids.ToList());

            return num;
        }
    }
}