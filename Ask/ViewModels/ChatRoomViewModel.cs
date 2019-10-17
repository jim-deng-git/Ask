using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkV3.Models;

namespace WorkV3.ViewModels
{
    public class ChatRoomViewModel
    {
        public ChatRoomModel ChatRoom { get; set; }
        public IEnumerable<ChatRoomMemberModel> ChatRoomMembers { get; set; }
        public IEnumerable<ChatRoomContentModel> ChatRoomContent { get; set; }
        public IEnumerable<ChatQuickCommentModel> ChatQuickComment { get; set; }
    }
}