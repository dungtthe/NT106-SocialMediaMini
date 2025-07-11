using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Client.Views.Notifications.Pages
{
    public partial class NotificationsPageView : Page
    {
        public NotificationsPageView()
        {
            InitializeComponent();
            NotificationsListBox.ItemsSource = LoadNotifications(); // Gán dữ liệu hardcode
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.NavigateUri != null)
            {
                Process.Start(new ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri) { UseShellExecute = true });
            }
        }

        private List<Notification> LoadNotifications()
        {
            return new List<Notification>
            {
                // Thông báo 1: Bài post mới
                new Notification
                {
                    id = 1,
                    type = "post",
                    content = "Nguyen Van A vừa đăng bài viết mới: 'Chào mọi người, hôm nay là một ngày đẹp trời!'",
                    link = new Uri("https://example.com/posts/12345", UriKind.Absolute),
                    sender = new Sender { id = 1001, name = "Nguyen Van A", avatar = "https://example.com/avatar1.jpg" },
                    timestamp = DateTime.Now.AddHours(-2), // 2 giờ trước
                    is_read = false,
                    post = new Post { id = 12345, title = "Chào mọi người, hôm nay là một ngày đẹp trời!", image = "https://example.com/post_image1.jpg" }
                },

                // Thông báo 2: Bình luận mới
                new Notification
                {
                    id = 2,
                    type = "comment",
                    content = "Tran Thi B vừa bình luận trên bài viết của bạn: 'Đúng là đẹp thật!'",
                    link = new Uri("https://example.com/posts/12345/comments/67890", UriKind.Absolute),
                    sender = new Sender { id = 1002, name = "Tran Thi B", avatar = "https://example.com/avatar2.jpg" },
                    timestamp = DateTime.Now.AddHours(-1).AddMinutes(-30), // 1 giờ 30 phút trước
                    is_read = false,
                    comment = new Comment { id = 67890, text = "Đúng là đẹp thật!", post_id = 12345 }
                },

                // Thông báo 3: Thích bài post
                new Notification
                {
                    id = 3,
                    type = "post_like",
                    content = "Le Van C đã thích bài viết của bạn: 'Chào mọi người, hôm nay là một ngày đẹp trời!'",
                    link = new Uri("https://example.com/posts/12345", UriKind.Absolute),
                    sender = new Sender { id = 1003, name = "Le Van C", avatar = "https://example.com/avatar3.jpg" },
                    timestamp = DateTime.Now.AddMinutes(-45), // 45 phút trước
                    is_read = true,
                    post = new Post { id = 12345, title = "Chào mọi người, hôm nay là một ngày đẹp trời!", image = "https://example.com/post_image1.jpg" }
                },

                // Thông báo 4: Thích bình luận
                new Notification
                {
                    id = 4,
                    type = "comment_like",
                    content = "Pham Thi D đã thích bình luận của bạn: 'Đúng là đẹp thật!'",
                    link = new Uri("https://example.com/posts/12345/comments/67890", UriKind.Absolute),
                    sender = new Sender { id = 1004, name = "Pham Thi D", avatar = "https://example.com/avatar4.jpg" },
                    timestamp = DateTime.Now.AddMinutes(-15), // 15 phút trước
                    is_read = false,
                    comment = new Comment { id = 67890, text = "Đúng là đẹp thật!", post_id = 12345 }
                }
            };
        }
    }

    // Model (giữ nguyên từ trước)
    public class Notification
    {
        public int id { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public Uri link { get; set; }
        public Sender sender { get; set; }
        public DateTime timestamp { get; set; }
        public bool is_read { get; set; }
        public Post post { get; set; }
        public Comment comment { get; set; }
    }

    public class Sender
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
    }

    public class Post
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
    }

    public class Comment
    {
        public int id { get; set; }
        public string text { get; set; }
        public int post_id { get; set; }
    }
}