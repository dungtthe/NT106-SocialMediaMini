using Client.Const;
using Client.Helpers;
using Client.LocalStorage;
using Client.Services.RealTimes;
using Client.ViewModels;
using Client.ViewModels.Posts;
using Client.Views.Chats.Pages;
using Client.Views.Posts.Pages;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PageViewType PageViewType = PageViewType.NONE;

        private static bool isFirst = true;
        public MainWindow()
        {
            PageViewType = PageViewType.CHAT_PAGE_VIEW;

            InitializeComponent();
            NotifyService.Init();
            if (!isFirst)
            {
                this.DataContext = MainWindowViewModel.GI();
                MainFrame.Navigate(new ChatPageView());
                return;
            }
            isFirst = false;
            MainFrame.Navigate(new ChatPageView());


            if (UserStore.Avatar == "no_img_user.png")
            {
                var uri = new Uri($"pack://application:,,,/Resources/Images/no_img_user.png", UriKind.Absolute);
                imgAvatar.ImageSource = new BitmapImage(uri);
            }
            else
            {
                try
                {
                    using var webClient = new WebClient();
                    byte[] data = webClient.DownloadData(ConfigConst.BaseApiUrl + "/api/file/image/" + UserStore.Avatar);
                    using var ms = new MemoryStream(data);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();

                    imgAvatar.ImageSource = image;
                }
                catch
                {
                    var uri = new Uri($"pack://application:,,,/Resources/Images/no_img_user.png", UriKind.Absolute);
                    imgAvatar.ImageSource = new BitmapImage(uri);
                }
                //imgAvatar.ImageSource = new BitmapImage(new Uri(UserStore.Avatar, UriKind.RelativeOrAbsolute));
            }

        }
        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewType = PageViewType.CHAT_PAGE_VIEW;
            MainFrame.Navigate(new ChatPageView());
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            PageViewType = PageViewType.POST_PAGE_VIEW;
            MainFrame.Navigate(new PostPageView());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                //this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}