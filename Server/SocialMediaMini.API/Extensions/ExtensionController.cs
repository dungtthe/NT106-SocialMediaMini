using Microsoft.AspNetCore.Mvc;
using SocialMediaMini.Common.Const;
using SocialMediaMini.Common.DTOs.Respone;

namespace SocialMediaMini.API.Extensions
{
    public static class ExtensionController
    {
        public static IActionResult ResponeMessageResult(this ControllerBase controller, ResponeMessage responeMessage)
        {
            return controller.StatusCode(responeMessage.HttpStatusCode, new
            {
                message = responeMessage.Message
            });
        }

        public static IActionResult InternalServerError(this ControllerBase controller)
        {
            return controller.StatusCode(HttpStatusCode.InternalServerError, HttpStatusCode.HeThongGapSuCo);
        }
    }
}
