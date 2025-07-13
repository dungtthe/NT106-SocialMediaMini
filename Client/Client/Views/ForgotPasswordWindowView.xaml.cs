using Client.Helpers;
using Client.Services;
using Client.ViewModels;
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
    /// Interaction logic for ForgotPasswordWindowView.xaml
    /// </summary>
    public partial class ForgotPasswordWindowView : Window
    {
        public ForgotPasswordWindowView()
        {
            InitializeComponent();
            MainWindow.PageViewType = PageViewType.NONE;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BacktoLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var BacktoLogin = new LoginWindowView();
            BacktoLogin.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string userName = txtUserName.Text;
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, "Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            var rsp = await UserService.RequestForgotPasswordAsync(new SocialMediaMini.Shared.Dto.Request.Request_ForgotPasswordDto()
            {
                Email = email,
                UserName = userName,
            });
            if (rsp.Item1)
            {
                ToastManager.AddToast(Const.Type.ToastType.Success, rsp.Item2);
            }
            else
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, rsp.Item2);
            }
        }
    }
}
