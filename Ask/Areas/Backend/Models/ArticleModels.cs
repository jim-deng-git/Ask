using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class ArticleModels
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public long MenuID { get; set; }
        public long CardNo { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime? IssueDate { get; set; }
        public string RemarkText { get; set; }
        public bool CustomIcon { get; set; }
        public string Icon { get; set; }
        public bool IsIssue { get; set; }
        public DateTime? IssueStart { get; set; }
        public DateTime? IssueEnd { get; set; }
        public string Link { get; set; }
        public bool IsOpenNew { get; set; }
        public string Archive { get; set; }
        public int Clicks { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public string LinkType { get; set; }
        public string LinkDetail { get; set; }
        public int ReadMode { get; set; }

        #region 主影片
        public bool isShowVideo { get; set; }
        public string VideoType { get; set; }
        public string VideoID { get; set; }
        public bool VideoImgIsCustom { get; set; }
        public string VideoImg { get; set; }
        #endregion
        public DateTime? ModifyTime { get; set; }
        
        public IEnumerable<WorkV3.Models.SitesModels> GetSites() {
            return DataAccess.ArticleDAO.GetItemSites(ID);
        }
    }
}