using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.Common.ResultPattern;
using SocialMediaMini.DataAccess;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SocialMediaMini.Service
{
    public interface IChatRoomService
    {
        Task<List<Respone_GetConversations.Conversation>> GetConversationsAsync(long userId);
        Task<Respone_ChatRoomDetail> GetChatRoomDetailAsync(long userId, long chatRoomId);
        Task<Tuple<List<long>, Respone_NotificationDTO.Respone_NotificationMessage>> AddMessageAsync(Request_AddMessageDTO data);
        Task ReadMessages(long userId, long chatRoomId);
        Task<Result<Respone_NotificationDTO.Respone_NotificationMessage>> RevokeMessageAsync(long userId, Request_RevokeMessage data);
    }
    public class ChatRoomService : IChatRoomService
    {
        private readonly SocialMediaMiniContext _dbContext;
        public ChatRoomService(SocialMediaMiniContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<long>, Respone_NotificationDTO.Respone_NotificationMessage>> AddMessageAsync(Request_AddMessageDTO data)
        {
            var result = new Tuple<List<long>, Respone_NotificationDTO.Respone_NotificationMessage>(
                new List<long>(),
                new Respone_NotificationDTO.Respone_NotificationMessage()
            );

            result.Item1.Add(data.UserId);

            if (string.IsNullOrEmpty(data.Content)) return null;

            var fSender = await _dbContext.Users.FindAsync(data.UserId);
            if (fSender == null)
            {
                return null;
            }
            var chatRoom = await _dbContext.ChatRooms.FindAsync(data.ChatRoomId);
            if (chatRoom == null || chatRoom.IsDelete)
            {
                return null;
            }
            if (chatRoom.IsGroupChat && !chatRoom.CanSendMessage)
            {
                return null;
            }

            var msg = new Message()
            {
                Content = data.Content,
                CreateAt = DateTime.Now,
                UserId = data.UserId,
                ParrentMessageId = data.ParrentMessageId,
                ChatRoomId = chatRoom.Id,
                ReactionType_UserId_Ids = "[]",
                ReadByUserIds = $"[{data.UserId}]",
                MessageType = MessageType.Text
            };

            await _dbContext.Messages.AddAsync(msg);


            var userIds = chatRoom.GetUserIds();
            foreach (var userId in userIds)
            {
                if (userId != data.UserId)
                {
                    await _dbContext.Notifications.AddAsync(new Notification()
                    {
                        UserId = userId,
                        NotificationType = SocialMediaMini.Shared.Const.Type.NotificationType.MESSAGE,
                        ReferenceId = chatRoom.Id,
                        Content = "Tin nhắn từ " + fSender.FullName,
                        IsRead = false,
                        CreateAt = DateTime.Now,
                    });
                    result.Item1.Add(userId);
                }
            }


            DataAccess.Models.Message parrentMsg = null;
            DataAccess.Models.AppUser parrentMsgSender = null;
            if (msg.ParrentMessageId != null)
            {
                parrentMsg = await _dbContext.Messages.FindAsync(msg.ParrentMessageId ?? -1);
                if (parrentMsg == null)
                {
                    return null;
                }

                parrentMsgSender = await _dbContext.Users.FindAsync(parrentMsg.UserId);
                if (parrentMsgSender == null)
                {
                    return null;
                }
            }


            await _dbContext.SaveChangesAsync();

            var nUser = new UserDto()
            {
                Id = fSender.Id,
                Avatar = fSender.GetFirstImage(),
                FullName = fSender.FullName
            };



            var nNotiMessage = new Respone_NotificationDTO.Respone_NotificationMessage()
            {
                ChatRoomId = chatRoom.Id,
                Id = msg.Id,
                Content = msg.Content,
                CreatedAt = msg.CreateAt.ToString("dd/MM/yyyy HH:mm:ss"),
                Sender = nUser,
            };
            if (parrentMsg == null)
            {
                nNotiMessage.Parrent = null;
            }
            else
            {
                nNotiMessage.Parrent = new Respone_NotificationDTO.Respone_NotificationMessage()
                {
                    ChatRoomId = chatRoom.Id,
                    Id = parrentMsg.Id,
                    Content = parrentMsg.Content,
                    CreatedAt = parrentMsg.CreateAt.ToString("dd/MM/yyyy HH:mm:ss"),
                    Sender = new UserDto()
                    {
                        Id = parrentMsgSender.Id,
                        FullName = parrentMsgSender.FullName,
                        Avatar = parrentMsgSender.GetFirstImage()
                    }
                };
            }


            nNotiMessage.NotificationType = NotificationType.MESSAGE;
            nNotiMessage.Message = "Tin nhắn mới";
            result = new Tuple<List<long>, Respone_NotificationDTO.Respone_NotificationMessage>(
                result.Item1,
                nNotiMessage
            );
            return result;
        }

        public async Task<Respone_ChatRoomDetail> GetChatRoomDetailAsync(long userId, long chatRoomId)
        {
            var rsp = new Respone_ChatRoomDetail();
            rsp.ChatRoomId = chatRoomId;
            var fChatRoom = (await _dbContext.ChatRooms.FirstOrDefaultAsync(c => c.Id == chatRoomId && !c.IsDelete));
            if (fChatRoom == null)
                return null;


            rsp.IsGroupChat = fChatRoom.IsGroupChat;
            var listUserIds = fChatRoom.GetUserIds();
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

                var findUser2 = await _dbContext.Users.FindAsync(userId2);
                if (findUser2 == null)
                {
                    return null;
                }

                rsp.Avatar = findUser2.GetFirstImage();
                rsp.RoomName = findUser2.FullName;
            }



            //message
            var messages = new List<Respone_ChatRoomDetail.Message>();
            var fMessages = await _dbContext.Messages.Where(m => m.ChatRoomId == chatRoomId).ToListAsync();
            List<AppUser> usersTemp = new List<AppUser>();
            foreach (var msg in fMessages)
            {
                var msgRsp = new Respone_ChatRoomDetail.Message();
                msgRsp.Id = msg.Id;
                msgRsp.Content = msg.Content;
                msgRsp.CreatedAt = msg.CreateAt.ToString("dd/MM/yyyy HH:mm:ss");

                //reaction
                var reactions = new List<ReactionDto>();
                var fReaction_user_ids = msg.GetReactionAndUserIds();
                foreach (var item in fReaction_user_ids)
                {
                    long userIdReact = item.Item2;
                    if (!usersTemp.Where(u => u.Id == userIdReact).Any())
                    {
                        var fuserReact = await _dbContext.Users.FindAsync(userIdReact);
                        if (fuserReact == null)
                        {
                            continue;
                        }
                        usersTemp.Add(fuserReact);
                    }

                    //add vao react
                    var appUserReact = usersTemp.Where(u => u.Id == userIdReact).FirstOrDefault();
                    var userReact = new UserDto()
                    {
                        Id = userIdReact,
                        FullName = appUserReact.FullName,
                        Avatar = appUserReact.GetFirstImage()
                    };
                    reactions.Add(new ReactionDto()
                    {
                        User = userReact,
                        ReactionType = item.Item1
                    });
                }
                msgRsp.Reactions = reactions;

                if (msg.MessageType == MessageType.Revoked)
                {
                    msgRsp.Content = "Tin nhắn đã bị thu hồi";
                }
                else
                {
                    msgRsp.Content = msg.Content;
                }

                if (!usersTemp.Where(u => u.Id == msg.UserId).Any())
                {
                    var fSender = await _dbContext.Users.FindAsync(msg.UserId);
                    if (fSender == null)
                    {
                        continue;
                    }
                    usersTemp.Add(fSender);
                }

                var sender = usersTemp.Where(u => u.Id == msg.UserId).FirstOrDefault();
                msgRsp.Sender = new UserDto()
                {
                    Id = msg.UserId,
                    FullName = sender.FullName,
                    Avatar = sender.GetFirstImage()
                };
                messages.Add(msgRsp);
            }
            rsp.Messages = messages;

            //avatar read cua msg cuoi cung
            var lastMsg = fMessages.OrderByDescending(m => m.Id).FirstOrDefault();
            if (lastMsg != null)
            {
                var userReadIds = lastMsg.GetUserIdsRead();
                var avatarReads = new List<string>();
                foreach (var userIdRead in userReadIds)
                {
                    if (!usersTemp.Where(u => u.Id == userIdRead).Any())
                    {
                        var fUserRead = await _dbContext.Users.FindAsync(userIdRead);
                        if (fUserRead == null)
                        {
                            continue;
                        }
                        usersTemp.Add(fUserRead);
                    }
                    var appUserRead = usersTemp.Where(u => u.Id == userIdRead).FirstOrDefault();
                    avatarReads.Add(appUserRead.GetFirstImage());
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
            foreach (var msg in fMessages)
            {
                var findNotice = await _dbContext.Notifications.Where(x => x.UserId == userId && !x.IsRead && x.NotificationType == NotificationType.MESSAGE && x.ReferenceId == chatRoomId).ToListAsync();
                if (findNotice != null)
                {
                    foreach (var notice in findNotice)
                    {
                        notice.IsRead = true;
                        _dbContext.Notifications.Update(notice);
                    }
                }
            }

            //msg cuoi doc toan bo
            if (lastMsg != null)
            {
                if (lastMsg.AddUserIdRead(userId))
                {
                    _dbContext.Messages.Update(lastMsg);
                }
            }
            await _dbContext.SaveChangesAsync();
            return rsp;
        }

        public async Task<List<Respone_GetConversations.Conversation>> GetConversationsAsync(long userId)
        {
            var result = new List<Respone_GetConversations.Conversation>();
            var findUser_ChatRoom = await _dbContext.User_ChatRooms.Include(x => x.ChatRoom).Where(x => x.UserId == userId && !x.IsLeft && !x.ChatRoom.IsDelete).ToListAsync();
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
                    var userIds = user_chatRoom.ChatRoom.GetUserIds();
                    foreach (var uId in userIds)
                    {
                        if (uId != userId)
                        {
                            var fUser2 = await _dbContext.Users.FindAsync(uId);
                            if (fUser2 != null)
                            {
                                rspItem.RoomName = fUser2.FullName;
                                rspItem.Avatar = fUser2.GetFirstImage();
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
                var lastMesg = await GetLastMessage(user_chatRoom.ChatRoomId);
                if (lastMesg == null)
                {
                    continue;
                }
                if (lastMesg.MessageType == MessageType.Revoked)
                {
                    rspItem.LastMessage = "Tin nhắn đã bị thu hồi";
                }
                else
                {
                    rspItem.LastMessage = lastMesg.Content;
                }

                rspItem.LastTime = lastMesg.CreateAt;
                var findNotice = await _dbContext.Notifications.Where(x => x.UserId == userId && !x.IsRead && x.NotificationType == NotificationType.MESSAGE && x.ReferenceId == user_chatRoom.ChatRoomId).ToListAsync();
                rspItem.UnReadMessageCount = findNotice.Count();
                result.Add(rspItem);
            }
            return result;

        }

        public async Task ReadMessages(long userId, long chatRoomId)
        {
            var lastMsg = await GetLastMessage(chatRoomId);
            if (lastMsg == null)
            {
                return;
            }
            lastMsg.AddUserIdRead(userId);
            var fNotices = await _dbContext.Notifications.Where(n => n.NotificationType == NotificationType.MESSAGE && !n.IsRead && n.UserId == userId).ToListAsync();
            foreach (var notification in fNotices)
            {
                notification.IsRead = true;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Result<Respone_NotificationDTO.Respone_NotificationMessage>> RevokeMessageAsync(long userId, Request_RevokeMessage data)
        {
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
            {
                return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }
            var fMessage = await _dbContext.Messages.FirstOrDefaultAsync(m => m.UserId == userId && m.Id == data.MessageId && m.ChatRoomId == data.ChatRoomId && m.MessageType != MessageType.Revoked);
            if (fMessage == null)
            {
                return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }
            if (fMessage.ChatRoom.IsGroupChat)
            {
                if (!fMessage.ChatRoom.CanSendMessage)
                {
                    return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.Forbidden, "Trưởng nhóm đã tắt tính năng nhắn tin");
                }
            }
            else
            {
                

                var listUserIds = fMessage.ChatRoom.GetUserIds();
                long ?userId2 = listUserIds.Where(u => u != userId).FirstOrDefault();
                if(userId2 == null)
                {
                    return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
                }

                //check block
                var listBlockUser1 = fUser.GetBlockIds();
                if(listBlockUser1.Contains(userId2.Value))
                {
                    return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.Forbidden, "Bạn và người này đã chặn nhau nên không thể sử dụng chức năng này!");
                }

                var fUser2 = await _dbContext.Users.FindAsync(userId2.Value);
                if (fUser2 == null)
                {
                    return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
                }

                //check block
                if (fUser2.GetBlockIds().Contains(userId))
                {
                    return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Failure(HttpStatusCode.Forbidden, "Bạn và người này đã chặn nhau nên không thể sử dụng chức năng này!");
                }
            }


            //ok
            fMessage.MessageType = MessageType.Revoked;
            await _dbContext.SaveChangesAsync();

            var dataRsp = new Respone_NotificationDTO.Respone_NotificationMessage()
            {
                ChatRoomId = fMessage.ChatRoomId,
                Id = fMessage.Id,
                Content = "Tin nhắn đã bị thu hồi",
                CreatedAt = fMessage.CreateAt.ToString("dd/MM/yyyy HH:mm:ss"),
                Sender = new UserDto()
                {
                    Id = fMessage.Id,
                    FullName = fMessage.User.FullName,
                    Avatar = fMessage.User.GetFirstImage(),
                }

            };
            if(fMessage.ParrentMessageId != null)
            {
                dataRsp.Parrent = new Respone_NotificationDTO.Respone_NotificationMessage()
                {
                    ChatRoomId = fMessage.ChatRoomId,
                    Id = fMessage.ParentMessage.Id,
                    Content = fMessage.ParentMessage.Content,
                    CreatedAt = fMessage.ParentMessage.CreateAt.ToString("dd/MM/yyyy HH:mm:ss"),
                    Sender = new UserDto()
                    {
                        Id = fMessage.ParentMessage.Id,
                        FullName = fMessage.ParentMessage.User.FullName,
                        Avatar = fMessage.ParentMessage.User.GetFirstImage(),
                    }
                };
            }

            return Result<Respone_NotificationDTO.Respone_NotificationMessage>.Success(dataRsp);
        }

        private async Task<Message> GetLastMessage(long chatRoomId)
        {
            return await _dbContext.Messages
               .Where(m => m.ChatRoomId == chatRoomId)
               .OrderByDescending(m => m.Id)
               .FirstOrDefaultAsync();
        }
    }
}
