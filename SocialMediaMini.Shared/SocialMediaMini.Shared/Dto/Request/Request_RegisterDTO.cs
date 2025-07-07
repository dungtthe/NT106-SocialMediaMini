using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Dto.Request
{
    public class Request_RegisterDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        [MinLength(6,ErrorMessage ="Tên tài khoản phải ít nhất 6 kí tự!")]
        [MaxLength(100,ErrorMessage ="Tên tài khoản không được vượt quá 100 kí tự!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 kí tự!")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 kí tự!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage ="Địa chỉ Email không hợp lệ!")]
        [MaxLength(300,ErrorMessage ="Địa chỉ Email không được quá 300 kí tự!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [MaxLength(30, ErrorMessage = "Số điện thoại không được quá 30 kí tự!")]
        public string PhoneNumber { get; set; }
    }
}
