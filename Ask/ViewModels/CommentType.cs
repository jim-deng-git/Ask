using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public enum CommentType
    {
        /// <summary>
        /// 關閉（不可回文）
        /// </summary>
        Closed = 0,
        /// <summary>
        /// 開放匿名回文
        /// </summary>
        All = 1,
        /// <summary>
        /// 開放會員回文
        /// </summary>
        MemberOnly = 2,
        /// <summary>
        /// 開放FB留言
        /// </summary>
        FB = 3
    }
    public class CommentTypeLibs
    {
        public static List<WorkV3.ViewModels.CommentType> GetCommitTypeList()
        {
            List<WorkV3.ViewModels.CommentType> ReplyItemList = new List<WorkV3.ViewModels.CommentType>();
            ReplyItemList.Add(WorkV3.ViewModels.CommentType.All);
            ReplyItemList.Add(WorkV3.ViewModels.CommentType.MemberOnly);
            ReplyItemList.Add(WorkV3.ViewModels.CommentType.FB);
            ReplyItemList.Add(WorkV3.ViewModels.CommentType.Closed);
            return ReplyItemList;
        }
    }
    public enum CommentLogType
    {
        /// <summary>
        /// 讚
        /// </summary>
        Good = 1,
        /// <summary>
        /// 不喜歡 (預留)
        /// </summary>
        Bad = 1
    }
}