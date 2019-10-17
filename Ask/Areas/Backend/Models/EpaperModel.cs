using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperModel
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime? Publish_Date_Start { get; set; }
        public DateTime? Publish_Date_End { get; set; }
        public string Style { get; set; }
        //public string Picture { get; set; }
        public long EpaperType_ID { get; set; }
        public string EpaperTypeName { get; set; } //Epaper_Type.Name
        public bool IsCurrent { get; set; }
        public bool IsIssue { get; set; }
        public bool IsPublication { get; set; }//wei 20180810 刊登刊頭日期
        public string LogoUrl { get; set; }//wei 20180810 刊頭LOGO連結
        public bool? ParagraphIsIssue { get; set; }
        public string TemplateName { get; set; }
        public string TemplateThumb { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string SendStatus
        {
            get
            {
                string ClsStr;
                string result;
                if (EpaperSend != null) //GetSendLatelyStatusChange
                {
                    var RealSendCount = WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetRealSendCount(EpaperSend.ID);
                    result = WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetEpaperStatusName(EpaperSend.Status, UseClassCode, out ClsStr, EpaperSend.SendTime, EpaperSend.SendCount, RealSendCount);
                }
                else
                    result = WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetEpaperStatusName("", UseClassCode, out ClsStr);

                AccordToStatReClsStr = ClsStr;
                return result;
            }
        }
        public string SendStatusTime
        {
            get
            {
                string result = "";
                if (EpaperSend != null) //GetSendLatelyStatusChange
                {
                    if(EpaperSend.Status == Models.SendStatus.Sended || EpaperSend.Status == Models.SendStatus.ManualBreak || EpaperSend.Status == Models.SendStatus.SystemBreak)
                        result = EpaperSend.StatusChangeTime.HasValue ? ((DateTime)EpaperSend.StatusChangeTime).ToString("yyyy/MM/dd HH:mm") : "";
                }
                return result;
            }
        }
        public long MenuID { get; set; }

        public EpaperAreaModel HeadAreaModel { get; set; }
        public IEnumerable<EpaperAreaModel> BodyAreaModel { get; set; }
        public IEnumerable<ParagraphModels> ParagraphModels { get; set; }
        public EpaperAreaModel FooterAreaModel { get; set; }
        public IEnumerable<EpaperToModules> EpaperModules { get; set; }
        public EpaperTypeModel EpaperTypeItem
        {
            get
            {
                if(this.EpaperType_ID != 0)
                    return WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetEpaperTypeItem(this.EpaperType_ID);
                else
                    return new EpaperTypeModel();
            }
        }
        public long oldEpaperID { get; set; }
        public EpaperSend EpaperSend { get; set; }
        /// <summary>
        /// 要使用的class代碼  
        /// </summary>
        public int UseClassCode { get; set; }
        /// <summary>
        /// 根據 SendStatus 內容 傳回要套在class的字串 (用在電子報分析>發送結果)
        /// </summary>
        public string AccordToStatReClsStr { get; set; }
    }
}