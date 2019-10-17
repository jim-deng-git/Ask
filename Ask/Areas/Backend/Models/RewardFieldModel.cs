using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class RewardFieldModel
    {
        public long ID { get; }
        public string Title { get; set; }
        public string TypeID { get; set; }
        public string Option { get; set; }
        public bool ShowInComboBox { get; set; }
        public bool DefaultSelect { get; set; }
        public bool DefaultNeeded { get; set; }
    }
}