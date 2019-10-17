using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WorkV3.Global
{
    /// <summary>
    /// 擴充方法
    /// </summary>
    public static class IEnumerableExtensions
    {
        public static IList<T> Clone<T>(this IList<T> self) 
        {
            IList<T> coppied = new List<T>(self);
            return coppied;
        }
    }
}