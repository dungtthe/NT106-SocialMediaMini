using Client.Const;
using Client.Helpers;
using Client.Models.Respone;
using Client.ViewModels.Posts;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
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
        public static async Task<ObservableCollection<PostViewModel.ItemPostViewModel>> GetFriendPostsAsync()
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/post/friend-post", true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var list = JsonConvert.DeserializeObject<List<Respone_GetFriendPosts.PostDTO>>(response.ResponseBody);
                    if (list != null)
                    {
                        var result = list.Select(dto => new PostViewModel.ItemPostViewModel
                        {
                            PostId = dto.PostId,
                            Content = dto.Content,
                            Images = new ObservableCollection<string>(dto.Images ?? new List<string>()),
                            CreateAt = dto.CreateAt,
                            UpdateAt = dto.UpdateAt,
                            CommentCount = dto.CommentCount,
                            User = new PostViewModel.ItemUserViewModel
                            {
                                FullName = dto.User?.FullName,
                                Avatar = dto.User?.Avatar
                            },
                            Reactions = new ObservableCollection<PostViewModel.ItemReactionViewModel>(
                                dto.Reactions?.Select(r => new PostViewModel.ItemReactionViewModel
                                {
                                    TypeReaction = r.TypeReaction,
                                    User = new PostViewModel.ItemUserViewModel
                                    {
                                        FullName = r.User?.FullName,
                                        Avatar = r.User?.Avatar
                                    }
                                }) ?? new List<PostViewModel.ItemReactionViewModel>())
                        });

                        return new ObservableCollection<PostViewModel.ItemPostViewModel>(result); // ✅ fix lỗi ở đây
                    }
                }
            }
            catch
            {
                // Log nếu cần
            }

            return null;
        }

    }
}
