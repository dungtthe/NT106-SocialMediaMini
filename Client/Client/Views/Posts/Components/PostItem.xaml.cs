using System;
using System.Diagnostics;
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

        private void ImageItemInPost_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Source != null)
            {
                var viewer = new ImageWindowView(img.Source);
                viewer.ShowDialog();                     
            }
        }
    }
}