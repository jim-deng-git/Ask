using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class GroupModels
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public int GroupType { get; set; }
        public int MemberCount { get; set; }
        public int Sort { get; set; }

        public IEnumerable<GroupPermissionModels> GetPermissions(long siteId)
        {
            return DataAccess.GroupPermissionDAO.GetGroupPermissions(Id, siteId);
        }

        public int SetPermissionsForAllSites(int permissionType)
        {
            return GroupDAO.SetPermissionTypeForAllSites(Id, permissionType);
        }

        public int SetPermissionForSingleSite(long siteId, int permissionType)
        {
            return GroupDAO.SetPermissionTypeForSingleSite(Id, siteId, permissionType);
        }
    }
}