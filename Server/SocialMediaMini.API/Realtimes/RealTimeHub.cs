using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialMediaMini.Shared.Const.Type;
using SocialMediaMini.Shared.Dto.Request;
using System.Diagnostics;

namespace SocialMediaMini.API.Realtimes
{

    [Authorize]
    public class RealtimeHub : Hub
    {

        public static HashSet<long> UserOnlineIds = new HashSet<long>();
        public async Task SendMessage(NotificationType notificationType, string data)
        {
            try
            {
                long userId = GetUserId();
                if (userId == 0)
                {
                    return;
                }
                NotifyService.Datas.Enqueue(new Tuple<long, NotificationType,string>(userId, notificationType,data));
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
