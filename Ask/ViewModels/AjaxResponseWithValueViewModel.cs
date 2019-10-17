using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    /// <summary>
    /// 購物車購買項目列表
    /// </summary>
    public class AjaxResponseWithValueViewModel<T>
    {
        public bool Success { get; set; } = true;
        public T Value { get; set; }
    }
}