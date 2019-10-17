using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class MemberShipRegOptionModels
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public int Sort { get; set; }
        public bool Selected { get; set; }
    }
}