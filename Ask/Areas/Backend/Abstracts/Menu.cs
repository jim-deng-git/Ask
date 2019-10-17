using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Abstracts
{
    public abstract class Menu
    {
        public long ID { get; set; }
        public long? ParentID { get; set; }
        public long SiteID { get; set; }

        /// <summary>
        /// GroupPermission 的選單類型：1: 前台選單, 2: 後台選單
        /// </summary>
        /// <returns></returns>
        public int Type { get; set; }

        public string SN { get; set; }
        public string Title { get; set; }
        public int Sort { get; set; }
        public bool IsLink { get; set; }
        public IEnumerable<Menu> Children { get; set; }
        public abstract IEnumerable<Menu> GetChildren();
        public abstract Menu GetParent();
    }
}