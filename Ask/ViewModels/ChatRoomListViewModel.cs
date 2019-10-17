using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.ViewModels
{
    public class ChatRoomListViewModel
    {
        public string ChatRoomID { get; set; }
        public long RoomMemberID { get; set; }
        public int RoomMemberType { get; set; }
        public bool Is_InRoom { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime IsReadTime { get; set; }
        public int Authority { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public int ChatRoomType { get; set; }

        public string RoomContent { get; set; }
        public int RoomContentType { get; set; }
        public DateTime Time { get; set; }

        public int RoomMemberCount { get; set; }

        //未讀
        public int NotRead { get; set; }

        public string IDStr { get; set; }
    }
}