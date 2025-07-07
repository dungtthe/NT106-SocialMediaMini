using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Const.Type
{
    public enum PostVisibilityType:byte
    {
        Private = 0, // Chỉ mình tôi
        Friend = 1, // Bạn bè
        Public = 2 // Công khai
    }
}
