using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ReceiptToItemsModel
    {
        public long ID { get; set; }
        public long ReceiptID { get; set; }
        public long TypeID { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Total { get; set; }
        public int Sort { get; set; }
    }
}