using Client.Const;
using Client.Const.Type;
using Client.Helpers;
using Client.LocalStorage;
using Client.Models.Respone;
using Client.Services;
using Client.Services.RealTimes;
using Client.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Client.ViewModels.Chats.ConversationViewModel.ItemChatRoomDetailViewModel;
using System.Windows.Threading;
using Client.Models.Request;

namespace Client.ViewModels.Chats
{
    public class ConversationViewModel : BaseViewModel
    {
        private static ConversationViewModel ins;
        public static ConversationViewModel GI()
        {
            if (ins == null)
            {
                ins = new ConversationViewModel();
            }
            return ins;
        }



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


        private ItemChatRoomViewModel _selectedChatRoom;
        public ItemChatRoomViewModel SelectedChatRoom
        {
            get { return _selectedChatRoom; }
            set
            {
                SetProperty(ref _selectedChatRoom, value, nameof(SelectedChatRoom));

                if (value != null)
                {
                    LoadChatRoomDetail(value.ChatRoomId);
                }
                else
                {
                    ChatRoomDetail = null;
                }
            }
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


            private long _chatRoomId;
            private long _leaderId;
            private string _avatar;
            private string _roomName;
            private bool _isGroupChat;
            private bool _canSendMessage;
            private bool _canAddMember;
            private int _countMember;

            private ObservableCollection<ItemMessageViewModel> _messages;
            private ObservableCollection<string> _avatarReads;

            public long ChatRoomId
            {
                get => _chatRoomId;
                set => SetProperty(ref _chatRoomId, value, nameof(ChatRoomId));
            }

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
            SendLoop();
            ReceiveLoop();
        }


        public void Init()
        {
            LoadChatRooms();
        }


