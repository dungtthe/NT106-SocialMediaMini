using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public enum MessageType
    {
        Other,               // Người khác gửi không reply
        OtherWithReply,      // Người khác gửi có reply
        Mine,                // Mình gửi không reply
        MineWithReply        // Mình gửi có reply
    }

}
