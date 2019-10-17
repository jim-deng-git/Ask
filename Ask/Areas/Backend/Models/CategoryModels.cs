using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class CategoryModels
    {

        public long ID { get; set; }
        public string Type { get; set; }
        public string Title{ get; set; }
        public string RemarkText { get; set; }
        public bool ShowStatus { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public string Image { get; set; }
        public int MemberSession { get; set; }

        /// <summary>
        /// 預設會員 1:一般會員
        /// </summary>
        public int PresetIdentity { get; set; } = 0;

    }
}