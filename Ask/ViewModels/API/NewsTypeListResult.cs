using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class NewsTypeListResult
    {
        /// <summary>
        /// 類別ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 類別名稱
        /// </summary>
        public string Name { get; set; }
    }
}