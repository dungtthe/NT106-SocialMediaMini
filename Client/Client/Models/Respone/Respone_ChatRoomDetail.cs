using Client.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Respone
{
    public class Respone_ChatRoomDetail
    {
        public class User
        {
            public long Id { get; set; }
            public string FullName { get; set; }
            public string Avatar { get; set; }
        }

        public class Reaction
        {
            public User User { get; set; }
            public Type_Reaction TypeReaction { get; set; }
        }

        public class Message
        {
            public long Id { get; set; }
            public string Content { get; set; }
            public string CreatedAt { get; set; }
            public User Sender { get; set; }
            public Message Parrent { get; set; }
            public List<Reaction> Reactions { get; set; }

        }
        public long LeaderId { get; set; }
        public string Avatar { get; set; }
        public string RoomName { get; set; }//neu la 2 nguoi thi dung ten nguoi do
        public List<Message> Messages { get; set; }
        public List<string> AvatarReads { get; set; }//danh sach nguoi da doc tin nhan(hien thi avatar) last message

        //dung cho group
        public bool IsGroupChat { get; set; }
        public bool CanSendMessage { get; set; }
        public bool CanAddMember { get; set; }
        public int CountMember { get; set; }
    }
}
