using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ZonesModels
    {
        public long No { get; set; }
        public int Ver { get; set; } = 1;
        public long SiteID { get; set; }
        public long PageNo { get; set; }
        public string SN { get; set; }
        public byte? Sort { get; set; } = 1;
        public string Background { get; set; }
        public short TotalWidth { get; set; } = 100;
        public short MainSpacing { get; set; } = 100;
        public short SubSpacing { get; set; } = 100;
        public short Boundary { get; set; } = 100;
        public byte ShowComputer { get; set; } = 1;
        public byte ShowMobile { get; set; } = 1;
        public byte ShowStatus { get; set; } = 1;
        public int StyleID { get; set; } = 1;
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

    }

}