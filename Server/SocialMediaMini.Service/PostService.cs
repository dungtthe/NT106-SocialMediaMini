using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SocialMediaMini.DataAccess.Infrastructure;
using SocialMediaMini.DataAccess.Models;
using SocialMediaMini.DataAccess.Repositories;
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
        private readonly IPostRepository _postRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IPostRepository postRepository, IAppUserRepository userRepository, ICommentRepository commentRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Respone_PostDetail.Post>> GetFriendPostsAsync(long userId)
        {
            var fUser = await _userRepository.GetSingleByIdAsync(userId);
            if (fUser == null)
                return null;
            //bạn bè của user
            var friendIds = JsonConvert.DeserializeObject<JArray>(fUser.FriendIds).ToObject<List<long>>();
            List<Respone_PostDetail.Post> result = new List<Respone_PostDetail.Post>();
            foreach (var friendId in friendIds)
            {
                //người đăng bài
                var fFriend = await _userRepository.GetSingleByIdAsync(friendId);
                if (fFriend == null)
                    continue;
                //lấy bài viết của bạn bè
                var fPosts = await _postRepository.FindAsync(p => p.UserId == friendId && !p.IsDeleted && p.PostType == PostType.BAI_VIET && p.PostVisibilityType != PostVisibilityType.Private);
                foreach (var post in fPosts)
                {
                    var imgsFriend = JsonConvert.DeserializeObject<JArray>(fFriend.Images).ToObject<List<string>>();
                    var friend = new Respone_PostDetail.User()
                    {
                        FullName = fFriend.FullName,
                        Avatar = imgsFriend[0]
                    };

                    //reaction
                    var fReaction_user_ids = JsonConvert.DeserializeObject<JArray>(post.ReactionType_UserId_Ids).ToObject<List<string>>();
                    var reactions = new List<Respone_PostDetail.Reaction>();
                    foreach (var item in fReaction_user_ids)
                    {
                        var ss = item.Split('_');
                        var fuserReact = await _userRepository.GetSingleByIdAsync(long.Parse(ss[1]));
                        if (fuserReact == null)
                        {
                            continue;
                        }
                        var imgsfuserReact = JsonConvert.DeserializeObject<JArray>(fuserReact.Images).ToObject<List<string>>();

                        reactions.Add(new Respone_PostDetail.Reaction()
                        {
                            User = new Respone_PostDetail.User()
                            {
                                FullName = fuserReact.FullName,
                                Avatar = imgsfuserReact[0]
                            },
                            ReactionType = (ReactionType)byte.Parse(ss[0])
                        });
                    }

                    //post
                    result.Add(new Respone_PostDetail.Post()
                    {
                        PostId = post.Id,
                        Content = post.Content,
                        Images = JsonConvert.DeserializeObject<JArray>(post.Images).ToObject<List<string>>(),
                        CreateAt = post.CreateAt,
                        UpdateAt = post.UpdateAt,
                        User = friend,
                        Reactions = reactions,
                        CommentCount = (await _commentRepository.FindAsync(c => c.PostId == post.Id)).Count()
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

            await _postRepository.AddAsync(post);
            await _unitOfWork.CommitAsync();
            return post.Id;
        }

        public async Task<Respone_PostDetail.Post> GetPostDetailAsync(long userRequestId, long postId)
        {
            var fUser = await _userRepository.GetSingleByIdAsync(userRequestId);
            if (fUser == null)
                return null;
            var fPost = (await _postRepository.FindAsync(p => p.Id == postId && !p.IsDeleted && p.PostType == PostType.BAI_VIET)).FirstOrDefault();
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
                fUserPost = await _userRepository.GetSingleByIdAsync(fPost.UserId);
                if (fUserPost == null)
                    return null;
            }
            else
            {
                fUserPost = fUser;
            }

            imgsUserPost = JsonConvert.DeserializeObject<List<string>>(fUserPost.Images);



            //rsp
            var userRsp = new Respone_PostDetail.User()
            {
                FullName = fUserPost.FullName,
                Avatar = imgsUserPost[0]
            };

            //reaction
            var fReaction_user_ids = JsonConvert.DeserializeObject<List<string>>(fPost.ReactionType_UserId_Ids);
            var reactions = new List<Respone_PostDetail.Reaction>();
            foreach (var item in fReaction_user_ids)
            {
                var ss = item.Split('_');
                var fuserReact = await _userRepository.GetSingleByIdAsync(long.Parse(ss[1]));
                if (fuserReact == null)
                {
                    continue;
                }
                var imgsfuserReact = JsonConvert.DeserializeObject<List<string>>(fuserReact.Images);

                reactions.Add(new Respone_PostDetail.Reaction()
                {
                    User = new Respone_PostDetail.User()
                    {
                        FullName = fuserReact.FullName,
                        Avatar = imgsfuserReact[0]
                    },
                    ReactionType = (ReactionType)byte.Parse(ss[0])
                });
            }


            var postRsp = new Respone_PostDetail.Post()
            {
                PostId = fPost.Id,
                Content = fPost.Content,
                Images = JsonConvert.DeserializeObject<List<string>>(fPost.Images),
                CreateAt = fPost.CreateAt,
                UpdateAt = fPost.UpdateAt,
                User = userRsp,
                Reactions = reactions,
                CommentCount = (await _commentRepository.FindAsync(c => c.PostId == fPost.Id)).Count()
            };

            return postRsp;
        }

        public async Task<List<Respone_PostDetail.Post>> GetMyPostsAsync(long userId)
        {
            var result = new List<Respone_PostDetail.Post>();
            var fUser = await _userRepository.GetSingleByIdAsync(userId);
            if (fUser == null)
                return null;
            var fPosts = await _postRepository.FindAsync(p => p.UserId == userId && !p.IsDeleted && p.PostType == PostType.BAI_VIET);

            var imgsUserPost = JsonConvert.DeserializeObject<List<string>>(fUser.Images);

            //rsp
            var userRsp = new Respone_PostDetail.User()
            {
                FullName = fUser.FullName,
                Avatar = imgsUserPost[0]
            };

            foreach (var post in fPosts)
            {
                //reaction
                var fReaction_user_ids = JsonConvert.DeserializeObject<List<string>>(post.ReactionType_UserId_Ids);
                var reactions = new List<Respone_PostDetail.Reaction>();
                foreach (var item in fReaction_user_ids)
                {
                    var ss = item.Split('_');
                    var fuserReact = await _userRepository.GetSingleByIdAsync(long.Parse(ss[1]));
                    if (fuserReact == null)
                    {
                        continue;
                    }
                    var imgsfuserReact = JsonConvert.DeserializeObject<List<string>>(fuserReact.Images);

                    reactions.Add(new Respone_PostDetail.Reaction()
                    {
                        User = new Respone_PostDetail.User()
                        {
                            FullName = fuserReact.FullName,
                            Avatar = imgsfuserReact[0]
                        },
                        ReactionType = (ReactionType)byte.Parse(ss[0])
                    });
                }


                var postRsp = new Respone_PostDetail.Post()
                {
                    PostId = post.Id,
                    Content = post.Content,
                    Images = JsonConvert.DeserializeObject<List<string>>(post.Images),
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt,
                    User = userRsp,
                    Reactions = reactions,
                    CommentCount = (await _commentRepository.FindAsync(c => c.PostId == post.Id)).Count()
                };


                result.Add(postRsp);
            }

            return result;
        }
    }
}
