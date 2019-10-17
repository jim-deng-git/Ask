using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 取得會員條款
    /// </summary>
    public class FaqResult
    {
        /// <summary>
        /// 會員條款標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 會員條款內容
        /// </summary>
        public List<string> Content { get; set; }
    }
}