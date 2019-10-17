using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;
using Dapper;
using System.Data.SqlClient;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class PagesDAO
    {

        public long  MenuAddPage(long SiteID,long MenuID, long MenuSN, string Title, bool appendIdToName = false) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            long NewPageNO = GetItem.NewSN();
            string sql =
                "INSERT Pages(No,Lang,Ver,SiteID,MenuID,SN,Title,ShowStatus,PublishStatus,Creator,CreateTime) VALUES(@No,@Lang,@Ver,@SiteID,@MenuID,@SN,@Title,@ShowStatus,@PublishStatus,@Creator,Getdate());\n";

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();
            para.Add("@No", NewPageNO);
            para.Add("@Lang", "zh-tw");
            para.Add("@Ver", 1);
            para.Add("@MenuID", MenuID);
            para.Add("@SiteID", SiteID);
            para.Add("@SN", MenuSN);
            para.Add("@PublishStatus", 1);
            para.Add("@ShowStatus", 1);
            db.ExecuteNonQuery(sql, para);


            if (appendIdToName)
            {
                sql = " Update Pages Set SN = SN + '_'+ NO Where No = @NO ;\n";
            }
            SQLData.ParameterCollection para2 = new SQLData.ParameterCollection();
            para2.Add("@No", NewPageNO);
            db.ExecuteNonQuery(sql, para2);

            
            return NewPageNO;

        }

        public static void SetPageInfo(PagesModels page) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Pages");
            tableObj.GetDataFromObject(page);

            string sql = $"Select 1 From Pages Where No = { page.No } AND Lang = '{ page.Lang.Replace("'", "''") }' AND Ver = { page.Ver } AND SiteID = { page.SiteID } AND MenuID = { page.MenuID }";
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                string[] removeFields = { "No", "Lang", "Ver", "SiteID", "MenuID", "Creator", "CreateTime" };
                foreach (string field in removeFields)
                    tableObj.Remove(field);

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@No", page.No);
                keys.Add("@Lang", page.Lang);
                keys.Add("@Ver", page.Ver);
                keys.Add("@SiteID", page.SiteID);
                keys.Add("@MenuID", page.MenuID);

                tableObj.Update(keys);
            }
        }


        public static void UpdatePageInfo(PagesModels page)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("Pages");
            tableObj.GetDataFromObject(page);

            tableObj.Remove("No");

            tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
            tableObj["ModifyTime"] = DateTime.Now;

            SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
            keys.Add("@No", page.No);

            tableObj.Update(keys);
        }
        public static PagesModels GetPageInfo(long SiteID, long PageNo)
        {
            PagesModels nData = new PagesModels();
            string Sql = "Select * from Pages where No=@No and SiteID=@SiteID ";
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<PagesModels>(
                    Sql,
                    new { No = PageNo, SiteID = SiteID });
                nData = res.FirstOrDefault();
            }
            return nData;
        }
        public static PagesModels GetPageInfo(long PageNo)
        {
            PagesModels nData = new PagesModels();
            string Sql = "Select * from Pages where No=@No ";
            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                var res = SqlCn.Query<PagesModels>(
                    Sql,
                    new { No = PageNo });
                nData = res.FirstOrDefault();
            }
            return nData;
        }
        #region 刪除

        public static void DelPagesByMenuID(long MenuID)
        {
            string Sql = "Delete Pages Where MenuID=@MenuID";

            using (var SqlCn = new SqlConnection(WebInfo.Conn))
            {
                SqlCn.Execute(Sql, new { MenuID = MenuID });
            }
        }

        #endregion

    }
}