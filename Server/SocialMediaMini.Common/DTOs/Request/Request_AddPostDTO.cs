using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.DTOs.Request
{
    public class Request_AddPostDTO
    {
        [Required(ErrorMessage = "Bài đăng không được để trống")]
        public string Content { get; set; }
        public string Images { get; set; }
        public byte PostVisibility { get; set; }
    }
}
