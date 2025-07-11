using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Respone
{
    public class Respone_LoginDTO
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Token { get; set; }
    }
}
