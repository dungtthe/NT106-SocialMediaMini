using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SocialMediaMini.Common.Const.Type;
using SocialMediaMini.Common.DTOs.Request;
using SocialMediaMini.Common.DTOs.Respone;
using SocialMediaMini.DataAccess.Repositories;
using SocialMediaMini.Service;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SocialMediaMini.API.Realtimes
{
    public class NotifyService : BackgroundService
    {
        public static ConcurrentQueue<Tuple<long, Request_AddNotificationDTO>> Datas = new ConcurrentQueue<Tuple<long, Request_AddNotificationDTO>>();


        private readonly IHubContext<RealtimeHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public NotifyService(IHubContext<RealtimeHub> hubContext, IServiceScopeFactory scopeFactory)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (Datas.TryDequeue(out var data))
                    {
                        var senderId = data.Item1;


                        #region message
                        if (data.Item2.NotificationType == Type_Notification.MESSAGE)
                        {
                            try
                            {
                                var msg = JsonConvert.DeserializeObject<Request_AddNotificationDTO.Message>(data.Item2.Data);
                                using var scope = _scopeFactory.CreateScope();
                                var _chatRoomService = scope.ServiceProvider.GetRequiredService<IChatRoomService>();
                                var resultAdd = await _chatRoomService.AddMessageAsync(new Request_AddMessageDTO()
                                {
                                    UserId = senderId,
                                    ChatRoomId = msg.ChatRoomId,
                                    Content = msg.Content,
                                    ParrentMessageId = msg.ParrentMessageId
                                });

                                var userIds = resultAdd.Item1;
                                if (userIds == null)
                                {
                                    continue;
                                }
                                var useridsTemp = new List<string>();
                                foreach (var userId in userIds)
                                {
                                    if (RealtimeHub.UserOnlineIds.Contains(userId))
                                    {
                                        useridsTemp.Add(userId + "");
                                    }
                                }
                                await _hubContext.Clients.Users(useridsTemp).SendAsync("ReceiveMessage", Type_Notification.MESSAGE, JsonConvert.SerializeObject(resultAdd.Item2));
                            }
                            catch { }
                        }

                        #endregion
                    }
                }
                catch { }
                await Task.Delay(20, stoppingToken);
            }
        }
    }

}
