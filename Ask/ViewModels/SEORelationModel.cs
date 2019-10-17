using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class SEORelationModel
    {
        public string Title { set; get; }//wei 20180914 集點SEO需要
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string SocialImage { get; set; }
    }
}