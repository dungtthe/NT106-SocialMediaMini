using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{
    public interface IUser_ChatRoomRepository : IRepository<User_ChatRoom>
    {

    }
    public class User_ChatRoomRepository : RepositoryBase<User_ChatRoom>, IUser_ChatRoomRepository
    {
        public User_ChatRoomRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
