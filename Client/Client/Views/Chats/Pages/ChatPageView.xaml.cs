using Client.ViewModels;
using Client.ViewModels.Chats;
using Client.Views.Toast;
using Newtonsoft.Json;
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
            //test
            // ToastManager.AddToast(Const.Type.ToastType.Success, DateTime.Now.ToString());
            SendMsg(parrentMsgId);
        }

        private void SendMsg(long parrentMessage)
        {
            btnSendMsg.IsEnabled = false;
            var content = txtMessageInput.Text;
            if (ConversationViewModel.GI().IsValidChatRoom())
            {
                if (content.EndsWith("\n"))
                {
                    content = content.TrimEnd('\r', '\n');
                }

                var data = new Tuple<long, string, long>(ConversationViewModel.GI().ChatRoomDetail.ChatRoomId, content, parrentMessage);
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


        }

        private void txtMessageInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!btnSendMsg.IsEnabled)
                return;

            if (e.Key == Key.Enter)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    return;
                }
                e.Handled = true;
                SendMsg(parrentMsgId);
                HideBoxReply();
            }
        }


        private long parrentMsgId = -1;
        private void itemMessage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel item)
            {
                var value = JsonConvert.DeserializeObject<Tuple<long, string, string>>(item.Tag.ToString());
                parrentMsgId = value.Item1;
                ShowBoxReply(value.Item2, value.Item3);
            }
        }

        private void ShowBoxReply(string senderName, string msgContent)
        {
            tblReplyNameSender.Text = "Trả lời: " + senderName;
            string[] s = msgContent.Split("\r");
            tblReplyContentMsg.Text = s[0];
            reply.Visibility = Visibility.Visible;
            inputFile.Visibility = Visibility.Collapsed;
        }

        private void HideBoxReply()
        {
            reply.Visibility = Visibility.Collapsed;
            inputFile.Visibility = Visibility.Visible;
        }

        private void btnCloseReplyBox_Click(object sender, RoutedEventArgs e)
        {
            HideBoxReply();
            parrentMsgId = -1;
        }
    }
}
