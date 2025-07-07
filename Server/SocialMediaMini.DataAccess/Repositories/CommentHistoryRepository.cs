using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{

    public interface ICommentHistoryRepository : IRepository<CommentHistory>
    {

    }
    public class CommentHistoryRepository : RepositoryBase<CommentHistory>, ICommentHistoryRepository
    {
        public CommentHistoryRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
