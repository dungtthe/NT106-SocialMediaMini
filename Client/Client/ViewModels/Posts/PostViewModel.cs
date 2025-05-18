using Client.Const.Type;
using Client.Helpers;
using Client.Models.Respone;
using Client.Services;
using Client.ViewModels.Chats;
using Client.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using static Client.ViewModels.Chats.ConversationViewModel;

namespace Client.ViewModels.Posts
{
    public class PostViewModel : BaseViewModel
    {

        private static PostViewModel ins;
        public static PostViewModel GI()
        {
            if (ins == null)
            {
                ins = new PostViewModel();
            }
            return ins;
        }

        public static void Reset()
        {
            ins?.Items?.Clear();
            NewPostQueue?.Clear();
        }



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
                    foreach (var item in data)
                    {
                        AddItemPostViewModel(item, false, true);
                    }
                }
                data = await PostService.GetMyPostsAsync();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        AddItemPostViewModel(item, true, false);
                    }
                }
            });



            Task.Run(async () =>
            {

                while (true)
                {
                    try
                    {
                        if (MainWindow.TypePage == MainWindow.TYPE_PAGE.POST_PAGE_VIEW)
                        {
                            if (NewPostQueue.TryDequeue(out var postId))
                            {
                                var postDetail = await PostService.GetPostDetailAsync(postId);
                                if (postDetail != null)
                                {
                                    AddItemPostViewModel(postDetail, true);
                                }
                            }
                        }


                        await Task.Delay(20);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

            });
        }

        private void AddItemPostViewModel(PostViewModel.ItemPostViewModel data)
        {
            UIHelpers.InvokeDispatcherUI(() => { Items.Add(data); });
        }


        private void AddItemPostViewModel(Respone_PostDetail.PostDTO dto, bool isAddFirst, bool isReRandom = false)
        {
            UIHelpers.InvokeDispatcherUI(() =>
            {
                var itemAdd = new PostViewModel.ItemPostViewModel
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
                };

                if (isAddFirst)
                {
                    Items.Insert(0, itemAdd);
                }
                else
                {
                    Items.Add(itemAdd);
                }

                if (isReRandom)
                {
                    var rnd = new Random();
                    var shuffled = Items.OrderBy(_ => rnd.Next()).ToList();
                    Items.Clear();
                    foreach (var item in shuffled)
                    {
                        Items.Add(item);
                    }
                }
            });
        }



        public static ConcurrentQueue<long> NewPostQueue = new ConcurrentQueue<long>();//id post

    }
}
