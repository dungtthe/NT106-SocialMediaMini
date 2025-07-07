using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.DTOs.Request
{
    public class Request_AddMessageDTO
    {
        public long UserId { get; set; }
        public long ChatRoomId { get; set; }
        public string Content { get; set; }
        public long? ParrentMessageId { get; set; }
    }
}
