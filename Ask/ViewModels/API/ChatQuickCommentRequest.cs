using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ChatQuickCommentRequest
    {
        /// <summary>
        /// Select、Insert、Delete
        /// </summary>
        public string Function { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}