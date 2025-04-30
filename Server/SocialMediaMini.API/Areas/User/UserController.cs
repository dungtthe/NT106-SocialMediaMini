using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialMediaMini.API.Areas.User
{
    [Area("User")]
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("testuser")]
        public async Task<IActionResult> Get()
        {
            await Task.CompletedTask;
            return Ok("test user");
        }
    }
}
