using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class AdvertisementSearchModel
    {
        public string keyword { get; set; }
        public string[] AdvertisementType { get; set; }
    }
}