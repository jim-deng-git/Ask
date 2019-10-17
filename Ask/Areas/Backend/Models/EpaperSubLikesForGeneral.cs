using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 電子報一般訂閱者喜好
    /// </summary>
    public class EpaperSubLikesForGeneral
    {
        public long EpaperSubscriber_ID { get; set; }
        public long Category_ID { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}