using Client.Const.Type;
using Client.LocalStorage;
using Client.Models.Respone;
using Client.ViewModels;
using Client.ViewModels.Chats;
using Client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.RealTimes
{
    public static class NotifyService
    {
        static HubConnection connection;
        public static void Init()
        {
            connection = new HubConnectionBuilder()
               .WithUrl($"https://localhost:7130/realtime?jwt={UserStore.Token}")
               .Build();

            Start();


            connection.Closed += async (error) =>
            {
                if (MainWindow.TypePage == MainWindow.TYPE_PAGE.NONE)
                {
                    return;
                }
                Start();
            };


            connection.On<byte, string>("ReceiveMessage", (type, data) =>
            {
                try
                {
                    if (type == Type_Notification.MESSAGE)
                    {

                        ConversationViewModel.MessagesReceive.Enqueue(data);


                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to parse message: " + ex.Message);
                }
            });

        }


        private static void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (MainWindow.TypePage == MainWindow.TYPE_PAGE.NONE)
                        {
                            DisConnect();
                            return;
                        }


                        await connection.StartAsync();
                        Debug.WriteLine("Connect ok");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Connect fail");
                        Debug.WriteLine(ex.Message);
                    }

                    await Task.Delay(2000);
                }
            });

        }


        public static async Task SendMessage(byte notificationType, string data)
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.InvokeAsync("SendMessage", notificationType, data);
            }
        }


        public static void DisConnect()
        {
            //tam thoi nhu nay da
            try
            {
                try
                {
                    connection.StopAsync();
                }
                catch { }
                try
                {
                    connection.DisposeAsync();
                }
                catch { }
            }
            catch { }
        }
    }
}
