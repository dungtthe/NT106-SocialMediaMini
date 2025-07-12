using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMediaMini.API.Extensions;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.Service;
using SocialMediaMini.Shared.Dto.Request;
using System.Net.Http;

namespace SocialMediaMini.API.Areas.User
{
    [Area("User")]
    [Route("api/chat-room")]
    [ApiController]
    [Authorize]
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRoomService _chatRoomService;
        public ChatRoomController(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversationsAsync()
        {
            try
            {
                var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
                long userId = 0;
                if(suserId==null || !long.TryParse(suserId, out userId))
                {
                    return Unauthorized();
                }

                var rsp = await _chatRoomService.GetConversationsAsync(userId);
                return Ok(rsp);
            }
            catch (Exception ex)
            {
                //await LoggerHelper.LogMsgAsync("GetConversationsAsync()", "", ex);
                return this.InternalServerError();
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetChatRoomDetailAsync(long id)
        {
            try
            {
                var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
                long userId = 0;
                if(suserId==null || !long.TryParse(suserId, out userId))
                {
                    return Unauthorized();
                }
                var rsp = await _chatRoomService.GetChatRoomDetailAsync(userId,id);
                return Ok(rsp);
            }
            catch (Exception ex)
            {
                //await LoggerHelper.LogMsgAsync("GetConversationsAsync()", "", ex);
                return this.InternalServerError();
            }
        }


        [HttpGet("readmsgs/{id}")]
        public async Task<IActionResult> ReadMessages(long id)
        {
            try
            {
                var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
                long userId = 0;
                if (suserId == null || !long.TryParse(suserId, out userId))
                {
                    return Unauthorized();
                }
                await _chatRoomService.ReadMessages(userId, id);
                return Ok();
            }
            catch (Exception ex)
            {
                //await LoggerHelper.LogMsgAsync("ReadMessages(long chatRoomId)", "", ex);
                return this.InternalServerError();
            }
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroupChatAsync(Request_CreateGroupchat request)
        {
            var suserId = this.HttpContext.User.FindFirst("UserId")?.Value;
            long userId = 0;
            if (suserId == null || !long.TryParse(suserId, out userId))
            {
                return Unauthorized();
            }
            var rs = await _chatRoomService.CreateGroupChatAsync(userId, request);
            return rs.ToActionResult();
        }
    }
}
