using Client.Helpers;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views.AddFriend.Pages
{
    /// <summary>
    /// Interaction logic for AddFriendPage.xaml
    /// </summary>
    public partial class AddFriendPage : Page
    {
        private List<UserModel> _users;

        public AddFriendPage()
        {
            InitializeComponent();
            LoadSuggestedFriends();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private async void LoadSuggestedFriends()
        {
            try
            {
                var response = await ApiHelpers.GetAsync(new ApiRequestGet("/api/user/suggested-friends", true)); // Sử dụng ApiRequestGet cho GET
                if (response.StatusCode == (int)System.Net.HttpStatusCode.OK) // So sánh với int
                {
                    _users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserModel>>(response.ResponseBody);
                }
                else
                {
                    _users = new List<UserModel>
                    {
                        new UserModel { Id = 1, FullName = "Nguyen Van A", Avatar = "/Resources/Images/meolag.jpg", Status = "Online" },
                        new UserModel { Id = 2, FullName = "Tran Thi B", Avatar = "/Resources/Images/meolag.jpg", Status = "Offline" },
                        new UserModel { Id = 3, FullName = "Le Van C", Avatar = "/Resources/Images/meolag.jpg", Status = "Online" },
                        new UserModel { Id = 4, FullName = "Pham Thi D", Avatar = "/Resources/Images/meolag.jpg", Status = "Offline" }
                    };
                }
                SuggestedFriendsList.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi tải danh sách bạn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _users = new List<UserModel>
                {
                    new UserModel { Id = 1, FullName = "Nguyen Van A", Avatar = "/Resources/Images/meolag.jpg", Status = "Online" },
                    new UserModel { Id = 2, FullName = "Tran Thi B", Avatar = "/Resources/Images/meolag.jpg", Status = "Offline" }
                };
                SuggestedFriendsList.ItemsSource = _users;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            SuggestedFriendsList.ItemsSource = _users?.Where(u => u.FullName.ToLower().Contains(searchText)).ToList();
        }

        private async void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UserModel user)
            {
                bool success = await SendFriendRequest(user);
                if (success)
                {
                    button.Content = "Đã gửi";
                    button.IsEnabled = false;
                    button.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
                }
                else
                {
                    //MessageBox.Show("Gửi yêu cầu thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<bool> SendFriendRequest(UserModel user)
        {
            try
            {
                var response = await ApiHelpers.PostAsync(new ApiRequest($"/api/user/send-friend-request?receiverId={user.Id}", "", true)); // Truyền body rỗng
                return response.StatusCode == (int)System.Net.HttpStatusCode.OK; // So sánh với int
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    namespace Client.Models
    {
        public class UserModel
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Avatar { get; set; }
            public string Status { get; set; }
        }
    }
}