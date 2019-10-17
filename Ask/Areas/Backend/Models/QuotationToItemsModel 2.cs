using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class QuotationToItemsModel
    {
        public long ID { get; set; }
        public long QuotationID { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int UnitPrice { get; set; }
        public int? People { get; set; }
        public int? Hours { get; set; }
        public int? Session { get; set; }
        public int? Discount { get; set; }
        public int Total { get; set; }
        public int Sort { get; set; }
    }

    public enum QuotationItemType
    {
        Human = 1,
        Service
    }
}