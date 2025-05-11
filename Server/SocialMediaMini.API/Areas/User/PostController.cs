using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaMini.API.Extensions;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.Service;

namespace SocialMediaMini.API.Areas.User
{
    [Area("User")]
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        [HttpGet("friend-post")]
        public async Task<IActionResult> GetFriendPostsAsync()
        {
            try
            {
                var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
                long userId = 0;
                if (suserId == null || !long.TryParse(suserId, out userId))
                {
                    return Unauthorized();
                }
                var rsp = await _postService.GetFriendPostsAsync(userId);
                return Ok(rsp);
            }
            catch (Exception ex)
            {
                await LoggerHelper.LogMsgAsync("GetFriendPostsAsync()", "", ex);
                return this.InternalServerError();
            }
        }
    }
}
