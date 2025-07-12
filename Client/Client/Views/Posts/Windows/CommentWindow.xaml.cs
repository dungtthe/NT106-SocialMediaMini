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
        private readonly CommentViewModel viewModel;
        public CommentWindow(long postId)
        {
            InitializeComponent();
            viewModel = new CommentViewModel(postId);
            this.DataContext = viewModel;
        }

        private void SendCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                viewModel.RequestAddComment(CommentTextBox.Text, null);//tạm thời chưa có trả lời
                CommentTextBox.Text = "";
                btnSend.IsEnabled = false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                btnSend.IsEnabled = false;
            }
            else
            {
                btnSend.IsEnabled = true;
            }
        }
    }
}