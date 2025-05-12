using Client.Const;
using Client.Helpers;
using Client.Models.Respone;
using Client.ViewModels.Chats;
using Client.ViewModels.Posts;
using Client.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ViewModels.Chats.ConversationViewModel.ItemChatRoomDetailViewModel;

namespace Client.Services
{
    public static class ChatRoomService
    {
        public static async Task<ConversationViewModel.ItemChatRoomDetailViewModel> GetChatRoomDetailAsync(long chatRoomId)
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet($"/api/chat-room/detail/{chatRoomId}", true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var data = JsonConvert.DeserializeObject<Respone_ChatRoomDetail>(response.ResponseBody);
                    if (data != null)
                    {
                        var viewModel = new ConversationViewModel.ItemChatRoomDetailViewModel
                        {
                            LeaderId = data.LeaderId,
                            Avatar = data.Avatar,
                            RoomName = data.RoomName,
                            IsGroupChat = data.IsGroupChat,
                            CanSendMessage = data.CanSendMessage,
                            CanAddMember = data.CanAddMember,
                            CountMember = data.CountMember,
                            AvatarReads = new ObservableCollection<string>(data.AvatarReads ?? new List<string>()),
                            Messages = new ObservableCollection<ItemMessageViewModel>()
                        };


                        for (int i = 0; i < viewModel.AvatarReads.Count; i++)
                        {
                            if (viewModel.AvatarReads[i] == "no_img_user.png" || viewModel.AvatarReads[i] == "no_img_group.png")
                            {
                                viewModel.AvatarReads[i] = "/Resources/Images/" + viewModel.AvatarReads[i];
                            }
                        }

                        if (viewModel.Avatar == "no_img_user.png" || viewModel.Avatar == "no_img_group.png")
                        {
                            viewModel.Avatar = "/Resources/Images/" + viewModel.Avatar;
                        }

                        // Mapping danh sách tin nhắn
                        foreach (var msg in data.Messages ?? new List<Respone_ChatRoomDetail.Message>())
                        {
                            var messageVM = new ItemMessageViewModel
                            {
                                Id = msg.Id,
                                Content = msg.Content,
                                CreatedAt = msg.CreatedAt,
                                Reactions = new ObservableCollection<ItemReactionViewModel>()
                            };

                            // Mapping Reactions
                            if (msg.Reactions != null)
                            {
                                foreach (var react in msg.Reactions)
                                {

                                    if (react.User.Avatar == "no_img_user.png" || react.User.Avatar == "no_img_group.png")
                                    {
                                        react.User.Avatar = "/Resources/Images/" + react.User.Avatar;
                                    }

                                    messageVM.Reactions.Add(new ItemReactionViewModel
                                    {
                                        TypeReaction = react.TypeReaction,
                                        User = new ItemUserViewModel
                                        {
                                            Id = react.User.Id,
                                            FullName = react.User.FullName,
                                            Avatar = react.User.Avatar
                                        }
                                    });
                                }
                            }

                            messageVM.Sender = new ItemUserViewModel
                            {
                                Id = msg.Sender.Id,
                                FullName = msg.Sender.FullName,
                                Avatar = msg.Sender.Avatar
                            };

                            var isMine = msg.Sender.Id == MainWindow.UserIdCur;
                            var hasParent = msg.Parrent != null;

                            if (isMine && hasParent)
                                messageVM.TypeMessage = TypeMessage.MineWithReply;
                            else if (isMine)
                                messageVM.TypeMessage = TypeMessage.Mine;
                            else if (hasParent)
                                messageVM.TypeMessage = TypeMessage.OtherWithReply;
                            else
                                messageVM.TypeMessage = TypeMessage.Other;


                            viewModel.Messages.Add(messageVM);
                        }

                        foreach (var msg in data.Messages)
                        {
                            if (msg.Parrent != null)
                            {
                                var msgVM = viewModel.Messages.Where(x => x.Id == msg.Id).FirstOrDefault();
                                msgVM.Parent= viewModel.Messages.Where(x => x.Id == msg.Parrent.Id).FirstOrDefault();
                            }
                        }

                        return viewModel;
                    }
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
