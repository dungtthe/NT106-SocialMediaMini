using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_PostDetail
    {
        public class Post
        {
            public long PostId { get; set; }
            public string Content { get; set; }
            public List<string> Images { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public UserDto User { get; set; }
            public List<ReactionDto> Reactions { get; set; }
            public long CommentCount { get; set; }
        }
    }
}
