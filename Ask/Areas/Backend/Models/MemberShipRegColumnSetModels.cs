using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipRegColumnSetModels
    {
        public long SiteID { get; set; }
        public string TableName { get; set; }
        public string ColumnTitle { get; set; }
        public string ColumnName { get; set; }
        public bool IsOpen { get; set; }
        public bool IsNeedValue { get; set; }
        public string OtherOption { get; set; }
        public List<MemberShipRegOptionModels> OtherOptionList { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}