using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_RevokeMessage
    {
        public long ChatRoomId { get; set; }
        public long MessageId { get; set; }
    }
}
