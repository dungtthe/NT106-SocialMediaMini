using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Respone
{
    public class Respone_NotificationDTO
    {
        public byte NotificationType { get; set; }
        public string Message { get; set; }


        public class Respone_NotificationMessage : Respone_NotificationDTO
        {
            public class User
            {
                public long Id { get; set; }
                public string FullName { get; set; }
                public string Avatar { get; set; }
            }

            public long ChatRoomId { get; set; }
            public long Id { get; set; }
            public string Content { get; set; }
            public string CreatedAt { get; set; }
            public User Sender { get; set; }
            public Respone_NotificationMessage Parrent { get; set; }
        }
    }
}
