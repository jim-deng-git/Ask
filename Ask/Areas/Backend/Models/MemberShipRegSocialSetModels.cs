using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipRegSocialSetModels
    {
        public long SiteID { get; set; }
        public WorkV3.Models.MemberType SocialType { get; set; }
        public string SocialTitle { get; set; }
        public bool IsOpen { get; set; }
        public int Sort { get; set; }
        public string SecretKey { get; set; }
        public string AppID { get; set; }
        public string Scope { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool BackendIsOpen { get; set; }
    }
}