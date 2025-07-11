using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Request
{
    public class Request_ReactOrUnReactPostDto
    {
        public long PostId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
