using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class PermissionGroupModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long GroupId { get; set; }

        public string PermissionName { get; set; }
    }
}