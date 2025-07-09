using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SocialMediaMini.Service;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SocialMediaMini.API.Realtimes
{
    public class NotifyService : BackgroundService
    {
        //userId, notificationType, data
        public static ConcurrentQueue<Tuple<long, NotificationType, string>> Datas = new ConcurrentQueue<Tuple<long, NotificationType, string>>();


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
                        if (data.Item2 == NotificationType.MESSAGE)
                        {
                            try
                            {
                                var msg = JsonConvert.DeserializeObject<Request_AddMessageDTO>(data.Item3);
                                msg.UserId = senderId;
                                using var scope = _scopeFactory.CreateScope();
                                var _chatRoomService = scope.ServiceProvider.GetRequiredService<IChatRoomService>();
                                var resultAdd = await _chatRoomService.AddMessageAsync(msg);

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
                                await _hubContext.Clients.Users(useridsTemp).SendAsync("ReceiveMessage", NotificationType.MESSAGE, JsonConvert.SerializeObject(resultAdd.Item2));
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
