using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Request
{
    public class Request_CreateGroupchat
    {
        public List<long> MemberIds { get; set; }

        [Required(ErrorMessage = "Tên nhóm không được để trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tin nhắn chào đóng không được để trống")]
        public string Message { get; set; }
    }
}
