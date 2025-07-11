using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class ReactionDto
    {
        public UserDto User { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
