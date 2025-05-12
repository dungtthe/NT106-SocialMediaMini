using Client.Const;
using Client.Const.Type;
using Client.Helpers;
using Client.Models.Respone;
using Client.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Chats
{
    public class ConversationViewModel : BaseViewModel
    {

        #region chatroom list
        public class ItemChatRoomViewModel : BaseItemViewModel
        {
            private long _chatRoomId;
            private string _roomName;
            private string _avatar;
            private string _lastMessage;
            private string _unReadMessageCount;
            private string _lastTime;

            // Chat Room
            public long ChatRoomId
            {
                get { return _chatRoomId; }
                set { SetProperty(ref _chatRoomId, value, nameof(ChatRoomId)); }
            }

            public string RoomName
            {
                get { return _roomName; }
                set { SetProperty(ref _roomName, value, nameof(RoomName)); }
            }

            public string Avatar
            {
                get { return _avatar; }
                set { SetProperty(ref _avatar, value, nameof(Avatar)); }
            }

            // Message
            public string LastMessage
            {
                get { return _lastMessage; }
                set { SetProperty(ref _lastMessage, value, nameof(LastMessage)); }
            }

            public string UnReadMessageCount
            {
                get { return _unReadMessageCount; }
                set { SetProperty(ref _unReadMessageCount, value, nameof(UnReadMessageCount)); }
            }

            public string LastTime
            {
                get { return _lastTime; }
                set { SetProperty(ref _lastTime, value, nameof(LastTime)); }
            }
        }
        private ObservableCollection<ItemChatRoomViewModel> _chatRooms;
        public ObservableCollection<ItemChatRoomViewModel> ChatRooms
        {
            get { return _chatRooms; }
            set { SetProperty(ref _chatRooms, value, nameof(ChatRooms)); }
        }
        #endregion

        #region detail chatroom
        public class ItemChatRoomDetailViewModel : BaseItemViewModel
        {

            public enum TypeMessage
            {
                Other,               // Người khác gửi không reply
                OtherWithReply,      // Người khác gửi có reply
                Mine,                // Mình gửi không reply
                MineWithReply        // Mình gửi có reply
            }



            public class ItemUserViewModel : BaseItemViewModel
            {
                private long _id;
                private string _fullName;
                private string _avatar;

                public long Id
                {
                    get => _id;
                    set => SetProperty(ref _id, value, nameof(Id));
                }

                public string FullName
                {
                    get => _fullName;
                    set => SetProperty(ref _fullName, value, nameof(FullName));
                }

                public string Avatar
                {
                    get => _avatar;
                    set => SetProperty(ref _avatar, value, nameof(Avatar));
                }
            }

            public class ItemReactionViewModel : BaseItemViewModel
            {
                private ItemUserViewModel _user;
                private Type_Reaction _typeReaction;

                public ItemUserViewModel User
                {
                    get => _user;
                    set => SetProperty(ref _user, value, nameof(User));
                }

                public Type_Reaction TypeReaction
                {
                    get => _typeReaction;
                    set => SetProperty(ref _typeReaction, value, nameof(TypeReaction));
                }
            }


            public class ItemMessageViewModel : BaseItemViewModel
            {
                private long _id;
                private string _content;
                private string _createdAt;
                private ItemUserViewModel _sender;
                private ItemMessageViewModel _parent;
                private ObservableCollection<ItemReactionViewModel> _reactions;
                private TypeMessage _typeMessage;
                public TypeMessage TypeMessage
                {
                    get => _typeMessage;
                    set => SetProperty(ref _typeMessage, value, nameof(TypeMessage));
                }
                public ItemUserViewModel Sender
                {
                    get => _sender;
                    set => SetProperty(ref _sender, value, nameof(Sender));
                }

                public long Id
                {
                    get => _id;
                    set => SetProperty(ref _id, value, nameof(Id));
                }

                public string Content
                {
                    get => _content;
                    set => SetProperty(ref _content, value, nameof(Content));
                }

                public string CreatedAt
                {
                    get => _createdAt;
                    set => SetProperty(ref _createdAt, value, nameof(CreatedAt));
                }

                public ItemMessageViewModel Parent
                {
                    get => _parent;
                    set => SetProperty(ref _parent, value, nameof(Parent));
                }

                public ObservableCollection<ItemReactionViewModel> Reactions
                {
                    get => _reactions;
                    set => SetProperty(ref _reactions, value, nameof(Reactions));
                }
            }



            private long _leaderId;
            private string _avatar;
            private string _roomName;
            private bool _isGroupChat;
            private bool _canSendMessage;
            private bool _canAddMember;
            private int _countMember;

            private ObservableCollection<ItemMessageViewModel> _messages;
            private ObservableCollection<string> _avatarReads;

            public long LeaderId
            {
                get => _leaderId;
                set => SetProperty(ref _leaderId, value, nameof(LeaderId));
            }

            public string Avatar
            {
                get => _avatar;
                set => SetProperty(ref _avatar, value, nameof(Avatar));
            }

            public string RoomName
            {
                get => _roomName;
                set => SetProperty(ref _roomName, value, nameof(RoomName));
            }

            public bool IsGroupChat
            {
                get => _isGroupChat;
                set => SetProperty(ref _isGroupChat, value, nameof(IsGroupChat));
            }

            public bool CanSendMessage
            {
                get => _canSendMessage;
                set => SetProperty(ref _canSendMessage, value, nameof(CanSendMessage));
            }

            public bool CanAddMember
            {
                get => _canAddMember;
                set => SetProperty(ref _canAddMember, value, nameof(CanAddMember));
            }

            public int CountMember
            {
                get => _countMember;
                set => SetProperty(ref _countMember, value, nameof(CountMember));
            }

            public ObservableCollection<ItemMessageViewModel> Messages
            {
                get => _messages;
                set => SetProperty(ref _messages, value, nameof(Messages));
            }

            public ObservableCollection<string> AvatarReads
            {
                get => _avatarReads;
                set => SetProperty(ref _avatarReads, value, nameof(AvatarReads));
            }
        }

        private ItemChatRoomDetailViewModel _chatRoomDetail;
        public ItemChatRoomDetailViewModel ChatRoomDetail
        {
            get { return _chatRoomDetail; }
            set { SetProperty(ref _chatRoomDetail, value, nameof(ChatRoomDetail)); }
        }


        #endregion


        public ConversationViewModel()
        {
            ChatRooms = new ObservableCollection<ItemChatRoomViewModel>();
            Task.Run(async () =>
            {
                while (true)
                {
                    await CallData();
                    await Task.Delay(5000);
                }
            });

            Task.Run(async () =>
            {
                ChatRoomDetail = await ChatRoomService.GetChatRoomDetailAsync(1);
                
            });
        }


        private async Task CallData()
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/chat-room/conversations", true));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var conversations = JsonConvert.DeserializeObject<List<Respone_GetConversations.ConversationDTO>>(response.ResponseBody);
                    if (conversations != null)
                    {
                        ChatRooms = new ObservableCollection<ItemChatRoomViewModel>();
                        foreach (var conversation in conversations)
                        {
                            var item = new ItemChatRoomViewModel
                            {
                                ChatRoomId = conversation.ChatRoomId,
                                RoomName = conversation.RoomName,
                                Avatar = conversation.Avatar,
                                LastMessage = conversation.LastMessage,
                                LastTime = TimeHelpers.CalculateTimeDifference(conversation.LastTime),
                            };
                            if (conversation.UnReadMessageCount > 9)
                            {
                                item.UnReadMessageCount = "9+";
                            }
                            else
                            {
                                item.UnReadMessageCount = conversation.UnReadMessageCount.ToString();
                            }
                            if (item.Avatar == "no_img_user.png" || item.Avatar == "no_img_group.png")
                            {
                                item.Avatar = "/Resources/Images/" + item.Avatar;
                            }

                            ChatRooms.Add(item);
                            ////test scroll
                            //Items.Add(item);
                            //Items.Add(item);
                            //Items.Add(item);
                            //Items.Add(item);
                        }
                    }

                }
            }
            catch { }
        }

    }
}
