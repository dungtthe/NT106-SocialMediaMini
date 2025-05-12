using System;
using System.Windows;
using System.Windows.Controls;
using Client.Views.Posts.Windows;

namespace Client.Views.Posts.Components
{
    public partial class PostItem : UserControl
    {
        public PostItem()
        {
            InitializeComponent();
        }

        public PostItemModel User
        {
            get { return (PostItemModel)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(PostItemModel), typeof(PostItem), new PropertyMetadata(null));

        public DateTime CreateAt
        {
            get { return (DateTime)GetValue(CreateAtProperty); }
            set { SetValue(CreateAtProperty, value); }
        }

        public static readonly DependencyProperty CreateAtProperty =
            DependencyProperty.Register("CreateAt", typeof(DateTime), typeof(PostItem), new PropertyMetadata(DateTime.Now));

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(PostItem), new PropertyMetadata("No content"));

        public string[] Images
        {
            get { return (string[])GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        public static readonly DependencyProperty ImagesProperty =
            DependencyProperty.Register("Images", typeof(string[]), typeof(PostItem), new PropertyMetadata(null));

        public ReactionModel Reactions
        {
            get { return (ReactionModel)GetValue(ReactionsProperty); }
            set { SetValue(ReactionsProperty, value); }
        }

        public static readonly DependencyProperty ReactionsProperty =
            DependencyProperty.Register("Reactions", typeof(ReactionModel), typeof(PostItem), new PropertyMetadata(new ReactionModel { Count = 0 }));

        public int CommentCount
        {
            get { return (int)GetValue(CommentCountProperty); }
            set { SetValue(CommentCountProperty, value); }
        }

        public static readonly DependencyProperty CommentCountProperty =
            DependencyProperty.Register("CommentCount", typeof(int), typeof(PostItem), new PropertyMetadata(0));

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            var commentWindow = new CommentWindow
            {
                Username = User?.FullName ?? "Unknown",
                Timestamp = CreateAt.ToString("dd/MM/yyyy HH:mm"),
                Content = Content ?? "No content"
            };
            commentWindow.Show();
        }
    }

    public class PostItemModel
    {
        public string FullName { get; set; }
        public string Avatar { get; set; }
    }

    public class ReactionModel
    {
        public int Count { get; set; }
    }
}