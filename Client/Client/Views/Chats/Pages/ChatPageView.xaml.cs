using Client.ViewModels.Chats;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views.Chats.Pages
{
    /// <summary>
    /// Interaction logic for ChatPageView.xaml
    /// </summary>
    public partial class ChatPageView : Page
    {
        public ChatPageView()
        {
            InitializeComponent();
            ConversationViewModel conversationViewModel = ConversationViewModel.GI();
            conversationViewModel.Init();
            DataContext = conversationViewModel;
            btnSendMsg.IsEnabled = false;
            txtMessageInput.Text = "";

            var viewModel = this.DataContext as ConversationViewModel;
            if (viewModel != null)
            {
                // Đăng ký khi danh sách tin nhắn thay đổi
                viewModel.PropertyChanged += (s, e) =>
                {
                    try
                    {

                        if (e.PropertyName == nameof(viewModel.ChatRoomDetail))
                        {
                            if (viewModel.ChatRoomDetail?.Messages != null)
                            {
                                viewModel.ChatRoomDetail.Messages.CollectionChanged += Messages_CollectionChanged;

                                //  Scroll ngay khi load vào phòng chat mới
                                Dispatcher.InvokeAsync(() =>
                                {
                                    if (viewModel.ChatRoomDetail.Messages.Count > 0)
                                    {
                                        lstMessages.ScrollIntoView(viewModel.ChatRoomDetail.Messages[^1]); // ^1: phần tử cuối
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                };

            }
        }
        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
                {
                    // Scroll đến tin nhắn cuối cùng
                    Dispatcher.InvokeAsync(() =>
                    {
                        lstMessages.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void btnSendMsg_Click(object sender, RoutedEventArgs e)
        {
            SendMsg();
        }

        private void SendMsg()
        {
            btnSendMsg.IsEnabled = false;

            if (ConversationViewModel.GI().IsValidChatRoom())
            {
                var data = new Tuple<long, string>(ConversationViewModel.GI().ChatRoomDetail.ChatRoomId, txtMessageInput.Text);
                ConversationViewModel.MessagesSend.Enqueue(data);
            }
            txtMessageInput.Text = "";
        }

        private void txtMessageInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMessageInput.Text.Trim()) || !ConversationViewModel.GI().IsValidChatRoom())
            {
                btnSendMsg.IsEnabled = false;
            }
            else
            {
                btnSendMsg.IsEnabled = true;
            }
        }

        private void txtMessageInput_KeyDown(object sender, KeyEventArgs e)
        {

            if (!btnSendMsg.IsEnabled)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Chặn xuống dòng 
                SendMsg(); // Gửi tin nhắn
            }
        }
    }
}
