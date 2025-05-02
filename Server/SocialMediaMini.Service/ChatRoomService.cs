using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.Common.Const.Type;
using SocialMediaMini.Common.DTOs.Respone;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialMediaMini.Service
{
    public interface IChatRoomService
    {
        Task<List<Respone_GetConversations.Conversation>> GetConversationsAsync(long userId);
    }
    public class ChatRoomService : IChatRoomService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUser_ChatRoomRepository _user_ChatRoomRepository;
        private readonly INotificationRepository _notificationRepository;
        public ChatRoomService(IMessageRepository messageRepository, IAppUserRepository appUserRepository, IUser_ChatRoomRepository user_ChatRoomRepository, INotificationRepository notificationRepository)
        {
            _messageRepository = messageRepository;
            _appUserRepository = appUserRepository;
            _user_ChatRoomRepository = user_ChatRoomRepository;
            _notificationRepository = notificationRepository;
        }


        public async Task<List<Respone_GetConversations.Conversation>> GetConversationsAsync(long userId)
        {
            var result = new List<Respone_GetConversations.Conversation>();
            var findUser_ChatRoom = await _user_ChatRoomRepository.FindWithIncludeAsync(x => x.UserId == userId && !x.IsLeft && !x.ChatRoom.IsDelete, x => x.ChatRoom);
            foreach (var user_chatRoom in findUser_ChatRoom)
            {
                var rspItem = new Respone_GetConversations.Conversation();
                rspItem.ChatRoomId = user_chatRoom.ChatRoomId;
                rspItem.LeaderId = user_chatRoom.ChatRoom.LeaderId;
                if (user_chatRoom.ChatRoom.IsGroupChat)
                {
                    rspItem.RoomName = user_chatRoom.ChatRoom.Name;
                    rspItem.Avatar = "no_img_group.png";
                }
                else
                {
                    var userIds = JsonConvert.DeserializeObject<JArray>(user_chatRoom.ChatRoom.UserIds).ToObject<List<long>>();
                    foreach (var uId in userIds)
                    {
                        if (uId != userId)
                        {
                            var fUser2 = await _appUserRepository.GetSingleByIdAsync(uId);
                            if (fUser2 != null)
                            {
                                rspItem.RoomName = fUser2.FullName;
                                string[] imgs = fUser2.Images != null
                                    ? JsonConvert.DeserializeObject<string[]>(fUser2.Images)
                                    : new string[] { "no_img_user.png" };
                                rspItem.Avatar = imgs[0];
                                break;
                            }
                        }
                    }
                    if (rspItem.RoomName == null)
                    {
                        continue;
                    }
                }
                rspItem.IsGroupChat = user_chatRoom.ChatRoom.IsGroupChat;
                var lastMesg = await _messageRepository.GetLastMessage(user_chatRoom.ChatRoomId);
                if (lastMesg == null)
                {
                    continue;
                }
                if (lastMesg.IsRevoked)
                {
                    rspItem.LastMessage = "Tin nhắn đã bị thu hồi";
                }
                else if(lastMesg.IsLink)
                {
                    rspItem.LastMessage = $"1 file đính kèm";
                }
                else
                {
                    rspItem.LastMessage = lastMesg.Content;
                }

                rspItem.LastTime = lastMesg.CreateAt;
                var findNotice = await _notificationRepository.FindAsync(x => x.UserId == userId && !x.IsRead && x.Type==Type_Notification.MESSAGE && x.ReferenceId==user_chatRoom.ChatRoomId);
                rspItem.UnReadMessageCount = findNotice.Count();
                result.Add(rspItem);
            }
            return result;

        }
    }
}
