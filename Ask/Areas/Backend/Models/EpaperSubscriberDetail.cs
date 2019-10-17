using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSubscriberDetail
    {
        public long ID { get; set; }
        public long EpaperSubscriber_ID { get; set; }
        public long EpaperType_ID { get; set; }
        public long? Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }

        public long EpaperTypeID { get; set; } //EpaperType.ID
        public string EpaperTypeName { get; set; } //EpaperType.Name
    }
}