using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkLib;
using WorkV3.Areas.Backend.Models;
using WorkV3.Areas.Backend.Abstracts;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class MenusDAO
    {

        /// <summary>
        /// 取得後台狀態樣式
        /// </summary>
        /// <param name="ShowStatus"></param>
        /// <returns></returns>
        public static string GetShowStatusClass(byte ShowStatus)
        {
            string RT = "";
            switch (ShowStatus)
            {
                case (byte)MenuShowStatus.Hide:
                    RT = "cc cc-eye-off";
                    break;
                case (byte)MenuShowStatus.Disabled:
                    RT = "cc cc-stop";
                    break;
                default:
                    RT = "";
                    break;
            }
            return RT;
        }

        /// <summary>
        /// 選單代碼是否存在
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="SN"></param>
        /// <returns></returns>
        public static bool isExistSN(long SiteID, string SN)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            bool RT = true;
            string Sql = "Select 1 From Menus Where SiteID={0} and SN='{1}'";
            DataTable dt = db.GetDataTable(string.Format(Sql, SiteID, SN));
            if (dt.Rows.Count > 0)
                RT = true;
            else
                RT = false;
            return RT;

        }

        /// <summary>
        /// 取得子選單
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static IEnumerable<MenusModels> GetChildren(long Id)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT * 
                                FROM MENUS 
                                WHERE ParentID = @ParentID ";

                IEnumerable<MenusModels> retValue = conn.Query<MenusModels>(sql, new { ParentID = Id });
                foreach (MenusModels menu in retValue)
                {
                    menu.GetChildren();
                }

                return retValue;
            }
        }

        public static IEnumerable<MenusModels> GetRootDataBySite(long site)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT * 
                                FROM Menus 
                                WHERE SiteID = @Site
                                AND ParentID = 0 ";
                return conn.Query<MenusModels>(sql, new { Site = site });
            }
        }

        public static long GetSiteId(long Id)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT SiteId 
                                FROM Menus
                                WHERE ID = @Id ";

                return conn.Query<long>(sql, new { Id = Id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <returns></returns>
        public static List<MenusModels> GetData(long SiteID, MenusAreaIDType AreaID = MenusAreaIDType.All)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string Sql = @"Select ID,SiteID,ParentID,AreaID,M.Title,SN,DataType,DataTypeValue,ShowStatus,M.Sort,Creator,CreateTime,Modifier,ModifyTime,CT.Code MenuCode, CT.ICON as MenuIcon, CT.URLAction MenuURLAction, CT.iFrameH MenuiFrameH, CT.iFrameW MenuiFrameW, CT.EditURLAction MenuEditURLAction, CT.ManageURL MenuManageURL, CT.IsNeedSN MenuIsNeedSN 
                           from Menus M 
                           inner join CardsType CT on DataType=CT.Code 
                           where SiteID=@SiteID";
                if (AreaID != MenusAreaIDType.All)
                {
                    Sql += " and  AreaID=" + AreaID;
                }
                Sql += " order by Sort";

                List<dynamic> queryResult = conn.Query<dynamic>(Sql, new { SiteID = SiteID }).ToList();
                List<MenusModels> result = new List<MenusModels>();

                for (int i = 0; i < queryResult.Count; i++)
                {
                    MenusModels item = ConvertDynamicToMenusModel(queryResult[i]);
                    result.Add(item);
                }

                return result;
            }
        }

        public static MenusModels GetInfo(long SiteID, long Id)
        {

            string Sql = @" Select ID,SiteID,ParentID,AreaID,M.Title,SN,DataType,DataTypeValue,ShowStatus,M.Sort,Creator,CreateTime,Modifier,ModifyTime, M.DisableDelete,
                                   CT.Code MenuCode,CT.ICON as MenuIcon, CT.URLAction MenuURLAction, CT.iFrameH MenuiFrameH, CT.iFrameW MenuiFrameW,
                                   CT.EditURLAction MenuEditURLAction, CT.ManageURL MenuManageURL, CT.IsNeedSN MenuIsNeedSN , M.DisableDelete
                            from Menus M 
                            inner join CardsType CT on DataType=CT.Code 
                            where SiteID={0} and ID={1} ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable dt = db.GetDataTable(string.Format(Sql, SiteID, Id));
            MenusModels _TempRow = new MenusModels();
            if (dt.Rows.Count > 0)
            {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }

        public static MenusModels GetInfo(long Id)
        {

            string Sql = @" Select ID,SiteID,ParentID,AreaID,M.Title,SN,DataType,DataTypeValue,ShowStatus,M.Sort,Creator,CreateTime,Modifier,ModifyTime, 
                                   CT.Code MenuCode,CT.ICON as MenuIcon, CT.URLAction MenuURLAction, CT.iFrameH MenuiFrameH, CT.iFrameW MenuiFrameW,
                                   CT.EditURLAction MenuEditURLAction, CT.ManageURL MenuManageURL, CT.IsNeedSN MenuIsNeedSN ,M.DisableDelete
                            from Menus M 
                            inner join CardsType CT on DataType=CT.Code 
                            where  ID={0} ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable dt = db.GetDataTable(string.Format(Sql, Id));
            MenusModels _TempRow = new MenusModels();
            if (dt.Rows.Count > 0)
            {
                _TempRow = CreateData(dt.Rows[0]);
            }

            return _TempRow;
        }
        #region 新增選單
        public static void Insert_Single(MenusModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject TableObj = db.GetTableObject("Menus");

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();

            TableObj.Add("Title", data.Title);
            TableObj.Add("ParentID", 0);
            TableObj.Add("ShowStatus", data.ShowStatus);
            TableObj.Add("Sort", 0);
            TableObj.Add("ID", data.ID);
            TableObj.Add("SN", data.SN);
            TableObj.Add("SiteID", data.SiteID);
            TableObj.Add("AreaID", data.AreaID);
            TableObj.Add("DataType", data.DataType);
            TableObj.Add("Creator", MemberDAO.SysCurrent.Id);
            TableObj.Add("CreateTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));
            TableObj.Add("DisableDelete", data.DisableDelete);
            TableObj.Insert();

        }

        #endregion

        #region 修改選單排序

        public static void UpdateSort(List<MenusModels> datas)
        {
            string fmt = "Update Menus Set AreaID = {0}, Sort = {1}, ParentID = {2} Where ID = {3} and SiteID={4};\n";
            StringBuilder sql = new StringBuilder();
            if (datas.Count > 0)
            {
                for (int i = 0, len = datas.Count; i < len; ++i)
                {
                    MenusModels menu = datas[i];
                    sql.AppendFormat(fmt, menu.AreaID, i + 1, menu.ParentID, menu.ID, menu.SiteID);
                }
                SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                db.ExecuteNonQuery(sql.ToString());
            }

        }
        #endregion

        #region MenuEdit_Single
        public static void Save_Single(MenusModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            SQLData.TableObject TableObj = db.GetTableObject("Menus");

            TableObj.Add("Title", data.Title);
            //TableObj.Add("ParentID", data.ParentID);
            TableObj.Add("ShowStatus", data.ShowStatus);
            //TableObj.Add("Sort", data.Sort);

            TableObj.Add("Modifier", MemberDAO.SysCurrent.Id);
            TableObj.Add("ModifyTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();
            para.Add("@ID", data.ID);
            TableObj.Update(para);
            PagesModels mPage = PagesDAO.GetPageInfo(data.SiteID, data.ID);
            if (mPage != null)
            {
                mPage.Title = data.Title;
                PagesDAO.SetPageInfo(mPage);
            }

        }
        #endregion


        public static void UpdateParentID(List<MenusModels> datas, long? parentID)
        {
            string fmt = "Update Menus Set ParentID = {1} Where ID = {0};\n";
            StringBuilder sql = new StringBuilder();
            if (datas.Count > 0)
            {
                for (int i = 0, len = datas.Count; i < len; ++i)
                {
                    MenusModels menu = datas[i];
                    sql.AppendFormat(fmt, menu.ID, parentID.HasValue?parentID.Value.ToString():"Null");
                }
                SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                db.ExecuteNonQuery(sql.ToString());
            }

        }
        #region 修改階層型選單 #MenuEdit_Folder
        public static void Save_Folder(MenusModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            SQLData.TableObject TableObj = db.GetTableObject("Menus");

            TableObj.Add("Title", data.Title);
            TableObj.Add("ShowStatus", data.ShowStatus);
            TableObj.Add("DataTypeValue", data.DataTypeValue);
            TableObj.Add("Modifier", MemberDAO.SysCurrent.Id);
            TableObj.Add("ModifyTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();
            para.Add("@ID", data.ID);
            TableObj.Update(para);

        }
        #endregion

        public static void SaveInfo(MenusModels data)
        {

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            SQLData.TableObject TableObj = db.GetTableObject("Menus");
            //TableObj.GetDataFromObject(data);

            TableObj.Add("Title", data.Title);
            TableObj.Add("ShowStatus", data.ShowStatus);

            SQLData.ParameterCollection para = new SQLData.ParameterCollection();
            if (data.ID == 0)
            {
                TableObj.Add("ID", GetItem.NewSN());
                TableObj.Add("SN", data.SN);
                TableObj.Add("SiteID", data.SiteID);
                TableObj.Add("AreaID", data.AreaID);
                TableObj.Add("ParentID", data.ParentID);
                TableObj.Add("DataType", data.DataType);
                TableObj.Add("Sort", data.Sort);
                TableObj.Add("Creator", MemberDAO.SysCurrent.Id);
                TableObj.Add("CreateTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));
                TableObj.Insert();
            }
            else
            {
                TableObj.Add("Modifier", MemberDAO.SysCurrent.Id);
                TableObj.Add("ModifyTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));
                para.Add("@ID", data.ID);
                TableObj.Update(para);
            }

        }


        #region CreateDataRow

        private static MenusModels ConvertDynamicToMenusModel(dynamic obj)
        {
            MenusModels nData = new MenusModels
            {
                ID = obj.ID,
                SiteID = obj.SiteID,
                ParentID = obj.ParentID,
                AreaID = obj.AreaID,
                Title = obj.Title,
                SN = obj.SN,
                DataType = obj.DataType,
                DataTypeValue = obj.DataTypeValue,
                ShowStatus = obj.ShowStatus,
                Sort = obj.Sort,
                Creator = obj.Creator,
                CreateTime = obj.CreateTime,
                Modifier = obj.Modifier,
                ModifyTime = obj.ModifyTime,
                MenuIcon = obj.MenuIcon,
                MenuURLAction = obj.MenuURLAction,
                MenuEditURLAction = obj.MenuEditURLAction,
                MenuiFrameH = obj.MenuiFrameH,
                MenuiFrameW = obj.MenuiFrameW,
                MenuCode = obj.MenuCode,
                MenuManageURL = GetManageURLs(obj.MenuManageURL),
                MenuIsNeedSN = obj.MenuIsNeedSN
            };

            return nData;
        }


        private static MenusModels CreateData(DataRow dr)
        {
            MenusModels nData = new MenusModels
            {
                ID = (long)dr["id"],
                SiteID = (long)dr["SiteID"],
                ParentID = (long)dr["ParentID"],
                AreaID = (byte)dr["AreaID"],
                Title = dr["Title"].ToString().Trim(),
                SN = dr["SN"].ToString().Trim(),
                DataType = dr["DataType"].ToString().Trim(),
                DataTypeValue = dr["DataTypeValue"].ToString().Trim(),
                ShowStatus = (byte)dr["ShowStatus"],
                Sort = (int)dr["Sort"],
                Creator = (long)dr["Creator"],
                CreateTime = dr["CreateTime"] as DateTime?,
                Modifier = dr["Modifier"] as long?,
                ModifyTime = dr["ModifyTime"] as DateTime?,
                MenuIcon = dr["MenuIcon"].ToString().Trim(),
                MenuURLAction = dr["MenuURLAction"].ToString().Trim(),
                MenuEditURLAction = dr["MenuEditURLAction"].ToString().Trim(),
                MenuiFrameH = (int)dr["MenuiFrameH"],
                MenuiFrameW = (int)dr["MenuiFrameW"],
                MenuCode = dr["MenuCode"].ToString(),
                MenuManageURL = GetManageURLs(dr["MenuManageURL"].ToString()),
                MenuIsNeedSN = (bool)dr["MenuIsNeedSN"],
                DisableDelete = (bool)dr["DisableDelete"]
            };
            return nData;
        }

        public static List<MenusModels.MenuManageURLs> GetManageURLs(string MenuManageURL)
        {
            List<MenusModels.MenuManageURLs> LItems = new List<MenusModels.MenuManageURLs>();
            if (!string.IsNullOrEmpty(MenuManageURL))
            {
                LItems = JsonConvert.DeserializeObject<List<MenusModels.MenuManageURLs>>(MenuManageURL);
            }
            else
            {
                LItems = null;
            }
            return LItems;
        }

        #endregion


        public static int Delete(long id)
        {
            string sql =
                "Delete Menus Where ID={0}";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, id.ToString()));
            }
            return num;
        }
        public static bool DeleteMenu(WorkV3.Areas.Backend.Models.MenusModels item, bool isDeleteAll)
        {
            var childMenus = WorkV3.Areas.Backend.Models.DataAccess.MenusDAO.GetChildren(item.ID);
            if (isDeleteAll) // 若全部刪除, 連子項目一併刪除, 否 => 模組模式不作處理
            {
                //
                var pageItem = WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.GetPageInfo(item.ID); // 選單鍵值=頁面鍵值 => 模組選單頁
                if (pageItem != null)
                {
                    var pageZones = WorkV3.Models.DataAccess.ZonesDAO.GetPageData(pageItem.SiteID, pageItem.No);
                    if (pageZones != null && pageZones.Count() > 0)
                    {
                        bool DeletePage = false;
                        foreach (WorkV3.Models.ZonesModels child in pageZones)
                        {
                            var pageCards = WorkV3.Models.DataAccess.CardsDAO.GetZoneData(child.SiteID, child.No);
                            if (pageCards != null && pageCards.Count > 0)
                            {
                                int totalCount = 0;
                                foreach (WorkV3.Models.CardsModels child_card in pageCards)
                                {
                                    switch (child_card.CardsType)
                                    {
                                        case "Article":
                                            var articleList = WorkV3.Models.DataAccess.ArticleDAO.GetItems(item.ID, (long?)null, int.MaxValue, 1, out totalCount);
                                            if (articleList != null && articleList.Count() > 0)
                                            {
                                                var deleteIDs = articleList.Select(s => s.ID);
                                                WorkV3.Areas.Backend.Models.DataAccess.ArticleDAO.Delete(deleteIDs); // 該方法會連同 cars, zones, pages, 一併刪除
                                            }
                                            break;
                                        case "ArticleIntro":
                                            var articleIntro = WorkV3.Models.DataAccess.ArticleIntroDAO.GetItem(child_card.No);
                                            if (articleIntro != null)
                                            {
                                                WorkV3.Areas.Backend.Models.DataAccess.ArticleIntroDAO.Delete(articleIntro.ID); // 該方法會連同 cars, zones, pages, 一併刪除
                                                DeletePage = true;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        if (!DeletePage)
                        {
                            WorkV3.Areas.Backend.Models.DataAccess.PagesDAO.DelPagesByMenuID(pageItem.MenuID);
                        }
                    }
                }
            }
            if (childMenus != null && childMenus.Count() > 0)
            {
                if (isDeleteAll) // 若全部刪除, 連子項目一併刪除
                {
                    foreach (WorkV3.Areas.Backend.Models.MenusModels child in childMenus)
                    {

                        DeleteMenu(child, isDeleteAll);

                    }
                }
                else //否, 則全部上移
                {
                    UpdateParentID(childMenus.ToList(), item.ParentID);
                }
            }
            return (Delete(item.ID) > 0);
        }
    }
}