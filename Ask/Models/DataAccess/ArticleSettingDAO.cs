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
    public class ArticleSettingDAO
    {
        public static ArticleSettingModels GetItem(long menuId) {
            string sql = $"Select * From ArticleSetting Where MenuID = { menuId }";

            ArticleSettingModels setting = null;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {                
                setting = conn.Query<ArticleSettingModels>(sql).SingleOrDefault();
            }

            if (setting == null)
                setting = new ArticleSettingModels {
                    MenuID = menuId,
                    Types = "all",
                    PagingMode = "頁碼分頁",
                    PageSize = 20,
                    IssueSetting = "0", // 刊登期間內的當期資料
                    SortMode = "時間排序",
                    SortField = "IsNull(IssueDate, '1900/01/01') Desc",
                    ReplyStatus = "開放匿名回文",
                    ReplyTitle = "留言區",
                    ReplyPageSize = 10,
                    ExtendReadOpen = true,
                    ExtendReadTitle = "推薦閱讀I",
                    ExtendReadMode = 1,
                    ExtendReadOpen2 = false,
                    ExtendReadTitle2 = "推薦閱讀II",
                    ExtendReadMode2 = 1,
                    ReadMode = 0
                };

            return setting;
        }

        public static void SetItem(ArticleSettingModels item) {            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ArticleSetting");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ArticleSetting Where MenuID = " + item.MenuID;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {                
                tableObj.Remove("MenuID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                tableObj.Update(item.MenuID);
            }
        }
    }
}