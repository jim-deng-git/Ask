using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipRegEmailManagersModels
    {

        public string ID { get; set; }
        public long SiteID { get; set; }
        public bool IsManager { get; set; }
        public string ManagerID { get; set; }
        public string ManagerName { get; set; }
        public string Email { get; set; }
        public int Sort { get; set; }
        public bool IsSelected { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}