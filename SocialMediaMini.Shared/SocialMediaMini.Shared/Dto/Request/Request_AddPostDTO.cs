using SocialMediaMini.Shared.Const.Type;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaMini.Shared.Dto.Request
{
    public class Request_AddPostDTO
    {
        [Required(ErrorMessage = "Bài đăng không được để trống")]
        public string Content { get; set; }
        public string Images { get; set; }
        public PostVisibilityType PostVisibilityType { get; set; }
    }
}
