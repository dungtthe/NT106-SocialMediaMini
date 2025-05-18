using Client.Services;
using Client.ViewModels.Chats;
using Client.ViewModels.Posts;
using Client.Views.Posts.Pages;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for LoginWindowView.xaml
    /// </summary>
    public partial class LoginWindowView : Window
    {
        public LoginWindowView()
        {
            InitializeComponent();
            MainWindow.TypePage = MainWindow.TYPE_PAGE.NONE;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var rs = await UserService.LoginAsync(PhoneNumberTextBox.Text, PasswordBox.Password);
            if (!rs)
            {
                return;
            }
            var mainWindowView = new MainWindow();
            mainWindowView.ShowDialog();
            ConversationViewModel.GI().Reset();
            PostViewModel.Reset();
        }

        private void RegisterTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindowView();
            registerWindow.ShowDialog();
        }

        private void ForgotPasswordTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindowView();
            forgotPasswordWindow.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
