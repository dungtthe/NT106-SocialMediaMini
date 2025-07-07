using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Shared.Const.Type
{
    public enum NotificationType:byte
    {
        MESSAGE = 0,
        POST = 1,
        COMMENT = 2,
        REACT = 3,
    }
}
