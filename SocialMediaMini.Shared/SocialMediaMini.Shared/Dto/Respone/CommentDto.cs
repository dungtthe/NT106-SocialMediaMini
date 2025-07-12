using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class CommentDto
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
        public UserDto User { get; set; }
        public List<ReactionDto> Reactions { get; set; }
    }
}
