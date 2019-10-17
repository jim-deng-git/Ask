using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class StorePlanServiceModel
    {
        public long ID { get; set; }
        public long ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool UploadFile { get; set; }
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Sort { get; set; }
    }
}