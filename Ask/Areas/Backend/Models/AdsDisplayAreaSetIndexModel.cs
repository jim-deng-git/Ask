using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 顯示位置設定 Index
    /// </summary>
    public class AdsDisplayAreaSetIndexModel
    {
        public IEnumerable<AdsDisplayAreaTrees> tree { get; set; }
    }
}