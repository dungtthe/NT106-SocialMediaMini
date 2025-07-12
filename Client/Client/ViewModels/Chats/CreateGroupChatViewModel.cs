using Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Chats
{
    public class CreateGroupChatViewModel : BaseViewModel
    {
        public class FriendSummaryViewModel : BaseViewModel
        {
            private long _userId;
            public long UserId
            {
                get => _userId;
                set => SetProperty(ref _userId, value, nameof(UserId));
            }

            private string _fullName;
            public string FullName
            {
                get => _fullName;
                set => SetProperty(ref _fullName, value, nameof(FullName));
            }

            private string _avatar;
            public string Avatar
            {
                get
                {
                    if (_avatar == "no_img_user.png")
                    {
                        return "pack://application:,,,/Resources/Images/no_img_user.png";
                    }
                    else if (string.IsNullOrEmpty(_avatar) || _avatar == "no_img_group.png")
                    {
                        return "pack://application:,,,/Resources/Images/no_img_group.png";
                    }
                    return _avatar;
                }
                set => SetProperty(ref _avatar, value, nameof(Avatar));
            }


            private bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected;
                set => SetProperty(ref _isSelected, value, nameof(IsSelected));
            }

        }



        private ObservableCollection<FriendSummaryViewModel> _friendItems;
        public ObservableCollection<FriendSummaryViewModel> FriendItems
        {
            get => _friendItems;
            set => SetProperty(ref _friendItems, value, nameof(FriendItems));
        }

        public CreateGroupChatViewModel()
        {
            LoadFriendsAsync();
        }

        private async void LoadFriendsAsync()
        {
            var rawFriends = await UserService.GetFriendsSummaryAsync();

            if (rawFriends == null) return;

            var mapped = rawFriends.Select(f => new CreateGroupChatViewModel.FriendSummaryViewModel
            {
                UserId = f.UserId,
                FullName = f.FullName,
                Avatar = f.Avatar
            });

            FriendItems = new ObservableCollection<CreateGroupChatViewModel.FriendSummaryViewModel>(mapped);
        }
    }
}
