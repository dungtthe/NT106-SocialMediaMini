using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_NotificationDTO
    {
        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }


        public class Respone_NotificationMessage : Respone_NotificationDTO
        {

            public long ChatRoomId { get; set; }
            public long Id { get; set; }
            public string Content { get; set; }
            public string CreatedAt { get; set; }
            public UserDto Sender { get; set; }
            public Respone_NotificationMessage Parrent { get; set; }
        }
    }
}
