using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class GetRegionsRequest
    {
        /// <summary>
        /// 區域代碼(若代碼為null，撈所有國家，否則撈該區域所有分區)
        /// </summary>
        public string RegionCode { get; set; }
    }
}