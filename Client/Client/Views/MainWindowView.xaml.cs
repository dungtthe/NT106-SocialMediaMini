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

        public static long UserIdCur = 1;//test


        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ChatPageView());
        }
        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChatPageView());
        }

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}