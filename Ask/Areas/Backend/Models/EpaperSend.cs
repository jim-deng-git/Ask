using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSend
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        public string Type { get; set; }
        public string UserFilter { get; set; }
        public string LikeFilter { get; set; }
        public string SubscribeFilter { get; set; }
        public bool IsSendNow { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? SendTimeEnd { get; set; }
        public string Status { get; set; }
        public DateTime? StatusChangeTime { get; set; }
        public long Epaper_ID { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        public List<EpaperSendList> SendList { get; set; }
        //--------------------------------------------------
        public string inputType { get; set; } //發送名單種類
        public string manualSendList { get; set; } //手動輸入發送名單
        public string SubscribeFilterArray { get; set; }
        public long EpaperType_ID { get; set; }
        public string LikeFilterArray { get; set; }
        public string UserFilterArray { get; set; }
        /// <summary>
        /// 是否為暫存
        /// </summary>
        public bool IsTemp { get; set; }
        /// <summary>
        /// 預估發送筆數
        /// </summary>
        public int SendCount
        {
            get
            {
                if (Epaper_ID == 0 || ID == 0)
                    return 0;
                else
                {
                    try
                    {
                        return WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.QuerySendListCountByEpaperID(Epaper_ID,ID);
                    }catch
                    {
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 預估發送筆數 for temp
        /// </summary>
        public int SendCountForTemp
        {
            get
            {
                if (Epaper_ID == 0 || ID == 0)
                    return 0;
                else
                {
                    try
                    {
                        return WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.QuerySendListCountByEpaperID(Epaper_ID, ID,true);
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 顯示預估/時間時間
        /// </summary>
        public string ShowEstimOrRealTimStr
        {
            get
            {
                if (string.IsNullOrEmpty(Status))
                    return string.Empty;
                if (SendCount == 0)
                    return string.Empty;
                try
                {
                    return WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GenEstimOrRealTimStr(SiteID,Status, SendCount, 1, SendTime, SendTimeEnd);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}