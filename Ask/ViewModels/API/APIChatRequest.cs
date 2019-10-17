using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class APIChatRequest : APISiteRequestBase
    {
        /// <summary>
        /// 聊天室ID
        /// </summary>
        public string ChatID { get; set; }
    }
}