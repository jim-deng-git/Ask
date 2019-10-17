using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ChatListResult
    {
        /// <summary>
        /// 聊天室ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 訊息類型 1:文字  2:圖片  3:檔案
        /// </summary>
        public int MessageType { get; set; }
        /// <summary>
        /// 圖片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 時間
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 人數
        /// </summary>
        public int PeopleCount { get; set; }
        /// <summary>
        /// 未讀數
        /// </summary>
        public int NotRead { get; set; }
        /// <summary>
        /// 聊天室類型 1:客服  2:群組  3:一對一
        /// </summary>
        public int ChatRoomType { get; set; }
    }

    public enum ChatListResultCode
    {
        [Message("成功")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("本會員暫無任何聊天室")]
        ChatNull,
        Exception
    }
}