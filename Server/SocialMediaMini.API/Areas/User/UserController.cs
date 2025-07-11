using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
