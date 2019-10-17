using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;
using System.Data.SqlClient;
namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class SiteSeoSettingDAO
    {
        /// <summary>
        /// 取得 model 內容
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static SiteSeoSettingModels GetItem(long siteID)
        {
            SiteSeoSettingModels item = null;
            string sql = $"Select * from [SiteSeoSetting] Where SiteID={siteID}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                DataRow dr = datas.Rows[0];
                item = new SiteSeoSettingModels()
                {
                    SiteID = (long)dr["SiteID"],
                    Robot = (bool)dr["Robot"],
                    Title = dr["Title"].ToString(),
                    Description = dr["Description"].ToString(),
                    Author = dr["Author"].ToString(),
                    Copyright = dr["Copyright"].ToString(),
                    Keywords = dr["Keywords"].ToString(),
                    GA = dr["GA"].ToString(),
                    GTM = dr["GTM"].ToString(),
                    Baidu = dr["Baidu"].ToString(),
                    Alexa = dr["Alexa"].ToString(),
                    GoogleSearch = dr["GoogleSearch"].ToString(),
                    BaiduMA = dr["BaiduMA"].ToString(),
                    Bing = dr["Bing"].ToString(),
                    IsExtraCode = (bool)dr["IsExtraCode"],
                    ExtraCode = dr["ExtraCode"].ToString(),
                    Creator = (long)dr["Creator"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    Modifier = (long)dr["Modifier"],
                    ModifyTime = (DateTime)dr["ModifyTime"]
                };
            }
            return item;
        }

        public static void SetStatus(long SiteID, bool Robot)
        {
            SiteSeoSettingModels item = new SiteSeoSettingModels();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@SiteID", SiteID);
            paraList.Add("@Robot", Robot);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string updateSQL = " UPDATE SiteSeoSetting SET Robot=@Robot WHERE SiteID=@SiteID ";
            db.ExecuteNonQuery(updateSQL, paraList);
        }
        public static bool SetItem(SiteSeoSettingModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("SiteSeoSetting");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM SiteSeoSetting WHERE  SiteID='{ item.SiteID }' ";

            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                tableObj["Creator"] = Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("SiteID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                if(string.IsNullOrEmpty(item.Title))
                    tableObj["Title"] = "";
                if (string.IsNullOrEmpty(item.Description))
                    tableObj["Description"] = "";
                if (string.IsNullOrEmpty(item.Author))
                    tableObj["Author"] = "";
                if (string.IsNullOrEmpty(item.Copyright))
                    tableObj["Copyright"] = "";
                if (string.IsNullOrEmpty(item.Keywords))
                    tableObj["Keywords"] = "";
                if (string.IsNullOrEmpty(item.GA))
                    tableObj["GA"] = "";
                if (string.IsNullOrEmpty(item.GTM))
                    tableObj["GTM"] = "";
                if (string.IsNullOrEmpty(item.Baidu))
                    tableObj["Baidu"] = "";
                if (string.IsNullOrEmpty(item.Alexa))
                    tableObj["Alexa"] = "";
                if (string.IsNullOrEmpty(item.GoogleSearch))
                    tableObj["GoogleSearch"] = "";
                if (string.IsNullOrEmpty(item.BaiduMA))
                    tableObj["BaiduMA"] = "";
                if (string.IsNullOrEmpty(item.Bing))
                    tableObj["Bing"] = "";
                if (string.IsNullOrEmpty(item.ExtraCode))
                    tableObj["ExtraCode"] = "";

                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.SiteID);
            }
            return true;
        }
        public static IEnumerable<string> GetKeywords(long siteId)
        {
            string sql = $"SELECT Keywords FROM SEO Where Keywords <> '' AND MenuID IN (SELECT ID FROM Menus WHERE SiteID = { siteId }) UNION  SELECT Keywords FROM SiteSeoSetting Where Keywords <> '' AND SiteID = { siteId }";
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            DataTable datas = db.GetDataTable(sql);
            List<string> wordList = new List<string>();
            foreach (DataRow dr in datas.Rows)
            {
                string[] words = dr["Keywords"].ToString().Trim().Split(';');
                foreach (string w in words)
                {
                    if(!wordList.Contains(w))
                        wordList.Add(w);
                }
            }

            return wordList;
        }
    }
}