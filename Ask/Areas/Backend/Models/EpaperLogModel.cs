using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperLogModel
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public long EpaperSendID { get; set; }
        public string Message { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public enum EpaperLogType
    {
        Normal,
        BlackList,
        Start, 
        End,
        Error,
        Exception
    }
}