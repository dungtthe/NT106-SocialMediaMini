using SocialMediaMini.Shared.Const.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.DataAccess.Models
{
    [Table("Notifications")]
    public class Notification:BaseModel
    {
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public NotificationType NotificationType { get; set; }//0 là thông báo tin nhắn, 1 là post, 2 là comment,3 là react

        public long ReferenceId { get; set; }// là tin nhắn thì tham chiếu tới chatroom

        public string Content {  get; set; }//là tin nhắn thì để rỗng cũng được
        public bool IsRead {  get; set; }
        public DateTime CreateAt { get; set; }
    }
}
