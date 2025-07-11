using Client.Const;
using Client.Helpers;
using Client.LocalStorage;
using Newtonsoft.Json;
using SocialMediaMini.Shared.Const;
using SocialMediaMini.Shared.Dto.Request;
using SocialMediaMini.Shared.Dto.Respone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ViewModels.Chats.ConversationViewModel;

namespace Client.Services
{
    public static class UserService
    {
        public static async Task<Tuple<bool,string>>LoginAsync(string userName,string password)
        {
            UserStore.Reset();
            var data = new Request_LoginDTO()
            {
                UserName = userName,
                Password = password
            };

            try
            {

                var response = await ApiHelpers.PostAsync(new ApiRequest("/api/user/login", JsonConvert.SerializeObject(data),false));
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var rspData = JsonConvert.DeserializeObject<Respone_LoginDTO>(response.ResponseBody);
                    if(rspData != null)
                    {
                        UserStore.UserIdCur=rspData.UserId;
                        UserStore.Avatar = rspData.Image;
                        UserStore.FullName=rspData.FullName;
                        UserStore.Token= rspData.Token;
                        return new Tuple<bool, string>(true, "Chào " + UserStore.FullName + "!");
                    }
                }
                else
                {
                    return new Tuple<bool, string>(false, response.ResponseBody);
                }
            }
            catch { }

            return new Tuple<bool, string>(false, "Không thể truy cập tới máy chủ. Vui lòng thử lại sau!");

        }
    }
}
