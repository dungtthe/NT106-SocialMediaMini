using Client.Const.Type;
using Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ViewModels.Chats.ConversationViewModel;

namespace Client.ViewModels.Posts
{
    public class PostViewModel : BaseViewModel
    {

        public class ItemUserViewModel : BaseViewModel
        {
            private string _fullName;
            public string FullName
            {
                get => _fullName;
                set => SetProperty(ref _fullName, value, nameof(FullName));
            }

            private string _avatar;
            public string Avatar
            {
                get => _avatar;
                set => SetProperty(ref _avatar, value, nameof(Avatar));
            }

        }
        public class ItemReactionViewModel : BaseViewModel
        {
            private ItemUserViewModel _user;
            public ItemUserViewModel User
            {
                get => _user;
                set => SetProperty(ref _user, value, nameof(User));
            }

            private Type_Reaction _typeReaction;
            public Type_Reaction TypeReaction
            {
                get => _typeReaction;
                set => SetProperty(ref _typeReaction, value, nameof(TypeReaction));
            }
        }

        public class ItemPostViewModel : BaseItemViewModel
        {
            private long _postId;
            public long PostId
            {
                get => _postId;
                set => SetProperty(ref _postId, value, nameof(PostId));
            }

            private string _content;
            public string Content
            {
                get => _content;
                set => SetProperty(ref _content, value, nameof(Content));
            }

            private ObservableCollection<string> _images;
            public ObservableCollection<string> Images
            {
                get => _images;
                set => SetProperty(ref _images, value, nameof(Images));
            }

            private DateTime _createAt;
            public DateTime CreateAt
            {
                get => _createAt;
                set => SetProperty(ref _createAt, value, nameof(CreateAt));
            }

            private DateTime _updateAt;
            public DateTime UpdateAt
            {
                get => _updateAt;
                set => SetProperty(ref _updateAt, value, nameof(UpdateAt));
            }

            private ItemUserViewModel _user;
            public ItemUserViewModel User
            {
                get => _user;
                set => SetProperty(ref _user, value, nameof(User));
            }

            private ObservableCollection<ItemReactionViewModel> _reactions;
            public ObservableCollection<ItemReactionViewModel> Reactions
            {
                get => _reactions;
                set => SetProperty(ref _reactions, value, nameof(Reactions));
            }

            private long _commentCount;
            public long CommentCount
            {
                get => _commentCount;
                set => SetProperty(ref _commentCount, value, nameof(CommentCount));
            }
        }

        private ObservableCollection<PostViewModel.ItemPostViewModel> _items;
        public ObservableCollection<PostViewModel.ItemPostViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value, nameof(Items)); }
        }
        public PostViewModel()
        {
            Items = new ObservableCollection<PostViewModel.ItemPostViewModel>();
            Task.Run(async () =>
            {

                var data = await PostService.GetFriendPostsAsync();
                if (data != null)
                {
                    Items = data;
                }

            });
        }

    }
}
