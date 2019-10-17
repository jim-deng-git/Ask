using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models.Base
{
    public abstract class Discountable
    {
        public int Count { get; set; }
        public float Price { get; set; }
    }
}