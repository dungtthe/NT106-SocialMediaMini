using Client.ViewModels.Posts;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.Views.Posts.Windows
{
    public partial class CommentWindow : Window
    {
        private long postId;
        public CommentWindow(long postId)
        {
            InitializeComponent();
            this.postId = postId;
            this.DataContext = new CommentViewModel(postId);
        }

        private void SendCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}