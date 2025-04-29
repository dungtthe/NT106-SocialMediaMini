using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {

    }
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
