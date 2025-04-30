using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaMini.API.Extensions;
using SocialMediaMini.Common.Const;
using SocialMediaMini.Common.DTOs.Request;
using SocialMediaMini.Service;

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
            try
            {
                var rsp = await _userService.RegisterAsync(data);
                return this.ResponeMessageResult(rsp);
            }
            catch(Exception ex)
            {
                return this.InternalServerError();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(Request_LoginDTO data)
        {
            try
            {
                var rsp = await _userService.LoginAsync(data);
                if (rsp.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new
                    {
                        message = rsp.Message
                    });
                }
                else
                {
                    return Ok(new
                    {
                        token = rsp.Token,
                        fullName = rsp.FullName,
                        image = rsp.Image
                    });
                }
            }
            catch (Exception ex)
            {
                return this.InternalServerError();
            }
        }

        
    }
}
