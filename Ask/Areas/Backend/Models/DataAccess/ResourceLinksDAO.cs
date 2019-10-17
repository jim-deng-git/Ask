using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class ResourceLinksDAO
    {

        #region 查詢

        #region GetInfo
        public static ResourceLinksModels GetInfo(long SiteID, long SourceNo, byte SourceType, int AreaID, long Id, int Ver)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string Sql = "Select * From ResourceLinks Where Id=@Id and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID";

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@ID", Id);
            para.Add("@SiteID", SiteID);
            para.Add("@SourceNo", SourceNo);
            para.Add("@SourceType", SourceType);
            para.Add("@Ver", Ver);
            para.Add("@AreaID", AreaID);
            DataTable dt = db.GetDataTable(Sql, para);
            
            ResourceLinksModels _TempRow = new ResourceLinksModels();
            if (dt.Rows.Count > 0)
            {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }

        public static ResourceLinksModels GetItem(long id) {
            return CommonDA.GetItem<ResourceLinksModels>("ResourceLinks", id);
        }

        public static IEnumerable<ResourceLinksModels> GetItems(long sourceNo, string code = null) {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn)) {
                string sql = "Select * From ResourceLinks Where {0} Order By IsNull(Sort, {1})";

                List<string> where = new List<string>();
                where.Add("SourceNo = " + sourceNo);

                if (!string.IsNullOrWhiteSpace(code))
                    where.Add(string.Format("Code = N'{0}'", code.Replace("'", "''")));

                IEnumerable<ResourceLinksModels> items =  conn.Query<ResourceLinksModels>(string.Format(sql, string.Join(" And ", where), int.MaxValue));
                foreach (ResourceLinksModels item in items)
                {
                    if (item.LinkType == ResourceLinkType.inLink.ToString())
                    {
                        int menuLev = 0;
                        Dictionary<int, WorkV3.Models.MenusModels> MenuList = new Dictionary<int, WorkV3.Models.MenusModels>();
                        if (!string.IsNullOrEmpty(item.Detail))
                        {
                            var pageInfo = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(long.Parse(item.Detail));
                            if (pageInfo != null && !string.IsNullOrEmpty(pageInfo.Title))
                            {
                                item.InlinkTitle = pageInfo.Title;
                                var page_menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(pageInfo.MenuID);
                                if (page_menuInfo != null)
                                {
                                    menuLev++;
                                    MenuList.Add(menuLev, page_menuInfo);
                                    while (page_menuInfo.ParentID != 0)
                                    {
                                        page_menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(page_menuInfo.ParentID);
                                        if (page_menuInfo != null && !string.IsNullOrEmpty(page_menuInfo.Title))
                                        {
                                            menuLev++;
                                            MenuList.Add(menuLev, page_menuInfo);
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                var menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(long.Parse(item.Detail));
                                if (menuInfo != null && !string.IsNullOrEmpty(menuInfo.Title))
                                {
                                    item.InlinkTitle = menuInfo.Title;
                                    menuLev++;
                                    MenuList.Add(menuLev, menuInfo);
                                    while (menuInfo.ParentID != 0)
                                    {
                                        menuInfo = WorkV3.Models.DataAccess.MenusDAO.GetInfo(menuInfo.ParentID);
                                        if (menuInfo != null && !string.IsNullOrEmpty(menuInfo.Title))
                                        {
                                            menuLev++;
                                            MenuList.Add(menuLev, menuInfo);
                                        }
                                        else
                                            break;
                                    }
                                }
                            }
                            if (MenuList != null && MenuList.Count > 0)
                            {
                                var orderMenus = MenuList.OrderByDescending(p => p.Key).Select(p => p.Value.Id);
                                item.InLinkPath = string.Join(",", orderMenus.ToArray());
                            }
                        }
                    }
                }
                return items;
            }
        }
        #endregion

        #endregion

        #region  連結選單
        /// <summary>
        /// 連結檔案選單
        /// </summary>
        /// <param name="data"></param>
        public static void Save_Menu(ResourceLinksModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            
            string Sql =
                "IF EXISTS (SELECT 1 FROM ResourceLinks WHERE ID=@ID and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID)" +

                " UPDATE ResourceLinks SET LinkType=@LinkType, LinkInfo=@LinkInfo, Descriptions=Descriptions, Detail=@Detail, ClickType=@ClickType, Modifier=@Modifier, ModifyTime=GetDate() WHERE ID=@ID and SiteID=@SiteID and SourceNo=@SourceNo and SourceType=@SourceType and Ver=@Ver and AreaID=@AreaID" +

                " ELSE" +

                " INSERT INTO ResourceLinks(ID,SiteID,SourceNo, SourceType,Ver,AreaID,LinkType,LinkInfo,Descriptions,Detail,ClickType,Creator,CreateTime)VALUES(@ID,@SiteID,@SourceNo,@SourceType,@Ver,@AreaID,@LinkType,@LinkInfo,@Descriptions,@Detail,@ClickType,@Creator,GetDate())";
            
            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            para.Add("@ID", data.Id);
            para.Add("@SiteID", data.SiteID);
            para.Add("@SourceNo", data.SourceNo);
            para.Add("@SourceType", data.SourceType);
            para.Add("@Ver", data.Ver);
            para.Add("@AreaID", data.AreaID);
            para.Add("@LinkType", data.LinkType);
            para.Add("@LinkInfo", data.LinkInfo);
            para.Add("@Descriptions", data.Descriptions);
            para.Add("@Detail", data.Detail);
            para.Add("@ClickType", data.ClickType);
            para.Add("@Creator", MemberDAO.SysCurrent.Id);
            para.Add("@Modifier", MemberDAO.SysCurrent.Id);
            db.ExecuteNonQuery(Sql, para);

        }


        #endregion

        public static int DeleteItemsExcept(long sourceNo, string code, IEnumerable<long> exceptIds = null) {
            string sql = "Delete ResourceLinks Where {0}";

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

        public static void SetItem(ResourceLinksModels item) {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("ResourceLinks");
            tableObj.GetDataFromObject(item);

            string sql = "Select 1 From ResourceLinks Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew) {                
                tableObj["ClickType"] = 0;
                tableObj["Creator"] = MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = DateTime.Now;

                tableObj.Insert();
            } else {
                string[] removeFields = { "ID", "SiteID", "SourceNo", "SourceType", "Ver", "AreaID", "ClickType", "Code", "Creator", "CreateTime" };
                foreach (string f in removeFields)
                    tableObj.Remove(f);

                tableObj["Modifier"] = MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = DateTime.Now;

                SQLData.ParameterCollection keys = new SQLData.ParameterCollection();
                keys.Add("@ID", item.Id);
                keys.Add("@SiteID", item.SiteID);
                keys.Add("@SourceNo", item.SourceNo);
                keys.Add("@SourceType", item.SourceType);
                keys.Add("@Ver", item.Ver);
                keys.Add("@AreaID", item.AreaID);

                tableObj.Update(keys);
            }
        }

        #region CreateDataRow        
        private static ResourceLinksModels CreateData(DataRow dr)
        {

            ResourceLinksModels nData = new ResourceLinksModels
            {
                Id = (long)dr["id"],
                SiteID = (long)dr["SiteID"],
                SourceNo = (long)dr["SourceNo"],
                SourceType = (byte)dr["SourceType"],
                Ver = (int)dr["Ver"],
                AreaID = (byte)dr["AreaID"],
                LinkType = dr["LinkType"].ToString().Trim(),
                LinkInfo = dr["LinkInfo"].ToString().Trim(),
                Descriptions = dr["Descriptions"].ToString().Trim(),
                Detail = dr["Detail"].ToString().Trim(),
                ClickType = (int)dr["ClickType"],
                Creator = (long)dr["Creator"],
                CreateTime = dr["CreateTime"] as DateTime?,
                Modifier = dr["Modifier"] as long?,
                ModifyTime = dr["ModifyTime"] as DateTime?
            };

            return nData;
        }
        #endregion
    }
}