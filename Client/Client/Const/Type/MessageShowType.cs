using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Const.Type
{
    public enum MessageShowType
    {
        Other,               // Người khác gửi không reply
        OtherWithReply,      // Người khác gửi có reply
        Mine,                // Mình gửi không reply
        MineWithReply        // Mình gửi có reply
    }
}
