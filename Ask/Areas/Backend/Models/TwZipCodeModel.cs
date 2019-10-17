using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 臺灣縣市行政區
    /// </summary>
    public class TwZipCodeModel
    {
        public int ParentId { get; set; }
        public string Title { get; set; }
        public int ZipCode { get; set; }
    }
}