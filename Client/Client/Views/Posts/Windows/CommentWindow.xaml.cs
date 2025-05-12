using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.Views.Posts.Windows
{
    public partial class CommentWindow : Window
    {
        public CommentWindow()
        {
            InitializeComponent();
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(CommentWindow), new PropertyMetadata("Unknown"));

        public string Timestamp
        {
            get { return (string)GetValue(TimestampProperty); }
            set { SetValue(TimestampProperty, value); }
        }

        public static readonly DependencyProperty TimestampProperty =
            DependencyProperty.Register("Timestamp", typeof(string), typeof(CommentWindow), new PropertyMetadata("Unknown"));

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(CommentWindow), new PropertyMetadata("No content"));

        private void SendCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                var commentBorder = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(1),
                    BorderBrush = (Brush)FindResource("ColorBlueOne"),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(10)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var avatar = new Image
                {
                    Source = new BitmapImage(new Uri("/Resources/Images/meolag.jpg", UriKind.Relative)),
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(0, 0, 10, 0),
                    //RenderOptions.BitmapScalingMode = BitmapScalingMode.HighQuality
                };

                var stackPanel = new StackPanel();
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "Current User",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                });
                stackPanel.Children.Add(new TextBlock
                {
                    Text = CommentTextBox.Text,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                });
                stackPanel.Children.Add(new TextBlock
                {
                    Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    FontSize = 10,
                    Foreground = Brushes.Gray
                });

                grid.Children.Add(avatar);
                Grid.SetColumn(avatar, 0);
                grid.Children.Add(stackPanel);
                Grid.SetColumn(stackPanel, 1);

                commentBorder.Child = grid;
                CommentsPanel.Children.Add(commentBorder);

                CommentTextBox.Text = string.Empty;
            }
        }
    }
}