using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class CardsTypeModels
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public bool isOpenCreate { get; set; }
        public bool isNeedSN { get; set; }
        public bool isIndexCards { get; set; }
        public int Types { get; set; }
        public string URLAction { get; set; }
        public string EditURLAction { get; set; }
        public int iFrameH { get; set; }
        public int iFrameW { get; set; }
        public List<ManageURLs> ManageURL { get; set; }
        //public class ManageURL
        //{
        //    public ManageURLs[] Property1 { get; set; }
        //}

        public class ManageURLs
        {
            public string icon { get; set; }
            public string Title { get; set; }
            public string URLAction { get; set; }
        }

    }


}