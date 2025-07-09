using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.DataAccess;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Service
{
    public interface IPostService
    {
        Task<List<Respone_PostDetail.Post>> GetFriendPostsAsync(long userId);
        Task<long> AddPostAsync(long userId, Request_AddPostDTO data);
        Task<Respone_PostDetail.Post> GetPostDetailAsync(long userRequestId, long postId);
        Task<List<Respone_PostDetail.Post>> GetMyPostsAsync(long userId);
    }
    public class PostService : IPostService
    {
        private readonly SocialMediaMiniContext _dbContext;
        public PostService(SocialMediaMiniContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<Respone_PostDetail.Post>> GetFriendPostsAsync(long userId)
        {
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
                return null;
            //bạn bè của user
            var friendIds = fUser.GetFriendIds();
            List<Respone_PostDetail.Post> result = new List<Respone_PostDetail.Post>();
            foreach (var friendId in friendIds)
            {
                //người đăng bài
                var fFriend = await _dbContext.Users.FindAsync(friendId);
                if (fFriend == null)
                    continue;
                //lấy bài viết của bạn bè
                var fPosts = await _dbContext.Posts.Where(p => p.UserId == friendId && !p.IsDeleted && p.PostType == PostType.BAI_VIET && p.PostVisibilityType != PostVisibilityType.Private).ToListAsync();
                foreach (var post in fPosts)
                {
                    var friend = new Respone_PostDetail.User()
                    {
                        FullName = fFriend.FullName,
                        Avatar = fFriend.GetFirstImage()
                    };

                    //reaction
                    var reactions = new List<Respone_PostDetail.Reaction>();
                    var fReaction_user_ids = post.GetReactionAndUserIds();
                    foreach (var item in fReaction_user_ids)
                    {
                       
                        var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                        if (fuserReact == null)
                        {
                            continue;
                        }

                        reactions.Add(new Respone_PostDetail.Reaction()
                        {
                            User = new Respone_PostDetail.User()
                            {
                                FullName = fuserReact.FullName,
                                Avatar = fuserReact.GetFirstImage()
                            },
                            ReactionType = item.Item1
                        });
                    }


                    //post
                    result.Add(new Respone_PostDetail.Post()
                    {
                        PostId = post.Id,
                        Content = post.Content,
                        Images = post.GetImages(),
                        CreateAt = post.CreateAt,
                        UpdateAt = post.UpdateAt,
                        User = friend,
                        Reactions = reactions,
                        CommentCount = await _dbContext.Comments.Where(c => c.PostId == post.Id).CountAsync(),
                    });
                }
            }
            return result;
        }

        public async Task<long> AddPostAsync(long userId, Request_AddPostDTO data)
        {
            var post = new DataAccess.Models.Post()
            {
                Content = data.Content,
                Images = data.Images,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                UserId = userId,
                PostType = PostType.BAI_VIET,
                PostVisibilityType = data.PostVisibilityType
            };

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            return post.Id;
        }

        public async Task<Respone_PostDetail.Post> GetPostDetailAsync(long userRequestId, long postId)
        {
            var fUser = await _dbContext.Users.FindAsync(userRequestId);
            if (fUser == null)
                return null;
            var fPost = (await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted && p.PostType == PostType.BAI_VIET));
            if (fPost == null)
            {
                return null;
            }

            if (userRequestId != fPost.UserId && fPost.PostVisibilityType == PostVisibilityType.Private)
            {
                return null;
            }

            var imgsUserPost = new List<string>();

            DataAccess.Models.AppUser fUserPost = null;
            if (userRequestId != fPost.UserId)
            {
                fUserPost = await _dbContext.Users.FindAsync(fPost.UserId);
                if (fUserPost == null)
                    return null;
            }
            else
            {
                fUserPost = fUser;
            }


            //rsp
            var userRsp = new Respone_PostDetail.User()
            {
                FullName = fUserPost.FullName,
                Avatar = fUserPost.GetFirstImage()
            };

            //reaction
            var reactions = new List<Respone_PostDetail.Reaction>();
            var fReaction_user_ids = fPost.GetReactionAndUserIds();
            foreach (var item in fReaction_user_ids)
            {
                var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                if (fuserReact == null)
                {
                    continue;
                }
                reactions.Add(new Respone_PostDetail.Reaction()
                {
                    User = new Respone_PostDetail.User()
                    {
                        FullName = fuserReact.FullName,
                        Avatar = fuserReact.GetFirstImage()
                    },
                    ReactionType = item.Item1
                });
            }


            var postRsp = new Respone_PostDetail.Post()
            {
                PostId = fPost.Id,
                Content = fPost.Content,
                Images = fPost.GetImages(),
                CreateAt = fPost.CreateAt,
                UpdateAt = fPost.UpdateAt,
                User = userRsp,
                Reactions = reactions,
                CommentCount = await _dbContext.Comments.Where(c => c.PostId == fPost.Id).CountAsync(),
            };

            return postRsp;
        }

        public async Task<List<Respone_PostDetail.Post>> GetMyPostsAsync(long userId)
        {
            var result = new List<Respone_PostDetail.Post>();
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
                return null;
            var fPosts = await _dbContext.Posts.Where(p => p.UserId == userId && !p.IsDeleted && p.PostType == PostType.BAI_VIET).ToListAsync();
            
            //rsp
            var userRsp = new Respone_PostDetail.User()
            {
                FullName = fUser.FullName,
                Avatar = fUser.GetFirstImage()
            };

            foreach (var post in fPosts)
            {
                //reaction
                var fReaction_user_ids = post.GetReactionAndUserIds();
                var reactions = new List<Respone_PostDetail.Reaction>();
                foreach (var item in fReaction_user_ids)
                {
                    var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                    if (fuserReact == null)
                    {
                        continue;
                    }
                    reactions.Add(new Respone_PostDetail.Reaction()
                    {
                        User = new Respone_PostDetail.User()
                        {
                            FullName = fuserReact.FullName,
                            Avatar = fuserReact.GetFirstImage()
                        },
                        ReactionType = item.Item1
                    });
                }

                var postRsp = new Respone_PostDetail.Post()
                {
                    PostId = post.Id,
                    Content = post.Content,
                    Images = post.GetImages(),
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt,
                    User = userRsp,
                    Reactions = reactions,
                    CommentCount = await _dbContext.Comments.Where(c => c.PostId == post.Id).CountAsync(),
                };


                result.Add(postRsp);
            }

            return result;
        }
    }
}
