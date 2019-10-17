using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkV3.Models
{
    public class PaymentToSiteModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string PaymentCode { get; set; }
        public float Fee { get; set; }
        public bool IsSupport { get; set; }

        [NotMapped]
        public string Title { get; set; }
    }
}