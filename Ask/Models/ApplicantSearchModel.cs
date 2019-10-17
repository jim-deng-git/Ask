using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Models
{
    public class ApplicantSearchModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 報名狀態 1.待審核 2.正取 3.備取 4.不通過 5.後台取消 6.前台取消
        /// </summary>
        public IEnumerable<int> Status { get; set; }

        /// <summary>
        /// 報名方式 0.後台報名 1.前台報名
        /// </summary>
        public IEnumerable<int> FrontEnd { get; set; }

        /// <summary>
        /// 報到狀態 0.尚未報到 1.已報到
        /// </summary>
        public IEnumerable<int> CheckIn { get; set; }

        /// <summary>
        /// 報名開始時間
        /// </summary>
        public DateTime? SignUpStart { get; set; }

        /// <summary>
        /// 報名結束時間
        /// </summary>
        public DateTime? SignUpEnd { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        public int Index { get; set; } = 0;

        public int Limit { get; set; } = WebInfo.PageSize;
    }
}