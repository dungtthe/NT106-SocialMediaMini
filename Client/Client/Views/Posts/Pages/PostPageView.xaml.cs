using Client.Const.Type;
using Client.Helpers;
using Client.LocalStorage;
using Client.Models.Request;
using Client.Services;
using Client.ViewModels.Posts;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views.Posts.Pages
{
    /// <summary>
    /// Interaction logic for PostPageView.xaml
    /// </summary>
    public partial class PostPageView : Page
    {
        // private static bool isFirst =true;
        public PostPageView()
        {
            InitializeComponent();
            this.DataContext = PostViewModel.GI();
            imgAvatar.ImageSource = new BitmapImage(new Uri(UserStore.Avatar, UriKind.RelativeOrAbsolute));
            //if (!isFirst)
            //{
            //    this.DataContext = PostViewModel();
            //    return;
            //}

            //isFirst = false;
            btnHuyPost.Visibility = Visibility.Hidden;
            txtContentPost.Text = "";
            btnDang.IsEnabled = false;
            listImgSelect.Children.Clear();
        }

        private void txtContentPost_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtContentPost.Text))
            {
                btnHuyPost.Visibility = Visibility.Hidden;
                btnDang.IsEnabled = false;
            }
            else
            {
                btnHuyPost.Visibility = Visibility.Visible;
                btnDang.IsEnabled = true;
            }
        }

        private void btnHuyPost_Click(object sender, RoutedEventArgs e)
        {
            txtContentPost.Text = "";
            listImgSelect.Children.Clear();
        }

        private void btnSelectImages_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh",
                Filter = "Ảnh PNG và JPG (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = true
            };


            if (dialog.ShowDialog() == true)
            {
               // listImgSelect.Children.Clear(); 

                foreach (var file in dialog.FileNames)
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                        }
                        bitmap.Freeze(); // tránh lỗi binding

                        int width = 190;
                        int height = 190;
                        var image = new Image
                        {
                            Source = bitmap,
                            Width = width,
                            Height = height,
                            Stretch = Stretch.UniformToFill,
                            Cursor = Cursors.Hand,
                            ToolTip = "Xóa ảnh này"
                        };


                        image.Clip = new RectangleGeometry
                        {
                            Rect = new Rect(0, 0, width, height),
                            RadiusX = 15,
                            RadiusY = 15
                        };

                        var border = new Border
                        {
                            Background = Brushes.Transparent,
                            CornerRadius = new CornerRadius(15),
                            ClipToBounds = true,
                            Width = width,
                            Height = height,
                            Margin = new Thickness(5),
                            Child = image
                        };

                        border.MouseLeftButtonUp += (s, e) =>
                        {
                            listImgSelect?.Children.Remove(border);
                        };

                        listImgSelect.Children.Add(border);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Không thể tải ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void btnDang_Click(object sender, RoutedEventArgs e)
        {
            btnDang.IsEnabled = false;

            var data = new Request_AddPostDTO()
            {
                Content = txtContentPost.Text,
                Images = JsonConvert.SerializeObject(GetAllImagesBase64()),
                PostVisibility = (byte)Type_PostVisibility.Friends
            };

            var rs = await PostService.AddPostAsync(data);

            btnHuyPost.Visibility = Visibility.Hidden;
            txtContentPost.Text = "";
            listImgSelect.Children.Clear();

            if (rs != null)
            {
                PostViewModel.NewPostQueue.Enqueue(rs.Value);
            }
            else
            {
                Debug.WriteLine("Post fail");
            }
        }

        private List<string> GetAllImagesBase64()
        {
            var result = new List<string>();
            foreach (var child in listImgSelect.Children)
            {
                if (child is Border border && border.Child is Image image && image.Source != null)
                {
                    var base64 = ImageHelpers.ImageSourceToBase64(image.Source);
                    if (!string.IsNullOrEmpty(base64))
                    {
                        result.Add(base64);
                    }
                }
            }
            return result;
        }

    }
}
