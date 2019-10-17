using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3
{
    public class GoogleMapViewModel
    {
        public string Address { get; set; }

        public decimal? Lat { get; set; }

        public decimal? Lng { get; set; }
    }
}