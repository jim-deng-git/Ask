using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 取得會員條款
    /// </summary>
    public class ArticleModuleRequest: APISiteRequestBase
    {
        public string SN { get; set; } = "news";
    }
}