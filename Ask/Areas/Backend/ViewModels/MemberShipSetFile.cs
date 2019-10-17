using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.ViewModels
{
    public class MemberShipSetFile
    {
        public string FileInfo { get; set; }
        public long FileSize { get; set; }
        public string FileSizeDesc { get; set; }
        public string ShowName { get; set; }
    }
}