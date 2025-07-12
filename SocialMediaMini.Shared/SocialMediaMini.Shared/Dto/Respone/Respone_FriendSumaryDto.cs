using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_FriendSumaryDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar {  get; set; }
    }
}
