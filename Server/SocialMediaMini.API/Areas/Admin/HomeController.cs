using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaMini.Common.Helpers;
using SocialMediaMini.DataAccess;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.Shared.Const.Type;

namespace SocialMediaMini.API.Areas.Admin
{
    [Area("Home")]
    [Route("api/admin")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly SocialMediaMiniContext _dbContext;
        public HomeController(SocialMediaMiniContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("/seed")]
        public async Task<IActionResult> Get()
        {

            //user
            var users = new List<AppUser>()
            {
                //1
                new AppUser()
                {
                    UserName = "thedung",
                    Password = SecurityHelper.HashPassword("123456"),
                    FullName = "Thế Dũng",
                    Email = "thedung@gmail.com",
                    PhoneNumber = "0123456789",
                    FriendIds = "[2,3,4,5]"
                },
                //2
                new AppUser()
                {
                    UserName = "nguyenvana",
                    Password = SecurityHelper.HashPassword("123456"),
                    FullName = "Nguyễn Văn A",
                    Email = "a@gmail.com",
                    PhoneNumber = "0123456788",
                    FriendIds = "[1,3,4,5]"
                },
                //3
                new AppUser()
                {
                    UserName = "nguyenvanb",
                    Password = SecurityHelper.HashPassword("123456"),
                    FullName = "Nguyễn Văn B",
                    Email = "b@gmail.com",
                    PhoneNumber = "0123456787",
                    FriendIds = "[1,2,4,5]"
                },
                //4
                new AppUser()
                {
                    UserName = "nguyenvanc",
                    Password = SecurityHelper.HashPassword("123456"),
                    FullName = "Nguyễn Văn C",
                    Email = "c@gmail.com",
                    PhoneNumber = "0123456786",
                     FriendIds = "[1,2,3,5]"
                },
                //5
                new AppUser()
                {
                    UserName = "nguyenvand",
                    Password = SecurityHelper.HashPassword("123456"),
                    FullName = "Nguyễn Văn D",
                    Email = "d@gmail.com",
                    PhoneNumber = "0123456785",
                     FriendIds = "[1,2,4,3]"
                },
            };
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //chatroom
            var chatRooms = new List<ChatRoom>()
            {
                //1
                //1-2
                new ChatRoom()
                {
                    UserIds = "[1,2]",
                }
                ,
                //2
                //1-3
                new ChatRoom()
                {
                    UserIds = "[1,3]",
                },
                //3
                //1-4
                new ChatRoom()
                {
                    UserIds = "[1,4]",
                },
                //4
                //1-5
                new ChatRoom()
                {
                    UserIds = "[1,5]",
                },
                //5
                //1-2-3
                new ChatRoom()
                {
                    UserIds = "[1,2,3]",
                    LeaderId = 1,
                    Name = "Ba anh em",
                    IsGroupChat = true,
                    CanAddMember = true,
                    CanSendMessage = true,
                }
            };
            await _dbContext.ChatRooms.AddRangeAsync(chatRooms);
            await _dbContext.SaveChangesAsync();
            //userchatroom
            var user_chatrooms = new List<User_ChatRoom>()
            {
                new User_ChatRoom()
                {
                    UserId = 1,
                    ChatRoomId =1
                },
                new User_ChatRoom()
                {
                    UserId = 2,
                    ChatRoomId =1
                },

                //
                new User_ChatRoom()
                {
                    UserId = 1,
                    ChatRoomId =2
                },
                new User_ChatRoom()
                {
                    UserId = 3,
                    ChatRoomId =2
                },
                //
                new User_ChatRoom()
                {
                    UserId = 1,
                    ChatRoomId =3
                },
                new User_ChatRoom()
                {
                    UserId = 4,
                    ChatRoomId =3
                },
                //
                new User_ChatRoom()
                {
                    UserId = 1,
                    ChatRoomId =4
                },
                new User_ChatRoom()
                {
                    UserId = 5,
                    ChatRoomId =4
                },
                //
                new User_ChatRoom()
                {
                    UserId = 1,
                    ChatRoomId =5
                },
                new User_ChatRoom()
                {
                    UserId = 2,
                    ChatRoomId =5
                },
                new User_ChatRoom()
                {
                    UserId = 3,
                    ChatRoomId =5
                }
            };
            await _dbContext.User_ChatRooms.AddRangeAsync(user_chatrooms);


            Random ran = new Random();
            //message
            var messageAdds = new List<Message>();
            chatRooms = await _dbContext.ChatRooms.ToListAsync();
            foreach (var chatRoom in chatRooms)
            {
                var sizeMsgRan = ran.Next(5, 10);
                var userIds = chatRoom.GetUserIds();
                for (int i = 1; i <= sizeMsgRan; i++)
                {
                    messageAdds.Add(new Message()
                    {
                        Content = "Tin nhắn seed " + i,
                        CreateAt = DateTime.Now,
                        UserId = userIds[ran.Next(0, userIds.Count)],
                        ChatRoomId = chatRoom.Id,
                        MessageType = MessageType.Text,
                        ChatRoom = chatRoom,
                    });
                }
            }
            //reply
            var temps = messageAdds.ToList();
            foreach (var msg in temps)
            {
                var sizeMsgRan = ran.Next(0, 2);
                var userIds = msg.ChatRoom.GetUserIds();
                for (int i = 1; i <= sizeMsgRan; i++)
                {
                    messageAdds.Add(new Message()
                    {
                        Content = "Tin nhắn trả lời " + msg.Content,
                        CreateAt = DateTime.Now,
                        UserId = userIds[ran.Next(0, userIds.Count)],
                        ChatRoomId = msg.ChatRoomId,
                        MessageType = MessageType.Text,
                        ParentMessage = msg
                    });
                }
            }

            await _dbContext.AddRangeAsync(messageAdds);
            await _dbContext.SaveChangesAsync();
            return Ok("seed ok");
        }


        [HttpGet("/seed2")]
        public async Task<IActionResult> Get2()
        {
            var fpost = await _dbContext.Posts.FindAsync((long)1);
            for(int i = 0; i < 15; i++)
            {
                await _dbContext.AddAsync(new Comment()
                {
                    Content = "Comment seed "+i,
                    CreateAt= DateTime.Now,
                    UserId = fpost.UserId,
                    PostId = fpost.Id,
                    ReactionType_UserId_Ids = "[]"
                });
            }
            await _dbContext.SaveChangesAsync();
            return Ok("seed ok");
        }
    }
}
