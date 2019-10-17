using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EstimateToItemsModel
    {
        public long ID { get; set; }
        public long EstimateID { get; set; }
        public long TypeID { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int Total { get; set; }
        public int Sort { get; set; }

        public string TypeName { get; set; }
        public string TypeColor { get; set; }
    }
}