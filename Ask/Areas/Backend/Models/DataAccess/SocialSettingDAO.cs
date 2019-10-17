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
    public class SocialSettingDAO
    {
        /// <summary>
        /// 取得 model 內容
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public static SocialSettingModels GetItem(long siteID)
        {
            SocialSettingModels item = null;
            string sql = $"Select * from [SocialSetting] Where SiteID={siteID}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                DataRow dr = datas.Rows[0];
                item = new SocialSettingModels()
                {
                    SiteID = (long)dr["SiteID"],
                    IsOpen = (bool)dr["IsOpen"],
                    SocialDefaultImage = dr["SocialDefaultImage"].ToString(),
                    IsHeaderOpenChannel = (bool)dr["IsHeaderOpenChannel"],
                    IsFooterOpenChannel = (bool)dr["IsFooterOpenChannel"],
                    IsEDMOpenChannel = (bool)dr["IsEDMOpenChannel"],
                    Creator = (long)dr["Creator"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    Modifier = (long)dr["Modifier"],
                    ModifyTime = (DateTime)dr["ModifyTime"]
                };
            }
            return item;
        }
        /// <summary>
        /// 取得 model 內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SocialRelationModels GetRelationItem(long id)
        {
            SocialRelationModels item = null;
            string sql = $"Select * from [SocialRelations] Where ID={id}";

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null && datas.Rows.Count > 0)
            {
                DataRow dr = datas.Rows[0];
                item = new SocialRelationModels()
                {
                    ID = dr["ID"].ToString(),
                    SiteID = (long)dr["SiteID"],
                    RelationType = (RelationType)((int)dr["RelationType"]),
                    SocialType = (WorkV3.Models.MemberType)((int)dr["SocialType"]),
                    SocialTitle = dr["SocialTitle"].ToString(),
                    IsOpen = (bool)dr["IsOpen"],
                    ShowType = (ShowType)((int)dr["ShowType"]),
                    Sort = (int)dr["Sort"],
                    LinkTitle = dr["LinkTitle"].ToString(),
                    LinkUrl = dr["LinkUrl"].ToString(),
                    Creator = (long)dr["Creator"],
                    CreateTime = (DateTime)dr["CreateTime"],
                    Modifier = (long)dr["Modifier"],
                    ModifyTime = (DateTime)dr["ModifyTime"]
                };
            }
            return item;
        }
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SocialRelationModels> GetRelationItems(long siteID, RelationType relationType)
        {
            List<SocialRelationModels> items = new List<SocialRelationModels>();

            string sql = $"Select * from [SocialRelations] WHERE SiteID={siteID} AND RelationType={(int)relationType} Order By Sort Asc";

            List<string> where = new List<string>();

            SQLData.Database db = new SQLData.Database(WebInfo.Conn);

            DataTable datas = db.GetDataTable(sql);
            if (datas != null)
            {
                foreach (DataRow dr in datas.Rows)
                {
                    items.Add(new SocialRelationModels
                    {
                        ID = dr["ID"].ToString(),
                        SiteID = (long)dr["SiteID"],
                        RelationType = (RelationType)((int)dr["RelationType"]),
                        SocialType = (WorkV3.Models.MemberType)((int)dr["SocialType"]),
                        SocialTitle = dr["SocialTitle"].ToString(),
                        SocialStyle = GetSocialTypeStyle((WorkV3.Models.MemberType)((int)dr["SocialType"])),
                        IsOpen = (bool)dr["IsOpen"],
                        ShowType = (ShowType)((int)dr["ShowType"]),
                        ShowTypeName = (ShowType)((int)dr["ShowType"]) == ShowType.More? "點more顯示" : "直接顯示",
                        Sort = (int)dr["Sort"],
                        LinkTitle = dr["LinkTitle"].ToString(),
                        LinkUrl = dr["LinkUrl"].ToString(),
                        Creator = (long)dr["Creator"],
                        CreateTime = (DateTime)dr["CreateTime"],
                        Modifier = (long)dr["Modifier"],
                        ModifyTime = (DateTime)dr["ModifyTime"]
                    });
                }
            }


            return items;
        }

        public static bool SetItem(SocialSettingModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("SocialSetting");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM SocialSetting WHERE  SiteID='{ item.SiteID }' ";

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
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.SiteID);
            }
            return true;
        }
        public static bool SetRelationItem(SocialRelationModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            SQLData.TableObject tableObj = db.GetTableObject("SocialRelations");
            tableObj.GetDataFromObject(item);
            DateTime now = DateTime.Now;
            string sql = $"SELECT * FROM SocialRelations WHERE  ID={ item.ID } ";
            string sql_sort = $"SELECT Max(Sort)  FROM SocialRelations WHERE  RelationType='{(int)item.RelationType}' AND SiteID={item.SiteID} ";
            bool isNew = db.GetFirstValue(sql) == null;
            string sortIndex = db.GetFirstValue(sql_sort).ToString();
            int newSortIndex = 1;
            if (!string.IsNullOrEmpty(sortIndex))
                newSortIndex = int.Parse(sortIndex) + 1;
            if (isNew)
            {
                tableObj["ID"] = item.ID;
                tableObj["SocialTitle"] = GetSocialTypeTitle(item.SocialType);
                tableObj["Sort"] = newSortIndex;
                tableObj["LinkTitle"] = item.LinkTitle == null ? "" : item.LinkTitle;
                tableObj["LinkUrl"] = item.LinkUrl == null?"":item.LinkUrl;
                tableObj["Creator"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["CreateTime"] = now;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Insert();
            }
            else
            {
                tableObj.Remove("ID");
                tableObj.Remove("Creator");
                tableObj.Remove("CreateTime");
                tableObj.Remove("Sort");
                tableObj["SocialTitle"] = item.SocialTitle == null ? "" : item.SocialTitle;
                tableObj["LinkTitle"] = item.LinkTitle == null ? "" : item.LinkTitle;
                tableObj["LinkUrl"] = item.LinkUrl == null ? "" : item.LinkUrl;
                tableObj["Modifier"] = WorkV3.Areas.Backend.Models.DataAccess.MemberDAO.SysCurrent.Id;
                tableObj["ModifyTime"] = now;
                tableObj.Update(item.ID);
            }
            return true;
        }

        public static int DeleteRelationDetail(long id)
        {

            string sql =
                "Delete [SocialRelations] Where ID In ({0})";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, id.ToString()));
            }

            return num;
        }

        public static void SortRelation(IEnumerable<SortItem> items)
        {
            if (items == null || items.Count() == 0)
                return;

            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();

            string sql = $"Select ID From SocialRelations Order By Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                IEnumerable<long> ids = conn.Query<long>(sql);

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = "Update SocialRelations Set Sort = {0} Where ID = {1}\n";
                foreach (long id in ids)
                {
                    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
                    foreach (SortItem item in updateItems)
                    {
                        sortSql.AppendFormat(fmt, index++, item.ID);
                        itemList.Remove(item);
                    }

                    if (!sortIds.Contains(id))
                    {
                        sortSql.AppendFormat(fmt, index++, id);
                    }
                }

                if (itemList.Count > 0)
                {
                    foreach (SortItem item in itemList.OrderBy(item => item.Index))
                        sortSql.AppendFormat(fmt, index++, item.ID);
                }

                conn.Execute(sortSql.ToString());
            }
        }
        public static string GetSocialTypeTitle(WorkV3.Models.MemberType socialType)
        {
            switch (socialType)
            {
                case WorkV3.Models.MemberType.FB:
                    return "Facebook";
                case WorkV3.Models.MemberType.Twitter:
                    return "Twitter";
                case WorkV3.Models.MemberType.Google:
                    return "Google +";
                case WorkV3.Models.MemberType.Line:
                    return "Line";
                case WorkV3.Models.MemberType.Pinterest:
                    return "Pinterest";
                case WorkV3.Models.MemberType.Bloger:
                    return "Bloger";
                case WorkV3.Models.MemberType.Plurk:
                    return "Plurk";
                case WorkV3.Models.MemberType.LinkedIn:
                    return "LinkedIn";
                case WorkV3.Models.MemberType.Weibo:
                    return "微博";
                case WorkV3.Models.MemberType.QQ:
                    return "QQ";
                case WorkV3.Models.MemberType.Baidu:
                    return "百度";
                case WorkV3.Models.MemberType.Renren:
                    return "人人網";
                case WorkV3.Models.MemberType.YouTube:
                    return "YouTube";
                case WorkV3.Models.MemberType.Vimeo:
                    return "Vimeo";
                case WorkV3.Models.MemberType.Instagram:
                    return "Instagram";
                case WorkV3.Models.MemberType.Tudou:
                    return "土豆網";
                case WorkV3.Models.MemberType.YK:
                    return "優酷"; //有缺的
                case WorkV3.Models.MemberType.Accupass:
                    return "Accupass 活動通"; //有缺的
                case WorkV3.Models.MemberType.Pinkoi:
                    return "Pinkoi"; //有缺的
                case WorkV3.Models.MemberType.Web:
                    return "網站"; //有缺的
                case WorkV3.Models.MemberType.Blog:
                    return "部落格"; //有缺的
                case WorkV3.Models.MemberType.Store:
                    return "賣場 / 網路商店"; //有缺的
                case WorkV3.Models.MemberType.Org:
                    return "機關 / 單位"; //有缺的
                case WorkV3.Models.MemberType.Channel:
                    return "頻道"; //有缺的
                case WorkV3.Models.MemberType.Album:
                    return "相簿"; //有缺的
                case WorkV3.Models.MemberType.Links:
                    return "連結"; //有缺的
                case WorkV3.Models.MemberType.Other:
                    return "其他"; //有缺的
            }
            return "";
        }
        public static string GetSocialTypeStyle(WorkV3.Models.MemberType socialType)
        {
            switch (socialType)
            {
                case WorkV3.Models.MemberType.FB:
                    return "cc-facebook";
                case WorkV3.Models.MemberType.Twitter:
                    return "cc-twitter";
                case WorkV3.Models.MemberType.Google:
                    return "cc-google-plus";
                case WorkV3.Models.MemberType.Line:
                    return "cc-line-text";
                case WorkV3.Models.MemberType.Pinterest:
                    return "cc-pinterest";
                case WorkV3.Models.MemberType.Bloger:
                    return "cc-bloger"; //有缺的
                case WorkV3.Models.MemberType.Plurk:
                    return "cc-plurk";
                case WorkV3.Models.MemberType.LinkedIn:
                    return "cc-linkedin";
                case WorkV3.Models.MemberType.Weibo:
                    return "cc-sina-weibo";
                case WorkV3.Models.MemberType.QQ:
                    return "cc-qqchat";
                case WorkV3.Models.MemberType.Baidu:
                    return "cc-baidu";
                case WorkV3.Models.MemberType.Renren:
                    return "cc-renren";
                case WorkV3.Models.MemberType.YouTube:
                    return "cc-youtube";
                case WorkV3.Models.MemberType.Vimeo:
                    return "cc-vimeo";
                case WorkV3.Models.MemberType.Instagram:
                    return "cc-instagram";
                case WorkV3.Models.MemberType.Tudou:
                    return "cc-tudou";
                case WorkV3.Models.MemberType.YK:
                    return "cc-youku";
                case WorkV3.Models.MemberType.Pinkoi:
                    return "cc-pinkoi";
                case WorkV3.Models.MemberType.Accupass:
                    return "cc-accupass";
                case WorkV3.Models.MemberType.Web:
                    return "cc-laptop";
                case WorkV3.Models.MemberType.Blog:
                    return "cc-reading";
                case WorkV3.Models.MemberType.Store:
                    return "cc-shop";
                case WorkV3.Models.MemberType.Org:
                    return "cc-flag";
                case WorkV3.Models.MemberType.Channel:
                    return "cc-channel";
                case WorkV3.Models.MemberType.Album:
                    return "cc-gallery";
                case WorkV3.Models.MemberType.Links:
                    return "cc-link-variant";
                case WorkV3.Models.MemberType.Other:
                    return "cc-lnr-bookmark";
            }
            return "";
        }
        public static string GetSocialTypeButtonStyle(WorkV3.Models.MemberType socialType)
        {
            switch (socialType)
            {
                case WorkV3.Models.MemberType.FB:
                    return "btn-social-facebook-o";
                case WorkV3.Models.MemberType.Twitter:
                    return "btn-social-twitter-o";
                case WorkV3.Models.MemberType.Google:
                    return "btn-social-google-plus-o";
                case WorkV3.Models.MemberType.Line:
                    return "btn-social-line-o";
                case WorkV3.Models.MemberType.Pinterest:
                    return "btn-social-pinterest-o";
                case WorkV3.Models.MemberType.Bloger:
                    return "btn-social-bloger-o"; //有缺的
                case WorkV3.Models.MemberType.Plurk:
                    return "btn-social-plurk-o";
                case WorkV3.Models.MemberType.LinkedIn:
                    return "btn-social-linkedin-o"; //有缺的
                case WorkV3.Models.MemberType.Weibo:
                    return "btn-social-sina-weibo-o";
                case WorkV3.Models.MemberType.QQ:
                    return "btn-social-qqchat-o";
                case WorkV3.Models.MemberType.Baidu:
                    return "btn-social-baidu-o"; //有缺的
                case WorkV3.Models.MemberType.Renren:
                    return "btn-social-renren-o";
            }
            return "";
        }
        public static string GetSocialTypeEDMImage(WorkV3.Models.MemberType socialType)
        {
            switch (socialType)
            {
                case WorkV3.Models.MemberType.FB:
                    return "icon-facebook.png";
                case WorkV3.Models.MemberType.Twitter:
                    return "icon-twitter.png";
                case WorkV3.Models.MemberType.Google:
                    return "icon-g+.png";
                case WorkV3.Models.MemberType.YouTube:
                    return "icon-youtube.png";
                case WorkV3.Models.MemberType.Vimeo:
                    return "icon-vimeo.png";
                case WorkV3.Models.MemberType.Instagram:
                    return "icon-instagram.png";
                case WorkV3.Models.MemberType.Pinterest:
                    return "icon-pinterest.png";
                case WorkV3.Models.MemberType.Bloger:
                    return "icon-blogger.png";
                case WorkV3.Models.MemberType.Plurk:
                    return "icon-plurk.png";
                case WorkV3.Models.MemberType.LinkedIn:
                    return "icon-linkedin.png";
                case WorkV3.Models.MemberType.Weibo:
                    return "icon-weibo.png";
                case WorkV3.Models.MemberType.QQ:
                    return "icon-qq.png"; 
                case WorkV3.Models.MemberType.Baidu:
                    return "icon-baidu.png";
                case WorkV3.Models.MemberType.Renren:
                    return "icon-renren.png";
                case WorkV3.Models.MemberType.Tudou:
                    return "icon-todou.png";
                case WorkV3.Models.MemberType.YK:
                    return "icon-youku.png";
                case WorkV3.Models.MemberType.Pinkoi:
                    return "icon-pinkoi.png";
                case WorkV3.Models.MemberType.Accupass:
                    return "icon-accupass.png";
                case WorkV3.Models.MemberType.Web:
                    return "icon-laptop.png";
                case WorkV3.Models.MemberType.Blog:
                    return "icon-reading.png";
                case WorkV3.Models.MemberType.Store:
                    return "icon-shop.png";
                case WorkV3.Models.MemberType.Org:
                    return "icon-flag.png";
                case WorkV3.Models.MemberType.Channel:
                    return "icon-channel.png";
                case WorkV3.Models.MemberType.Album:
                    return "icon-gallery.png";
                case WorkV3.Models.MemberType.Links:
                    return "icon-link-variant.png";
                case WorkV3.Models.MemberType.Other:
                    return "icon-lnr-bookmark.png";
            }
            return "";
        }

        public static void SetRelationStatus(long ID, bool IsOpen)
        {
            SocialRelationModels item = new SocialRelationModels();
            SQLData.ParameterCollection paraList = new SQLData.ParameterCollection();
            paraList.Add("@ID", ID);
            paraList.Add("@IsOpen", IsOpen);
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string updateSQL = " UPDATE SocialRelations SET IsOpen=@IsOpen WHERE ID=@ID ";
            db.ExecuteNonQuery(updateSQL, paraList);
        }

        public static int DeleteRelations(IEnumerable<long> ids)
        {
            if (ids == null || ids.Count() == 0)
                return 0;

            string sql =@"Delete SocialRelations Where ID In ({0});";

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(string.Format(sql, string.Join(", ", ids)));
            }

            return num;
        }

        public static void SortRelations(RelationType relationType, IEnumerable<SortItem> items)
        {
            if (items == null || items.Count() == 0)
                return;

            IEnumerable<long> sortIds = items.Select(item => item.ID);
            List<SortItem> itemList = items.ToList();

            string sql = $"Select ID From SocialRelations WHERE RelationType={(int)relationType} Order By Sort";
            using (SqlConnection conn = new SqlConnection(WebInfo.Conn))
            {
                IEnumerable<long> ids = conn.Query<long>(sql);

                int index = 1;
                System.Text.StringBuilder sortSql = new System.Text.StringBuilder();
                string fmt = "Update SocialRelations Set Sort = {0} Where ID = {1}\n";
                foreach (long id in ids)
                {
                    IEnumerable<SortItem> updateItems = itemList.Where(item => item.Index <= index).OrderBy(item => item.Index);
                    foreach (SortItem item in updateItems)
                    {
                        sortSql.AppendFormat(fmt, index++, item.ID);
                        itemList.Remove(item);
                    }

                    if (!sortIds.Contains(id))
                    {
                        sortSql.AppendFormat(fmt, index++, id);
                    }
                }

                if (itemList.Count > 0)
                {
                    foreach (SortItem item in itemList.OrderBy(item => item.Index))
                        sortSql.AppendFormat(fmt, index++, item.ID);
                }

                conn.Execute(sortSql.ToString());
            }
        }
    }
}