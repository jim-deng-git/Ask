using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace WorkV3.Areas.Backend.Models.DataAccess
{
    public class GroupPermissionDAO
    {
        public static IEnumerable<GroupPermissionModels> GetGroupPermissions(long groupId, long siteId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" SELECT DISTINCT c.* 
                                FROM [Group] a
                                JOIN [GroupSitePermissionType] b on (a.ID = b.GroupID and b.SiteID = @SiteID)
                                JOIN [GroupPermission] c ON (b.GroupID = c.GroupID and b.PermissionType = c.PermissionType)
                                WHERE a.ID = @GroupID";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("SiteID", siteId);
                param.Add("GroupID", groupId);

                return conn.Query<GroupPermissionModels>(sql, param);
            }
        }

        public static int SetItem(GroupPermissionModels permission)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                string sql = @" INSERT INTO GroupPermission([MenuType], [MenuID], [GroupID], [SiteID], [PermissionType]) 
                                VALUES (@MenuType, @MenuID, @GroupID, @SiteID, @PermissionType)
";
                return conn.Execute(sql, permission);
            }
        }

        public static bool HaverPermission(long groupId, long siteId, int menuType, long menuId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                GroupModels group = GroupDAO.GetItem(groupId, siteId);

                string sql = @" SELECT 1 FROM GroupPermission 
                                WHERE [GroupID] = @GroupID AND [SiteID] = @SiteID AND [MenuType] = @MenuType AND [MenuID] = @MenuID ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("@GroupID", groupId);
                param.Add("@SiteID", siteId);
                param.Add("@MenuType", menuType);
                param.Add("@MenuID", menuId);

                IEnumerable<GroupPermissionModels> permission = conn.Query<GroupPermissionModels>(sql, param);

                if(group.GroupType == 1) // 黑名單模式，找到的話 return false
                {
                    return permission.Count() == 0 ? true : false;
                }
                else // 白名單模式，找到的話 return true
                {
                    return permission.Count() == 0 ? false : true;
                }
            }
        }

        public static int SetItems(List<GroupPermissionModels> permissions, long groupId, long siteId)
        {
            using (var conn = new SqlConnection(WebInfo.Conn))
            {
                GroupModels group = GroupDAO.GetItem(groupId, siteId);
                string sql = @" DELETE FROM GroupPermission WHERE GroupID = @GroupID AND PermissionType = @PermissionType ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("GroupID", groupId);
                param.Add("PermissionType", group.GroupType);

                conn.Execute(sql, param);
            }

            if (permissions == null)
                return 0;

            int retValue = 0;
            foreach(GroupPermissionModels permission in permissions)
            {
                retValue += SetItem(permission);
            }

            return retValue;
        }

        public static int DeleteItem(long id)
        {
            return CommonDA.Delete("GroupPermission", new List<long>{id});
        }
    }
}