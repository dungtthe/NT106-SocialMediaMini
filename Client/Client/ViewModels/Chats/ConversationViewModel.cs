using Client.Const;
using Client.Helpers;
using Client.Models.Respone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Chats
{
    public class ConversationViewModel : BaseViewModel
    {
        public class ItemViewModel : BaseItemViewModel
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




        private List<ItemViewModel> _items;
        public List<ItemViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value, nameof(Items)); }
        }
        public  ConversationViewModel()
        {
            Items = new List<ItemViewModel>();
            Task.Run(async () =>
            {
                while (true)
                {
                    await CallData();
                    await Task.Delay(5000);
                }
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
                        Items = new List<ItemViewModel>();
                        foreach (var conversation in conversations)
                        {
                            var item = new ItemViewModel
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

                            Items.Add(item);
                        }
                    }

                }
            }
            catch { }
        }

    }
}
