using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Messages")]
    public class Message:BaseModel
    {
        public string Content { get; set; }
        public bool IsLink { get; set; }
        public DateTime CreateAt { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        public long? ParrentMessageId { get; set; }
        [ForeignKey(nameof(ParrentMessageId))]
        public Message ParentMessage { set; get; }

        public long ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; }

        public string ReactionType_UserId_Ids { get; set; }
        public string ReadByUserIds { get; set; }
        public bool IsRevoked { get; set; }
    }
}
