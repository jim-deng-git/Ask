using System;
using WorkV3.Common;
namespace WorkV3.Areas.Backend.Models
{
    public class EpaperToPages
    {
        public long ID { get; set; }
        public long EpaperModuleID { get; set; }
        public long PageID { get; set; }
        public int Sort { get; set; }
    }
}