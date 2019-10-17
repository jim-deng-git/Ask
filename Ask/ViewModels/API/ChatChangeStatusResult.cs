using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public enum ChatChangeStatusResultCode
    {
        [Message("成功")]
        Success,
        [Message("需要登入")]
        NeedLogin,
        [Message("無此聊天室")]
        ChatNull,
        [Message("聊天室無此會員")]
        ChatMemberNull,
        [Message("狀態輸入錯誤 請輸入 in 或 out")]
        StatusError,
        Exception
    }
}