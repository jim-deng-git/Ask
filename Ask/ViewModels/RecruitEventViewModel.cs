using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class RecruitEventViewModel
    {
        #region Recruit
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        public bool CustomIcon { get; set; }
        public string Icon { get; set; }
        #endregion

        /// <summary>
        /// 工作ID
        /// </summary>
        public long JobID { get; set; }
        /// <summary>
        /// 應徵ID
        /// </summary>
        public long ApplicationID { get; set; }
        /// <summary>
        /// 工作日期
        /// </summary>
        public DateTime DateStart { get; set; }
        /// <summary>
        /// 工作日期
        /// </summary>
        public DateTime? DateEnd { get; set; }
        /// <summary>
        /// 工作時間
        /// </summary>
        public string TimeStart { get; set; }
        /// <summary>
        /// 工作時間
        /// </summary>
        public string TimeEnd { get; set; }
        /// <summary>
        /// 工作名稱
        /// </summary>
        public string JobName { get; set; }
    }
}