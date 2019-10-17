using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public partial class KeywordModels
    {
        public long ID { get; set; }
        public long KeywordQueriedID { get; set; }
        public bool IsIssue { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }
        public int Sort { get; set; }
    }
}
