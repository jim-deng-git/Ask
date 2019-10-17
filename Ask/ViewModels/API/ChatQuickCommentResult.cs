using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public enum ChatQuickCommentResultCode
    {
        [Message("成功")]
        Success,
        [Message("需要登入")]
        NeedLogin,
        [Message("無此功能")]
        FunctionNull,
        [Message("已有重複項目")]
        InsertError,
        Exception
    }
}