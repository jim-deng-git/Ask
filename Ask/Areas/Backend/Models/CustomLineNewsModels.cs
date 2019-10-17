using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class CustomLineNewsModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public DateTime SelectDate { get; set; }
        public long SelectMenuID { get; set; }
        public long SelectID { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? Total { get; set; }

    }
    public class CustomLineNewsDataModels
    {
        public long ID { get; set; }
        public long SourceID { get; set; }
        public long SiteID { get; set; }
        public DateTime SelectDate { get; set; }
        public long SelectMenuID { get; set; }
        public long SelectID { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? Total { get; set; }
        public string Title { get; set; }
        public string MenuTitle { get; set; }

    }
    public class CustomLineNewsSearchModels
    {
        public long SiteID { get; set; }
        public long SelectMenuID { get; set; }
        public string SelectDate { get; set; }
        public long SourceID { get; set; }
        public string Key { get; set; }
    
    }
}