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
            try
            {
                var rsp = await _userService.RegisterAsync(data);
                if (rsp.HttpStatusCode == HttpStatusCode.Ok)
                {
                    return Ok(new
                    {
                        success = true,
                        message = rsp.Message,
                        userId = rsp.UserId,
                        fullName = rsp.FullName,
                        image = rsp.Image,
                        token = rsp.Token
                    });
                }
                else
                {
                    return StatusCode((int)rsp.HttpStatusCode, new
                    {
                        success = false,
                        message = rsp.Message
                    });
                }
            }
            catch (Exception ex)
            {
               // await LoggerHelper.LogMsgAsync("RegisterAsync(Request_RegisterDTO data)", JsonConvert.SerializeObject(data), ex);
                return this.InternalServerError();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(Request_LoginDTO data)
        {
            var result = await _userService.LoginAsync(data);
            return result.ToActionResult();
        }
    }
}
