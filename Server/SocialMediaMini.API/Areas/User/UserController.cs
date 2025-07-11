using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMediaMini.API.Extensions;
using SocialMediaMini.Common.Helpers;
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
                return this.ResponeMessageResult(rsp);
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
                        userId = rsp.UserId,
                        token = rsp.Token,
                        fullName = rsp.FullName,
                        image = rsp.Image
                    });
                }
            }
            catch (Exception ex)
            {
                //await LoggerHelper.LogMsgAsync("LoginAsync(Request_LoginDTO data)", JsonConvert.SerializeObject(data), ex);
                return this.InternalServerError();
            }
        }


    }
}
