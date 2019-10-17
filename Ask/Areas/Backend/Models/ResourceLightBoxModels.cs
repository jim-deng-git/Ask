using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;
using WorkLib;

namespace WorkV3.Areas.Backend.Models
{
    public class ResourceLightBoxModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long SourceNo { get; set; }
        public byte SourceType { get; set; }
        public int Ver { get; set; }
        public byte AreaID { get; set; }
        public string Code { get; set; }
        public string BtnName { get; set; }
        public string BtnColor { get; set; }
        public string Title { get; set; }
        public string Spec { get; set; }        
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}