        private void LoadChatRooms()
        {
            Task.Run(async () =>
            {
                try
                {
                    var data = await ChatRoomService.GetConversationsAsync();
                    if (data != null)
                    {
                        UIHelpers.InvokeDispatcherUI(async () =>
                        {
                            //ChatRooms = data;
                            var selectedTemp = SelectedChatRoom;
                            ChatRooms.Clear();
                            foreach (var item in data)
                                ChatRooms.Add(item);
                            if (selectedTemp != null)
                            {
                                SelectedChatRoom = ChatRooms.Where(c => c.ChatRoomId == selectedTemp.ChatRoomId).FirstOrDefault();
                            }

                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        //private void InvokeDispatcherUI(Action action)
        //{

        //    Application.Current.Dispatcher.BeginInvoke(action);
        //}
      

        private void LoadChatRoomDetail(long chatRoomId)
        {
            ChatRoomDetail = null;
            Task.Run(async () =>
            {
                try
                {

                    var data = await ChatRoomService.GetChatRoomDetailAsync(chatRoomId);

                    if (data != null)
                    {
                        UIHelpers.InvokeDispatcherUI(() =>
                        {
                            ChatRoomDetail = data;
                            if (IsValidChatRoom())
                            {
                                SelectedChatRoom.UnReadMessageCount = "0";
                            }
                        });
                    }
                    else
                    {
                        ChatRoomDetail = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        #region notification
        public static ConcurrentQueue<Tuple<long, string>> MessagesSend = new ConcurrentQueue<Tuple<long, string>>();//hàng đợi message gửi lên server(chatroomid-msg)
        public static ConcurrentQueue<string> MessagesReceive = new ConcurrentQueue<string>();//hàng đợi nhận từ server

        private void SendLoop()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (MainWindow.TypePage == MainWindow.TYPE_PAGE.CHAT_PAGE_VIEW)
                    {
                        try
                        {
                            if (MessagesSend.TryDequeue(out var data))
                            {
                                var chatRoomId = data.Item1;
                                var content = data.Item2;


                                var dataSend = new Request_AddNotificationDTO()
                                {
                                    NotificationType = Type_Notification.MESSAGE,
                                    Data = JsonConvert.SerializeObject(new Request_AddNotificationDTO.Message()
                                    {
                                        ChatRoomId = chatRoomId,
                                        Content = content,
                                        ParrentMessageId = null//nao nho lam cai tra loi tin nhan
                                    })
                                };

                                await NotifyService.SendMessage(dataSend.NotificationType, dataSend.Data);
                            }
                        }
                        catch (Exception ex)//nao lam thong bao exception
                        {

                        }
                    }
                    else if (MainWindow.TypePage == MainWindow.TYPE_PAGE.NONE)
                    {
                        MessagesSend.Clear();
                    }
                    await Task.Delay(10);
                }
            });
        }


        public bool IsValidChatRoom()
        {
            if (ChatRoomDetail == null || SelectedChatRoom == null || ChatRoomDetail.ChatRoomId != SelectedChatRoom.ChatRoomId)
            {
                return false;
            }
            return true;
        }

        private void ReceiveLoop()
        {
            Task.Run(async () =>
            {
                //tam thoi de nhu nay di
                while (true)
                {
                    if (MainWindow.TypePage == MainWindow.TYPE_PAGE.CHAT_PAGE_VIEW)
                    {
                        try
                        {
                            if (MessagesReceive.TryDequeue(out var data))
                            {
                                var dataRecive = JsonConvert.DeserializeObject<Respone_NotificationDTO.Respone_NotificationMessage>(data);
                                if (dataRecive != null && IsValidChatRoom())
                                {
                                    if (dataRecive.ChatRoomId == ChatRoomDetail.ChatRoomId)
                                    {
                                        var msgNew = new ItemChatRoomDetailViewModel.ItemMessageViewModel()
                                        {
                                            Id = dataRecive.Id,
                                            Content = dataRecive.Content,
                                            CreatedAt = dataRecive.CreatedAt,
                                            Sender = new ItemChatRoomDetailViewModel.ItemUserViewModel()
                                            {
                                                Id = dataRecive.Sender.Id,
                                                FullName = dataRecive.Sender.FullName,
                                                Avatar = dataRecive.Sender.Avatar
                                            },
                                            Reactions = new ObservableCollection<ItemChatRoomDetailViewModel.ItemReactionViewModel>()
                                        };

                                        if (dataRecive.Parrent != null)
                                        {
                                            msgNew.Parent = ChatRoomDetail.Messages.Where(m => m.Id == dataRecive.Parrent.Id).FirstOrDefault();
                                        }
                                        var isMine = msgNew.Sender.Id == UserStore.UserIdCur;
                                        var hasParent = msgNew.Parent != null;

                                        if (isMine && hasParent)
                                            msgNew.TypeMessage = TypeMessage.MineWithReply;
                                        else if (isMine)
                                            msgNew.TypeMessage = TypeMessage.Mine;
                                        else if (hasParent)
                                            msgNew.TypeMessage = TypeMessage.OtherWithReply;
                                        else
                                            msgNew.TypeMessage = TypeMessage.Other;


                                        try
                                        {

                                            UIHelpers.InvokeDispatcherUI(() =>
                                            {
                                                ChatRoomDetail.Messages.Add(msgNew);
                                                SelectedChatRoom.LastMessage = msgNew.Content;
                                                SelectedChatRoom.UnReadMessageCount = "0";
                                                Debug.WriteLine(msgNew.Content);
                                            });


                                        }
                                        catch (Exception e)
                                        {
                                            Debug.WriteLine(e.Message);
                                        }

                                        if (dataRecive.Sender.Id != UserStore.UserIdCur)
                                        {
                                            ChatRoomService.ReadMessages(dataRecive.ChatRoomId);
                                        }
                                    }
                                    else
                                    {
                                        LoadChatRooms();
                                        Debug.WriteLine("dataRecive.ChatRoomId-" + dataRecive.ChatRoomId + "ChatRoomDetail.ChatRoomId-" + ChatRoomDetail.ChatRoomId);
                                    }
                                }
                                else
                                {
                                    if (ChatRoomDetail == null)
                                    {
                                        Debug.WriteLine("ChatRoomDetail == null");
                                    }
                                    if (SelectedChatRoom == null)
                                    {
                                        Debug.WriteLine("SelectedChatRoom==null");

                                    }
                                    LoadChatRooms();
                                }
                            }

                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else if (MainWindow.TypePage == MainWindow.TYPE_PAGE.NONE)
                    {
                        MessagesReceive.Clear();
                    }
                    await Task.Delay(10);
                }
            });
        }



        #endregion

        public void Reset()
        {
            MessagesReceive.Clear();
            MessagesSend.Clear();
            ChatRooms = new ObservableCollection<ItemChatRoomViewModel>();
            SelectedChatRoom = null;
            ChatRoomDetail = null;
        }

    }
}
