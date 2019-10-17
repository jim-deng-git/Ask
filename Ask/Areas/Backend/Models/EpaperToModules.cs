using System;
using System.Collections;
using System.Collections.Generic;
using WorkV3.Common;
namespace WorkV3.Areas.Backend.Models
{
    public class EpaperToModules
    {
        public long ID { get; set; }
        public long EpaperID { get; set; }
        public string ModuleName { get; set; }
        public string ModuleID { get; set; }
        public string Title { get; set; }
        public bool IsOpen { get; set; }
        public int Sort { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public IEnumerable<ViewModels.EpaperToPageViewModel> PageList { get; set; }
    }
}