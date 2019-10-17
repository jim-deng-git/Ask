using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class StorePlanContractModel
    {
        public long ID { get; set; }
        public long ParentID { get; set; }
        public long SiteID { get; set; }
        public int Lang { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}