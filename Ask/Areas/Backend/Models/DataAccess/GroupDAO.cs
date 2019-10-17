using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WorkV3.Common;
using WorkV3.Areas.Backend.Models;
using WorkLib;
using Dapper;
using WorkV3.Areas.Backend.Models.DataAccess;
using System.Data.SqlClient;

namespace WorkV3.Areas.Backend.Models
{
    public class GroupDAO
    {
        /// <summary>
        /// 抓取 Group 物件包含 PermissionType
        /// </summary>
        /// <param name="id"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static GroupModels GetItem(long id, long siteId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT a.[ID], a.[Name], a.[Icon], a.[Color], a.[Desc], a.[Status], b.[PermissionType] as GroupType
                            FROM [Group] a
                            LEFT JOIN GroupSitePermissionType b ON (a.ID = b.GroupID AND b.SiteID = @SiteID)
                            WHERE a.ID = @ID ";

                GroupModels retValue = conn.Query<GroupModels>(sql, new { SiteID = siteId, ID = id }).FirstOrDefault();

                retValue.GroupType = retValue.GroupType == 0 ? 1 : retValue.GroupType;
                return retValue;
            }
        }

        public static GroupModels GetItem(long id)
        {
            return CommonDA.GetItem<GroupModels>("[Group]", id);
        }

        public static IEnumerable<GroupModels> GetItems()
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                List<GroupModels> items = new List<GroupModels>();
                string sql = @" Select a.*, b.MemberCount 
                                From [Group] a
                                LEFT JOIN ( SELECT GroupId, COUNT(*) as MemberCount
                                       FROM Member 
                                       GROUP BY GroupId ) b ON (a.ID = b.GroupId)
                                Order By a.IsSupremeAuthority DESC, a.Sort ";
                
                return conn.Query<GroupModels>(sql).ToList();
            }
        }

        public static int SetPermissionTypeForAllSites(long groupId, int permissionType = 2)
        {
            int retValue = 0;
            List<WorkV3.Models.SitesModels> sites = WorkV3.Models.DataAccess.SitesDAO.GetDatas();
            foreach(WorkV3.Models.SitesModels site in sites)
            {
                retValue += SetPermissionTypeForSingleSite(groupId, site.Id, permissionType);
            }

            return retValue;
        }

        public static int SetPermissionTypeForSingleSite(long groupId, long siteId, int permissionType = 2)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = $@" SELECT 1 
                            FROM GroupSitePermissionType 
                            WHERE SiteID = @SiteID AND GroupID = @GroupID ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@SiteID", siteId);
                param.Add("@GroupID", groupId);

                bool isNew = conn.Query<int>(sql, param).Count() == 0;
                if(isNew)
                {
                    sql = @" INSERT INTO GroupSitePermissionType(SiteID, GroupID, PermissionType) VALUES(@SiteID, @GroupID, @PermissionType) ";
                    GroupSitePermissionTypeModels permission = new GroupSitePermissionTypeModels();
                    permission.GroupID = groupId;
                    permission.SiteID = siteId;
                    permission.PermissionType = permissionType;

                    return conn.Execute(sql, permission);
                }
                else
                {
                    sql = @" UPDATE GroupSitePermissionType SET PermissionType = @PermissionType WHERE SiteID = @SiteID AND GroupID = @GroupID ";

                    return conn.Execute(sql,  new { PermissionType = permissionType, SiteID = siteId, GroupID = groupId });
                }
            }
        }

        public static void SetItem(GroupModels item)
        {
            SQLData.Database db = new SQLData.Database(WebInfo.Conn);
            string sql = "Select 1 From [Group] Where ID = " + item.Id;
            bool isNew = db.GetFirstValue(sql) == null;
            if (isNew)
            {
                item.SetPermissionsForAllSites(2);
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    sql = $"INSERT INTO [Group]([ID], [Name], [Desc], [Status], [Color], [Icon]) VALUES({ WorkLib.GetItem.NewSN()},@Name, @Desc, @Status, @Color, @Icon) ";
                    conn.Execute(sql, item);
                }
            }
            else
            {
                item.SetPermissionForSingleSite(PageCache.SiteID, item.GroupType);
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
                {
                    sql = @"UPDATE [Group] SET [Name]=@Name,[Desc]=@Desc,[Status]=@Status, [Color]=@Color, [Icon]=@Icon WHERE ID=@ID ";
                    conn.Execute(sql, item);
                }
            }
        }

        public static void PermissionGroupSetting(long id,long SiteID ,string[] arrPermissionGroup)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = $"DELETE FROM PermissionGroup WHERE GroupId=@GroupId and SiteID=@SiteID";
                conn.Execute(sql, new { GroupId = id , SiteID = SiteID});

                for (int i = 0; i < arrPermissionGroup.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arrPermissionGroup[i]))
                    {
                        sql = $"INSERT INTO PermissionGroup(GroupId ,SiteID ,PermissionName) VALUES(@GroupId, @SiteID ,@PermissionName)";
                        conn.Execute(sql, new { GroupId = id, SiteID= SiteID, PermissionName = arrPermissionGroup[i] });
                    }
                }
            }
        }

        public static List<MemberModels> GetMemberByGroup(long id)
        {
            List<MemberModels> ret = new List<MemberModels>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "SELECT * FROM Member WHERE GroupID=@GroupID ";
                ret = conn.Query<MemberModels>(sql, new { GroupID = id }).ToList();
            }

            return ret;
        }

        public static int DeletePermission(IEnumerable<long> ids)
        {
            if (ids?.Count() == 0)
                return 0;

            string permissionIds = string.Join(", ", ids);
            string getNormalGroupSql = $@" (SELECT ID FROM [Group] WHERE ID IN ({permissionIds}) AND IsSupremeAuthority <> 1) ";

            string sql =
                $@" Delete [Group] Where ID In ({getNormalGroupSql}) ; 
                    UPDATE Member SET GroupId=0 
                    WHERE GroupID IN {getNormalGroupSql} ;
                    DELETE GroupPermission WHERE GroupId IN {getNormalGroupSql};"; // 2018-06-12 neil 修改刪除規則: 保留最高管理者

            int num = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                num = conn.Execute(sql);
            }

            return num;
        }


        public static List<PermissionGroupModels> GetPermissionGroup(long id)
        {
            List<PermissionGroupModels> ret = new List<PermissionGroupModels>();

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(WebInfo.Conn))
            {
                string sql = "SELECT * FROM PermissionGroup WHERE GroupID=@GroupID ";
                ret = conn.Query<PermissionGroupModels>(sql, new { GroupID = id }).ToList();
            }

            return ret;
        }
    }
}