using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class AnalysisPageLogViewModel
    {
        public string SiteID { get; set; }
        public string SiteSN { get; set; }
        public string No { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string SN { get; set; }
        public string Creator { get; set; }
        public long TotalViewCount { get; set; }
        public long TotalMemberViewCount { get; set; }
        public long TotalUserCount { get; set; }
        public List<AnalysisPageLogViewModel> PageLogViewList { get; set; }

        public static string[] MachineNames = new string[] {
                "Macintosh",
                "iPhone",
                "Windows",
                "Nexus",
                "iPad"
        };
        public static Dictionary<string, string> Browsers = new Dictionary<string, string>  {
                 {"Opera", "Opera" },
                 {"Firefox", "Firefox" },
                 {"Chrome", "Chrome" },
                 {"Safari", "Safari" },
                 {"IE 11", "like gecko" },
                 {"IE 10", "10.0" },
                 {"IE 9", "9.0" },
                 {"IE 8", "8.0" },
                 {"IE 7", "7.0" },
                 {"IE 6", "6.0" },
                 {"其他", "Others" }
        };
        public static Dictionary<string, string> OSs = new Dictionary<string, string>  {
            {"Win 10" ,"Windows NT 10.0"},
            {"Win 8.1", "Windows NT 6.3"},
            {"Win 8.0", "Windows NT 6.2"},
            {"Win 7", "Windows NT 6.1"},
            {"Win Vista", "Windows NT 6.0"},
            {"Win 2003", "Windows NT 5.2"},
            {"Win XP", "Windows NT 5.1"},
            {"Win 2000", "Windows NT 5.0"},
            {"其他", "Others" }
        };
        public static Dictionary<string, int[]> Ages = new Dictionary<string, int[]> {
            {"6歲以下", new int[] { 0, 6 }},
            {"7~12歲", new int[] { 7, 12 }},
            {"13~15歲", new int[] { 13, 15 }},
            {"16~18歲", new int[] { 16, 18 }},
            {"19~20歲", new int[] { 19, 20 } },
            {"21~24歲", new int[] { 21, 24 }},
            {"25~29歲", new int[] { 25, 29 }},
            {"30~39歲", new int[] { 30, 39 }},
            {"40~54歲", new int[] { 40, 54 }},
            {"55~64歲", new int[] { 55, 64 }},
            {"65~80歲", new int[] { 65, 80 }},
            {"81歲以上", new int[] { 81, 999 }}
        };
        /// <summary>
        /// 取得裝置名稱
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetMachineNumber(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "其他";
            
            foreach (string machineName in MachineNames)
            {
                if (userAgent.Contains(machineName))
                    return machineName;
            }
            return "其他";
        }
        /// <summary>
        /// 取得裝置名稱
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetMachineUserAgentCondition(string machine, string logicType)
        {
            if (string.IsNullOrEmpty(machine))
                return "";
            if (MachineNames.Contains(machine))
            {
                return logicType + " UserAgent LIKE '%" + machine + "%' ";
            }
            else
            {
                string cond = "";
                foreach (string machineName in MachineNames)
                {
                    if (!string.IsNullOrEmpty(cond))
                        cond += " AND ";
                    cond +=  " ( UserAgent NOT LIKE '%" + machine + "%') ";
                } 
                return logicType +" ( "+ cond+" ) ";
            }
        }
        /// <summary>
        /// 取得瀏覽器名稱
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetBrowserNumber(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "其他";
            foreach (string bkey in Browsers.Keys)
            {
                bool isOpera = userAgent.Contains("Opera");
                if (userAgent.Contains(bkey))
                    return bkey;
                if (userAgent.Contains("compatible") && userAgent.Contains("MSIE") && !isOpera)
                {
                    if (userAgent.Contains(Browsers[bkey]))
                        return bkey;
                }
            }
            return "其他";
        }
        /// <summary>
        /// 取得瀏覽器的條件設定
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetBrowserUserAgentCondition(string browser, string logicType)
        {
            if (string.IsNullOrEmpty(browser))
                return "";
            if (browser == "其他")
            {
                string cond = "";
                foreach (string key in Browsers.Keys)
                {
                    if (!string.IsNullOrEmpty(cond))
                        cond += " AND ";
                    cond += " ( UserAgent NOT LIKE '%" + Browsers[key] + "%') ";
                }
                return logicType + " ( " + cond + " ) ";
            }
            if (browser.IndexOf("IE") >= 0)
            {
                if(browser == "IE 11")
                    return logicType + " UserAgent LIKE '%" + Browsers[browser] + "%' ";
                else
                    return logicType + " (UserAgent LIKE '%" + Browsers[browser] + "%' AND UserAgent LIKE '%compatible%' AND UserAgent LIKE '%MSIE%') ";
            }
            if (Browsers.Keys.Contains(browser))
            {
                return logicType + " UserAgent LIKE '%" + Browsers[browser] + "%' ";
            }
            return logicType + " (1=1) ";
        }
        /// <summary>
        /// 取得 OS 名稱
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetOSNumber(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "其他";
          
            foreach (string osName in OSs.Keys)
            {
                if (userAgent.Contains(OSs[osName]))
                    return osName;
            }
            return "其他";
        }
    }
}