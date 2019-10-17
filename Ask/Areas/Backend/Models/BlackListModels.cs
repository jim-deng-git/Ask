using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    /// <summary>
    /// 非商拍申請黑名單
    /// </summary>
    public class BlackListModels
    {
        /// <summary>
        /// ID
        /// </summary>
		public int ID { get; set; }
        /// <summary>
        /// 申請單位
        /// </summary>
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 聯絡人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别 1：先生；0：女士
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 聯絡人手機
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 聯絡人Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 黑名單原因 1：無故不到；2：超時拍攝；3：其他違規；4：学校
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 違規時間
        /// </summary>
        public string BreachDatetime { get; set; }
        /// <summary>
        /// 違規紀錄或備註
        /// </summary>
        public string BreachNote { get; set; }
        /// <summary>
        /// Creator
        /// </summary>
        public long Creator { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Modifier
        /// </summary>
        public long Modifier { get; set; }
        /// <summary>
        /// ModifyTime
        /// </summary>
        public DateTime ModifyTime { get; set; }

        #region 擴展屬性

        /// <summary>
        /// 聯絡人性別 1：先生；0：女士
        /// </summary>
        public string GenderText
        {
            get
            {
                string text = "";
                switch (this.Gender)
                {
                    case "1":
                        text = "先生";
                        break;
                    case "0":
                        text = "女士";
                        break;
                }
                return text;
            }
        }

        /// <summary>
        /// 黑名單原因
        /// </summary>
        public string ReasonExt
        {
            get
            {
                string str = "";
                if (this.Reason == "1")
                    str = "無故不到";
                if (this.Reason == "2")
                    str = "超時拍攝";
                if (this.Reason == "3")
                    str = "其他違規";
                if (this.Reason == "4")
                    str = "学校";
                return str;
            }
        }

        /// <summary>
        /// 違規時間
        /// </summary>
        public string BreachDatetimeFmt
        {
            get
            {
                if (this.BreachDatetime==null) return "";
                return WorkV3.Golbal.PubFunc.DateTimeFmt(DateTime.Parse(this.BreachDatetime));
            }
        }

        /// <summary>
        /// 黑名單原因(本月次數)
        /// </summary>
        public int ReasonTimes { get; set; }
        /// <summary>
        /// 本月合併次數
        /// </summary>
        public int TotalTimes { get; set; }

        #endregion

    }

    public class BlackList_Search
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 黑名單原因 1：無故不到；2：超時拍攝；3：其他違規
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 違規期間 - start
        /// </summary>
        public string BreachDatetimeStart { get; set; }
        /// <summary>
        /// 違規期間 - end
        /// </summary>
        public string BreachDatetimeEnd { get; set; }
        /// <summary>
        /// 違規次數 1:本月單項超過次數; 2:本月合併超過次數
        /// </summary>
        public string BreachTimes { get; set; }
    }
}