using Client.Helpers;
using Client.Services;
using Client.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindowView.xaml
    /// </summary>
    public partial class RegisterWindowView : Window
    {
        public RegisterWindowView()
        {
            InitializeComponent();
            MainWindow.PageViewType = PageViewType.NONE;
        }

        private void LogInTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindowView();
            loginWindow.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string phoneNumber = txtPhone.Text;
            string accountName = txtAccount.Text;
            string password = realPassword1;
            string confirmPassword = realPassword2;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, "Vui lòng điền đầy đủ tất cả các trường.");
                return;
            }

            if (password != confirmPassword)
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, "Mật khẩu và xác nhận mật khẩu không khớp.");
                return;
            }

            var rs = await UserService.RegisterAsync(new SocialMediaMini.Shared.Dto.Request.Request_RegisterDTO()
            {
                Email = email,
                Password = password,
                UserName = accountName,
                PhoneNumber = phoneNumber,
            });


            if (!rs.Item1)
            {
                ToastManager.AddToast(Const.Type.ToastType.Error, rs.Item2);
                return;
            }
            ToastManager.AddToast(Const.Type.ToastType.Success, rs.Item2);

            this.Close();

        }

        private string realPassword1 = string.Empty;
        private string realPassword2 = string.Empty;
        private bool isUpdatingPassword1 = false;
        private bool isUpdatingPassword2 = false;

        private void txtPass2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword2)
                return;

            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;

            int caretIndex = txtBox.CaretIndex;
            int changeLength = txtBox.Text.Length - realPassword2.Length;

            if (changeLength > 0)
            {
                // User typed something new
                string added = txtBox.Text.Substring(caretIndex - changeLength, changeLength);
                realPassword2 = realPassword2.Insert(caretIndex - changeLength, added);
            }
            else if (changeLength < 0)
            {
                // User deleted
                int removeStart = caretIndex;
                int removeCount = Math.Abs(changeLength);
                if (removeStart < realPassword2.Length)
                    realPassword2 = realPassword2.Remove(removeStart, removeCount);
            }

            isUpdatingPassword2 = true;
            txtBox.Text = new string('•', realPassword2.Length);
            txtBox.CaretIndex = caretIndex;
            isUpdatingPassword2 = false;
        }

        private void txtPass1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword1)
                return;

            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;

            int caretIndex = txtBox.CaretIndex;
            int changeLength = txtBox.Text.Length - realPassword1.Length;

            if (changeLength > 0)
            {
                string added = txtBox.Text.Substring(caretIndex - changeLength, changeLength);
                realPassword1 = realPassword1.Insert(caretIndex - changeLength, added);
            }
            else if (changeLength < 0)
            {
                int removeStart = caretIndex;
                int removeCount = Math.Abs(changeLength);
                if (removeStart < realPassword1.Length)
                    realPassword1 = realPassword1.Remove(removeStart, removeCount);
            }

            isUpdatingPassword1 = true;
            txtBox.Text = new string('•', realPassword1.Length);
            txtBox.CaretIndex = caretIndex;
            isUpdatingPassword1 = false;
        }

       
    }
}