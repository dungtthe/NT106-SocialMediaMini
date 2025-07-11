using Client.Helpers;
using Client.Services;
using Client.ViewModels;
using Client.ViewModels.Chats;
using Client.ViewModels.Posts;
using Client.Views.Posts.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            MainWindow.PageViewType = PageViewType.NONE;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var rs = await UserService.LoginAsync(PhoneNumberTextBox.Text, realPassword);
            if (!rs.Item1)
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, rs.Item2);
                return;
            }
            ToastManager.AddToast(Const.Type.ToastType.Success, rs.Item2);
            var mainWindowView = new MainWindow();
            this.Hide();
            mainWindowView.ShowDialog();
            this.Show();
            MainWindow.PageViewType = PageViewType.NONE;
            ConversationViewModel.GI().Reset();
            PostViewModel.Reset();
        }

        private void RegisterTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindowView();
            this.Hide();
            registerWindow.ShowDialog();
            this.Show();
            //this.Close();
        }

        private void ForgotPasswordTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindowView();
            forgotPasswordWindow.ShowDialog();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MainWindow.PageViewType = PageViewType.NONE;
            ConversationViewModel.GI().Reset();
            PostViewModel.Reset();
            ToastManager.Stop();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            
        }

        private string realPassword = string.Empty;
        private bool isUpdatingPassword = false;
        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword)
                return;

            var txtBox = sender as TextBox;

            int caretIndex = txtBox.CaretIndex;
            int changeLength = txtBox.Text.Length - realPassword.Length;

            if (changeLength > 0)
            {
                // User typed something new
                string added = txtBox.Text.Substring(caretIndex - changeLength, changeLength);
                realPassword = realPassword.Insert(caretIndex - changeLength, added);
            }
            else if (changeLength < 0)
            {
                // User deleted
                int removeStart = caretIndex;
                int removeCount = Math.Abs(changeLength);
                if (removeStart < realPassword.Length)
                    realPassword = realPassword.Remove(removeStart, removeCount);
            }

            isUpdatingPassword = true;
            txtBox.Text = new string('•', realPassword.Length);
            txtBox.CaretIndex = caretIndex;
            isUpdatingPassword = false;
        }
    }
}
