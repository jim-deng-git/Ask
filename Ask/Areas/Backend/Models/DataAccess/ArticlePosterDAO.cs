using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Areas.Backend.Models;
using WorkV3.Common;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ArticlePosterDAO
    {
        public static ArticlePosterModels GetItem(long id) {
            return CommonDA.GetItem<ArticlePosterModels>("ArticlePoster", id);
        }

        public static void SetItem(ArticlePosterModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ArticlePoster");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ArticlePoster Where ID = " + item.ID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                tableObj.Remove("ID");
                tableObj.Remove("MenuID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.ID);
            }
        }

        public static int Delete(long id) {
            long[] ids = { id };
            return Delete(ids);
        }

        public static int Delete(IEnumerable<long> ids) {
            return CommonDA.Delete("ArticlePoster", ids);            
        }

        public static IEnumerable<ArticlePosterModels> GetItems(long menuId) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ArticlePoster Where MenuID = {0} Order By Sort";

                return conn.Query<ArticlePosterModels>(string.Format(sql, menuId));
            }
        }

        public static IEnumerable<ArticlePosterModels> GetIssueItems(long menuId) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ArticlePoster Where MenuID = {0} And IsIssue = 1 Order By Sort";

                return conn.Query<ArticlePosterModels>(string.Format(sql, menuId));
            }
        }

        public static void Sort(long menuId, IEnumerable<SortItem> items) {
            CommonDA.Sort("ArticlePoster", items, "MenuID = " + menuId);
        }

        public static void ToggleIssue(long id) {
            CommonDA.ToggleIssue("ArticlePoster", id);
        }
    }
}