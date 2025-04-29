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

    }
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
