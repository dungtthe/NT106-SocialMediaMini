using Client.Helpers;
using Client.Services;
using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Posts
{
    public class CommentViewModel : BaseViewModel
    {
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
        }

        public class ItemReactionViewModel : BaseItemViewModel
        {
            private ItemUserViewModel _user;
            private ReactionType _reactionType;

            public ItemUserViewModel User
            {
                get => _user;
                set => SetProperty(ref _user, value, nameof(User));
            }

            public ReactionType ReactionType
            {
                get => _reactionType;
                set => SetProperty(ref _reactionType, value, nameof(ReactionType));
            }
        }

        public class ItemViewModel : BaseViewModel
        {

            private long _id;
            public long Id
            {
                get => _id;
                set => SetProperty(ref _id, value, nameof(Id));
            }

            private string _content;
            public string Content
            {
                get => _content;
                set => SetProperty(ref _content, value, nameof(Content));
            }

            private string _createdAt;
            public string CreatedAt
            {
                get => _createdAt;
                set => SetProperty(ref _createdAt, value, nameof(CreatedAt));
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
        }

        private ObservableCollection<ItemViewModel> _items;
        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value, nameof(Items));
        }


        private long postId;
        public CommentViewModel(long postId)
        {
            this.postId = postId;
            Items = new ObservableCollection<ItemViewModel>();
            LoadData();
        }


        private void LoadData()
        {
            Task.Run(async () =>
            {
                var data = await PostService.GetCommentsByPostIdAsync(this.postId);
                UIHelpers.InvokeDispatcherUI(() =>
                {
                    if (data != null)
                    {
                        var mappedItems = data.Select(dto => new ItemViewModel
                        {
                            Id = dto.Id,
                            Content = dto.Content,
                            CreatedAt = dto.CreatedAt,
                            User = new ItemUserViewModel
                            {
                                Id = dto.User.Id,
                                FullName = dto.User.FullName,
                                Avatar = dto.User.Avatar
                            },
                            Reactions = new ObservableCollection<ItemReactionViewModel>(
                               dto.Reactions?.Select(r => new ItemReactionViewModel
                               {
                                   ReactionType = r.ReactionType,
                                   User = new ItemUserViewModel
                                   {
                                       Id = r.User.Id,
                                       FullName = r.User.FullName,
                                       Avatar = r.User.Avatar
                                   }
                               }) ?? new List<ItemReactionViewModel>())
                        });

                        foreach (var item in mappedItems)
                        {
                            Items.Add(item);
                        }
                    }
                });

            });
        }
    }
}
