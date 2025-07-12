using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Response_FriendResponseDto
    {
        public long Id { get; set; }
        public UserDto Sender { get; set; }
        public UserDto Receiver { get; set; }
        public string Status { get; set; } // "Pending", "Accepted", "Rejected"
        public DateTime CreatedAt { get; set; }
    }
}
