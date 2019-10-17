using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels.API
{
    public class GetRecruitChatRequest : RecruitCollectionSetRequest
    {
        /// <summary>
        /// 1:客服 2:群組
        /// </summary>
        public int ChatRoomType { get; set; }
    }
}