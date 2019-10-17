using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class MemberInfo
    {
        /// <summary>
        /// 會員主鍵值
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 會員帳號(未加密)
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 會員基本資料, 基本資料欄位依 web.config 設定是否需加密, 以 json 字串回傳
        /// </summary>
        public List<ColumnField> Fields { get; set; }
    }
}