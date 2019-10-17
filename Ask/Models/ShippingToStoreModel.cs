using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkV3.Models
{
    public class ShippingToStoreModel
    {
        public long ID { get; set; }
        public long StoreID { get; set; }
        public string ShippingCode { get; set; }
        public float Fee { get; set; }
        public bool IsSupport { get; set; }

        [NotMapped]
        public string Title { get; set; }
    }
}