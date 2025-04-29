using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{
    public interface IChatRoomRepository : IRepository<ChatRoom>
    {

    }
    public class ChatRoomRepository : RepositoryBase<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
