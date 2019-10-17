using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;
using WorkLib;

namespace WorkV3.Areas.Backend.Models
{
    public class ResourceVoicesModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long SourceNo { get; set; }
        public byte SourceType { get; set; }
        public int Ver { get; set; }
        public byte AreaID { get; set; }        
        public string Code { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }
        public string ShowName { get; set; }
        public int? Sort { get; set; }        
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}