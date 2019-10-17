using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class CustomLineNewsDAO
    {
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static IEnumerable<CustomLineNewsModels> GetItems(CustomLineNewsSearchModels search, int pageSize, int pageIndex, out int recordCount)
        {           
            List<CustomLineNewsModels> items = new List<CustomLineNewsModels>();
            if (search == null)
            {
                recordCount = 0;
                return items;
            }

            string sql = "Select ID,[SelectDate], (Select Count(*) from [CustomLINENewsData] Where [SourceID] =L.ID) AS Total from [CustomLINENews] L Where {0} Order By SelectDate Desc";
           
            List<string> where = new List<string>();           
            where.Add("SiteID = " + search.SiteID);
          
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new CustomLineNewsModels
                    {
                        ID = (long)dr["ID"],
                        Total = (int)dr["Total"],
                        SelectDate = (DateTime)dr["SelectDate"]
                    });
                }
            }


            return items;
        }


        /// <summary>
        /// 新增資料列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static IEnumerable<CustomLineNewsDataModels> GetDataSearchItems(CustomLineNewsSearchModels search, int pageSize, int pageIndex, out int recordCount)
        {
            List<CustomLineNewsDataModels> items = new List<CustomLineNewsDataModels>();
            recordCount = 0;
            if (search == null)
            {
                return items;
            }
            #region 找資料
             
            if (search.SiteID <= 0 | search.SelectMenuID <= 0)
            {
                return items;
            }
            string DataType = "";
            string sql;
            List<string> where = new List<string>();
            where.Add("SiteID = " + search.SiteID);
            where.Add("MenuID = " + search.SelectMenuID);
            MenusModels M = MenusDAO.GetInfo(search.SiteID, search.SelectMenuID);
            if (M.DataType != null)
            {
                DataType = M.DataType;
                switch (DataType.ToLower())
                {
                    case "events":
                        sql = "Select ID as SelectID, Title From Events Where {0} Order By sort Asc ";

                        if (!string.IsNullOrWhiteSpace(search.Key))
                        {
                            string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                            //where.Add(string.Format("(Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
                            where.Add(string.Format("Title {0}", key));
                        }
                        break;
                    case "article":
                    default:
                        if (!string.IsNullOrWhiteSpace(search.Key))
                        {
                            string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                            //where.Add(string.Format("(Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
                            where.Add(string.Format("Title {0}", key));
                        }
                        sql = "Select ID as SelectID, Title From Article Where {0} Order By sort Asc";

                        break;

                }
                SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);
                if (datas != null)
                {
                    foreach (DataRow dr in datas.Rows)
                    {
                        items.Add(new CustomLineNewsDataModels
                        {
                            SelectMenuID = M.ID,
                            MenuTitle = M.Title,
                            SelectID = (long)dr["SelectID"],
                            Title = dr["Title"].ToString()
                        });
                    }
                
                }
            }

            #endregion

     
            return items;

            
        }

        /// <summary>
        /// 已加入資料列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static IEnumerable<CustomLineNewsDataModels> GetDataItems(CustomLineNewsSearchModels search, int pageSize, int pageIndex, out int recordCount)
        {
            List<CustomLineNewsDataModels> items = new List<CustomLineNewsDataModels>();
            recordCount = 0;
            if (search == null)
            {
                return items;
            }
            #region 找資料

            if (search.SiteID <= 0 | search.SelectMenuID <= 0)
            {
                return items;
            }
            string DataType = "";
            string sql = "Select M.Title MenuTitle,M.DataType," +
            " Case M.DataType " +
            " WHEN 'Article' " +
            " THEN (Select Title from Article A Where LND.SelectID = A.ID And LND.SiteID = A.SiteID And LND.SelectMenuID = A.MenuID) " +
            " WHEN 'Event' " +
            " THEN (Select Title from[Events] A Where LND.SelectID = A.ID And LND.SiteID = A.SiteID And LND.SelectMenuID = A.MenuID) " +
            " ELSE " +
            " '' " +
            " End Title " +
            " , LND.* " +
            " From CustomLINENewsData LND " +
            " Inner Join Menus M On LND.SelectMenuID= M.ID " +


            " Where SourceID = {}; ";
            List<string> where = new List<string>();
            where.Add("SourceID = " + search.SiteID);
            where.Add("MenuID = " + search.SelectMenuID);
            MenusModels M = MenusDAO.GetInfo(search.SiteID, search.SelectMenuID);
            if (M.DataType != null)
            {
                DataType = M.DataType;
                switch (DataType.ToLower())
                {
                    case "events":
                        sql = "Select ID as SelectID, Title From Events Where {0} Order By sort Asc ";

                        if (!string.IsNullOrWhiteSpace(search.Key))
                        {
                            string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                            //where.Add(string.Format("(Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
                            where.Add(string.Format("Title {0}", key));
                        }
                        break;
                    case "article":
                    default:
                        if (!string.IsNullOrWhiteSpace(search.Key))
                        {
                            string key = string.Format("Like N'%{0}%'", search.Key.Replace("'", "''"));
                            //where.Add(string.Format("(Title {0} OR Exists(Select 1 From Paragraph Where SourceNo = A.ID And (Title {0} OR Contents {0})))", key));
                            where.Add(string.Format("Title {0}", key));
                        }
                        sql = "Select ID as SelectID, Title From Article Where {0} Order By sort Asc";

                        break;

                }
                SQLData.Database db = new SQLData.Database(WebInfo.Conn);
                DataTable datas = db.GetPageData(string.Format(sql, string.Join(" And ", where)), pageSize, pageIndex, out recordCount);
                if (datas != null)
                {
                    foreach (DataRow dr in datas.Rows)
                    {
                        items.Add(new CustomLineNewsDataModels
                        {
                            SelectMenuID = M.ID,
                            MenuTitle = M.Title,
                            SelectID = (long)dr["SelectID"],
                            Title = dr["Title"].ToString()
                        });
                    }

                }
            }

            #endregion


            return items;


        }

        /// <summary>
        /// 找尋選單
        /// </summary>
        /// <param name="SiteID"></param>
        /// <returns></returns>
        public static IEnumerable<MenusModels> GetMenusItems(long SiteID)
        {
            List<MenusModels> items = new List<MenusModels>();
            string sql = "Select ID, Title From Menus where {0} order by AreaID Asc, Sort desc";

            List<string> where = new List<string>();
            where.Add("SiteID = " + SiteID);
            where.Add("[DataType] in('Article','Event')" );　/*,'ArticleIntro'*/

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(string.Format(sql, string.Join(" And ", where)));
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new MenusModels
                    {
                        ID = (long)dr["Id"],
                        Title = dr["Title"].ToString()
                    });
                }
            }
            return items;
        }

        /// <summary>
        /// 檢查日期是否存在
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="SelectDate"></param>
        /// <returns></returns>
        public static bool isExistSelectDate(long SiteID, string SelectDate)
        {
            bool RT = false;

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select Count(*) Total From [CustomLINENews] where {0}";

            List<string> where = new List<string>();
            where.Add("SiteID = " + SiteID);
            where.Add("[SelectDate] ='" + SelectDate +"'");　
            object datas = db.GetFirstValue(string.Format(sql, string.Join(" And ", where)));
            if (uCheck.IsNumeric(datas))
            {
                int Total = (int)(datas);
                if (Total > 0)
                    RT = true;
            }
            return RT;
        }

        public static void InsertLineNewsData(long SiteID, string SelectDate,long Sid, long SelectMenuID, IEnumerable<long> SelectIDs)
        {
            long userID = MemberDAO.SysCurrent.Id;

            #region 主table
            string Sql = "";
            if (isExistSelectDate(SiteID, SelectDate))
            {
                Sql = "update CustomLINENews Set Modifier = " + userID + ", ModifyTime= Getdate() where SiteID={0} and ID={1}";

            }
            else
            {
                Sql = "INSERT INTO [dbo].[CustomLINENews]([ID],[SiteID],[SelectDate],[Creator],[CreateTime])VALUES({1},{0},'" + SelectDate + "', " + userID + " ,GetDate())";
                Sid = GetItem.NewSN();
            }
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                conn.Execute(string.Format(Sql, SiteID, Sid));
            }
            #endregion

            #region 子資料
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            string fmt = " IF Not EXISTS (SELECT 1 FROM [CustomLINENewsData] WHERE [SourceID]={0} and SiteID={1} and SelectMenuID={2} and SelectID={3})\n " +
                       " INSERT INTO [dbo].[CustomLINENewsData]([ID],[SourceID],[SiteID],[SelectMenuID],[SelectID],[Creator],[CreateTime])VALUES({4},{0},{1} ,{2} ,{3} ," + userID + " ,GetDate()) \n ";
            int i = 0;
            if (SelectIDs.Count() > 0)
            {
                foreach (long SelectID in SelectIDs)
                    sql.AppendFormat(fmt, Sid, SiteID, SelectMenuID, SelectID, GetItem.NewSN(), ++i);
                
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    conn.Execute(sql.ToString());
                }
            }
            #endregion


        }
    }
}