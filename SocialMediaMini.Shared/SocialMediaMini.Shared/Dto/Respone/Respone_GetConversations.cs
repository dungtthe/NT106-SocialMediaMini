using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_GetConversations
    {
        public class Conversation
        {
            //chat room
            public long ChatRoomId { get; set; }
            public long LeaderId { get; set; }
            public string RoomName { get; set; }
            public string Avatar { get; set; }
            public bool IsGroupChat { get; set; }
            //message
            public string LastMessage { get; set; }
            public int UnReadMessageCount { get; set; }
            public DateTime LastTime { get; set; }
        }
    }
}
