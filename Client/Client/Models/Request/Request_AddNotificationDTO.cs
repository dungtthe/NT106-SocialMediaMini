using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Request
{
    public class Request_AddNotificationDTO
    {
        public class Message
        {
            public long ChatRoomId { get; set; }
            public string Content { get; set; }
            public long? ParrentMessageId { get; set; }
        }

        public byte NotificationType { get; set; }
        public string Data { get; set; }
    }

}
