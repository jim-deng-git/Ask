using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class IntraIPlimitModel
    {
        public int ID { get; set; }
        public OpenStatus OpenStatus { get; set; } = OpenStatus.Open;
        public string IP_Begin { get; set; }
        public string IP_End { get; set; }
        public long? IP_BeginNum { get; set; }
        public long? IP_EndNum { get; set; }

        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public bool IsSystemSet { get; set; } = false;
    }
    public enum OpenStatus
    {
        /// <summary>
        /// 0: 只限制所設定的 ip
        /// </summary>
        Close = 0,
        /// <summary>
        /// 1 : 只開放所設定的 ip
        /// </summary>
        Open = 1
    }
}