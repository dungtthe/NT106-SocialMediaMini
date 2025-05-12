using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.Common.Const.Type;
using SocialMediaMini.Common.DTOs.Respone;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static SocialMediaMini.Common.DTOs.Respone.Respone_GetFriendPosts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialMediaMini.Service
{
    public interface IChatRoomService
    {
        Task<List<Respone_GetConversations.Conversation>> GetConversationsAsync(long userId);
        Task<Respone_ChatRoomDetail> GetChatRoomDetailAsync(long userId, long chatRoomId);
    }
    public class ChatRoomService : IChatRoomService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUser_ChatRoomRepository _user_ChatRoomRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ChatRoomService(IMessageRepository messageRepository, IAppUserRepository appUserRepository, IUser_ChatRoomRepository user_ChatRoomRepository, INotificationRepository notificationRepository, IChatRoomRepository chatRoomRepository,IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _appUserRepository = appUserRepository;
            _user_ChatRoomRepository = user_ChatRoomRepository;
            _notificationRepository = notificationRepository;
            _chatRoomRepository = chatRoomRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Respone_ChatRoomDetail> GetChatRoomDetailAsync(long userId, long chatRoomId)
        {
            var rsp = new Respone_ChatRoomDetail();
            var fChatRoom = (await _chatRoomRepository.FindAsync(c => c.Id == chatRoomId && !c.IsDelete)).FirstOrDefault();
            if (fChatRoom == null)
                return null;


            rsp.IsGroupChat = fChatRoom.IsGroupChat;
            var listUserIds = JsonConvert.DeserializeObject<List<long>>(fChatRoom.UserIds);
            if (fChatRoom.IsGroupChat)
            {
                rsp.LeaderId = fChatRoom.LeaderId;
                rsp.Avatar = fChatRoom.Avatar;
                rsp.RoomName = fChatRoom.Name;
                rsp.CanAddMember = fChatRoom.CanAddMember;
                rsp.CanSendMessage = fChatRoom.CanSendMessage;
                rsp.CountMember = listUserIds.Count;
            }
            else
            {
                long userId2 = listUserIds.Where(u => u != userId).FirstOrDefault();
                if (userId2 == 0)
                {
                    return null;
                }

                var findUser2 = await _appUserRepository.GetSingleByIdAsync(userId2);
                if (findUser2 == null)
                {
                    return null;
                }

                var imgs = JsonConvert.DeserializeObject<List<string>>(findUser2.Images);
                rsp.Avatar = imgs[0];
                rsp.RoomName = findUser2.FullName;
            }



            //message
            var messages = new List<Respone_ChatRoomDetail.Message>();
            var fMessages = await _messageRepository.FindAsync(m => m.ChatRoomId == chatRoomId);
            List<AppUser> usersTemp = new List<AppUser>();
            foreach (var msg in fMessages)
            {
                var msgRsp = new Respone_ChatRoomDetail.Message();
                msgRsp.Id = msg.Id;
                msgRsp.Content = msg.Content;
                msgRsp.CreatedAt = msg.CreateAt.ToString("dd/MM/yyyy HH:mm:ss");

                //reaction
                var fReaction_user_ids = JsonConvert.DeserializeObject<List<string>>(msg.ReactionType_UserId_Ids);
                var reactions = new List<Respone_ChatRoomDetail.Reaction>();
                foreach (var item in fReaction_user_ids)
                {
                    var ss = item.Split('_');
                    long userIdReact = long.Parse(ss[1]);
                    if (!usersTemp.Where(u => u.Id == userIdReact).Any())
                    {
                        var fuserReact = await _appUserRepository.GetSingleByIdAsync(long.Parse(ss[1]));
                        if (fuserReact == null)
                        {
                            continue;
                        }
                        usersTemp.Add(fuserReact);
                    }

                    //add vao react
                    var appUserReact = usersTemp.Where(u => u.Id == userIdReact).FirstOrDefault();
                    var userReact = new Respone_ChatRoomDetail.User()
                    {
                        Id = userIdReact,
                        FullName = appUserReact.FullName,
                        Avatar = appUserReact.Images != null ? JsonConvert.DeserializeObject<string[]>(appUserReact.Images)[0] : "no_img_user.png"
                    };
                    reactions.Add(new Respone_ChatRoomDetail.Reaction()
                    {
                        User = userReact,
                        TypeReaction = (Type_Reaction)int.Parse(ss[0])
                    });
                }
                msgRsp.Reactions = reactions;

                if (msg.IsRevoked)
                {
                    msgRsp.Content = "Tin nhắn đã bị thu hồi";
                }
                else if (msg.IsLink)
                {
                    msgRsp.Content = "1 file đính kèm";
                }
                else
                {
                    msgRsp.Content = msg.Content;
                }
                messages.Add(msgRsp);
            }
            rsp.Messages = messages;

            //avatar read cua msg cuoi cung
            var lastMsg = fMessages.OrderByDescending(m => m.Id).FirstOrDefault();
            if (lastMsg != null)
            {
                var userReadIds = JsonConvert.DeserializeObject<List<long>>(lastMsg.ReadByUserIds);
                var avatarReads = new List<string>();
                foreach (var userIdRead in userReadIds)
                {
                    if (!usersTemp.Where(u => u.Id == userIdRead).Any())
                    {
                        var fUserRead = await _appUserRepository.GetSingleByIdAsync(userIdRead);
                        if (fUserRead == null)
                        {
                            continue;
                        }
                        usersTemp.Add(fUserRead);
                    }
                    var appUserRead = usersTemp.Where(u => u.Id == userIdRead).FirstOrDefault();
                    avatarReads.Add(appUserRead.Images != null ? JsonConvert.DeserializeObject<string[]>(appUserRead.Images)[0] : "no_img_user.png");
                }
                rsp.AvatarReads = avatarReads;
            }
            else
            {
                rsp.AvatarReads = new List<string>();
            }


            //parent
            foreach (var msg in fMessages)
            {
                if (msg.ParrentMessageId != null)
                {
                    var parentMsg = messages.Where(m => m.Id == msg.ParrentMessageId).FirstOrDefault();
                    var msgCur = messages.Where(m => m.Id == msg.Id).FirstOrDefault();
                    if (msgCur != null)
                    {
                        msgCur.Parrent = parentMsg;
                    }
                }

            }

            //notification
            foreach(var msg in fMessages)
            {
                var findNotice = await _notificationRepository.FindAsync(x => x.UserId == userId && !x.IsRead && x.Type == Type_Notification.MESSAGE && x.ReferenceId == msg.Id);
                if (findNotice != null)
                {
                    foreach (var notice in findNotice)
                    {
                        notice.IsRead = true;
                        _notificationRepository.Update(notice);
                    }
                }
            }

            //msg cuoi doc toan bo
            if (lastMsg != null)
            {
                var userReadIds = JsonConvert.DeserializeObject<List<long>>(lastMsg.ReadByUserIds);
                if (!userReadIds.Contains(userId))
                {
                    userReadIds.Add(userId);
                    lastMsg.ReadByUserIds = JsonConvert.SerializeObject(userReadIds);
                    _messageRepository.Update(lastMsg);
                }
            }
            await _unitOfWork.CommitAsync();
            return rsp;
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
                    rspItem.Avatar = user_chatRoom.ChatRoom.Avatar;
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
                else if (lastMesg.IsLink)
                {
                    rspItem.LastMessage = $"1 file đính kèm";
                }
                else
                {
                    rspItem.LastMessage = lastMesg.Content;
                }

                rspItem.LastTime = lastMesg.CreateAt;
                var findNotice = await _notificationRepository.FindAsync(x => x.UserId == userId && !x.IsRead && x.Type == Type_Notification.MESSAGE && x.ReferenceId == user_chatRoom.ChatRoomId);
                rspItem.UnReadMessageCount = findNotice.Count();
                result.Add(rspItem);
            }
            return result;

        }
    }
}
