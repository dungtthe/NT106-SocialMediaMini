using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Respone
{
    public class Respone_LoginDTO
    {
        public int HttpStatusCode { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Token { get; set; }
    }
}
