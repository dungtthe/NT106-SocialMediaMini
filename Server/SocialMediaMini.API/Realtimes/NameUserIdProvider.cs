using Microsoft.AspNetCore.SignalR;

namespace SocialMediaMini.API.Realtimes
{

    //định nghĩa lại cách lấy Context.UserIdentifier;
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value;
        }
    }
}
