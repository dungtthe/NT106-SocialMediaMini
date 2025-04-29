using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("User_ChatRoom")]
    public class User_ChatRoom
    {
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        public long ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; }

        public bool IsLeft { get; set; }
    }
}
