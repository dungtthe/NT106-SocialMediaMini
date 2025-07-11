using Client.Helpers;
using Client.Services;
using System;
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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string userName = PhoneNumberTextBox.Text; // Sử dụng PhoneNumberTextBox làm UserName
            string password = realPassword;
            string confirmPassword = realConfirmPassword;
            string email = "sample@gmail.com"; // Không có TextBox cho Email, cần thêm nếu cần
            //string phoneNumber = PhoneNumberTextBox.Text;
            string phoneNumber = "123456789";


            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) /*||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(phoneNumber)*/)
            {
                (new MessageBox.Error("Vui lòng điền đầy đủ thông tin!")).ShowDialog();
                return;
            }

            if (password != confirmPassword)
            {
                (new MessageBox.Error("Mật khẩu và xác nhận mật khẩu không khớp!")).ShowDialog();
                return;
            }

            // Gọi UserService.RegisterAsync
            RegisterAsync(userName, password, email, phoneNumber).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        (new MessageBox.Error("Đăng ký thất bại!")).ShowDialog();
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<bool> RegisterAsync(string userName, string password, string email, string phoneNumber)
        {
            //try
            //{
            //    bool success = await UserService.RegisterAsync(userName, password, email, phoneNumber);
            //    return success;
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            return false;
        }

        private string realPassword = string.Empty;
        private bool isUpdatingPassword = false;
        private string realConfirmPassword = string.Empty;


        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword)
                return;

            var txtBox = sender as TextBox;
            int caretIndex = txtBox.CaretIndex;
            int changeLength = txtBox.Text.Length - realPassword.Length;

            if (changeLength > 0)
            {
                string added = txtBox.Text.Substring(caretIndex - changeLength, changeLength);
                realPassword = realPassword.Insert(caretIndex - changeLength, added);
            }
            else if (changeLength < 0)
            {
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

        private void ConfirmPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword)
                return;

            var txtBox = sender as TextBox;
            int caretIndex = txtBox.CaretIndex;
            int changeLength = txtBox.Text.Length - realConfirmPassword.Length;

            if (changeLength > 0)
            {
                string added = txtBox.Text.Substring(caretIndex - changeLength, changeLength);
                realConfirmPassword = realConfirmPassword.Insert(caretIndex - changeLength, added);
            }
            else if (changeLength < 0)
            {
                int removeStart = caretIndex;
                int removeCount = Math.Abs(changeLength);
                if (removeStart < realConfirmPassword.Length)
                    realConfirmPassword = realConfirmPassword.Remove(removeStart, removeCount);
            }

            isUpdatingPassword = true;
            txtBox.Text = new string('•', realConfirmPassword.Length);
            txtBox.CaretIndex = caretIndex;
            isUpdatingPassword = false;
        }
    }
}