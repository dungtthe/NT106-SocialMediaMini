using Client.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Respone
{
    public class Respone_GetFriendPosts
    {
        public class UserDTO
        {
            public string FullName { get; set; }
            public string Avatar { get; set; }
        }
        public class ReactionDTO
        {
            public UserDTO User { get; set; }
            public Type_Reaction TypeReaction { get; set; }
        }
        public class PostDTO
        {
            public long PostId { get; set; }
            public string Content { get; set; }
            public List<string> Images { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public UserDTO User { get; set; }
            public List<ReactionDTO> Reactions { get; set; }
            public long CommentCount { get; set; }
        }
    }
}
