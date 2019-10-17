using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得商品資訊送出參數格式
    /// </summary>
    public class FlapsRequest
    {
        public string IS { get; set; } = "52229532"; // 指令集
        public string Method { get; set; } // 指令名稱

        public void SetMethod(string methodName)
        {
            Method = $"iPIS/{methodName}";
        }

        public void SetCommandSet(string commandSetName)
        {
            IS = commandSetName;
        }
    }
}