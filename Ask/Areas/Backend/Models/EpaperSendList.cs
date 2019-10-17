using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSendList
    {
        public long ID { get; set; }
        public long EpaperSend_ID { get; set; }
        public long EpaperSubscriber_ID { get; set; }
        public bool IsSended { get; set; }
        public DateTime? SendTime { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }
        public long ReadIdentifier { get; set; }
        public string TemporarilyEmail { get; set; }

        public EpaperSubscriber EpaperSubscriber { get; set; }
    }
}