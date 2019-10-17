using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Common;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class ApplicationListViewModel
    {
        public long ID { get; set; }
        public long RecruitID { get; set; }
        public long RecruitJobID { get; set; }
        /// <summary>
        /// 是否是歷史工作紀錄
        /// </summary>
        public bool IsOld { get; set; }
        public FormCheckStatus CheckStatus { get; set; }
        public string RecruitTitle { get; set; }
        public string JobName { get; set; }
        public SalaryOption SalaryOption { get; set; }
        public int Salary { get; set; }
        public string JobContent { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        /// <summary>
        /// 1:同工作地點、2:其他地點
        /// </summary>
        public int CheckInPlace { get; set; }
        public string PlaceRegions { get; set; }
        public string PlaceAddress { get; set; }
        public string PlaceName { get; set; }
        public bool CustomIcon { get; set; }
        public string Icon { get; set; }
    }
}