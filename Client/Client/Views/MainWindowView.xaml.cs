using Client.LocalStorage;
using Client.Services.RealTimes;
using Client.ViewModels;
using Client.ViewModels.Posts;
using Client.Views.Chats.Pages;
using Client.Views.Posts.Pages;
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

        public enum TYPE_PAGE
        {
            NONE,//vi du o window login thì la none
            CHAT_PAGE_VIEW,
            POST_PAGE_VIEW
        }

        public static TYPE_PAGE TypePage = TYPE_PAGE.NONE;

        private static bool isFirst = true;
        public MainWindow()
        {
            TypePage = TYPE_PAGE.CHAT_PAGE_VIEW;

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

            imgAvatar.ImageSource = new BitmapImage(new Uri(UserStore.Avatar, UriKind.RelativeOrAbsolute));

        }
        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            TypePage = TYPE_PAGE.CHAT_PAGE_VIEW;
            MainFrame.Navigate(new ChatPageView());
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            TypePage = TYPE_PAGE.POST_PAGE_VIEW;
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