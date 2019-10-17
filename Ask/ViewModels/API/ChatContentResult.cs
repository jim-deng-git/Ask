using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class ChatContentResult
    {
        /// <summary>
        /// 聊天室Title
        /// </summary>
        public string ChatRoomTitle { get; set; }
        /// <summary>
        /// 聊天室圖片
        /// </summary>
        public string ChatRoomImage { get; set; }
        /// <summary>
        /// 聊天室成員
        /// </summary>
        public List<ChatRoomMember> ChatRoomMembers { get; set; }
        /// <summary>
        /// 聊天室內容
        /// </summary>
        public List<object> ChatRoomContent { get; set; }

        /// <summary>
        /// 一般訊息
        /// </summary>
        public ChatRoomMessage ChatRoomMessage { get; set; }
        /// <summary>
        /// 圖片
        /// </summary>
        public ImageMessage ImageMessage { get; set; }
        /// <summary>
        /// 檔案
        /// </summary>
        public FileMessage FileMessage { get; set; }
    }

    public class ChatRoomMember
    {
        /// <summary>
        /// 會員ID
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// 會員名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 會員頭貼
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 會員頭貼(檔名)
        /// </summary>
        public string ImageFileName { get; set; }
        /// <summary>
        /// 會員種類 1:管理員 2:一般會員
        /// </summary>
        public int MemberType { get; set; }
        /// <summary>
        /// 進入時間
        /// </summary>
        public DateTime EnterTime { get; set; }
        /// <summary>
        /// 已讀時間
        /// </summary>
        public DateTime ReadTime { get; set; }
    }

    public class ChatRoomMessage
    {
        /// <summary>
        /// 會員ID
        /// </summary>
        public string MemberID { get; set; }
        /// <summary>
        /// 會員名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 會員頭貼
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 訊息ID
        /// </summary>
        public string MessageID { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 訊息種類 1:文字 2:圖片 3:檔案
        /// </summary>
        public int MessageType { get; set; }
        /// <summary>
        /// 訊息時間
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 已讀數
        /// </summary>
        public int isRead { get; set; }
    }

    public class ImageMessage : ChatRoomMessage
    {
        /// <summary>
        ///圖片寬
        /// </summary>
        public int ImgWidth { get; set; }
        /// <summary>
        ///圖片高
        /// </summary>
        public int ImgHeight { get; set; }
        /// <summary>
        /// 檔案連結
        /// </summary>
        public string FileUrl { get; set; }
    }

    public class FileMessage : ChatRoomMessage
    {
        /// <summary>
        /// 檔案連結
        /// </summary>
        public string FileUrl { get; set; }
    }

    public enum ChatContentResultCode
    {
        [Message("成功")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("無此聊天室")]
        ChatNull,
        Exception
    }
}