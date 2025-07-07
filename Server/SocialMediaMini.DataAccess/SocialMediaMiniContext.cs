using Microsoft.EntityFrameworkCore;
using SocialMediaMini.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess
{
    public class SocialMediaMiniContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentHistory> CommentHistories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostHistory> PostHistories { get; set; }
        public DbSet<User_ChatRoom> User_ChatRooms { get; set; }

        public SocialMediaMiniContext(DbContextOptions<SocialMediaMiniContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region add default
            //AppUser
            modelBuilder.Entity<AppUser>()
                .Property(e => e.Images)
                .HasDefaultValue("[\"no_img_user.png\"]");
            modelBuilder.Entity<AppUser>()
                .Property(e => e.FriendIds)
                .HasDefaultValue("[]");
            modelBuilder.Entity<AppUser>()
                .Property(e => e.BlockIds)
                .HasDefaultValue("[]");

            //Post
            modelBuilder.Entity<Post>()
                .Property(e => e.Images)
                .HasDefaultValue("[]");

            modelBuilder.Entity<Post>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");

            //comment
            modelBuilder.Entity<Comment>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");

            //message
            modelBuilder.Entity<Message>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");
            modelBuilder.Entity<Message>()
                .Property(e => e.ReadByUserIds)
                .HasDefaultValue("[]");
            #endregion

            #region không cho phép tự xóa khóa ngoại
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                 .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
            #endregion không cho phép tự xóa khóa ngoại

            #region add khóa chính

            #endregion

            modelBuilder.Entity<User_ChatRoom>()
            .HasKey(cd => new { cd.UserId, cd.ChatRoomId });
        }

    }
}
