using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Infrastructure
{
    public interface IDbFactory
    {
        SocialMediaMiniContext Init();
    }


    public class DbFactory : IDbFactory
    {
        private readonly DbContextOptions<SocialMediaMiniContext> options;
        private SocialMediaMiniContext dbContext;

        public DbFactory(DbContextOptions<SocialMediaMiniContext> options)
        {
            this.options = options;
        }

        public SocialMediaMiniContext Init()
        {
            return dbContext ??= new SocialMediaMiniContext(options);
        }
    }
}
