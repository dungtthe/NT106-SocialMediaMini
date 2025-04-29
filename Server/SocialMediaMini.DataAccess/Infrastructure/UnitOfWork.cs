using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Infrastructure
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private SocialMediaMiniContext dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }
        public SocialMediaMiniContext DbContext => dbContext ?? (dbContext = dbFactory.Init());

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

    }
}
