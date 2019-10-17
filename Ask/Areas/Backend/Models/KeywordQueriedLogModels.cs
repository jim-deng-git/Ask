using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public partial class KeywordQueriedLogModels
    {
        public long ID { get; set; }
        public long KeywordQueriedID { get; set; }
        public DateTime QueryTime { get; set; }
    }
}
