using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    /// <summary>
    /// 區域代碼及名稱
    /// </summary>
    public class GetRegionsResult
    {
        /// <summary>
        /// 代碼
        /// </summary>
        public string RegionCode { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
    }

    public enum GetRegionsResultCode
    {
        [Message("")]
        Success,
        [Message("此地區無分區")]
        NoRegion,
        Exception
    }
}