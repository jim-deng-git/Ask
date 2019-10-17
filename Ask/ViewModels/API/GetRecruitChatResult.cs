using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class GetRecruitChatResult
    {
        /// <summary>
        /// 聊天室Title
        /// </summary>
        public string ChatRoomID { get; set; }
        /// <summary>
        /// 是否為新聊天室
        /// </summary>
        public bool isNewChat { get; set; }
    }
    
    public enum GetRecruitChatResultCode
    {
        [Message("成功")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("尚未應徵")]
        NotApplication,
        [Message("尚未應徵成功")]
        NotApplicationsuccess,
        [Message("查無任何聊天室")]
        ChatNull,
        [Message("請輸入正確代號")]
        Error,
        Exception
    }
}