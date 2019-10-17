using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Areas.Backend.Models
{
    public class EpaperSubscriber
    {
        public long ID { get; set; }
        public long SiteID { get; set; }
        /// <summary>
        /// dbo.MemberShip.ID
        /// </summary>
        public long? Member_ID { get; set; }
        public string Email { get; set; }
        public string Likes { get; set; }
        public int? FailureTimes { get; set; }
        public bool IsBlack { get; set; }
        public bool IsPause { get; set; }
        public long? Creator { get; set; }
        /// <summary>
        /// 喜好身分
        /// </summary>
        public string identity { get; set; }
        public string favority { get; set; }
        /// <summary>
        /// 黑名單原因
        /// </summary>
        public string BlackReason { get; set; }
        /// <summary>
        /// 違規時間
        /// </summary>
        public DateTime? ViolationTime { get; set; }
        /// <summary>
        /// 違規紀錄或備註
        /// </summary>
        public string ViolationDesc { get; set; }
        public DateTime CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 是否為會員
        /// </summary>
        public bool IsMember
        {
            get
            {
                if (Member_ID != null)
                {
                    return WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.CheckMemberIsExist((long)Member_ID);
                }
                else
                    return false;

                //return Member_ID != null ? true : false;
            }
        }
        /// <summary>
        /// 黑名單資訊
        /// </summary>
        public EpaperBlackListInfo EpaperBlackListInfo { get; set; }
        public List<EpaperSubscriberDetail> Detail { get; set; }
        public WorkV3.Models.MemberShipModels MemberShip { get; set; }
        /// <summary>
        /// 取得訂閱資訊
        /// </summary>
        public void GetSubscriberDetail()
        {
            if (this.ID != 0)
                this.Detail = WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.GetSubscriberDetail(this.ID);
            else
                this.Detail = new List<EpaperSubscriberDetail>();
        }
        public void GetMemberShip()
        {
            if (this.Member_ID != null)
            {
                var mem = WorkV3.Areas.Backend.Models.DataAccess.CommonDA.GetItem<WorkV3.Models.MemberShipModels>("MemberShip", (long)this.Member_ID);
                this.MemberShip = mem != null ? mem : new WorkV3.Models.MemberShipModels();
            }
            else
                this.MemberShip = new WorkV3.Models.MemberShipModels();
        }
        public IEnumerable<string> GetFlags(long siteId)
        {
            var flags = WorkV3.Models.DataAccess.UserFlagDAO.GetFlags(siteId, Email, MemberShip.Mobile, MemberShip.Phone, MemberShip.IDCard);
            return flags;
        }
        /// <summary>
        /// 將訂閱者名單上面的會員ID擦掉
        /// </summary>
        public void ClearMemberIdInSubscriber()
        {
            if(ID != 0)
                WorkV3.Areas.Backend.Models.DataAccess.EpaperDAO.ClearMemberIdInSubscriber(ID);
        }
    }
}