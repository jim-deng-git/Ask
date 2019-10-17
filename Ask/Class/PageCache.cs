using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;

namespace WorkV3
{
    public class PageCache
    {

        public static PageCache GetTempDataPageCache(System.Web.Mvc.TempDataDictionary TempData)
        {
            if (TempData["PageCache"] == null)
                return null;

            PageCache pageCache = (PageCache)TempData["PageCache"];
            return pageCache;
        }
        public static void SetTempDataPageCache(System.Web.Mvc.TempDataDictionary TempData, PageCache pageCache)
        {
            TempData["PageCache"] = pageCache;
        }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public long PageNO { get; set; }
        public string SiteSN { get; set; }
        public string SiteName { get; set; }
        public string PageSN { get; set; }
        public string UploadVPath
        {
            get { return UpdFileInfo.GetVPathByMenuID(SiteID, MenuID); }
        }

        //private  string _SiteName;
        //public  string SiteName
        //{
        //    get { return _SiteName; }
        //    set
        //    {
        //        _SiteName = value;
        //        Reset();
        //    }
        //}

        //private  string _SiteSN;
        //public  string SiteSN
        //{
        //    get { return _SiteSN; }
        //    set
        //    {
        //        _SiteSN = value;
        //        Reset();
        //    }
        //}

        //private  long _SiteID;
        //public  long SiteID
        //{
        //    get { return _SiteID; }
        //    set
        //    {
        //        _SiteID = value;
        //        Reset();
        //    }
        //}

        //private  long _MenuID;
        //public  long MenuID
        //{
        //    get { return _MenuID; }
        //    set
        //    {
        //        _MenuID = value;
        //        Reset();
        //    }
        //}

        //private  long _PageNO;
        //public  long PageNO
        //{
        //    get { return _PageNO; }
        //    set
        //    {
        //        _PageNO = value;
        //        Reset();
        //    }
        //}

        //private  string _PageSN;
        //public  string PageSN
        //{
        //    get { return _PageSN; }
        //    set
        //    {
        //        _PageSN = value;
        //        Reset();
        //    }
        //}
        //private  string _UploadVPath = string.Empty;
        //public  string UploadVPath
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_UploadVPath))
        //        {
        //            _UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
        //        }
        //        return _UploadVPath;
        //    }
        //    private set { }
        //}

        //#region 私有方法

        //private  void Reset()
        //{
        //    _UploadVPath = string.Empty;
        //}

        //#endregion 私有方法
    }
}