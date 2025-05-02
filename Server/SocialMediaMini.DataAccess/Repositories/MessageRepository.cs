using Microsoft.EntityFrameworkCore;
using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<Message> GetLastMessage(long chatRoomId);
    }
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public async Task<Message> GetLastMessage(long chatRoomId)
        {
            var msg = await DbContext.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();  
            return msg;  
        }

    }
}
