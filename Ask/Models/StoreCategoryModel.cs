using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class StoreCategoryModel
    {
        private Interface.IStoreCategoryRepository<StoreCategoryModel> categoryRepository = new Repository.StoreCategoryRepository();
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public string CoverImg { get; set; }
        public string MainVision { get; set; }
        public string Intro { get; set; }
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; } = null;
        public DateTime? ModifyTime { get; set; } = null;
    }
}