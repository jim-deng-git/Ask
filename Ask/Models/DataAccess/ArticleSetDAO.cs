using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models.DataAccess;
using WorkLib;
using Dapper;

namespace WorkV3.Models.DataAccess
{
    public class ArticleSetDAO
    {
        public static ArticleSetModels GetItem(long cardNo) {
            string sql = $"Select * From ArticleSet Where CardNo = { cardNo }";

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {                
                return conn.Query<ArticleSetModels>(sql).SingleOrDefault();
            }
        }

        public static void SetItem(ArticleSetModels item) {            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ArticleSet");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ArticleSet Where CardNo = " + item.CardNo;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {                
                tableObj.Remove("CardNo");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.CardNo);
            }
        }
    }
}