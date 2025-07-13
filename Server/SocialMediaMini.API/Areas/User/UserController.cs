using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using SocialMediaMini.API.Extensions;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.Common.ResultPattern;
using SocialMediaMini.Service;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Dto.Request;

namespace SocialMediaMini.API.Areas.User
{
    [Area("User")]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(Request_RegisterDTO data)
        {
            var result = await _userService.RegisterAsync(data);
            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(Request_LoginDTO data)
        {
            var result = await _userService.LoginAsync(data);
            return result.ToActionResult();
        }

        [HttpGet("friends-summary")]
        public async Task<IActionResult> GetFriendsSummaryAsync()
        {
            var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
            long userId = 0;
            if (suserId == null || !long.TryParse(suserId, out userId))
            {
                return Unauthorized();
            }
            var rs = await _userService.GetFriendsSummaryAsync(userId);
            return rs.ToActionResult();
        }


        [HttpPost("forgot-password")] 
        public async Task<IActionResult> RequestForgotPasswordAsync(Request_ForgotPasswordDto request)
        {
            var rs = await _userService.RequestForgotPasswordAsync(request.Email);
            return rs.ToActionResult();
        }

        [HttpGet("/validate-reset-password/{token}")]
        public async Task<IActionResult> ValidateResetPasswordTokenAsync(string token)
        {
            var rs = await _userService.ValidateResetPasswordTokenAsync(token);
            return rs.ToActionResult();
        }

    }
}
