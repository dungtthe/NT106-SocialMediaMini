using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialMediaMini.Common.MyCollections;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using System.Diagnostics;

namespace SocialMediaMini.API.Realtimes
{

    [Authorize]
    public class RealtimeHub : Hub
    {
        public static MyHashSet<long> UserOnlineIds = new MyHashSet<long>();

        public async Task SendMessage(NotificationType notificationType, string data)
        {
            try
            {
                long userId = GetUserId();
                if (userId == 0)
                {
                    return;
                }
                var notification = new Request_AddNotificationDTO
                {
                    NotificationType = notificationType,
                    Data = data
                };
                NotifyService.Datas.Enqueue(new Tuple<long, Request_AddNotificationDTO>(userId, notification));
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            UserOnlineIds.Add(GetUserId());
            Debug.WriteLine($"conntected: " + GetUserId());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            UserOnlineIds.Remove(GetUserId());
            Debug.WriteLine($"disconnected: " + GetUserId());
        }

        private long GetUserId()
        {
            var suserId = Context.User?.FindFirst("UserId")?.Value;
            long userId = 0;
            if (suserId == null || !long.TryParse(suserId, out userId))
            {
                return 0;
            }
            return userId;
        }
    }
}
