using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkLib;

public class WebInfo
{
    public static readonly string Conn = System.Configuration.ConfigurationManager.ConnectionStrings["dbConn"].ToString();

    // 若設置為 true，則會壓縮頁面中的 js 和 css
    public static readonly bool EnableOptimizations = (GetItem.appSet("EnableOptimizations") as bool?) ?? true;

    public static readonly int PageSize = (int)GetItem.appSet("PageSize");
    #region 設定項目

    /// <summary>
    /// 後台sys session key
    /// </summary>
    public static readonly string SysMemSkey = "SysUser";
    /// <summary>
    /// memeber session key
    /// </summary>
    public static readonly string MemSkey = "MemUser";

    /// <summary>
    /// 後臺登入頁
    /// </summary>
    public static string BackendUrl { get { return WorkLib.uUrl.GetURL().TrimEnd('/') + "/backend/Login"; } }

    /// <summary>
    /// 前臺root地址，後臺寄信需要用到（http://localhost/HuaShan/w/huashan1914/）
    /// </summary>
    public static string WebRootUrl { get { return WorkLib.uUrl.GetURL().TrimEnd('/') + "/w/huashan1914"; } }

    #region 日期
    public const string DateFmt = "yyyy-MM-dd";                 // 日期顯示
    public const string DateTimeFmt = "yyyy-MM-dd HH:mm:ss";    // 時間日期顯示
    public const string DateTimeNotS = "yyyy-MM-dd HH:mm";      // 時間日期顯示（無秒）
    #endregion

    #region 日期
    public const string DateFmtView = "yyyy.MM.dd";                 // 日期顯示
    #endregion

    #endregion

    //public static DateTime now = DateTime.Parse("2017/12/01"); // DateTime.Now;

    #region 網站設定項

    public static readonly string StyleHeader = "1";
    public static readonly string StyleFooter = "4";

    #endregion
}
