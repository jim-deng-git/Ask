using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WorkLib;
using WorkV3.Areas.Backend.Models;
using WorkV3.Golbal;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class SysLogDAO
    {
        public static void SaveInfo(SysLogModels data)
        {
            
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            SQLData.TableObject TableObj = db.GetTableObject("Syslog");
            TableObj.Add("ID", GetItem.NewSN());
         
            if (MemberDAO.SysCurrent != null)
            {
                TableObj.Add("MemberID", MemberDAO.SysCurrent.Id);
            }

            if (data.MgrNo != null)
                TableObj.Add("MgrNo", data.MgrNo);

            TableObj.Add("Actions", data.Actions);
            
            if (data.SiteID != null)
                TableObj.Add("SiteID", data.SiteID);

            if (data.MenuID != null)
                TableObj.Add("MenuID", data.MenuID);

            if (data.SourceID != null)
                TableObj.Add("SourceID", data.SourceID);

            TableObj.Add("ReMark", data.ReMark);
            TableObj.Add("AddTime", DateTime.Now.ToString(WebInfo.DateTimeFmt));

            string IP = GetItem.IPAddr();
            TableObj.Add("IP", IP);
            TableObj.Add("IPNum", GetItem.GetIPNum(IP));
                           
            TableObj.Insert();
        
        }

        public static List<SysLogModels> GetItems(int pageSize, int pageIndex, out int recordCount, SysLogSearchModel search)
        {
            string KW = search.KW ?? "";

            string SDate = !string.IsNullOrEmpty(search.SDate) ? search.SDate : "1911/1/1";
            string EDate = !string.IsNullOrEmpty(search.EDate) ? search.EDate : "9999/12/31";

            return GetItems(pageSize, pageIndex, out recordCount, Convert.ToDateTime(SDate), Convert.ToDateTime(EDate), KW, search.Actions);
        }

        public static List<SysLogModels> GetItems(int pageSize, int pageIndex, out int recordCount, DateTime SDate, DateTime EDate, string KW = "", int[] actions = null, long memberId = 0)
        {
            List<SysLogModels> items = new List<SysLogModels>();

            string sql = @"SELECT s.*,m.LoginID as MemberLoginID,m.Name as MemberName, site.Title AS SiteTitle,
                                Case WHEN n.Title IS NOT NULL THEN n.Title WHEN p.Title IS NOT NULL THEN p.Title ELSE '' END as MenuTitle 
                                FROM SysLog s 
                                JOIN Member m  ON s.MemberID=m.ID 
                                LEFT JOIN Sites site ON site.ID=s.SiteID 
                                LEFT JOIN Menus n ON n.ID=s.MenuID 
                                LEFT JOIN Pages p ON p.No=s.SourceID 
                                    WHERE AddTime>='" + SDate.ToString("yyyy/MM/dd HH:mm:ss") + "' AND  AddTime<='" + EDate.ToString("yyyy/MM/dd HH:mm:ss") + "' ";
            if (!string.IsNullOrEmpty(KW))
            {
                sql += $"AND (m.LoginID like '%{KW.Replace("'", "''")}%' or m.Name like '%{KW.Replace("'", "''")}%' or m.LoginID like '%{KW.Replace("'", "''")}%' or s.IP like '%{KW.Replace("'", "''")}%') ";
            }
            if (memberId != 0)
            {
                sql += $" AND MemberID = {memberId} ";
            }
            if (actions != null)
            {
                string strActions = string.Join(", ", actions);
                sql += $" AND Actions IN ({strActions}) ";
            }

            items = CommonDA.GetPageData<SysLogModels>(sql, pageSize, pageIndex, out recordCount, null, "ORDER BY AddTime DESC").ToList();

            for (int i = 0; i < items.Count; i++)
            {
                SysLogModels item = items[i];
                items[i].MgrNoName = ((SysMgrNoName)Convert.ToByte(item.MgrNo)).ToString();
                string menuTitle = "", pageTitle = "";
                long? siteID = null, menuID = null, pageID = null;
                if (item.SiteID.HasValue)
                {
                    siteID = item.SiteID.Value;
                    if (item.MenuID.HasValue && item.SourceID.HasValue)
                    {
                        menuID = item.MenuID.Value;
                        pageID = item.SourceID.Value;
                    }
                    else if (item.MenuID.HasValue)
                    {
                        menuID = item.MenuID.Value;
                    }
                    else if (item.SourceID.HasValue)
                    {
                        menuID = item.SourceID.Value;
                    }
                    if (siteID.HasValue && menuID.HasValue && pageID.HasValue)
                    {
                        List<WorkV3.Models.BreadCrumbsModels> BreadCrumbs = WorkV3.Models.DataAccess.MenusDAO.GetBreadCrumbs(siteID.Value, menuID.Value, pageID.Value);
                        //WorkLib.WriteLog.Write(true, BreadCrumbs.Count.ToString());
                        foreach (WorkV3.Models.BreadCrumbsModels breadModel in BreadCrumbs)
                        {
                            menuTitle += breadModel.Title + ">";
                            pageTitle = breadModel.PagesTitle;
                        }
                    }
                   else if (siteID.HasValue && menuID.HasValue)
                    {
                        List<WorkV3.Models.BreadCrumbsModels> BreadCrumbs = WorkV3.Models.DataAccess.MenusDAO.GetBreadCrumbs(siteID.Value, menuID.Value, 0);
                        foreach (WorkV3.Models.BreadCrumbsModels breadModel in BreadCrumbs)
                        {
                            menuTitle += breadModel.Title + ">";
                            pageTitle = breadModel.PagesTitle;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(menuTitle))
                {
                    menuTitle = menuTitle.Trim('>');
                    item.MenuTitle = menuTitle;
                }
                if (!string.IsNullOrEmpty(pageTitle))
                    item.PageTitle = pageTitle;
                //if (item.SiteID.HasValue && item.MenuID.HasValue && )
                //{
                //    List<WorkV3.Models.BreadCrumbsModels> BreadCrumbs = WorkV3.Models.DataAccess.MenusDAO.GetBreadCrumbs(item.SiteID.Value, item.MenuID.Value, item.SourceID);
                //}

                //items[i].MenuTitle = 
                items[i].ActionsName = ((SysActionsName)Convert.ToByte(item.Actions)).ToString();
            }

            return items;
        }


        public static List<SysLogModels> GetAll()
        {
            List<SysLogModels> items = new List<SysLogModels>();

            string sql = "SELECT s.*,m.LoginID as MemberLoginID,m.Name as MemberName,n.Title as MenuTitle FROM SysLog s JOIN Member m  ON s.MemberID=m.ID LEFT JOIN Menus n ON n.ID=s.MenuID ";

            sql += " ORDER BY s.AddTime DESC ";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            foreach (DataRow dr in datas.Rows)
            {
                SysLogModels m = new SysLogModels();
                m.Id = (long)dr["ID"];
                m.MemberID = (long)dr["MemberID"];
                m.MemberName = dr["MemberName"].ToString().Trim();
                m.MemberLoginID = dr["MemberLoginID"].ToString().Trim();
                m.MgrNo = Convert.ToByte(dr["MgrNo"]);
                m.MgrNoName = ((SysMgrNoName)Convert.ToByte(dr["MgrNo"])).ToString();
                m.Actions = Convert.ToByte(dr["Actions"]);
                m.ActionsName = ((SysActionsName)Convert.ToByte(dr["Actions"])).ToString();
                m.MenuID = Convert.ToInt64(m.MenuID ?? 0);
                m.MenuTitle= dr["MenuTitle"].ToString().Trim();
                m.ReMark = dr["ReMark"].ToString().Trim();
                m.AddTime = (DateTime)dr["AddTime"];
                m.IP = dr["IP"].ToString().Trim();

                items.Add(m);
            }


            return items;
        }
    }
}