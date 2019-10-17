using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models.Interface;

namespace WorkV3.ViewModels.Flaps
{
    /// <summary>
    /// 動能取得商品資訊送出參數格式
    /// </summary>
    public class FlapsRequestFormat<TRequest> where TRequest: IFlapsApiSetRequestMethod, new()
    {
        public FlapsRequestFormat()
        {
            FWS = new TRequest();
        }
        public TRequest FWS { get; set; }

        /// <summary>
        /// 設定 api 名稱
        /// </summary>
        /// <param name="methodName"></param>
        public void SetMethod(string methodName)
        {
            FWS.SetMethod(methodName);
        }

        /// <summary>
        /// 設定指令集名稱
        /// </summary>
        /// <param name="commandSetName"></param>
        public void SetCommandSet(string commandSetName)
        {
            FWS.SetCommandSet(commandSetName);
        }
    }
}