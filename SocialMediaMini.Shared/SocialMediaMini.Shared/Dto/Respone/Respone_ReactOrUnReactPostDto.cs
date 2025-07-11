using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_ReactOrUnReactPostDto
    {
        public long PostId { get; set; }
        public ReactionDto Reaction { get; set; }
    }
}
