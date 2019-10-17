using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ShippingToSiteModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string ShippingCode { get; set; }
        public float Fee { get; set; }
        public bool IsSupport { get; set; }

        [NotMapped]
        public string Title { get; set; }
        [NotMapped]
        public int ShippingRegion { get; set; }

    }
}