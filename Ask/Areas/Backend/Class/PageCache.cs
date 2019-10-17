using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;

namespace WorkV3.Areas.Backend
{
    public class PageCache
    {

        private static string _SiteName;
        public static string SiteName
        {
            get { return _SiteName; }
            set
            {
                _SiteName = value;
                Reset();
            }
        }

        private static long _SiteID;
        public static long SiteID
        {
            get { return _SiteID; }
            set
            {
                _SiteID = value;
                Reset();
            }
        }

        private static long _MenuID;
        public static long MenuID
        {
            get { return _MenuID; }
            set
            {
                _MenuID = value;
                Reset();
            }
        }

        private static string _UploadVPath = string.Empty;
        public static string UploadVPath
        {
            get
            {
                if (string.IsNullOrEmpty(_UploadVPath))
                {
                    _UploadVPath = UpdFileInfo.GetVPathByMenuID(SiteID, MenuID);
                }
                return _UploadVPath;
            }
            private set { }
        }

        #region 私有方法

        private static void Reset()
        {
            _UploadVPath = string.Empty;
        }

        #endregion 私有方法
    }
}