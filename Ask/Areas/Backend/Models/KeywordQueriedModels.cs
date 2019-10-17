using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public partial class KeywordQueriedModels
    {
        public long ID { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }
}
