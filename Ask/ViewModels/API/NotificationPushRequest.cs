using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class NotificationPushRequest
    {
        public string Secret { get; set; }
        /// <summary>
        /// 要推播的帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 來自於哪(News、Story、MindTest)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 根據type，除了其它不用帶，因為要讓app知道他要進入那個單元的哪筆資料
        /// </summary>
        public string ID { get; set; }
    }

    public enum NotificationPushType
    {
        none,
        news,
        story,
        mindTest
    }

    public enum NotificationPushResult
    {
        [Message("")]
        Success,
        [Message("Secret錯誤")]
        SecretError,
        [Message("Type錯誤")]
        TypeWrong,
        Exception
    }
}