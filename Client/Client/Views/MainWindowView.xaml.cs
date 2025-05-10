using Client.Views.Blog;
using Client.Views.Chats.Pages;
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
            MainFrame.Navigate(new BlogPageView());
        }
    }
}