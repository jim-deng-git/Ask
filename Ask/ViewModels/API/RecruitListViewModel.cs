using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class RecruitListRequest : APISiteRequestBase
    {
        /// <summary>
        /// 第幾頁(一頁20筆)
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 搜尋關鍵字, 可為null
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 搜尋類別, 可為null
        /// </summary>
        public long[] Types { get; set; }
        /// <summary>
        /// 是否只取得活動中的資料
        /// </summary>
        public bool IsEvent { get; set; } = false;
        /// <summary>
        /// 職缺ID, 可為null
        /// </summary>
        public long? RecruitID { get; set; }
        /// <summary>
        /// 是否帶出工作時間內的, 可為null
        /// </summary>
        public bool Status { get; set; } = false;
    }

    public class RecruitListResult
    {
        /// <summary>
        /// 職缺ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 職缺標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 職缺薪水
        /// </summary>
        public IEnumerable<string> Salary { get; set; }
        /// <summary>
        /// 職缺類別
        /// </summary>
        public string Types { get; set; }
        public List<RecruitJobList> RecruitJobList { get; set; }
        /// <summary>
        /// 職缺圖片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 職缺圖片寬
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// 職缺圖片高
        /// </summary>
        public int ImageHeight { get; set; }
    }

    public class RecruitJobList
    {
        /// <summary>
        /// 應徵ID
        /// </summary>
        public long ApplicationId { get; set; }
        /// <summary>
        /// 工作類型ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 工作類型名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 車資
        /// </summary>
        public string CarFare { get; set; }
        /// <summary>
        /// 供餐
        /// </summary>
        public string Meal { get; set; }
        /// <summary>
        /// 薪水
        /// </summary>
        public string Salary { get; set; }
        /// <summary>
        /// 應徵狀態
        /// </summary>
        public string CheckStatus { get; set; }
        /// <summary>
        /// 是否可以取消應徵
        /// </summary>
        public bool IsClientCancel { get; set; }
    }

}