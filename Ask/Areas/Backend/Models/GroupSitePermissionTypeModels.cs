using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class GroupSitePermissionTypeModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long GroupID { get; set; }
        public int PermissionType { get; set; }
    }
}