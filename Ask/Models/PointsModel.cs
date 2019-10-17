using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class PointsModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long MemberShipID { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public decimal Point { get; set; }
        public bool IsManually { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int PointType { get; set; } = 0;
    }
}