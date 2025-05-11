using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMediaMini.Common.Const.Type;
using SocialMediaMini.Common.DTOs.Respone;
using SocialMediaMini.DataAccess.Repositories;
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
        Task<List<Respone_GetFriendPosts.Post>> GetFriendPostsAsync(long userId);
    }
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IAppUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;
        public PostService(IPostRepository postRepository, IAppUserRepository userRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }
        public async Task<List<Respone_GetFriendPosts.Post>> GetFriendPostsAsync(long userId)
        {
            var fUser = await _userRepository.GetSingleByIdAsync(userId);
            if (fUser == null)
                return null;
            //bạn bè của user
            var friendIds = JsonConvert.DeserializeObject<JArray>(fUser.FriendIds).ToObject<List<long>>();
            List<Respone_GetFriendPosts.Post> result = new List<Respone_GetFriendPosts.Post>();
            foreach (var friendId in friendIds)
            {
                //người đăng bài
                var fFriend = await _userRepository.GetSingleByIdAsync(friendId);
                if (fFriend == null)
                    continue;
                //lấy bài viết của bạn bè
                var fPosts = await _postRepository.FindAsync(p => p.UserId == friendId && !p.IsDeleted && p.PostType == Type_Post.BAI_VIET && p.PostVisibility!=(byte)Type_PostVisibility.Private );
                foreach (var post in fPosts)
                {
                    var imgsFriend = JsonConvert.DeserializeObject<JArray>(fFriend.Images).ToObject<List<string>>();
                    var friend = new Respone_GetFriendPosts.User()
                    {
                        FullName = fFriend.FullName,
                        Avatar = imgsFriend[0]
                    };

                    //reaction
                    var fReaction_user_ids = JsonConvert.DeserializeObject<JArray>(post.ReactionType_UserId_Ids).ToObject<List<string>>();
                    var reactions = new List<Respone_GetFriendPosts.Reaction>();
                    foreach (var item in fReaction_user_ids)
                    {
                        var ss = item.Split('_');
                        var fuserReact = await _userRepository.GetSingleByIdAsync(long.Parse(ss[1]));
                        if (fuserReact == null)
                        {
                            continue;
                        }
                        var imgsfuserReact = JsonConvert.DeserializeObject<JArray>(fuserReact.Images).ToObject<List<string>>();

                        reactions.Add(new Respone_GetFriendPosts.Reaction()
                        {
                            User = new Respone_GetFriendPosts.User()
                            {
                                FullName = fuserReact.FullName,
                                Avatar = imgsfuserReact[0]
                            },
                            TypeReaction = (Type_Reaction)int.Parse(ss[0])
                        });
                    }

                    //post
                    result.Add(new Respone_GetFriendPosts.Post()
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
    }
}
