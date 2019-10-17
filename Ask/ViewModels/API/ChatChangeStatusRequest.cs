using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ChatChangeStatusRequest
    {
        /// <summary>
        /// 聊天室ID
        /// </summary>
        public string ChatID { get; set; }
        /// <summary>
        /// 狀態, 進:in  出:out 
        /// </summary>
        public string Status { get; set; }
    }
}