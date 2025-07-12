using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Const.Type
{
    public enum MessageType : byte
    {
        Text = 0, // Tin nhắn thông thường
        Image = 1,
        Sticker = 2,
        File = 3,
        Revoked = 4
    }
}
