using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{
    public interface IPostHistoryRepository : IRepository<PostHistory>
    {

    }
    public class PostHistoryRepository : RepositoryBase<PostHistory>, IPostHistoryRepository
    {
        public PostHistoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
