using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class StatisticConditionModels
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string LabelColor { get; set; } = "light-grey";
        public StatisticType StatisticType { get; set; } = StatisticType.SummaryViewCount;
        public bool ShowStatus { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public long StatisticValue { get; set; } = 0;
        public int Sort { get; set; }
        public string GetStatisticTypeName(StatisticType statisticType)
        {
            switch (statisticType)
            {
                case StatisticType.SummaryViewCount:
                    return "瀏覽量";
                case StatisticType.DailyViewCount:
                    return "每日瀏覽人次";
                case StatisticType.MemberViewCount:
                    return "每日會員瀏覽";
            }
            return "?";
        }
        public string GetConditionContent(long siteID)
        {
            string content = "";
            var details =  Models.DataAccess.StatisticConditionDAO.GetDetailItems(siteID, ID);
            if (details != null && details.Count() > 0)
            {
                foreach (Models.StatisticConditionDetailModels detail in details)
                {
                    content += (string.IsNullOrEmpty(content) ? "" : "<br/>") + string.Format("<span style='color:{2}'>【{0}】</span>{1} ", detail.GetAnalysisTypeName(detail.AnalysisType),
                        detail.GetAnalysisItemsContent(siteID, detail.AnalysisType, detail.AnalysisItems), ColorBar(LabelColor));
                }
            }
            return content;
        }
        public IEnumerable<StatisticConditionDetailModels> Details { get; set; }
        /// <summary>
        /// 每日點閱記錄
        /// </summary>
        public IEnumerable<ViewModels.AnalysisDailyLogViewModel> LogDailyList { get; set; }
        public static string ColorBar(string colorName)
        {
            switch (colorName)
            {
                case "red":
                    return "#FF1744";
                case "orange":
                    return "#ef6c00";
                case "yellow":
                    return "#fdd835";
                case "green":
                    return "#43A047";
                case "light-green":
                    return "#8bc34a";
                case "blue":
                    return "#2196F3";
                case "teal":
                    return "#009688";
                case "deep-purple":
                    return "#673ab7";
                case "gold":
                    return "#ac7224";
                case "light-grey":
                    return "#bdbdbd";
                case "grey":
                    return "#616161";
                case "black":
                    return "#000000";
                default:
                    return "";
            }
        }
    }
    public enum StatisticType
    {
        /// <summary>
        /// 瀏覽量
        /// </summary>
        SummaryViewCount = 0 ,
        /// <summary>
        /// 每日瀏覽人次
        /// </summary>
        DailyViewCount  = 1 ,
        /// <summary>
        /// 每日會員瀏覽
        /// </summary>
        MemberViewCount = 2 
    }
}