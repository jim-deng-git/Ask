using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public enum DeleteMsgResultCode
    {
        [Message("刪除成功")]
        Success,
        [Message("網站尚未建立")]
        SiteNull,
        [Message("需要登入")]
        NeedLogin,
        [Message("查無此聊天室")]
        ChatNull,
        [Message("聊天室尚有資料，無法刪除")]
        DeleteError,
        Exception
    }
}