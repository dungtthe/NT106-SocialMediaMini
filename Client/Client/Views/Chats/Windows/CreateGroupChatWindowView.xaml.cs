using Client.Services;
using Client.ViewModels;
using Client.ViewModels.Chats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Client.Views.Chats.Windows
{
    /// <summary>
    /// Interaction logic for CreateGroupChatWindowView.xaml
    /// </summary>
    public partial class CreateGroupChatWindowView : Window
    {

        private CreateGroupChatViewModel ViewModel;
        public CreateGroupChatWindowView()
        {
            InitializeComponent();
            ViewModel = new CreateGroupChatViewModel();
            DataContext = ViewModel;
            btnCreate.IsEnabled = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var selecteds = ViewModel.FriendItems.Where(f=>f.IsSelected).ToList();
            var userIds = new List<long>();
            foreach(var selected in selecteds)
            {
                userIds.Add(selected.UserId);
            }

            var rs = await ChatRoomService.CreateGroupChatAsync(new SocialMediaMini.Shared.Dto.Request.Request_CreateGroupchat()
            {
                MemberIds = userIds,
                Name = txtGroupName.Text,
                Message = txtGroupName.Text,
            });
            if (rs != null)
            {

                //tam thoi load lai cho le
                ConversationViewModel.GI().LoadChatRooms();
                this.Close();
                return;
            }
            txtGroupName.Text = "";

        }

        private void txtGroupName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtGroupName.Text))
            {
                btnCreate.IsEnabled = false;
            }
            else
            {
                btnCreate.IsEnabled = true;
            }
        }
    }
}
