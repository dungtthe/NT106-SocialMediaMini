using Microsoft.EntityFrameworkCore;
using SocialMediaMini.DataAccess.Models;
using System;

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
        public DbSet<FriendRequest> FriendRequests { get; set; }

        public SocialMediaMiniContext(DbContextOptions<SocialMediaMiniContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region add default
            // AppUser
            modelBuilder.Entity<AppUser>()
                .Property(e => e.Images)
                .HasDefaultValue("[\"no_img_user.png\"]");
            modelBuilder.Entity<AppUser>()
                .Property(e => e.FriendIds)
                .HasDefaultValue("[]");
            modelBuilder.Entity<AppUser>()
                .Property(e => e.BlockIds)
                .HasDefaultValue("[]");
            modelBuilder.Entity<AppUser>()
                .Property(e => e.Avatar)
                .HasDefaultValue("/Resources/Images/meolag.jpg")
                .HasMaxLength(255);
            modelBuilder.Entity<AppUser>()
                .Property(e => e.Status)
                .HasDefaultValue("Offline")
                .HasMaxLength(20);

            // Post
            modelBuilder.Entity<Post>()
                .Property(e => e.Images)
                .HasDefaultValue("[]");
            modelBuilder.Entity<Post>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");

            // Comment
            modelBuilder.Entity<Comment>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");

            // Message
            modelBuilder.Entity<Message>()
                .Property(e => e.ReactionType_UserId_Ids)
                .HasDefaultValue("[]");
            modelBuilder.Entity<Message>()
                .Property(e => e.ReadByUserIds)
                .HasDefaultValue("[]");

            // FriendRequest
            modelBuilder.Entity<FriendRequest>()
                .Property(e => e.Status)
                .HasDefaultValue("Pending");
            modelBuilder.Entity<FriendRequest>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FriendRequest>()
                .Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");
            #endregion

            #region không cho phép tự xóa khóa ngoại
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                 .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
            #endregion

            #region add khóa chính
            modelBuilder.Entity<User_ChatRoom>()
                .HasKey(cd => new { cd.UserId, cd.ChatRoomId });
            modelBuilder.Entity<FriendRequest>()
                .HasKey(fr => fr.Id);
            #endregion

            #region cấu hình quan hệ và chỉ số
            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany()
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FriendRequest>()
                .HasIndex(fr => new { fr.SenderId, fr.ReceiverId })
                .IsUnique();
            #endregion            
        }
    }
}