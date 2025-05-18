using SocialMediaMini.Common.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.DTOs.Respone
{
    public class Respone_PostDetail
    {
        public class User
        {
            public string FullName { get; set; }
            public string Avatar { get; set; }
        }
        public class Reaction
        {
            public User User { get; set; }
            public Type_Reaction TypeReaction { get; set; }
        }
        public class Post
        {
            public long PostId { get; set; }
            public string Content { get; set; }
            public List<string> Images { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public User User { get; set; }
            public List<Reaction> Reactions { get; set; }
            public long CommentCount { get; set; }
        }
    }
}
