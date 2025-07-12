using Client.Helpers;
using Client.LocalStorage;
using Client.Services;
using Client.ViewModels.Posts;
using Client.Views.Chats.Windows;
using Client.Views.Posts.Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views.Posts.Components
{
    public partial class PostItem : UserControl
    {
        public PostItem()
        {
            InitializeComponent();
        }

        private void ImageItemInPost_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Source != null)
            {
                var viewer = new ImageWindowView(img.Source);
                viewer.ShowDialog();
            }
        }

        private async void btnReact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                long postId = long.Parse(btn.Tag.ToString());
                var result = await PostService.ReactOrUnReactPost(postId, SocialMediaMini.Shared.Const.Type.ReactionType.Love);
                if (result != null)
                {
                    UIHelpers.InvokeDispatcherUI(() =>
                    {
                        var fPost = PostViewModel.GI().Items.FirstOrDefault(i => i.PostId == result.PostId);
                        if (fPost != null)
                        {
                            var reactions = fPost.Reactions;
                            if (result.Reaction == null)
                            {
                                var fReact = reactions.FirstOrDefault(r => r.User.Id == UserStore.UserIdCur);
                                if (fReact != null)
                                {
                                    reactions.Remove(fReact);
                                    fPost.CurrentUserHasReacted = false;
                                }
                            }
                            else
                            {
                                reactions.Add(new PostViewModel.ItemReactionViewModel()
                                {
                                    ReactionType = result.Reaction.ReactionType,
                                    User = new PostViewModel.ItemUserViewModel()
                                    {
                                        Id = result.Reaction.User.Id,
                                        Avatar = result.Reaction.User.Avatar,
                                        FullName = result.Reaction.User.FullName,
                                    }
                                });
                                fPost.CurrentUserHasReacted = true;
                            }
                            fPost.ReactionCount = reactions.Count;
                        }
                    });
                }
            }
        }

        private void btnShowComment_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                CommentWindow commentWindow = new CommentWindow(long.Parse(btn.Tag.ToString()));
                this.Opacity = 0.4;
                commentWindow.ShowDialog();
                this.Opacity = 1;
            }
        }
    }
}