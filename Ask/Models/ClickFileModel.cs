using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ClickFileModel
    {
        public long ID { get; set; }

        public string FileInfo { get; set; }

        public string ShowName { get; set; }

        public string FileMimeType { get; set; }

        public int FileSize { get; set; }
    }
}