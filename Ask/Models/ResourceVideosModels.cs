using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Golbal;
using WorkLib;

namespace WorkV3.Models
{
    public class ResourceVideosModels
    {
        public long ID { get; set; }        
        public string Type { get; set; }
        public string Link { get; set; }
        public byte? SizeType { get; set; }
        public string PlayMode { get; set; }
        public bool IsAuto { get; set; }
        public bool IsRepeat { get; set; }
        public bool ShowSpec { get; set; }
        public string Spec { get; set; }
        public bool ShowDuration { get; set; }
        public string Duration { get; set; }
        public bool ShowViews { get; set; }
        public bool ScreenshotIsCustom { get; set; }
        public string Screenshot { get; set; }        
    }
}