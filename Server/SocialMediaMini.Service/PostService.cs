using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.Common.ResultPattern;
using SocialMediaMini.DataAccess;
using SocialMediaMini.Shared.Const;
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
        Task<Result<Respone_ReactOrUnReactPostDto>> ReactOrUnReactPostAsync(long userId, Request_ReactOrUnReactPostDto request);
        Task<Result<List<CommentDto>>> GetCommentsAsync(long userId, long postId);
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
                    var friend = new UserDto()
                    {
                        Id = fFriend.Id,
                        FullName = fFriend.FullName,
                        Avatar = fFriend.GetFirstImage()
                    };

                    //reaction
                    var reactions = new List<ReactionDto>();
                    var fReaction_user_ids = post.GetReactionAndUserIds();
                    foreach (var item in fReaction_user_ids)
                    {

                        var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                        if (fuserReact == null)
                        {
                            continue;
                        }

                        reactions.Add(new ReactionDto()
                        {
                            User = new UserDto()
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
            var userRsp = new UserDto()
            {
                Id = fUser.Id,
                FullName = fUserPost.FullName,
                Avatar = fUserPost.GetFirstImage()
            };

            //reaction
            var reactions = new List<ReactionDto>();
            var fReaction_user_ids = fPost.GetReactionAndUserIds();
            foreach (var item in fReaction_user_ids)
            {
                var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                if (fuserReact == null)
                {
                    continue;
                }
                reactions.Add(new ReactionDto()
                {
                    User = new UserDto()
                    {
                        Id = fuserReact.Id,
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
            var userRsp = new UserDto()
            {
                Id = fUser.Id,
                FullName = fUser.FullName,
                Avatar = fUser.GetFirstImage()
            };

            foreach (var post in fPosts)
            {
                //reaction
                var fReaction_user_ids = post.GetReactionAndUserIds();
                var reactions = new List<ReactionDto>();
                foreach (var item in fReaction_user_ids)
                {
                    var fuserReact = await _dbContext.Users.FindAsync(item.Item2);
                    if (fuserReact == null)
                    {
                        continue;
                    }
                    reactions.Add(new ReactionDto()
                    {
                        User = new UserDto()
                        {
                            Id = fuserReact.Id,
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

        public async Task<Result<Respone_ReactOrUnReactPostDto>> ReactOrUnReactPostAsync(long userId, Request_ReactOrUnReactPostDto request)
        {
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
            {
                return Result<Respone_ReactOrUnReactPostDto>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }

            var fPost = await _dbContext.Posts.FindAsync(request.PostId);
            if (fPost == null)
            {
                return Result<Respone_ReactOrUnReactPostDto>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }


            if (userId != fPost.UserId)
            {
                if (fPost.PostVisibilityType == PostVisibilityType.Private)
                {
                    return Result<Respone_ReactOrUnReactPostDto>.Failure(HttpStatusCode.Forbidden, "Có lỗi xảy ra. Vui lòng thử lại sau");
                }

                var friendIds = fPost.User.GetFriendIds();
                if (fPost.PostVisibilityType == PostVisibilityType.Friend)
                {
                    if (!friendIds.Contains(userId))
                    {
                        return Result<Respone_ReactOrUnReactPostDto>.Failure(HttpStatusCode.Forbidden, "Có lỗi xảy ra. Vui lòng thử lại sau");
                    }
                }
            }

            var reactionType = fPost.ReactOrUnReact(userId, request.ReactionType);

            if (fUser.Id != fPost.UserId && reactionType.HasValue)
            {
                await _dbContext.Notifications.AddAsync(new DataAccess.Models.Notification()
                {
                    UserId = fPost.UserId,
                    NotificationType = NotificationType.POST,
                    ReferenceId = fPost.Id,
                    Content = $"{fUser.UserName} đã thả cảm xúc tới bài viết của bạn"
                });
            }

            await _dbContext.SaveChangesAsync();

            var rs = Result<Respone_ReactOrUnReactPostDto>.Success(new Respone_ReactOrUnReactPostDto() { PostId = fPost.Id });

            if (reactionType.HasValue)
            {
                rs.Value.Reaction = new ReactionDto()
                {
                    ReactionType = reactionType.Value,
                    User = new UserDto()
                    {
                        Id = userId,
                        FullName = fUser.FullName,
                        Avatar = fUser.GetFirstImage(),
                    }
                };
            }
            return rs;
        }

        public async Task<Result<List<CommentDto>>> GetCommentsAsync(long userId, long postId)
        {
            var fUser = await _dbContext.Users.FindAsync(userId);
            if (fUser == null)
            {
                return Result<List<CommentDto>>.Failure(HttpStatusCode.NotFound, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }

            var fPost = await _dbContext.Posts.FindAsync(postId);
            if (fPost == null || (fPost.PostVisibilityType == PostVisibilityType.Private && userId != fPost.UserId))
            {
                return Result<List<CommentDto>>.Failure(HttpStatusCode.NotFound, "Không tìm thấy bài đăng.");
            }


            var commentsDto = new List<CommentDto>();
            var fComments = await _dbContext.Comments.Where(c=>c.PostId == postId).ToListAsync();
            foreach(var item in fComments)
            {
                var commentDto = new CommentDto()
                {
                    Id = item.Id,
                    Content = item.Content,
                    CreatedAt = item.CreateAt.ToString("dd/MM/yyyy HH:mm:ss"),
                    User = new UserDto()
                    {
                        Id = item.UserId,
                        FullName = item.User.FullName,
                        Avatar = item.User.GetFirstImage()
                    }
                };

                //reaction
                var fReaction_user_ids = item.GetReactionAndUserIds();
                var reactions = new List<ReactionDto>();
                foreach (var fReaction_user_id in fReaction_user_ids)
                {
                    var fuserReact = await _dbContext.Users.FindAsync(fReaction_user_id.Item2);
                    if (fuserReact == null)
                    {
                        continue;
                    }
                    reactions.Add(new ReactionDto()
                    {
                        User = new UserDto()
                        {
                            Id = fuserReact.Id,
                            FullName = fuserReact.FullName,
                            Avatar = fuserReact.GetFirstImage()
                        },
                        ReactionType = fReaction_user_id.Item1
                    });
                }
                commentDto.Reactions = reactions;

                commentsDto.Add(commentDto);
            }

            return Result<List<CommentDto>>.Success(commentsDto);
        }
    }
}
