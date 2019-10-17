using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class SocialRelationModels
    {
        public string ID { get; set; }
        public long SiteID { get; set; }
        public bool IsOpen { get; set; }
        public RelationType RelationType { get; set; }
        public WorkV3.Models.MemberType SocialType { get; set; } 
        public string SocialTitle { get; set; }
        public string SocialStyle { get; set; }
        public ShowType ShowType { get; set; }
        public string ShowTypeName { get; set; }
        public long Sort { get; set; }
        public string LinkTitle { get; set; }
        public string LinkUrl { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
    public enum RelationType
    {
        //0 社群分享
        Share =0,
        //1 官方頻道
        Channel = 1,

    }
    public enum ShowType
    {
        //0 直接顯示
        Show = 0,
        //1 點 more 顯示
        More = 1,

    }
}