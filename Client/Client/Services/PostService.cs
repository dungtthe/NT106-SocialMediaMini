using Client.Const;
using Client.Helpers;
using Client.LocalStorage;
using Client.ViewModels;
using Client.ViewModels.Posts;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ViewModels.Chats.ConversationViewModel;

namespace Client.Services
{
    public static class PostService
    {
        public static async Task<List<Respone_PostDetail.Post>> GetFriendPostsAsync()
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/post/friend-post", true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var list = JsonConvert.DeserializeObject<List<Respone_PostDetail.Post>>(response.ResponseBody);
                    return list;
                }
            }
            catch
            {
                // Log nếu cần
            }

            return null;
        }
         
        public static async Task<List<Respone_PostDetail.Post>> GetMyPostsAsync()
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/post/myposts", true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var list = JsonConvert.DeserializeObject<List<Respone_PostDetail.Post>>(response.ResponseBody);
                    return list;
                }
            }
            catch
            {
                // Log nếu cần
            }

            return null;
        }


        public static async Task<long?> AddPostAsync(Request_AddPostDTO data)
        {
            try
            {
                var response = await ApiHelpers.PostAsync(new ApiRequest("/api/post/add", JsonConvert.SerializeObject(data), true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var idPostNew = JsonConvert.DeserializeObject<long?>(response.ResponseBody);
                    if (idPostNew != null)
                    {
                        return idPostNew;
                    }
                }
            }
            catch { }

            return null;
        }

        public static async Task<Respone_PostDetail.Post> GetPostDetailAsync(long postId)
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/post/detail/" + postId, true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var data = JsonConvert.DeserializeObject<Respone_PostDetail.Post>(response.ResponseBody);
                    return data;
                }
            }
            catch
            {
                // Log nếu cần
            }

            return null;
        }

        public static async Task<Respone_ReactOrUnReactPostDto> ReactOrUnReactPost(long postId, ReactionType reactionType)
        {
            try
            {
                var data = new Request_ReactOrUnReactPostDto()
                {
                    PostId = postId,
                    ReactionType = reactionType
                };
                var response = await ApiHelpers.PatchAsync(new ApiRequest("/api/post/react", JsonConvert.SerializeObject(data),true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    return JsonConvert.DeserializeObject<Respone_ReactOrUnReactPostDto>(response.ResponseBody);
                }

                ToastManager.AddToast(Const.Type.ToastType.Error, response.ResponseBody);
            }
            catch
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, "Có lỗi xảy ra. Vui lòng thử lại sau");
            }
            return null;
        }



        public static async Task <List<CommentDto>> GetCommentsByPostIdAsync(long postId)
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/post/comment/" + postId, true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var data = JsonConvert.DeserializeObject<List<CommentDto>>(response.ResponseBody);
                    return data;
                }
            }
            catch
            {
                
            }
            return null;
        }
    }
}
